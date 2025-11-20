using System;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.AutomaticExchange
{
    public class AutomaticExchangeCurrency
    {
        [JsonProperty("currency_id")]
        public Guid CurrencyId { get; set; }
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }
    }
}
