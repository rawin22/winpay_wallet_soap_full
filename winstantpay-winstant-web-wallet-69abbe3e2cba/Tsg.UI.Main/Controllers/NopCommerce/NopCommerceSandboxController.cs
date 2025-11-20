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
    /// Nopcommerce sandbox class
    /// </summary>
    [Authorize]
    [RoutePrefix("Sandbox")]
    public class NopCommerceSandboxController : BaseController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);    

        /// <summary>
        /// Create new order
        /// </summary>
        /// <param name="token">public token key for identifier user</param>
        /// <param name="order">cripted order parameter</param>
        /// <returns>View with model (if error model will be empty)</returns>
        /// <exception cref="Exception">Cheking exception if data is corrupted</exception>
        [HttpGet]
        [Route("NewOrderPayment")]
        public ActionResult NopCommerceNewOrderPayment(string token, string order)
        {
            NopCommerceModel model = new NopCommerceModel();
            model.IsSandbox = true;

            List<Exception> errorsList = new List<Exception>();
            var checker = true;
            try
            {
                if (AppSecurity.CurrentUser == null)
                {
                    return RedirectToAction("Login", "User", new {returnUrl = HttpContext.Request.Url.AbsoluteUri});
                }

                NopCommerceApiLogic.NopCommercePreparePaymentProcess(token, order, ref model, out checker, out errorsList);
                if (model.NeedToRedirect)
                    return Redirect(String.Format(model.UrlReturn + "/?tx={0}", model.Custom));
                if (!checker)
                {
                    throw new Exception(errorsList[0]?.Message ?? "Data corrupted");
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                model.IsSuccess = false;
                model.Message = e.Message;
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Login", "User",
                    new { message = e.Message, returnUrl = HttpContext.Request.Url?.AbsoluteUri ?? "" });

            }
            return View(model);
        }

        /// <summary>
        /// Post method for registration payment and redirect to nopcommerce site
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("NewOrderPayment")]
        public ActionResult NopCommerceNewOrderPayment(NopCommerceModel model, string token, string order)
        {
            try
            {
                if (AppSecurity.CurrentUser == null)
                {
                    return RedirectToAction("Login", "User", new { returnUrl = HttpContext.Request.Url.AbsoluteUri });
                }
                model.IsSandbox = true;
                model.NeedToRedirect = false;
                List<Exception> errorsList = new List<Exception>();
                var checker = true;
                NopCommerceApiLogic.NopCommerceDoPaymentProcess(token, order, ref model, out checker, out errorsList);
                if (model.NeedToRedirect)
                    return Redirect(String.Format(model.UrlReturn + "/?tx={0}", model.Custom));
            }
            catch (Exception e)
            {
                _logger.Error(e);

                model.IsSuccess = false;
                model.Message = e.Message;
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