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
    public class ApiPaymentCountryController : ApiController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Get method for payment countries list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Route("api/Payment/Country")]
        public IHttpActionResult Get()
        {
            var result = new ApiPaymentCountriesResponse()
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

                _logger.Info("ApiPaymentCountryController, Before getting payment countries list");
                var countryMethod = new PaymentCountryMethods(ui);
                result.CountriesList = countryMethod.All();
                _logger.Info("ApiPaymentCountryController, After getting payment countries list");

                result.Success = true;
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK", UserMessage = "Bank search successfully" };
            }
            catch (Exception e)
            {
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
            }

            return Ok(result);
        }
    }
}