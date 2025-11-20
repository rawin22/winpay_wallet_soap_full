using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Newtonsoft.Json;
using Tsg.Business.Model.TSGgpwsbeta;
using Tsg.UI.Main.APIModels;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;

namespace Tsg.UI.Main.APIControllers
{
    [ApiFilter]
    public class ApiCreateInstantPaymentController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            var result = new ApiInstantPaymentModel.InstantPaymentPageModelResponse
            {
                Success = false,
                Error = new Error() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty },
                Data = new ApiInstantPaymentModel.SendedDataByInstantPayment()
            };

            UserInfo ui;
            UserRepository userRepository = new UserRepository();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    var instantPaymentData = new ApiNewInstantPaymentViewModel(ui);
                    result.Data.CurrencyList = instantPaymentData.AvailableCurrencies.Select(s=>s.Value).ToList();
                    result.Data.ToAlias = instantPaymentData.PriorUsedAliases.Select(s=>s.Value).ToList();
                    result.Data.FromAlias = instantPaymentData.AccountAliases.Select(s => s.Value).ToList();
                    result.Success = true;
                    result.Error = new Error(){ Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK", UserMessage = "Data for instant payment selected correct" };
                }
                else
                {
                    result.Error = new Error(){ Code = ApiErrors.ErrorCodeState.CheckUserError, DeveloperMessage = "Data for user not correct", UserMessage = "Data for user not correct" };
                }
            }
            catch (ApiException apiException)
            {
                result.Error = apiException.CustomError;
            }
            catch (Exception e)
            {
                result.Error = new Error() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
            }
            return Ok(result);
        }
        
        [HttpPost]
        public IHttpActionResult Post([FromBody]ApiInstantPaymentModel.ApiInstantPaymentResponse model)
        {
            var result = new StandartResponse()
            {
                Success = false,
                Error = new Error() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty }
            };
            UserInfo ui;
            UserRepository userRepository = new UserRepository();

            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (!(isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui)))
                {
                    result.Error = new Error() { Code = ApiErrors.ErrorCodeState.CheckUserError, DeveloperMessage = "Data for user not correct", UserMessage = "Data for user not correct" };
                    return Ok(result);
                }
                bool isDefValue = false;
                var listSpecParams = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(w => Attribute.IsDefined(w, typeof(RequiredAttribute)));
                foreach (var propertyInfo in listSpecParams)
                {
                    var x = CheckValueByType.GetDefault(propertyInfo.PropertyType);
                    var @equals = propertyInfo.GetValue(model) is string ?  String.IsNullOrEmpty(propertyInfo.GetValue(model).ToString()) : Equals(x, propertyInfo.GetValue(model));
                    isDefValue |= @equals;
                }
                if(isDefValue)
                {
                    result.Error = new Error() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Error. Cann't not be value is null", UserMessage = "Error. Value is null" };
                    return Ok(result);
                }
                Guid? paymentId;
                ApiNewInstantPaymentViewModel apiNewInstantPayment = new ApiNewInstantPaymentViewModel(ui);
                apiNewInstantPayment.FromCustomer = model.FromCustomer;
                apiNewInstantPayment.ToCustomer = model.ToCustomer;
                apiNewInstantPayment.Amount = Convert.ToDecimal(model.Amount);
                apiNewInstantPayment.CurrencyCode = model.CurrencyCode;
                apiNewInstantPayment.Memo = model.Memo;
                apiNewInstantPayment.InstantPay = model.InstantPay;
                apiNewInstantPayment.Create(out paymentId);
                if (paymentId != null)
                {
                    result.Success = true;
                    result.Error = new Error() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK", UserMessage = "Instant payment will be creatad succesifully" };
                }
            }
            catch (Exception e)
            {
                result.Error = new Error() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
            }
            return Ok(result);
        }

        //// PUT: api/ApiInstantPayment/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        // DELETE: api/ApiInstantPayment/5
        //public void Delete(int id)
        //{
        //}
    }
}
