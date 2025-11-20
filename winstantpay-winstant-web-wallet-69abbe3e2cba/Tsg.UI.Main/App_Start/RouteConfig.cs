using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Tsg.UI.Main
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            //routes.MapRoute(
            //    name: "lang",
            //    url: "{lang}/{controller}/{action}/{id}",
            //    constraints: new { lang = @"en|ru|fr|ph|ar|th|cn|km" },
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
            //    namespaces: new[] { "Tsg.UI.Main.Base.Controllers" }
            //);
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional/*, lang = "en"*/},
                namespaces: new[] { "Tsg.UI.Main.Base.Controllers" }
            );

        }
    }
}