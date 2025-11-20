using System;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using TSGWebApi.Logic;
using TSGWebApi.Models;

namespace TSGWebApi.Controllers
{
    public class EwalletGetNewOrderKeyController : ApiController
    {
        [ResponseType(typeof(TsgApiModels.TokenGuid))]
        public TsgApiModels.TokenGuid Post([FromBody] TsgApiModels.Merchant value)
        {
            CommonMethods.PrintInfo("Call method -> Ewallet_GetNewOrderKeyController", true);
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
                        string.IsNullOrEmpty(value.MerchantWebSiteCallBackAddress))
                        throw new Exception("Secret key or callback site url cann't be null. Please set them.");
                    Guid merchantGuid;
                    if (Guid.TryParse(value.MerchantUniqueKey, out merchantGuid))
                        tokenGuid = ApiLogic.GetNewOrderKey(merchantGuid, value.MerchantWebSiteCallBackAddress);
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
                    CommonMethods.PrintError(exception);
                }
            }
            return tokenGuid;
        }
    }
}