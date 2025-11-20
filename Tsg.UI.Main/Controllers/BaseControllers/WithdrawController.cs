using Autofac.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.Business.Model.TSGgpwsbeta;
using Tsg.UI.Main.Models;
using TSG.ServiceLayer.InstantPayment;

namespace Tsg.UI.Main.Controllers
{
    /// <summary>
    /// Withdraw function controller
    /// </summary>
    public class WithdrawController : Controller
    {
        private readonly IInstantPaymentReceiveMethods _instantPaymentReceiveService;
        private readonly IInstantPaymentReceiveMappingMethods _instantPaymentReceiveMappingService;
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="instantPaymentReceiveService"></param>
        /// <param name="instantPaymentReceiveMappingService"></param>
        public WithdrawController(IInstantPaymentReceiveMethods instantPaymentReceiveService, IInstantPaymentReceiveMappingMethods instantPaymentReceiveMappingService)
        {
            _instantPaymentReceiveService = instantPaymentReceiveService;
            _instantPaymentReceiveMappingService = instantPaymentReceiveMappingService;
        }

        // GET: Withdraw
        /// <summary>
        /// Create withdraw
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(Guid? id)
        {
            return View();
            //if (id.HasValue)
            //{
            //    return View(new NewInstantPaymentViewModel(id));
            //}

            //return View(new NewInstantPaymentViewModel());
        }

        // GET: Withdraw
        /// <summary>
        /// Get merchant details
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Merchant(Guid id)
        {
            try
            {
                _logger.Debug(string.Format("receive ID: {0} ", id));
                var receive = PrepareInstantPaymentReceiveDetails(id);
                _logger.Debug(string.Format("receive details : {0} ", JsonConvert.SerializeObject(receive)));
                var customerDetails = GetCustomerDetailsFromAlias(receive.Alias);
                _logger.Debug(string.Format("customerDetails IsCashOut  : {0} ", JsonConvert.SerializeObject(customerDetails.IsCashOut)));
                if (!customerDetails.IsCashOut)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = "Not a merchant's QR code"
                    });
                }

                return Json(new
                {
                    isSuccess = true,
                    message = "Merchant found",
                    receiveId = receive.InstantPaymentReceiveId,
                    isMerchant = true,
                    merchantAlias = receive.Alias,
                    currencyCode = receive.Currency,
                    amount = receive.Amount
                });
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Merchant Error  : {0} ", JsonConvert.SerializeObject(ex)));
                return Json(new
                {
                    isSuccess = false,
                    message = "Getting merchant details error"
                });
            }
            
            //return View();

            //if (Id.HasValue)
            //{
            //    return View(PrepareNewIntantPaymentViewModelByReceiveId(receiveId.Value));
            //}
            //return View();
        }

        /// <summary>
        /// Create new withdraw
        /// </summary>
        /// <param name="model">Create withdraw model</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(NewWithdrawViewModel model)
        {
            _logger.Debug(string.Format("model: {0}", JsonConvert.SerializeObject(model)));
            model.Create(out Guid paymentId, out string paymentReference);
            if (paymentId == Guid.Empty)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "Payment creation failed",
                    paymentId,
                    paymentReference
                });
            }

            return Json(new
            {
                isSuccess = true,
                message = "Payment created",
                paymentId,
                paymentReference
            });
        }

        /// <summary>
        /// Confirm withdraw
        /// </summary>
        /// <param name="model">New withdraw model</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Confirm(NewWithdrawViewModel model)
        {
            _logger.Debug(string.Format("model: {0}", JsonConvert.SerializeObject(model)));
            var isSuccess = model.Confirm();
            
            return Json(new
            {
                isSuccess
            });
        }

        [NonAction]
        private NewInstantPaymentViewModel PrepareNewIntantPaymentViewModelByReceiveId(Guid receiveId)
        {
            var mappings = _instantPaymentReceiveMappingService.GetByInstantPaymentReceiveId(receiveId).Obj;
            var receive = _instantPaymentReceiveService.GetAll().Obj.Where(r => r.Id == receiveId).FirstOrDefault();
            var model = new NewInstantPaymentViewModel();

            if (receive != null)
            {
                model.InstantPaymentReceiveId = receive.Id;
                model.ToCustomer = receive.Alias;
                model.Amount = receive.Amount;
                model.CurrencyCode = receive.Currency;
                model.InstantPay = receive.Invoice;
                model.Memo = receive.Memo;
            }

            _logger.Debug(string.Format("PrepareNewIntantPaymentViewModelByReceiveId, model: {0}", JsonConvert.SerializeObject(model)));
            return model;
        }

        [NonAction]
        private InstantPaymentReceiveDetailsViewModel PrepareInstantPaymentReceiveDetails(Guid instantPaymentReceiveId)
        {
            try
            {
                var model = new InstantPaymentReceiveDetailsViewModel();

                var receive = _instantPaymentReceiveService.GetAll().Obj.FirstOrDefault(r => r.Id == instantPaymentReceiveId);

                if (receive != null)
                {
                    model.InstantPaymentReceiveId = receive.Id;
                    model.Alias = receive.Alias;
                    model.Currency = receive.Currency;
                    model.Amount = receive.Amount;
                    model.Invoice = receive.Invoice;
                    model.CreatedDate = receive.CreatedDate;
                    model.AttachedFileName = receive.AttachedFileName;
                    
                    // Check if memo field data exists
                    if (receive.Memo != null && receive.Memo.Length > 50)
                    {
                        InstantPaymentReceiveMemoViewModel memo = JsonConvert.DeserializeObject<InstantPaymentReceiveMemoViewModel>(receive.Memo);

                        model.Memo = memo.Memo;
                        model.Name = memo.Name;
                        model.Address = memo.Address;
                        model.Email = memo.Email;
                    }
                    model.ShortenedUrl = receive.ShortenedUrl;
                }

                return model;
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("PrepareInstantPaymentReceiveDetails error: {0}", JsonConvert.SerializeObject(ex)));
                return null;
            }
        }


        /// <summary>
        /// Is the customer is a merchant
        /// </summary>
        /// <returns>True: customer is a merchant</returns>
        [NonAction]
        public bool IsMerchant(string alias)
        {
            var data = GetCustomerDetailsFromAlias(alias);
            if (data.IsCashOut)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Get customer details
        /// </summary>
        /// <returns>Customer data</returns>
        [NonAction]
        public CustomerGetSingleData GetCustomerDetailsFromAlias(string alias)
        {
            var bankUserName = ConfigurationManager.AppSettings["kycLogin"] ?? string.Empty;
            var bankUserPassword = ConfigurationManager.AppSettings["kycPassword"] ?? string.Empty;
            IgpService service = new IgpService(bankUserName, bankUserPassword, "");

            var response = service.GetCustomerGetSingleFromAlias(alias);
            _logger.Debug(string.Format("GetCustomerGetSingleFromAlias response: {0}", JsonConvert.SerializeObject(response)));
            if (!response.ServiceResponse.HasErrors && response.CustomerGetSingleData.IsCashOut)
            {
                return response.CustomerGetSingleData;
            }

            return new CustomerGetSingleData();
        }
    }
}