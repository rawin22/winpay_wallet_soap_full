using System;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using WinstantPayDb;
using Tsg.UI.Main.APIModels;
using Tsg.UI.Main.APIControllers;

namespace TSGWebApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class EwalletRegisterUserInSystemController : ApiController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [ResponseType(typeof(TsgApiModels.ResultAnswer))]
        public TsgApiModels.ResultAnswer Post([FromBody] TsgApiModels.UserInfo value)
        {
            _logger.Info("Call method -> Register user in ewallet system");
            var result = new TsgApiModels.ResultAnswer
            {
                ResultCode = HttpStatusCode.NotImplemented,
                ResultText = "Not implemented",
                ResultDate = DateTime.Now
            };
            lock (result)
            {
                try
                {
                    if (string.IsNullOrEmpty(value.OrderGuid) || string.IsNullOrEmpty(value.Stoke) ||
                        string.IsNullOrEmpty(value.UserTokenInTsgSystem) || string.IsNullOrEmpty(value.UserName))
                        throw new Exception("Values cann't be null. Please set them.");
                    Guid orderGuid;
                    Guid userGuid;

                    if (Guid.TryParse(value.OrderGuid, out orderGuid) &&
                        Guid.TryParse(value.UserTokenInTsgSystem, out userGuid))
                        result = ApiLogic.CheckingUserLogin(value.UserName, userGuid, value.Stoke, orderGuid);
                    else
                        throw new Exception("Values is corrupted. Please call to ewallet system admin.");
                }
                catch (Exception exception)
                {
                    result.ResultDate = DateTime.Now;
                    result.ResultText = exception.Message;
                    result.ResultCode = HttpStatusCode.NotImplemented;

                    _logger.Error(exception);
                }
            }
            return result;
        }
    }
}