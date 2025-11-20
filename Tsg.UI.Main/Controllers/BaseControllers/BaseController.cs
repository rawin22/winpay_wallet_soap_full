using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Tsg.UI.Main.Models.Attributes;

namespace Tsg.UI.Main.Controllers
{
    [OutputCache(Duration = 1)]
    public class BaseController : Controller
    {
        public BaseController()
        {
            
            if (HttpContext == null || !HttpContext.User.Identity.IsAuthenticated)
                CheckUser();
        }

        public ActionResult CheckUser()
        {
            return RedirectToAction("Login", "User");
        }

        public string CurrentLangCode { get; protected set; }

        //public Language CurrentLang { get; protected set; }
        public static CultureInfo ResolveCulture(System.Web.Routing.RequestContext requestContext)
        {
            string[] languages = requestContext.HttpContext.Request.UserLanguages;

            if (languages == null || languages.Length == 0)
                return null;

            try
            {
                string language = languages[0].ToLowerInvariant().Trim();
                return CultureInfo.CreateSpecificCulture(language);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        protected void SetCurrentLang(string lang)
        {
            CurrentLangCode = lang;
        }
        
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {

            var langs = requestContext.HttpContext.Request.UserLanguages;
            int lc = langs?.Length ?? 0;
            CultureInfo culture = ResolveCulture(requestContext);
            if (culture != null &&
                requestContext.RouteData.Values["controller"].ToString() == "User" && requestContext.RouteData.Values["action"].ToString() == "Login" &&
                String.IsNullOrEmpty(requestContext.HttpContext.Session["lang"]?.ToString() ?? ""))
            {
                var region = new RegionInfo(culture.LCID);
                requestContext.HttpContext.Session["lang"] = culture;
            }

            var x = requestContext.HttpContext.Session["lang"];
            
            if (x != null && !String.IsNullOrEmpty(x.ToString()))
            {
                CultureInfo ci = null;

                switch (x.ToString().ToLower())
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
            }

            else if (requestContext.RouteData.Values["lang"] != null && requestContext.RouteData.Values["lang"] as string != "null")
            {
                CurrentLangCode = requestContext.RouteData.Values["lang"] as string;
                //CurrentLang = Repository.Languages.FirstOrDefault(p => p.Code == CurrentLangCode);
                requestContext.HttpContext.Session["lang"] = CurrentLangCode;

                var ci = new CultureInfo(CurrentLangCode);
                Thread.CurrentThread.CurrentUICulture = ci;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
            }
            base.Initialize(requestContext);
        }


    }
}

