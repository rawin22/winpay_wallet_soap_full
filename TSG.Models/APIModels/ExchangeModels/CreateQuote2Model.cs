using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.ExchangeModels
{
    public class ApiFXDealQuoteCreateRequest
    {
        [JsonProperty("from_account")] [Required] public string From { get; set; }
        [JsonProperty("to_account")] [Required] public string To { get; set; }
        [JsonProperty("currency_code")] [Required] public string PaymentCurrencyCode { get; set; }
        [JsonProperty("amount")] [Required] public string Amount { get; set; }
        [JsonProperty("deal_type")] [Required] public string DealType { get; set; }
        [JsonProperty("window_open_date")] public string WindowOpenDate { get; set; }
        [JsonProperty("final_value_date")] public string FinalValueDate { get; set; }
    }

    public class ApiCreateQuota2Response : StandartResponse
    {
        public ApiFXDealQuoteCreateRequest Data { get; set; }
    }
}