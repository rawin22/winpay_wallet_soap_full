using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.ExchangeModels
{
    public class CreateQuoteModel
    {
        [JsonProperty("from_account")] [Required] public string From { get; set; }
        [JsonProperty("to_account")] [Required] public string To { get; set; }
        [JsonProperty("currency_code")] [Required] public string PaymentCurrencyCode { get; set; }
        [JsonProperty("amount")] [Required] public string Amount { get; set; }
    }
    public class CreateQuotaResponse : StandartResponse
    {
        [JsonProperty("create_quota")] public CreateQuoteModel Data { get; set; }
    }
}