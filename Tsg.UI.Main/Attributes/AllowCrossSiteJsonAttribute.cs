using System.Web.Http.Filters;

namespace Tsg.UI.Main.Attributes
{
    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Response?.Headers.Add("Access-Control-Allow-Origin", "*");

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}