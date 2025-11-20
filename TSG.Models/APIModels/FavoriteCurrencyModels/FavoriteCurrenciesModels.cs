using System.Collections.Generic;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.FavoriteCurrencyModels
{
    public class FavoriteCurrenciesModels
    {
        public class FavoriteCurrencyList : StandartResponse
        {
            [JsonProperty("list_favorite_currencies")] public List<CurrencyViewModel> FavoriteCurrencies { get; set; }
        }
        
        public class FavoriteCurrencyUpdateResult : StandartResponse
        {
            [JsonProperty("is_favorite_currency")] public bool ? IsFavoriteCurrency { get; set; }

            //[JsonProperty("currency_Id")] public int CurrencyId { get; set; }
            [JsonProperty("currency_Id")] public string CurrencyCode { get; set; }
        }
    }
}