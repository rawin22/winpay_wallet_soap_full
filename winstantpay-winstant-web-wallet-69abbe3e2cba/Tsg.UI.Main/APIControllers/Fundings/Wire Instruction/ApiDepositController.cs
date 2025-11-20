using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Newtonsoft.Json;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.Business.Model.TSGgpwsbeta;
using Tsg.UI.Main.App_LocalResources;
using TSG.Models.APIModels;
using TSG.Models.APIModels.ExchangeModels;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Controllers;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using Tsg.UI.Main.ApiMethods.FX;
using TSG.Models.APIModels.Wire_Instructions;
using Tsg.UI.Main.ApiMethods.Fundings;

namespace Tsg.UI.Main.APIControllers
{
    /// <summary>
    /// Methods for new exchange quote
    /// </summary>
    [ApiFilter]
    public class ApiDepositController : ApiController
    {
        /// <summary>
        /// Post method for create new deposit
        /// </summary>
        /// <param name="model">Model for booking a FX deal</param>
        /// <returns>Deposit information</returns>
        /// <exception cref="Exception">Unspecified errors</exception>
        [HttpPost]
        public IHttpActionResult Post([FromBody]ApiDepositCreateRequest model)
        {
            var result = new ApiDepositCreateResponse()
            {
                Success = false,
                InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty }
            };
            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();

            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (!(isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui)))
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.CheckUserError, DeveloperMessage = "Data for user not correct", UserMessage = "Data for user not correct" };
                    return Ok(result);
                }
                bool isDefValue = false;
                var listSpecParams = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(w => Attribute.IsDefined(w, typeof(RequiredAttribute)));
                foreach (var propertyInfo in listSpecParams)
                {
                    var x = CheckValueByType.GetDefault(propertyInfo.PropertyType);
                    var @equals = propertyInfo.GetValue(model) is string ? String.IsNullOrEmpty(propertyInfo.GetValue(model).ToString()) : Equals(x, propertyInfo.GetValue(model));
                    isDefValue |= @equals;
                }
                if (isDefValue)
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "InfoBlock. can't not be value is null", UserMessage = "InfoBlock. Value is null" };
                    return Ok(result);
                }

                DepositMethods depositMethod = new DepositMethods(ui);
                var depositCreateResponse = depositMethod.Create(model);

                if (depositCreateResponse.DepositInformation != null && !String.IsNullOrEmpty(depositCreateResponse.DepositInformation.DepositId))
                {
                    result.DepositInformation = depositCreateResponse.DepositInformation;
                    result.Success = true;
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK. Deposit is created successfully", UserMessage = "OK. Deposit is created successfully" };
                }
                else
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = "Deposit creation is failed", UserMessage = "Deposit creation is failed" };
                    result.Errors = depositCreateResponse.Errors;
                }
            }
            catch (Exception e)
            {
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
            }
            return Ok(result);
        }
    }
}
