using System.Web;
using System.Web.Mvc;
using Tsg.UI.Main.APIControllers;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Attributes;

namespace Tsg.UI.Main
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeUserAttribute());
        }
    }
}