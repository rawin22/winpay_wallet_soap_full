using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Http;
using Tsg.UI.Main.ApiMethods.Reports;
using TSG.Models.APIModels;
using TSG.Models.APIModels.Reports.Accounting;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Repository;

namespace Tsg.UI.Main.APIControllers.Reports.Accounts
{
    /// <summary>
    /// Get account statement details
    /// </summary>
    [ApiFilter]
    public class ApiAccountStatementDetailsController : ApiController
    {
        /// <summary>
        /// test
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="period"></param>
        /// <param name="dateOfStart"></param>
        /// <param name="dateOfEnd"></param>
        /// <param name="loadedcount"></param>
        /// <param name="needtoload"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet]
        [Route("~/api/ApiAccountStatementDetails/{accountNumber}/{period:int?}")]
        [Route("~/api/ApiAccountStatementDetails/{accountNumber}/{period:int?}/{needtoload:int?}")]
        [Route("~/api/ApiAccountStatementDetails/{accountNumber}/{period:int?}/{loadedcount:int?}/{needtoload:int?}")]
        [Route("~/api/ApiAccountStatementDetails/{accountNumber}/{dateOfStart}")]
        [Route("~/api/ApiAccountStatementDetails/{accountNumber}/{dateOfStart}/{period:int?}/{needtoload:int?}")]
        [Route("~/api/ApiAccountStatementDetails/{accountNumber}/{dateOfStart}/{period:int?}/{loadedcount:int?}/{needtoload:int?}")]
        [Route("~/api/ApiAccountStatementDetails/{accountNumber}/{dateOfStart}/{dateOfEnd}")]
        [Route("~/api/ApiAccountStatementDetails/{accountNumber}/{dateOfStart}/{dateOfEnd}/{period:int?}/{needtoload:int?}")]
        [Route("~/api/ApiAccountStatementDetails/{accountNumber}/{dateOfStart}/{dateOfEnd}/{period:int?}/{loadedcount:int?}/{needtoload:int?}")]
        public IHttpActionResult GetInfo(string accountNumber, int period = -1, string dateOfStart = "", string dateOfEnd = "", int loadedcount = 0, int needtoload = 10)
        {
            var result = new AccountingModels.AccountReport
            {
                Success = false,
                InfoBlock = new InfoBlock()
                {
                    Code = ApiErrors.ErrorCodeState.EmptyResult,
                    DeveloperMessage = "Empty result. Method started.",
                    UserMessage = String.Empty
                },
                Info = new AccountingModels.AccountInfo(),
                Entries = new List<AccountingModels.AccountsEntry>()
            };

            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    DatePeriod periodEnum = DatePeriod.Today;
                    DateTime dtStart = new DateTime();
                    DateTime dtEnd = new DateTime();
                    bool ignorePeriod = false;

                    var currDate = DateTime.Now;
                    var startDay = new DateTime(currDate.Year, currDate.Month, currDate.Day, 0, 0, 0);
                    var endDay = new DateTime(currDate.Year, currDate.Month, currDate.Day, 23, 59, 59).AddDays(5);

                    if (!Guid.TryParse(accountNumber, out var accountGuid))
                        throw new Exception("Invalid account number");
                    
                        periodEnum = (DatePeriod)period;
                        switch (periodEnum)
                        {
                            case DatePeriod.Today: dtStart = startDay; dtEnd = endDay; break;
                            case DatePeriod.LastWeek: dtStart = startDay.AddDays(-7); dtEnd = endDay; break;
                            case DatePeriod.LastMonth: dtStart = startDay.AddMonths(-1); dtEnd = endDay; break;
                            case DatePeriod.Last3Months: dtStart = startDay.AddMonths(-3); dtEnd = endDay; break;
                            case DatePeriod.Last6Months: dtStart = startDay.AddMonths(-6); dtEnd = endDay; break;
                            default: dtStart = startDay; dtEnd = endDay; break;
                        }
                    
                    if (!String.IsNullOrEmpty(dateOfStart))
                        dtStart = DateTime.ParseExact(dateOfStart.Replace(".", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    if (!String.IsNullOrEmpty(dateOfEnd))
                    {
                        dtEnd = DateTime.ParseExact(dateOfEnd.Replace(".", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        dtEnd = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 23, 59, 59).AddDays(5);
                    }
                    
                    if (dtStart > dtEnd)
                        throw new Exception("Start date must be less than end date");

                    AccountsApiMethods accMeth = new AccountsApiMethods(ui);
                    result = accMeth.GetAccountReport(accountGuid, dtStart, dtEnd, loadedcount, needtoload);
                    if (result.Success) return Ok(result);
                    return Content(HttpStatusCode.BadRequest, result);
                }
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.CheckUserError, DeveloperMessage = "Data for user not correct", UserMessage = "Data for user not correct" };
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
    }
}