using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    /// 
    /// </summary>
    [ApiFilter]
    public class ApiAccountController : ApiController
    {
        /// <summary>
        /// Get account report
        /// </summary>
        /// <returns>List of accounts currencies</returns>
        [HttpGet]
        [Route("~/api/ReportAccStatementStartInfo")]
        public IHttpActionResult Get()
        {
            var result = new ApiExchangeModel.GetAccountValue
            {
                Success = false,
                InfoBlock = new InfoBlock()
                {
                    Code = ApiErrors.ErrorCodeState.EmptyResult,
                    DeveloperMessage = "Empty result. Method started.",
                    UserMessage = String.Empty
                },
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

        
    }
}
