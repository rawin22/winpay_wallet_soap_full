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
    public class ApiCreateExchangeRecordController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            var result = new ApiExchangeModel.GetAccountValue
            {
                Success = false,
                Error = new Error() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty },
                Data = new List<ApiExchangeModel.CurrencyAndAccAmount>()
            };

            UserInfo ui;
            UserRepository userRepository = new UserRepository();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    result.Data = UiHelper.ApiGetAccountBalances(ui).Select(s => new ApiExchangeModel.CurrencyAndAccAmount { AccountId = s.Value, AccountText = s.Text }).ToList();
                    result.Success = true;
                    result.Error = new Error() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK", UserMessage = "Data for instant payment selected correct" };
                }
                else
                {
                    result.Error = new Error() { Code = ApiErrors.ErrorCodeState.CheckUserError, DeveloperMessage = "Data for user not correct", UserMessage = "Data for user not correct" };
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
        public IHttpActionResult Post([FromBody]UpdateTokenExchangeModel model)
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
                    var @equals = propertyInfo.GetValue(model) is string ? String.IsNullOrEmpty(propertyInfo.GetValue(model).ToString()) : Equals(x, propertyInfo.GetValue(model));
                    isDefValue |= @equals;
                }
                if (isDefValue)
                {
                    result.Error = new Error() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Error. Cann't not be value is null", UserMessage = "Error. Value is null" };
                    return Ok(result);
                }
                ExchangeController apiExchangeRec = new ExchangeController(ui);
                ExchangeModel cuExchangeModel = new ExchangeModel();
                cuExchangeModel.BuyAmount = model.BuyAmount;
                cuExchangeModel.FromAccountId = model.FromAccountId;
                cuExchangeModel.ToAccountId = model.ToAccountId;
                cuExchangeModel.SellCurrency = model.SellCurrency;
                Guid quota = new Guid();
                if(!String.IsNullOrEmpty(model.QuotaId) && Guid.TryParse(model.QuotaId, out quota))
                    cuExchangeModel.FxDealQuoteResult = new FXDealQuoteCreateResponse() { Quote = new FXDealQuoteData() { QuoteId = model.QuotaId } };

                var qoutaRes = apiExchangeRec.ApiCurrencyExchange(cuExchangeModel);

                if (qoutaRes.FxDealQuoteBookResult != null && !qoutaRes.FxDealQuoteBookResult.ServiceResponse.HasErrors)
                {
                    result.Success = true;
                    result.Error = new Error()
                    {
                        Code = ApiErrors.ErrorCodeState.Success,
                        DeveloperMessage = "OK",
                        UserMessage = String.Format("{0} {1} {2} {3}.", qoutaRes.FxDealQuoteResult.Quote.SellAmount, GlobalRes.ExchangeCurrencyExchangePage4_MessageFromAcc, qoutaRes.FxDealQuoteResult.Quote.SellCurrencyCode, GlobalRes.ExchangeCurrencyExchangePage4_MessageAcc2) +
                                     "\r\n" + String.Format("{0} {1} {2} {3}.", qoutaRes.FxDealQuoteResult.Quote.BuyAmount, GlobalRes.ExchangeCurrencyExchangePage4_MessageToAcc, qoutaRes.FxDealQuoteResult.Quote.BuyCurrencyCode, GlobalRes.ExchangeCurrencyExchangePage4_MessageAcc)
                    };
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
