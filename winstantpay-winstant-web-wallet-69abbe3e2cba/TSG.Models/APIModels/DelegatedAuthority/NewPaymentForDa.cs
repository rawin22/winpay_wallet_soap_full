using Newtonsoft.Json;

namespace TSG.Models.APIModels.DelegatedAuthority
{
    public class NewPaymentForDa
    {
        [JsonProperty("daCode")] public string DelegatedAuthorityCode { get; set; }
        [JsonProperty("ccy")] public string CurrencyCode { get; set; }
        [JsonProperty("reasonForPayment")] public string ReasonForPayment { get; set; }
        [JsonProperty("pin")] public string PinCode { get; set; }
        [JsonProperty("amount")] public decimal Amount { get; set; }
        [JsonProperty("merchant_alias")] public string MerchantAlias { get; set; }
    }
}