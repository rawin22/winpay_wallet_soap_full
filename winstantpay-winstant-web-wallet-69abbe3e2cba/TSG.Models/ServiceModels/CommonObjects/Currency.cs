using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TSG.Models.ServiceModels
{
    public class Currency
    {
        [JsonProperty("currency_id")]public int Currency_CcyId { get; set; }
        [JsonProperty("currency_code")]public string Currency_CcyCode { get; set; }
        [JsonProperty("currency_name")]public string Currency_CcyName { get; set; }
        [JsonProperty("currency_symbol")]public string Currency_CcySymbol { get; set; }
    }
}
