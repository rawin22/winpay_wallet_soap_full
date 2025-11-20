using System.Collections.Generic;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.AutomaticExchange
{
    public class AutomaticExchangeCurrencies : StandartResponse
    {
        [JsonProperty("user_liquids")]
        public List<AutomaticExchangeCurrency> UserLiquids { get; set; }
        [JsonProperty("avaliable_liquids")]
        public List<AutomaticExchangeCurrency> AvaliableLiquids { get; set; }
    }
}
