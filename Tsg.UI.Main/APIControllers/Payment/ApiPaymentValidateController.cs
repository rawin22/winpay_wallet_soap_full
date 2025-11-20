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
    public class ApiPaymentValidateController : ApiController
    {
        /// <summary>
        /// Post method for payout validation
        /// </summary>
        /// <param name="request">ApiValidatePayoutRequest</param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Post([FromBody] ApiValidatePaymentRequest request)
        {
            var result = new ApiValidatePaymentResponse()
            {
                Success = false,
                InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty },                          
            };
            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();

            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (!(isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui)))
                {
                    return Unauthorized();
                }

                bool isDefValue = false;
                var listSpecParams = request.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(w => Attribute.IsDefined(w, typeof(RequiredAttribute)));
                foreach (var propertyInfo in listSpecParams)
                {
                    var x = CheckValueByType.GetDefault(propertyInfo.PropertyType);
                    var @equals = propertyInfo.GetValue(request) is string ? String.IsNullOrEmpty(propertyInfo.GetValue(request).ToString()) : Equals(x, propertyInfo.GetValue(request));
                    isDefValue |= @equals;
                }

                if (isDefValue)
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "InfoBlock. can't not be value is null", UserMessage = "InfoBlock. Value is null" };
                    return Ok(result);
                }

                PaymentMethods pm = new PaymentMethods(ui);
                var paymentId =Guid.Empty;
                var errorMessage = String.Empty;

                var isValid = pm.Validate(request);
                
                if (!isValid)
                {                
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Payment is invalid, ErrorMessage = " + errorMessage, UserMessage = "Payment is invalid"};
                    result.Errors = pm.Errors;

                    return Ok(result);
                }

                result.Success = true;
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK", UserMessage = "Payment is valid" };
            }
            catch (Exception e)
            {
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
            }

            return Ok(result);
        }       
    }
}