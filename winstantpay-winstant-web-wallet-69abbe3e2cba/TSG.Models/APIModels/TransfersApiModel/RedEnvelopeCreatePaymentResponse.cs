using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.TransfersApiModel
{
    public class RedEnvelopeCreatePaymentResponse : StandartResponse
    {
        [JsonProperty("wpay_ids_list")] public List<string> WPayIdList { get; set; }
        [JsonProperty("currency_code_list")] public List<string> CurrencyList { get; set; }
    }
}