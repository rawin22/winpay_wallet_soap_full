using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using StaticExtensions;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.ApiMethods.AutomaticExchange;
using Tsg.UI.Main.ApiMethods.Payments;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.DTO.InstantPayment;
using TSG.Models.ServiceModels.AutomaticExchange;
using TSG.Models.ServiceModels.Shop;
using TSG.ServiceLayer.AutomaticExchange.DependencyLiquidForUserServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidCcyListServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidOverDraftUserServiceMethods;
using TSG.ServiceLayer.InstantPayment;
using TSG.ServiceLayer.Users;
using TSG.ServiceLayer.WinstantPayShop.ShopInfoService;
using TSG.ServiceLayer.WinstantPayShop.ShopOrderItemsService;
using TSG.ServiceLayer.WinstantPayShop.ShopOrdersService;
using TSG.ServiceLayer.WinstantPayShop.ShopPaymentService;
using TSG.ServiceLayer.WinstantPayShop.ShopProductImagesService;
using TSG.ServiceLayer.WinstantPayShop.ShopProductService;

namespace Tsg.UI.Main.Controllers
{
    /// <summary>
    /// Payout controller
    /// </summary>
    public class PayoutController : BaseController
    {
        private readonly IShopOrdersService _shopOrdersService;
        private readonly IShopOrderItemsService _shopOrderItemsService;
        private readonly IShopProductService _shopProductService;
        private readonly IShopProductImagesService _shopProductImagesService;
        private readonly IShopInfoService _shopInfoService;
        private readonly IShopPaymentService _shopPaymentService;
        private readonly ILiquidCcyListServiceMethods _liquidCcyListServiceMethods;
        private readonly IDependencyLiquidForUserServiceMethods _dependencyLiquidForUserService;
        private readonly ILiquidOverDraftUserServiceMethods _liquidOverDraftUserServiceMethods;
        private readonly IUsersServiceMethods _usersServiceMethods;
        private readonly IInstantPaymentReceiveMethods _instantPaymentReceiveService;
        private readonly IInstantPaymentReceiveMappingMethods _instantPaymentReceiveMappingService;

        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shopOrdersService"></param>
        /// <param name="shopOrderItemsService"></param>
        /// <param name="shopProductService"></param>
        /// <param name="shopProductImagesService"></param>
        /// <param name="shopInfoService"></param>
        /// <param name="shopPaymentService"></param>
        /// <param name="dependencyLiquidForUserService"></param>
        /// <param name="liquidCcyListServiceMethods"></param>
        /// <param name="liquidOverDraftUserServiceMethods"></param>
        /// <param name="usersServiceMethods"></param>
        /// <param name="instantPaymentReceiveService"></param>
        /// <param name="instantPaymentReceiveMappingService"></param>
        public PayoutController(IShopOrdersService shopOrdersService, IShopOrderItemsService shopOrderItemsService, IShopProductService shopProductService,
            IShopProductImagesService shopProductImagesService, IShopInfoService shopInfoService, IShopPaymentService shopPaymentService,
            IDependencyLiquidForUserServiceMethods dependencyLiquidForUserService, ILiquidCcyListServiceMethods liquidCcyListServiceMethods,
            ILiquidOverDraftUserServiceMethods liquidOverDraftUserServiceMethods, IUsersServiceMethods usersServiceMethods, IInstantPaymentReceiveMethods instantPaymentReceiveService, IInstantPaymentReceiveMappingMethods instantPaymentReceiveMappingService)
        {
            _shopOrdersService = shopOrdersService;
            _shopOrderItemsService = shopOrderItemsService;
            _shopProductService = shopProductService;
            _shopProductImagesService = shopProductImagesService;
            _shopInfoService = shopInfoService;
            _shopPaymentService = shopPaymentService;
            _dependencyLiquidForUserService = dependencyLiquidForUserService;
            _liquidCcyListServiceMethods = liquidCcyListServiceMethods;
            _liquidOverDraftUserServiceMethods = liquidOverDraftUserServiceMethods;
            _usersServiceMethods = usersServiceMethods;
            _instantPaymentReceiveService = instantPaymentReceiveService;
            _instantPaymentReceiveMappingService = instantPaymentReceiveMappingService;
        }

        private static readonly string cryptoCurrency = ConfigurationManager.AppSettings["cryptoCurrencies"];


        /// <summary>
        /// Get: Payout/Create 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create(Guid? id)
        {
            var model = new NewPayoutViewModel();
            var customerId = AppSecurity.CurrentUser.OrganisationId;
            model.ValueDate = DateTime.Now.ToString("yyyy-MM-dd");
            model.CustomerId = customerId;

            if (id != null && id != Guid.Empty && id.HasValue)
            {
                _logger.Info("Payout - Create - Before Copy - id: " + id);
                model.Copy(id.Value);
                _logger.Info(Newtonsoft.Json.JsonConvert.SerializeObject(model));
            }
            else
            {
                _logger.Info("Payout - Create - Before Copy - id error ");
                _logger.Info(Newtonsoft.Json.JsonConvert.SerializeObject(model));
            }

            return View(model);
        }

