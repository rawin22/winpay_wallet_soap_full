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
    public class ApiPaymentController : ApiController
    {
        /// <summary>
        /// Retrieve a details of payment by payment ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet]
        [Route("api/Payment/{id}")]
        public IHttpActionResult Get(Guid id)
        {
            var result = new ApiPaymentDetailsResponse()
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

                result.PaymentDetails = pm.PrepareDetails(id);
                if (result.PaymentDetails == null)
                    throw new Exception("Null object");
                
                result.InfoBlock = new InfoBlock(){ Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "Ok", UserMessage = "Payout Details is retrieved successfully"};
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

        /// <summary>
        /// Retrieve a list of payouts
        /// </summary>
        /// <returns>A list of payouts</returns>
        /// <exception cref="Exception"></exception>
        [HttpGet]
        [Route("api/Payout")]
        public IHttpActionResult Get()
        {
            var result = new PayoutSearchResponse()
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

                result.PayoutsList = pm.PreparePayoutsList();
                if (result.PayoutsList == null)
                    throw new Exception("Null object");

                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "Ok", UserMessage = "Payouts list is retrieved successfully" };
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

        /// <summary>
        /// Post method for payout creation
        /// </summary>
        /// <param name="request">payout creation request</param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Post([FromBody]ApiCreatePaymentRequest request)
        {
            var result = new ApiCreatePaymentResponse()
            {
                Success = false,
                InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty },
                // PayoutDetails = new ApiPayoutDetailsModel()                
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
                
                var createdPaymentResponse = pm.Create(request);

                if (createdPaymentResponse != null && createdPaymentResponse.Payment != null && !String.IsNullOrEmpty(createdPaymentResponse.Payment.PaymentId))
                {
                    result.Payment = createdPaymentResponse.Payment;
                    result.Success = true;
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK. Payment is created successfully", UserMessage = "Payment is created successfully" };
                }
                else
                {
                    result.Errors = createdPaymentResponse.Errors;
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = "Payment creation is failed", UserMessage = "Payment creation is failed" };
                }
            }
            catch (Exception e)
            {
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
            }

            return Ok(result);
        }

        /// <summary>
        /// Retrieve a details of payment by payment ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpDelete]
        [Route("api/Payment")]
        public IHttpActionResult Delete([FromBody] ApiDeletePaymentResquest request)
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

                if (!String.IsNullOrEmpty(errorMessage))
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Payment deletion failed, ErrorMessage = " + errorMessage, UserMessage = "Payment deletion failed, ErrorMessage = " + errorMessage };
                }
                else
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "Ok", UserMessage = "Payment is deleted successfully" };
                    result.Success = true;
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