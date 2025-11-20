using System;
using System.Globalization;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using WinstantPayDb;
using TSG.Models.APIModels;
using Tsg.UI.Main.APIControllers;

namespace TSGWebApi.Controllers
{
    public class EwalletPaymentController : ApiController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [ResponseType(typeof(TsgApiModels.TokenGuid))]
        public TsgApiModels.AnswerForOrder Post([FromBody] TsgApiModels.GetOrderFromMerchant value)
        {
            _logger.Info("Call method -> Ewallet_PaymentController");
            var result = new TsgApiModels.AnswerForOrder();
            result.ResultAnswer = new TsgApiModels.ResultAnswer
            {
                ResultCode = HttpStatusCode.NoContent,
                ResultDate = DateTime.Now,
                ResultText = ""
            };
            lock (result)
            {
                try
                {
                    var isPay = value.IsPay.Equals("1") ? true : false;
                    var merchantGuid = Guid.Parse(value.MerchantUniqueKey);
                    var orderGuid = Guid.Parse(value.OrderToken);
                    var userGuid = Guid.Parse(value.UserUniqueKey);
                    var quantity = decimal.Parse(value.Quantity, CultureInfo.InvariantCulture);

                    if (merchantGuid == default(Guid) || orderGuid == default(Guid) || userGuid == default(Guid))
                        throw new Exception("Used incorrects parameters");

                    if (quantity <= 0)
                        throw new Exception("Used incorrects parameters");

                    if (isPay) result = ApiLogic.PaymentOrderProcess(merchantGuid, orderGuid, userGuid, quantity);

                    else result = ApiLogic.GetInstantantForOrder(merchantGuid, orderGuid, userGuid, quantity);
                }
                catch (Exception e)
                {
                    result.ResultAnswer.ResultCode = HttpStatusCode.BadRequest;
                    result.ResultAnswer.ResultDate = DateTime.Now;
                    result.ResultAnswer.ResultText = e.Message;
                    _logger.Error(e);
                }
            }
            return result;
        }
    }
}