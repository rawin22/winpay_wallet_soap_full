using Newtonsoft.Json;

namespace TSG.Models.APIModels.ExchangeModels
{ 
    public class CurrencyModel : ApiCommonModel.BalanceData
    {
        [JsonProperty("currency_code")]public string CurrencyCode { get; set; }
    }
}