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

namespace Tsg.UI.Main.APIControllers.InstantPayment
{
    [ApiFilter]
    public class ApiInstantPaymentHistoryController : ApiController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpGet]
        [Route("/api/InstantPaymentHistory/{paymentId:string}/{loadedcount:int?}/{needtoload:int?}/{status:int?}")]
        public IHttpActionResult Get(string paymentId = "", int loadedcount = 0, int needtoload = 25, int status = 0)
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

                    Business.Model.TsgGPWebService.InstantPaymentStatus instantPaymentStatus = (Business.Model.TsgGPWebService.InstantPaymentStatus)status;
                    _logger.Debug(instantPaymentStatus);
                    var payments = nipm.PrepareLatestInstantPayments(instantPaymentStatus);


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
                        PaymentRef = s.PaymentReference.ToString(),
                        CreationDate = s.CreatedTime.Ticks,
                        HistoryBalanceData = new ApiCommonModel.BalanceData()
                        {
                            Amount = s.IsIncome ? s.Amount : (-1) * s.Amount,
                            CurrencyName = s.Currency
                        },
                        IsSentPayment = !s.IsIncome,
                        From = s.FromCustomerAlias,
                        To = s.ToCustomerAlias,
                        Status = s.Status,

                    }).ToList();
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