        /// <summary>
        /// Create payout
        /// Post: Payout/Create
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NewPayoutViewModel model)
        {            
            try
            {
                Guid? paymentId = new Guid();
                model.FeeAmount = 0;
                model.FeeAmountCurrencyCode = model.AmountCurrencyCode;
                //model.CountryCode = AppSecurity.CurrentUser.
                model.BeneficiaryName = model.BeneficiaryFirstName + " " + model.BeneficiaryLastName;
                model.BeneficiaryAddress1 = model.BeneficiaryStreetAddress1;
                // model.BeneficiaryIdentificationTypeId = 1;
                // model.BeneficiaryOccupationTypeId = 1;
                // model.BeneficiaryTypeId = 1;
                model.InitiatingInstitutionSameAsOrderingInstitution = true;
                // model.SendingInstitutionOccupationTypeId = 1;
                model.SendingInstitutionSameAsOrderingInstitution = true;

                //if (model.IsPaymentValid() && model.Validate())
                if (model.Validate())
                {
                    model.Create(out paymentId);
                    if (paymentId != null && paymentId.HasValue)
                    {
                        model.PaymentId = paymentId.HasValue ? paymentId.Value : Guid.Empty;
                        model.HasError = false;
                        model.Errors.Add(new ErrorViewModel()
                        {
                            MessageDetails = "Payout is successfully created"
                        });

                        return RedirectToAction("Details", "Payout", new { id = paymentId });
                    }
                    else
                    {
                        model.HasError = true;
                        model.Errors.Add(new ErrorViewModel()
                        {
                            MessageDetails = "Creating Payout is failed"
                        });
                    }
                }               
                else
                {
                    model.HasError = true;
                    model.Errors.Add(new ErrorViewModel()
                    {
                        MessageDetails = "Payment validation is failed. Payout creation is failed"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

            return View(model);
        }

        /// <summary>
        /// Create payout
        /// Post: Payout/Create
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSubmit(NewPayoutViewModel model)
        {
            try
            {
                Guid? paymentId;
                model.FeeAmount = 0;
                model.FeeAmountCurrencyCode = model.AmountCurrencyCode;
                //model.CountryCode = AppSecurity.CurrentUser.
                model.BeneficiaryName = model.BeneficiaryFirstName + " " + model.BeneficiaryLastName;
                model.BeneficiaryAddress1 = model.BeneficiaryStreetAddress1;
                // model.BeneficiaryIdentificationTypeId = 1;
                // model.BeneficiaryOccupationTypeId = 1;
                // model.BeneficiaryTypeId = 1;
                model.InitiatingInstitutionSameAsOrderingInstitution = true;
                // model.SendingInstitutionOccupationTypeId = 1;
                model.SendingInstitutionSameAsOrderingInstitution = true;

                if (model.Validate())
                {
                    model.Create(out paymentId);
                    if (paymentId.HasValue)
                    {
                        model.PaymentId = paymentId.HasValue ? paymentId.Value : Guid.Empty;
                        model.HasError = false;
                        model.Errors.Add(new ErrorViewModel()
                        {
                            MessageDetails = "Payout is successfully created"
                        });

                        // Submit payment
                        var payout = new PayoutDetailsViewModel(paymentId.Value);
                        payout.PrepareDetails();
                        payout.Submit();
                        return RedirectToAction("Details", "Payout", new { id = paymentId });

                    }
                    else
                    {
                        model.HasError = true;
                        model.Errors.Add(new ErrorViewModel()
                        {
                            MessageDetails = "Creating Payout is failed"
                        });
                    }

                }
                else
                {
                    model.HasError = true;
                    model.Errors.Add(new ErrorViewModel()
                    {
                        MessageDetails = "Creating Payout is failed"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

            return View(model);
        }
        /// <summary>
        /// Get payment details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(Guid id)
        {
            var model = new PayoutDetailsViewModel(id);

            model.MailModel = new UserMailModel();
            model.PrepareDetails();
            if (model.PaymentId != default)
            {             
                return View(model);
            }

            model.Errors.Add(new ErrorViewModel
            {
                Message = "Payment doesn't exist or you doesn't have permission",
                Type = ErrorType.Error,
                Code = 0,
                MessageDetails = "",
                FieldName = "",
                FieldValue = ""
            });

            return View(model);           
        }

        /// <summary>
        /// Get payout history
        /// </summary>
        /// <param name="model">Payout search view model</param>
        /// <returns></returns>
        public ActionResult History(PayoutSearchViewModel model)
        {
            model.PreparePayouts();
            
            return View(model);
        }

        /// <summary>
        /// Submit payout
        /// </summary>
        /// <returns></returns>
        public ActionResult Submit(Guid id)
        {
            var payout = new PayoutDetailsViewModel(id);
            payout.PrepareDetails();
            payout.Submit();

            return RedirectToAction("Details", "Payout", new { id = id });
        }

        /// <summary>
        /// Delete payout
        /// </summary>
        /// <returns></returns>
        public ActionResult Delete(Guid id)
        {
            var payout = new PayoutDetailsViewModel(id);
            payout.PrepareDetails();
            payout.Delete();

            return RedirectToAction("History", "Payout", new { id = id });
        }

        private void CheckPayoutWhiteListed()
        {
            _logger.Debug("Payout - CheckPayoutWhiteListed: " + AppSettingHelper.IsPayoutWhiteListed());

            if (!AppSettingHelper.IsPayoutWhiteListed())
            {
                Response.Redirect("~/User/Login");
            }
        }
    }
}
