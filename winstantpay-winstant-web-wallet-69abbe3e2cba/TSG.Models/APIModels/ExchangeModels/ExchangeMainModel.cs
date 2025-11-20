using System.Collections.Generic;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.ExchangeModels
{
    public class ExchangeMainModel : StandartResponse
    {
        /// <summary>
        /// List of currencies
        /// </summary>
        [JsonProperty("list_of_currencies")] public List<CurrencyModel> ListOfCurrencies { get; set; } = new List<CurrencyModel>();
    }
}