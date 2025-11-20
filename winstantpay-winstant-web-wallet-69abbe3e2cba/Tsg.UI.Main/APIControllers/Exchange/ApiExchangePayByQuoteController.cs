using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.App_LocalResources;
using TSG.Models.APIModels;
using TSG.Models.APIModels.ExchangeModels;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Controllers;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Repository;

namespace Tsg.UI.Main.APIControllers
{
    /// <summary>
    /// Method for booking qoute
    /// </summary>
    [ApiFilter]
    public class ApiExchangePayByQuoteController : ApiController
    {
        /// <summary>
        /// Booking exchange by quote
        /// </summary>
        /// <param name="model">Quote model info</param>
        /// <returns>Message info</returns>
        [HttpPost]
        public IHttpActionResult Post([FromBody] UpdateTokenExchangeModel model)
        {
            var result = new StandartResponse()
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
                ExchangeController apiExchangeRec = new ExchangeController(ui);
                ExchangeModel cuExchangeModel = new ExchangeModel();
                cuExchangeModel.BuyAmount = model.BuyAmount;
                cuExchangeModel.FromAccountId = model.FromAccountId;
                cuExchangeModel.ToAccountId = model.ToAccountId;
                cuExchangeModel.SellCurrency = model.SellCurrency;
                cuExchangeModel.FxDealQuoteResult = new FXDealQuoteCreateResponse()
                {
                    Quote = new FXDealQuoteData()
                    {
                        QuoteId = model.QuotaId,
                        ExpirationTime = new DateTime(model.ExpirationTime).ToString(),
                        BuyAmount = model.BuyRateAmount,
                        SellAmount = model.SellRateAmount,
                        BuyCurrencyCode = model.BuyCCy,
                        SellCurrencyCode = model.SellCCy,
                        Rate = model.Rate
                    }
                };

                var qoutaRes = apiExchangeRec.ApiCurrencyExchange(cuExchangeModel);

                if (qoutaRes.FxDealQuoteBookResult != null && !qoutaRes.FxDealQuoteBookResult.ServiceResponse.HasErrors)
                {
                    string userInfo = $"{qoutaRes.FxDealQuoteResult.Quote.SellAmount} {GlobalRes.ExchangeCurrencyExchangePage4_MessageFromAcc} {qoutaRes.FxDealQuoteResult.Quote.SellCurrencyCode} {GlobalRes.ExchangeCurrencyExchangePage4_MessageAcc2}" +
                                $"\r\n {qoutaRes.FxDealQuoteResult.Quote.BuyAmount} {GlobalRes.ExchangeCurrencyExchangePage4_MessageToAcc} {qoutaRes.FxDealQuoteResult.Quote.BuyCurrencyCode} {GlobalRes.ExchangeCurrencyExchangePage4_MessageAcc}";

                    result.Success = true;
                    result.InfoBlock = new InfoBlock()
                    {
                        Code = ApiErrors.ErrorCodeState.Success,
                        DeveloperMessage = "OK",
                        UserMessage = userInfo
                    };
                }
                else
                {
                    result.Success = false;
                    result.InfoBlock = new InfoBlock()
                    {
                        Code = ApiErrors.ErrorCodeState.UnspecifiedError,
                        DeveloperMessage = qoutaRes.FxDealQuoteBookResult != null ?
                            $"Message {qoutaRes.FxDealQuoteBookResult.ServiceResponse.Responses[0].Message} \r\n" +
                            $"Message Deteils {qoutaRes.FxDealQuoteBookResult.ServiceResponse.Responses[0].MessageDetails}"
                            : "Unspecified error",

                        UserMessage = qoutaRes.FxDealQuoteBookResult != null ? qoutaRes.FxDealQuoteBookResult.ServiceResponse.Responses[0].Message : "Unspecified error"
                    };

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