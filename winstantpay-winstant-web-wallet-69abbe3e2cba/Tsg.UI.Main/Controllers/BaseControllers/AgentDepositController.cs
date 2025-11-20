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
using Tsg.UI.Main.Models.Security;
using TSG.ServiceLayer.InstantPayment;

namespace Tsg.UI.Main.Controllers
{
    /// <summary>
    /// Deposit function controller
    /// </summary>
    public class AgentDepositController : Controller
    {
        private readonly IInstantPaymentReceiveMethods _instantPaymentReceiveService;
        private readonly IInstantPaymentReceiveMappingMethods _instantPaymentReceiveMappingService;
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="instantPaymentReceiveService"></param>
        /// <param name="instantPaymentReceiveMappingService"></param>
        public AgentDepositController(IInstantPaymentReceiveMethods instantPaymentReceiveService, IInstantPaymentReceiveMappingMethods instantPaymentReceiveMappingService)
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
            var model = new NewDepositViewModel();
            model.IsMerchant = IsMerchant();

            return View(model);
        }

        // GET: Withdraw
        /// <summary>
        /// Get merchant details
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Merchant()
        {
            var merchantDetails = GetMerchantDetails();
            _logger.Debug(string.Format("merchantDetails IsCashIn  : {0} ", JsonConvert.SerializeObject(merchantDetails.IsCashIn)));
            
            return Json(new
            {
                isSuccess = true,
                isMerchant = merchantDetails.IsCashIn,
            });
        }

        // GET: Member
        /// <summary>
        /// Get member details
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Member(Guid id)
        {
            _logger.Debug(string.Format("receive ID: {0} ", id));
            var receive = PrepareInstantPaymentReceiveDetails(id);
            _logger.Debug(string.Format("receive details : {0} ", JsonConvert.SerializeObject(receive)));
            //var customerDetails = GetCustomerDetailsFromAlias(receive.Alias);
            //_logger.Debug(string.Format("customerDetails IsCashOut  : {0} ", JsonConvert.SerializeObject(customerDetails.IsCashOut)));
            //if (!customerDetails.IsCashOut)
            //{
            //    return Json(new
            //    {
            //        isSuccess = false,
            //        message = "Not a merchant's QR code"
            //    });
            //}

            return Json(new
            {
                isSuccess = true,
                message = "Member found",
                receiveId = receive.InstantPaymentReceiveId,
                memberAlias = receive.Alias,
                currencyCode = receive.Currency,
            });            
        }

        /// <summary>
        /// Create new withdraw
        /// </summary>
        /// <param name="model">Create withdraw model</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(NewDepositViewModel model)
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

                InstantPaymentReceiveMemoViewModel memo = JsonConvert.DeserializeObject<InstantPaymentReceiveMemoViewModel>(receive.Memo);

                model.Memo = memo.Memo;
                model.Name = memo.Name;
                model.Address = memo.Address;
                model.Email = memo.Email;

                model.ShortenedUrl = receive.ShortenedUrl;
            }

            return model;
        }


        /// <summary>
        /// Check whether the customer is a merchant
        /// </summary>
        /// <returns>True: customer is a merchant</returns>
        [NonAction]
        public bool IsMerchant()
        {
            var data = GetMerchantDetails();
            if (data.IsCashIn)
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
            if (!response.ServiceResponse.HasErrors && response.CustomerGetSingleData.IsCashOut)
            {
                return response.CustomerGetSingleData;
            }

            return new CustomerGetSingleData();
        }

        /// <summary>
        /// Get merchant details
        /// </summary>
        /// <returns>Customer data</returns>
        [NonAction]
        public CustomerGetSingleData GetMerchantDetails()
        {
            IgpService service = new IgpService(AppSecurity.CurrentUser.AccessToken, AppSecurity.CurrentUser.UserId);

            var response = service.CustomerGetSingle();
            if (!response.ServiceResponse.HasErrors)
            {
                return response.CustomerGetSingleData;
            }

            return new CustomerGetSingleData();
        }
    }
}