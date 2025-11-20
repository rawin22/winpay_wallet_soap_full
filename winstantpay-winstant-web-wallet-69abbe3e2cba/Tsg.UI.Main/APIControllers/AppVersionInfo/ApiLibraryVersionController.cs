
using System.Configuration;
using System.Web.Http;
using Tsg.UI.Main.ApiMethods;
using TSG.Models.APIModels;

namespace Tsg.UI.Main.APIControllers.AppVersionInfo
{
    [AllowAnonymous]
    public class ApiLibraryVersionController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            LibraryVersionMethod versionMethod = new LibraryVersionMethod();
            return Ok(versionMethod.GetVersion());
        }
    }
}