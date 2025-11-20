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

namespace Tsg.UI.Main.APIControllers
{
    /// <summary>
    /// Methods for new exchange quote
    /// </summary>
    [ApiFilter]
    public class ApiCreateExchangeController : ApiController
    {
        /// <summary>
        /// Get info for create new qoute
        /// </summary>
        /// <returns>List of accounts currencies</returns>
        [HttpGet]
        public IHttpActionResult Get()
        {
            var result = new ApiExchangeModel.GetAccountValue
            {
                Success = false,
                InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty },
                Data = new List<ApiExchangeModel.CurrencyAndAccAmount>()
            };

            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    result.Data = UiHelper.ApiGetAccountBalancesNoAvailable(ui).Select(s => new ApiExchangeModel.CurrencyAndAccAmount { AccountId = s.Value, AccountText = s.Text }).ToList();
                    result.Success = true;
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK", UserMessage = "List successfully received" };
                }
                else
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.CheckUserError, DeveloperMessage = "Data for user not correct", UserMessage = "Data for user not correct" };
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

        /// <summary>
        /// Post method for create new qoute
        /// </summary>
        /// <param name="model">Model for create new qoute</param>
        /// <returns>Quote details</returns>
        /// <exception cref="Exception">Unspecified errors</exception>
        [HttpPost]
        public IHttpActionResult Post([FromBody]ApiFXDealQuoteCreateRequest model)
        {
            var result = new UpdateTokenResponse()
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

                if (model.DealType.ToLower() == "fwdo" && String.IsNullOrEmpty(model.FinalValueDate))
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "InfoBlock. final_value_date is invalid", UserMessage = "InfoBlock. Value is null" };
                    return Ok(result);
                }

                if (model.DealType.ToLower() == "fwdw" && (String.IsNullOrEmpty(model.FinalValueDate) || String.IsNullOrEmpty(model.WindowOpenDate)))
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "InfoBlock. window_open_date or final_value_date is invalid", UserMessage = "InfoBlock. Value is null" };
                    return Ok(result);
                }

                ExchangeController apiExchangeRec = new ExchangeController(ui);
                ExchangeModel cuExchangeModel = new ExchangeModel();
                cuExchangeModel.BuyAmount = Convert.ToDecimal(model.Amount);
                cuExchangeModel.FromAccountId = model.From;
                cuExchangeModel.ToAccountId = model.To;
                cuExchangeModel.SellCurrency = model.PaymentCurrencyCode;
                cuExchangeModel.QuoteType = model.DealType;
                cuExchangeModel.WindowOpenDate = model.WindowOpenDate;
                cuExchangeModel.FinalValueDate = model.FinalValueDate;

                var currTime = DateTime.UtcNow.Ticks;
                var qoutaRes = apiExchangeRec.ApiCreateQuoteExchange2(cuExchangeModel);

                if (!qoutaRes.FxDealQuoteResult.ServiceResponse.HasErrors)
                {
                    string userInfo =
                        model.From == model.PaymentCurrencyCode ?
                            $"{qoutaRes.FxDealQuoteResult.Quote.SellAmount} will get you {qoutaRes.FxDealQuoteResult.Quote.BuyAmount}\r\n" +
                            $"Exchange rate: {qoutaRes.FxDealQuoteResult.Quote.Rate}"
                                :
                            $"{qoutaRes.FxDealQuoteResult.Quote.BuyAmount} will cost you {qoutaRes.FxDealQuoteResult.Quote.SellAmount}\r\n" +
                            $"Exchange rate: {qoutaRes.FxDealQuoteResult.Quote.Rate}";

                    result.Data = new UpdateTokenExchangeModel()
                    {
                        BuyAmount = cuExchangeModel.BuyAmount,
                        FromAccountId = cuExchangeModel.FromAccountId,
                        SellCurrency = cuExchangeModel.SellCurrency,
                        ToAccountId = cuExchangeModel.ToAccountId,
                        QuotaId = qoutaRes.FxDealQuoteResult.Quote.QuoteId,
                        CreatedTime = currTime,
                        QuotaTime = Convert.ToDateTime(qoutaRes.FxDealQuoteResult.Quote.ExpirationTime).Ticks - Convert.ToDateTime(qoutaRes.FxDealQuoteResult.Quote.QuoteTime).Ticks,
                        ExpirationTime = currTime + Convert.ToDateTime(qoutaRes.FxDealQuoteResult.Quote.ExpirationTime).Ticks - Convert.ToDateTime(qoutaRes.FxDealQuoteResult.Quote.QuoteTime).Ticks,
                        SellRateAmount = $"{qoutaRes.FxDealQuoteResult.Quote.SellAmount}",
                        BuyRateAmount = $"{qoutaRes.FxDealQuoteResult.Quote.BuyAmount}",
                        SellCCy = qoutaRes.FxDealQuoteResult.Quote.SellCurrencyCode,
                        BuyCCy = qoutaRes.FxDealQuoteResult.Quote.BuyCurrencyCode,
                        Rate = qoutaRes.FxDealQuoteResult.Quote.Rate
                    };
                    result.Success = true;
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK. Exchange token created succesifully", UserMessage = userInfo };
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
