using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Tsg.UI.Main.ApiMethods.Payments;
using TSG.Models.APIModels;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels.InstantPayment;

namespace Tsg.UI.Main.APIControllers.MobileAppControllers
{
    [ApiFilter]
    public class ApiMainPageController : ApiController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpGet]

        [Route("~/api/ApiMainPage/{paymentId:string}")]
        [Route("~/api/ApiMainPage/{paymentId:string}/{needtoload:int?}")]
        [Route("~/api/ApiMainPage/{paymentId:string}/{loadedcount:int?}/{needtoload:int?}")]
        public IHttpActionResult Get(string paymentId = "", int loadedcount = 0, int needtoload = 25)
        {
            var result = new ApiInstantPaymentHistoryModel.MainPageModel();
            UserRepository userRepository = new UserRepository();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out var ui))
                {
                    var instantPaymentData = new ApiNewInstantPaymentViewModel();
                    NewInstantPaymentMethods nipm = new NewInstantPaymentMethods(ui);

                    var payments = nipm.PrepareLatestInstantPayments();


                    if (!String.IsNullOrEmpty(paymentId))
                    {
                        if (!Guid.TryParse(paymentId, out var paymentGuid))
                            throw new Exception("Invalid payment Id. Set correct payment Id");

                        var lastPayment = payments.FirstOrDefault(f => f.PaymentId == paymentGuid);
                        if (lastPayment == null)
                            throw new Exception("Invalid payment Id. Payment does not find");
                        int paymentState = payments.IndexOf(lastPayment);

                        payments = payments.Skip(paymentState).ToList();

                    }

                    if (loadedcount > 0)
                    {
                        payments = payments.Skip(loadedcount).ToList();
                    }

                    payments = payments.Take(needtoload).ToList();

                    result.PaymentHistory = payments.Select(s => new ApiInstantPaymentHistoryModel.HistoryData
                    {
                        PaymentId = s.PaymentId.ToString(),
                        CreationDate = s.CreatedTime.Ticks,
                        HistoryBalanceData = new ApiCommonModel.BalanceData()
                        {
                            Amount = s.IsIncome ? s.Amount : (-1) * s.Amount,
                            CurrencyName = s.Currency
                        },
                        IsSentPayment = !s.IsIncome,
                        TextInfo = s.IsIncome ? $"Recieved from {s.FromCustomerAlias}" : $"Sent to {s.ToCustomerAlias}"
                    }).ToList();
                    result.BalanceDatasList = nipm.PrepareAccountBalances().Balances
                        .Select(s => new ApiCommonModel.BalanceData { Amount = s.Balance, CurrencyName = s.CCY, IsBaseCurrency = s.CCY == s.BaseCCY, AccountId = s.AccountId, AccountNumber = s.AccountNumber}).ToList();
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