using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.Reports.Accounting
{
    public class AccountingModels
    {
        public class AccountInfo
        {
            [JsonProperty("account_id")] public string AccountId { get; set; }
            [JsonProperty("account_number")] public string AccountNumber { get; set; }
            [JsonProperty("account_name")] public string AccountName { get; set; }
            [JsonProperty("account_ccy")] public string AccountCCY { get; set; }
            [JsonProperty("account_scale")] public int AccountCurrencyScale { get; set; }
            [JsonProperty("account_begining_balance")] public decimal BeginningBalance { get; set; }
            [JsonProperty("account_ending_balance")] public decimal EndingBalance { get; set; }
        }

        public class AccountsEntry
        {
            [JsonProperty("balance")] public decimal Balance { get; set; }
            [JsonProperty("amount")] public decimal Amount { get; set; }
            [JsonProperty("value_date")] public long ValueDate { get; set; }
            [JsonProperty("refence")] public string ItemReference { get; set; }
            [JsonProperty("bank_memo")] public string BankMemo { get; set; }
        }

        public class AccountReport : StandartResponse
        {
            [JsonProperty("account_info")] public AccountInfo Info { get; set; }
            [JsonProperty("list_of_entry")] public List<AccountsEntry> Entries { get; set; }
        }
    }
}