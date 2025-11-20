using System.Web.Http;

namespace TSGWebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional }
            );
            //config.Routes.MapHttpRoute(
            //    name: "help_ui_shortcut",
            //    routeTemplate: "help",
            //    defaults: null,
            //    constraints: null,
            //    handler: new RedirectHandler(SwaggerDocsConfig.DefaultRootUrlResolver, "/doc/v1"));
        }
    }
}