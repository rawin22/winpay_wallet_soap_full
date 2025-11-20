using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Tsg.UI.Main.APIControllers;
using TSG.Models.APIModels;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.NopCommerce;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.NopCommerceChekingRules;
using Tsg.UI.Main.Repository;

namespace Tsg.UI.Main.Controllers
{
    /// <summary>
    /// Nopcommerce payment class
    /// </summary>
    [Authorize]
    [RoutePrefix("PaymentOrder")]
    public class NopCommercePaymentOrderController : BaseController
    {
        readonly log4net.ILog _logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Create new order
        /// </summary>
        /// <param name="order">cripted order parameter</param>
        /// <returns>View with model (if error model will be empty)</returns>
        /// <exception cref="Exception">Cheking exception if data is corrupted</exception>
        [HttpGet]
        [Route("NewOrderPayment")]
        public ActionResult NopCommerceNewOrderPayment(string token, string order)
        {
            NopCommerceModel model = new NopCommerceModel();
            try
            {
                if (AppSecurity.CurrentUser == null)
                {
                    return RedirectToAction("Login", "User",
                        new { returnUrl = HttpContext.Request.Url?.AbsoluteUri ?? "" });
                }

                model.IsSandbox = false;
                model.NeedToRedirect = false;
                List<Exception> errorsList = new List<Exception>();
                var checker = true;
                NopCommerceApiLogic.NopCommercePreparePaymentProcess(token, order, ref model, out checker, out errorsList);
                if(model.NeedToRedirect)
                    return Redirect(String.Format(model.UrlReturn + "/?tx={0}", model.Custom));
            }
            catch (Exception e)
            {
                _logger.Error(e);
                model.IsSuccess = false;
                model.Message = e.Message;
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Login", "User",
                    new { message = e.Message, returnUrl = HttpContext.Request.Url?.AbsoluteUri ?? ""});

            }

            return View(model);
        }

        /// <summary>
        /// Post method for registration payment and redirect to nopcommerce site
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("NewOrderPayment")]
        public ActionResult NopCommerceNewOrderPayment(string token, string order, NopCommerceModel model)
        {
            try
            {
                if (AppSecurity.CurrentUser == null)
                {
                    return RedirectToAction("Login", "User", new { returnUrl = HttpContext.Request.Url?.AbsoluteUri ?? "" });
                }
                model.IsSandbox = false;
                model.NeedToRedirect = false;
                List<Exception> errorsList = new List<Exception>();
                var checker = true;
                NopCommerceApiLogic.NopCommerceDoPaymentProcess(token, order, ref model, out checker, out errorsList);
                if (model.NeedToRedirect && errorsList.Count==0)
                    return Redirect(String.Format(model.UrlReturn + "/?tx={0}", model.Custom));
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            return View(model);
        }

        /// <summary>
        /// Decline order to nopcommerce system
        /// </summary>
        /// <param name="declineUrl">Cancel URL</param>
        /// <param name="orderId">Custom order ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("DeclineNopcommerceOrder")]
        public ActionResult DeclineNopcommerceOrder(string declineUrl, string orderId)
        {
            return Redirect($"{declineUrl}?orderId={orderId}");
        }
    }
}