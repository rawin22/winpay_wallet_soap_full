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
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Controllers;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;

namespace Tsg.UI.Main.APIControllers
{
    /// <summary>
    /// Method for update quote if quote expired
    /// </summary>
    [ApiFilter]
    public class ApiExchangeUpdateQuotaRecordController : ApiController
    {
        /// <summary>
        /// Method for update quote if quote expired
        /// </summary>
        /// <param name="model">Model for create new qoute</param>
        /// <returns>Quote details</returns>
        /// <exception cref="Exception">Unspecified errors</exception>
        [HttpPost]
        public IHttpActionResult Post([FromBody] CreateQuoteModel model)
        {
            var result = new UpdateTokenResponse()
            {
                Success = false,
                Data = null,
                InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty }
            };
            Models.Security.UserInfo ui;
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
                foreach (var propertyInfo in listSpecParams.Where(w=>!w.Name.Equals("QuotaId")).ToList())
                {
                    var x = CheckValueByType.GetDefault(propertyInfo.PropertyType);
                    var @equals = propertyInfo.GetValue(model) is string ? String.IsNullOrEmpty(propertyInfo.GetValue(model).ToString()) : Equals(x, propertyInfo.GetValue(model));
                    isDefValue |= @equals;
                }
                if (isDefValue)
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "InfoBlock. Cann't not be value is null", UserMessage = "InfoBlock. Value is null" };
                    return Ok(result);
                }
                ExchangeController apiExchangeRec = new ExchangeController(ui);
                ExchangeModel cuExchangeModel = new ExchangeModel();
                cuExchangeModel.BuyAmount = Convert.ToDecimal(model.Amount);
                cuExchangeModel.FromAccountId = model.From;
                cuExchangeModel.ToAccountId = model.To;
                cuExchangeModel.SellCurrency = model.PaymentCurrencyCode;
                var qoutaRes = apiExchangeRec.ApiCreateQuoteExchange(cuExchangeModel);

                if (!qoutaRes.FxDealQuoteResult.ServiceResponse.HasErrors)
                {
                    result.Data = new UpdateTokenExchangeModel()
                    {
                        BuyAmount = cuExchangeModel.BuyAmount,
                        FromAccountId = cuExchangeModel.FromAccountId,
                        SellCurrency = cuExchangeModel.SellCurrency,
                        ToAccountId = cuExchangeModel.ToAccountId,
                        QuotaId = qoutaRes.FxDealQuoteResult.Quote.QuoteId,
                        LiveTime = DateTime.UtcNow.Ticks + (qoutaRes.FxDealQuoteResult.Quote.ExpirationTime.Ticks - qoutaRes.FxDealQuoteResult.Quote.QuoteTime.Ticks)
                    };
                    result.Success = true;
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK", UserMessage = "Exchange token updated succesifully" };
                }
                else
                {
                    throw new Exception("Quota doesn't created");
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
