using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Tsg.UI.Main.Models;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.Business.Model.Classes;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Models.Attributes;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using System.Diagnostics;

namespace Tsg.UI.Main.Controllers
{
    [System.Web.Mvc.Authorize()]
    public class HomeController : BaseController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private SqlConnection _con;
        //CheckoutModel cm = new CheckoutModel();

        private void Connection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["ewalletDbCon"].ToString();
            _con = new SqlConnection(conStr);
        }
        [AllowAnonymous]
        public ActionResult ChangeCulture(string lang)
        {
            CultureInfo ci = null;
            switch (lang)
            {
                case "ar": ci = new CultureInfo("ar-AE"); break;
                case "ph": ci = new CultureInfo("en-PH"); break;
                case "fr": ci = new CultureInfo("fr"); break;
                case "th": ci = new CultureInfo("th"); break;
                case "ru": ci = new CultureInfo("ru"); break;
                case "cn": ci = new CultureInfo("zh-CN"); break;
                case "km": ci = new CultureInfo("km-KH"); break;
                default: ci = new CultureInfo("en-US"); break;
            }

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
            Thread.CurrentThread.CurrentUICulture = ci;

            HttpContext.Session["lang"] = lang;
            base.SetCurrentLang(lang);

            return Json(new StandartResponse(true, "Ok"));
        }

        public ActionResult Index()
        {
            if (Tsg.UI.Main.Models.Security.AppSecurity.CurrentUser == null)
                return RedirectToAction("Login", "User");

            UserRepository userRepo = new UserRepository();
            AppSecurity.CurrentUser.ShowWelcomeMessage =
                userRepo.GetWelcomeMessageStatus(AppSecurity.CurrentUser.UserName);

            AppSecurity.CurrentUser.WelcomeMessage = AppSecurity.CurrentUser.ShowWelcomeMessage ? userRepo.GetUserWelcomeMessage(AppSecurity.CurrentUser.UserName): string.Empty;

            if (Tsg.UI.Main.Models.Security.AppSecurity.CurrentUser.Role == UserRoleType.User)
            {
                DashboardViewModel model = new DashboardViewModel();
                model.PrepareAccountBalances();
                model.PrepareLatestInstantPayments();
                model.PrepareCreatedStatusPayouts();
                model.PrepareCreatedStatusInstantPayments();
                //var logo = model.GetWhiteLabelProfileLogo();
                
                //Debug.WriteLine("logo: " + logo);
                //if(logo != null)
                //{
                //    ViewBag.headerLogoBase64String = "data:image/png;base64," + Convert.ToBase64String(logo, 0, logo.Length);
                //    // ViewBag.headerLogo = logo;
                //}
                var emailLogoUrl = model.GetWhiteLabelProfileEmailLogoUrl();
                if (emailLogoUrl != null)
                {
                    Debug.WriteLine("emailLogoUrl: " + emailLogoUrl);
                    WriteWhiteLabelProfileEmailLogoUrlToCookie(emailLogoUrl);
                }
                return View(model);
            }
            else
            {
                return View();
            }
        }

        public ActionResult CheckoutIndex(Guid orderToken)
        {
            CheckoutModel model = new CheckoutModel();
            string customerAlias = "";
            try
            {
                Guid userInTsgSystem;
                if (Guid.TryParse(AppSecurity.CurrentUser.UserId, out userInTsgSystem))
                {
                    NewInstantPaymentViewModel paymentViewModel = new NewInstantPaymentViewModel(userInTsgSystem);
                    paymentViewModel.GetLastCustomerAlias();

                    Connection();
                    _con.Open();
                    SqlCommand com = _con.CreateCommand();
                    com.Connection = _con;
                    try
                    {
                        com = new SqlCommand("CheckOrderIfExist", _con);
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@orderToken", orderToken);
                        var resMerchantId = (int)com.ExecuteScalar();
                        com.Parameters.Clear();
                        com = new SqlCommand("GetMerchantName", _con);
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@merchantId", resMerchantId);
                        var resMerchantName = com.ExecuteScalar().ToString();
                        model.OrderToken = orderToken;
                        var nopCommerce = new NopCommerceRepository();
                        var avaliableAccounts = nopCommerce.PrepareAccountBalances();
                        if (paymentViewModel.AccountAliases == null || avaliableAccounts == null || (paymentViewModel.AccountAliases != null && paymentViewModel.AccountAliases.Count == 0))
                        {
                            if (paymentViewModel.AccountAliases != null && paymentViewModel.AccountAliases.Count == 0)
                                throw new Exception(GlobalRes.HomeController_CheckoutIndex_Error_NotAlias);
                            throw new Exception(GlobalRes.HomeController_CheckoutIndex_Error_NotExist);
                        }

                        model.AliasesList = paymentViewModel.AccountAliases;
                        model.AvailableAccounts = avaliableAccounts;
                        model.CustomerName = AppSecurity.CurrentUser.FirstName + " " + AppSecurity.CurrentUser.LastName;
                        model.MerchantName = resMerchantName;
                        
                        _con.Close();
                        return View(model);
                    }
                    catch (Exception e)
                    {
                        _logger.Error("CheckoutIndex Error:\n\r--->" + e.Message);
                        _con.Close();
                        return RedirectToAction("CheckoutLogin", "User",
                            new { src = "instant-checkout", token = orderToken, message = e.Message });
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = GlobalRes.Home_HomeController_CheckoutIndex_ApplicationError;
                    _logger.Error("CheckoutIndex can't understandind userGuid in TSG");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("CheckoutLogin", "User",
                    new { src = "instant-checkout", token = orderToken });
            }
            return View();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult CheckoutIndex(CheckoutModel cm)
        {
            try
            {
                Guid userInTsgSystem;
                if (Guid.TryParse(AppSecurity.CurrentUser.UserId, out userInTsgSystem))
                {
                    
                    PaymentRepository payment = new PaymentRepository();
                    var checkPaymentResult = payment.ConfirmOrder(cm, out var resMerchantSite);

                    if (checkPaymentResult.IsSuccess.HasValue && (bool)checkPaymentResult.IsSuccess)
                        return Redirect(String.Format("{0}?orderToken={1}&userToken={2}", resMerchantSite, cm.OrderToken, userInTsgSystem));
                    
                    ViewBag.ErrorMessage = GlobalRes.Home_HomeController_CheckoutIndex_ApplicationError;
                    return RedirectToAction("CheckoutIndex", "Home", new { orderToken = cm.OrderToken });
                }
            }
            catch (Exception exception)
            {
                _logger.Info("Confirm Order Error : " + exception.Message);
                _con.Close();
            }
            return View();
        }

        

        [System.Web.Mvc.HttpGet]
        public ActionResult GetLatestPayments()
        {
            if (!Tsg.UI.Main.Models.Security.AppSecurity.CurrentUser.UserName.Equals("admin"))
            {
                DashboardViewModel model = new DashboardViewModel();
                model.PrepareLatestInstantPayments();

                // ReSharper disable once Mvc.ViewNotResolved
                return View(model);
            }
            else
            {
                // ReSharper disable once Mvc.ViewNotResolved
                return View();
            }

        }

        public ActionResult Dashboard()
        {
            return Json("Ok");
        }

        public ActionResult Accounts()
        {
            ViewBag.ShowCustomFilter = true;
            AccountsViewModel model = new AccountsViewModel();
            model.PrepareAccountBalances();
            model.PrepareFavoriteCurrency();
            return View(model);
        }

        public ActionResult AccountStatement(Guid? id)
        {
            AccountStatementViewModel model;
            if (id == null || String.IsNullOrEmpty(id.ToString()))
                model = new AccountStatementViewModel();
            else model = new AccountStatementViewModel((Guid)id);
            model.PrepareDetails();
            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult AccountStatement(AccountStatementViewModel model)
        {
            model.IsFirstTime = false;
            try
            {
                model.SafetyStartDate = DateTime.ParseExact(model.StartDate.Replace(".", "/"), "dd/MM/yyyy", new CultureInfo("en-US"));
                model.SafetyEndDate = DateTime.ParseExact(model.EndDate.Replace(".", "/"), "dd/MM/yyyy", new CultureInfo("en-US"));
                model.PrepareDetails();
                model.Data.Entries = model.Data.Entries.OrderByDescending(ob => ob.ValueDate).ToArray();
            }
            catch (Exception e)
            {
                _logger.Error(e);
                ViewBag.IsSuccessResForAction = false;
                ViewBag.ActionResultText = e.Message;
            }
            return View(model);
        }

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Test()
        {
            Tsg.UI.Main.Models.Security.AppSecurity.CurrentUser = new UserInfo()
            {
                FirstName = "Test",
                LastName = "Test",
                UserId = "",
                UserName = ""
            };
            return Json("Ok");
        }

        private void WriteWhiteLabelProfileEmailLogoUrlToCookie(string url)
        {
            HttpCookie myCookie = new HttpCookie("headerLogoUrl");
            DateTime now = DateTime.Now;

            // Set the cookie value.
            myCookie.Value = url;

            // Set the cookie expiration date.
            myCookie.Expires = now.AddDays(14);

            // Add the cookie.
            Response.Cookies.Add(myCookie);
        }
    }
}