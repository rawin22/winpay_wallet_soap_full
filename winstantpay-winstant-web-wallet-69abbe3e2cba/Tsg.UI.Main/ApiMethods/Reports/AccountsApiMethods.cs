using System;
using System.Collections.Generic;
using System.Linq;
using TSG.Models.APIModels;
using TSG.Models.APIModels.Reports.Accounting;
using Tsg.UI.Main.Models.Security;

namespace Tsg.UI.Main.ApiMethods.Reports
{
    public class AccountsApiMethods : BaseApiMethods
    {
        public AccountsApiMethods(UserInfo ui) : base(ui)
        {
            _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public AccountingModels.AccountReport GetAccountReport(Guid accountId, DateTime startDate, DateTime endDate, int loadedCount, int needtoload)
        {
            var res = Service.GetAccontStatements(accountId, startDate, endDate);
            return new AccountingModels.AccountReport()
            {
                Success = !res.ServiceResponse.HasErrors,
                InfoBlock = new InfoBlock()
                {
                    Code = !res.ServiceResponse.HasErrors ? ApiErrors.ErrorCodeState.Success : ApiErrors.ErrorCodeState.UnspecifiedError,
                    DeveloperMessage = $"{res.ServiceResponse.Responses[0].Message} \r\n {res.ServiceResponse.Responses[0].MessageDetails}",
                    UserMessage = res.ServiceResponse.Responses[0].Message
                },
                Info = res.ServiceResponse.HasErrors ? new AccountingModels.AccountInfo() :
                    new AccountingModels.AccountInfo()
                    {
                        AccountId = res.AccountInfo.AccountId,
                        AccountCCY= res.AccountInfo.AccountCCY,
                        AccountName = res.AccountInfo.AccountName,
                        AccountCurrencyScale = res.AccountInfo.AccountCurrencyScale,
                        AccountNumber= res.AccountInfo.AccountNumber,
                        BeginningBalance = res.AccountInfo.BeginningBalance,
                        EndingBalance = res.AccountInfo.EndingBalance
                    },
                Entries = res.ServiceResponse.HasErrors ? new List<AccountingModels.AccountsEntry>() :
                    res.Entries.OrderByDescending(ob=>ob.ValueDate).Select(s=>new AccountingModels.AccountsEntry
                    {
                        Amount = (-1) * s.AmountDebit + s.AmountCredit,
                        Balance = res.AccountInfo.BeginningBalance + ((-1) * s.AmountDebit + s.AmountCredit),
                        BankMemo = s.BankMemo,
                        ItemReference = s.ItemReference,
                        ValueDate = Convert.ToDateTime(s.ValueDate).Ticks
                    }).Skip(loadedCount).Take(needtoload).ToList()
            };
        }
    }
}