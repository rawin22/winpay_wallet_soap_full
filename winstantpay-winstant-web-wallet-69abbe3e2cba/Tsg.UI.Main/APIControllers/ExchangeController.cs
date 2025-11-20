using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tsg.UI.Main.APIModels;
using Tsg.UI.Main.APIModels.ExchangeModels;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Repository;

namespace Tsg.UI.Main.APIControllers
{
    [ApiFilter]
    public class ExchangeController : ApiController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public IHttpActionResult Get()
        {
            var result = new ExchangeMainModel();
            UserRepository userRepository = new UserRepository();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out var ui))
                {
                    var instantPaymentData = new ApiNewInstantPaymentViewModel(ui);
                    
                    result.ListOfCurrencies = instantPaymentData.PrepareAccountBalances().Balances
                        .Select(s => new CurrencyModel { Amount = s.Balance, CurrencyName = s.Currency, CurrencyCode = s.AccountId.ToString() }).ToList();
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, UserMessage = string.Empty, DeveloperMessage = "Ok" };
                    result.Success = true;
                }
            }
            catch (ApiException apiException)
            {
                result.InfoBlock = apiException.CustomInfoBlock;
                _logger.Error(apiException);
            }
            catch (Exception e)
            {
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
                _logger.Error(e);
            }
            return Ok(result);
        }
    }
}
