using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Tsg.UI.Main
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // var corsAttr = new EnableCorsAttribute("*", "*", "*");
            //config.EnableCors(corsAttr);
            config.EnableCors();
            //defaults: new { controller = "ShortenedUrlProcess", action = "UrlRedirect" })
            //config.ad
            config.Routes.MapHttpRoute(
                name: "ApiInstantPaymentReceive",
                routeTemplate: "api/InstantPaymentReceive/{id}",
                defaults: new { controller = "ApiInstantPaymentReceive", id = RouteParameter.Optional }
            );

            // Payment
            // Approve Payment
            config.Routes.MapHttpRoute(
                name: "ApiPaymentApprove",
                routeTemplate: "api/Payment/Approve/{id}",
                defaults: new { controller = "ApiPaymentApprove", id = RouteParameter.Optional }
            );

            // Submit payment
            config.Routes.MapHttpRoute(
                name: "ApiPaymentSubmit",
                routeTemplate: "api/Payment/Submit",
                defaults: new { controller = "ApiPaymentSubmit"}
            );

            // Delete payment
            config.Routes.MapHttpRoute(
                name: "ApiPaymentDelete",
                routeTemplate: "api/Payment/Delete",
                defaults: new { controller = "ApiPaymentDelete"}
            );

            config.Routes.MapHttpRoute(
                name: "ApiPaymentValidate",
                routeTemplate: "api/Payment/Validate",
                defaults: new { controller = "ApiPaymentValidate"}
            );

            // Payment countries list
            config.Routes.MapHttpRoute(
                name: "ApiPaymentCountry",
                routeTemplate: "api/Payment/Country",
                defaults: new { controller = "ApiPaymentCountry" }
            );

            // Payment national codes list
            config.Routes.MapHttpRoute(
                name: "ApiPaymentNationalCode",
                routeTemplate: "api/Payment/NationalCode",
                defaults: new { controller = "ApiPaymentNationalCode" }
            );

            // Instant Payment
            config.Routes.MapHttpRoute(
                name: "ApiPayment",
                routeTemplate: "api/Payment/{id}",
                defaults: new { controller = "ApiPayment", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ApiInstantPaymentHistory",
                routeTemplate: "api/InstantPaymentHistory",
                defaults: new { controller = "ApiInstantPaymentHistory" }
            );

            // Trusted key
            // Delete trusted key
            config.Routes.MapHttpRoute(
                name: "ApiDeleteDelegatedAuthority",
                routeTemplate: "api/DelegatedAuthority/Delete/{id}",
                defaults: new { controller = "ApiDeleteDelegatedAuthorityLimits", id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "ApiLiquidCurrenciesUserSetting",
                routeTemplate: "api/LiquidCurrenciesUserSetting",
                defaults: new { controller = "ApiLiquidCurrenciesUserSetting" }
            );

            // Convert & exchange
            config.Routes.MapHttpRoute(
                name: "ApiCreateExchange",
                routeTemplate: "api/CreateExchange",
                defaults: new { controller = "ApiCreateExchange" }
            );

            config.Routes.MapHttpRoute(
                name: "ApiFXDealQuoteBook",
                routeTemplate: "api/FXDealQuote/Book",
                defaults: new { controller = "ApiFXDealQuoteBook" }
            );

            config.Routes.MapHttpRoute(
                name: "ApiFXDealQuote",
                routeTemplate: "api/FXDealQuote",
                defaults: new { controller = "ApiFXDealQuote" }
            );

            // Deposit
            config.Routes.MapHttpRoute(
                name: "ApiDeposit",
                routeTemplate: "api/Deposit",
                defaults: new { controller = "ApiDeposit" }
            );

            // User
            // Delete user alias
            config.Routes.MapHttpRoute(
                name: "ApiDeleteUserAlias",
                routeTemplate: "api/DeleteUserAlias",
                defaults: new { controller = "ApiDeleteUserAlias" }
            );

            // Bank directory search API
            config.Routes.MapHttpRoute(
                name: "ApiBankDirectorySearch",
                routeTemplate: "api/BankDirectorySearch",
                defaults: new { controller = "ApiBankDirectorySearch"}
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.XmlFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data"));

        }
        //public static void DisposeAndLogout()
        //{

        //    HttpContext context = HttpContext.Current;
        //    try
        //    {
        //        FormsAuthentication.SignOut();
        //        FormsAuthentication.Initialize();
        //        Roles.DeleteCookie();
        //        context.Session.Clear();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.HandleException(ex);
        //    }
        //    finally
        //    {
        //        FormsAuthentication.RedirectToLoginPage();
        //        //or can send to some other page
        //        string OriginalUrl = context.Request.RawUrl;
        //        string LoginPageUrl = @"~\Login.aspx";
        //        context.Response.Redirect(String.Format("{0}?ReturnUrl={1}", LoginPageUrl, context.Server.UrlEncode(OriginalUrl)));
        //    }
        //}
    }
}