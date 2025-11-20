using System.Configuration;
using System.Web.Http;
using TSG.Models.APIModels;

namespace Tsg.UI.Main.APIControllers.AppVersionInfo
{
    [AllowAnonymous]
    public class ApiAppVersionInfoController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Json(new StandartResponse<string>(ConfigurationManager.AppSettings["lastAppVersion"],true, "Last version for mobile app"));
        }
    }
}