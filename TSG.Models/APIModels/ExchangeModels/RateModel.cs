using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.ExchangeModels
{
    public class RateModel
    {
        [JsonProperty("rate_buy_amount")]public string BuyRateAmount { get; set; }
        [JsonProperty("rate_buy_currcode")]public string BuyCCy { get; set; }
        [JsonProperty("rate_sell_amount")]public string SellRateAmount { get; set; }
        [JsonProperty("rate_sell_currcode")]public string SellCCy { get; set; }
        [JsonProperty("rate_rate")]public string Rate { get; set; }
    }
}