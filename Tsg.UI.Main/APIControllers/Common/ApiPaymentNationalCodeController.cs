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
using TSG.Models.APIModels.Common;
using Newtonsoft.Json;
using Tsg.UI.Main.ApiMethods.Common;

namespace Tsg.UI.Main.APIControllers.Common
{
    /// <summary>
    /// API for payment countries list
    /// </summary>
    [ApiFilter]
    public class ApiPaymentNationalCodeController : ApiController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Get method for payment countries list
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[Route("api/Payment/Country")]
        public IHttpActionResult Post([FromBody] ApiPaymentNationalCodeRequest request)
        {
            var result = new ApiPaymentNationalCodeResponse()
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


                var nationalCodeMethod = new PaymentNationalCodeMethods(ui);

                _logger.Info("ApiPaymentNationalCodeController, Before getting payment national codes list");
                if (request != null && !String.IsNullOrEmpty(request.CurrencyCode))
                {
                    result.NationalCodesList = nationalCodeMethod.ByCurrency(request.CurrencyCode);
                }
                else
                {
                    result.NationalCodesList = nationalCodeMethod.All();
                }

                _logger.Info("ApiPaymentNationalCodeController, After getting payment national codes list");

                result.Success = true;
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK", UserMessage = "Getting payment national codes list successfully" };
            }
            catch (Exception e)
            {
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
            }

            return Ok(result);
        }
    }
}