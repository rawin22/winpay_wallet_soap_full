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
using Tsg.UI.Main.APIModels.ExchangeModels;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Controllers;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;

namespace Tsg.UI.Main.APIControllers
{
    [ApiFilter]
    public class ApiExchangeUpdateQuotaRecordController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post([FromBody]UpdateTokenExchangeModel model)
        {
            var result = new UpdateTokenResponse()
            {
                Success = false,
                Data = null,
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
                foreach (var propertyInfo in listSpecParams.Where(w=>!w.Name.Equals("QuotaId")).ToList())
                {
                    var x = CheckValueByType.GetDefault(propertyInfo.PropertyType);
                    var @equals = propertyInfo.GetValue(model) is string ? String.IsNullOrEmpty(propertyInfo.GetValue(model).ToString()) : Equals(x, propertyInfo.GetValue(model));
                    isDefValue |= @equals;
                }
                if (isDefValue)
                {
                    result.Error = new Error() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Error. Cann't not be value is null", UserMessage = "Error. Value is null" };
                    return Ok(result);
                }
                Guid? paymentId;
                ExchangeController apiExchangeRec = new ExchangeController(ui);
                ExchangeModel cuExchangeModel = new ExchangeModel();
                cuExchangeModel.BuyAmount = model.BuyAmount;
                cuExchangeModel.FromAccountId = model.FromAccountId;
                cuExchangeModel.ToAccountId = model.ToAccountId;
                cuExchangeModel.SellCurrency = model.SellCurrency;

                var qoutaRes = apiExchangeRec.ApiCreateQuoteExchange(cuExchangeModel);
                if (!qoutaRes.FxDealQuoteResult.ServiceResponse.HasErrors)
                {
                    model.QuotaId = qoutaRes.FxDealQuoteResult.Quote.QuoteId;
                    result.Data = model;
                    result.Success = true;
                    result.Error = new Error() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK", UserMessage = "Exchange token updated succesifully" };
                }
            }
            catch (Exception e)
            {
                result.Error = new Error() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
            }
            return Ok(result);
        }
    }
}
