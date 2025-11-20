using Newtonsoft.Json;

namespace TSG.Models.APIModels
{
    public class ApiCommonModel
    {
        public class BalanceData
        {
            [JsonProperty("account_id")] public string AccountId { get; set; }
            [JsonProperty("account_number")] public string AccountNumber { get; set; }
            [JsonProperty("currencyname")]public string CurrencyName { get; set; }
            [JsonProperty("amount")]public decimal Amount { get; set; }
            [JsonProperty("is_base_currency")]public bool IsBaseCurrency { get; set; }
        }
    }
}