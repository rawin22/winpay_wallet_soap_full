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
using TSG.Models.APIModels.Payment;
using Tsg.UI.Main.ApiMethods.Payouts;
using System.Reflection;
using Tsg.UI.Main.Extensions;
using System.ComponentModel.DataAnnotations;
using Tsg.Business.Model.TSGgpwsbeta;

namespace Tsg.UI.Main.APIControllers.Payment
{
    /// <summary>
    /// API for payout
    /// </summary>
    [ApiFilter]
    public class ApiPaymentDeleteController : ApiController
    {
        /// <summary>
        /// Delete a payment
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        [Route("api/Payout/Delete")]
        public IHttpActionResult Post([FromBody] ApiDeletePaymentResquest request)
        {
            var result = new ApiDeletePaymentResponse()
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

                PaymentMethods pm = new PaymentMethods(ui);
               
                // Delete the payment
                var errorMessage = String.Empty;
                var deleteResponse = pm.Delete(request);

                if (deleteResponse.Payment != null && !String.IsNullOrEmpty(deleteResponse.Payment.PaymentId))
                {
                    result.Payment = deleteResponse.Payment;
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK. Payment is deleted successfully", UserMessage = "OK. Payment is deleted successfully" };
                    result.Success = true;
                }
                else
                {
                    result.Errors = deleteResponse.Errors;
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Payment deletion failed, ErrorMessage = " + errorMessage, UserMessage = "Payment deletion failed, ErrorMessage = " + errorMessage };
                }
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