using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using Tsg.UI.Main.ApiMethods.Payments;
using TSG.Models.APIModels;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels.InstantPayment;
using WinstantPay.Common.Object;

namespace Tsg.UI.Main.APIControllers.InstantPayment
{
    [ApiFilter]
    public class ApiPaymentDetailingController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet]
        [Route("api/ApiPaymentDetailing/{paymentId?}")]
        public IHttpActionResult Get(string paymentId = "")
        {
            var result = new PaymentHistoryInfoModel()
            {
                Success = false,
                InfoBlock = new InfoBlock()
                {
                    Code = ApiErrors.ErrorCodeState.EmptyResult,
                    DeveloperMessage = string.Empty,
                    UserMessage = string.Empty
                }
            };

            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (!(isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui)))
                    return Unauthorized();

                if (!Guid.TryParse(paymentId, out var paymentGuid))
                    throw new Exception("Payment isn't correct");

                InstantPaymentMethods ipm = new InstantPaymentMethods(ui);

                result.PaymentDetails = ipm.PrepareDetails(paymentGuid);
                if (result.PaymentDetails == null)
                    throw new Exception("Null object");

                var photoId = ipm.GetImageLink(result.PaymentDetails, String.Empty, ui);

                result.PaymentDetails.ImageUrl =
                    $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/SharedViewer/PaymentDetails?photoId={photoId}";
                
                result.InfoBlock = new InfoBlock(){ Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "Ok", UserMessage = "Info getted successfully"};
                result.Success = true;
            }
            catch (ApiException apiException)
            {
                result.InfoBlock = apiException.CustomInfoBlock;
            }
            catch (Exception e)
            {
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
            }
            return Ok(result);
        }
    }
}