using System;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using WinstantPayDb;
using TSG.Models.APIModels;
using Tsg.UI.Main.APIControllers;
using Tsg.UI.Main.Attributes;

namespace TSGWebApi.Controllers
{
    //[AllowCrossSiteJson]
    public class EwalletGetNewOrderKeyController : ApiController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [ResponseType(typeof(TsgApiModels.TokenGuid))]
        public TsgApiModels.TokenGuid Post([FromBody] TsgApiModels.Merchant value)
        {
            _logger.Info("Call method -> Ewallet_GetNewOrderKeyController");
            var tokenGuid = new TsgApiModels.TokenGuid
            {
                ResultAnswer = new TsgApiModels.ResultAnswer
                {
                    ResultCode = HttpStatusCode.NotImplemented,
                    ResultText = "Not implemented",
                    ResultDate = DateTime.Now
                },
                KeyOfTokenGuid = ""
            };
            lock (tokenGuid)
            {
                try
                {
                    if (string.IsNullOrEmpty(value.MerchantUniqueKey) ||
                        string.IsNullOrEmpty(value.MerchantWebSiteCallBackAddress)||
                        string.IsNullOrEmpty(value.MerchantPublicKeyToken))
                        throw new Exception("Secret key or callback site url can't be null. Please set them.");
                    Guid merchantGuid;
                    if (Guid.TryParse(value.MerchantUniqueKey, out merchantGuid))
                        tokenGuid = ApiLogic.GetNewOrderKey(merchantGuid, value.MerchantWebSiteCallBackAddress, "USD");
                    else
                        throw new Exception(
                            "Secret key for merchant is corrupted. Please call to ewallet system admin.");
                }
                catch (Exception exception)
                {
                    tokenGuid.KeyOfTokenGuid = string.Empty;
                    tokenGuid.ResultAnswer = new TsgApiModels.ResultAnswer
                    {
                        ResultDate = DateTime.Now,
                        ResultText = exception.Message,
                        ResultCode = HttpStatusCode.NotImplemented
                    };
                    _logger.Error(exception);
                }
            }
            return tokenGuid;
        }
    }
}