using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace TSG.Models.APIModels
{
    public class CurrencyViewModel
    {
        //[Display(Name = "Id")]
        //[JsonProperty("id")]
        //public int CurrencyId { get; set; }

        [Display(Name = "Code")]
        [JsonProperty("code")]
        public string CurrencyCode { get; set; }

        [Display(Name = "Name")]
        [JsonProperty("name")]
        public string CurrencyName { get; set; }

        [Display(Name = "Amount Scale")]
        [JsonProperty("amount_scale")]
        public int CurrencyAmountScale { get; set; }

        [Display(Name = "Rate Scale")]
        [JsonProperty("rate_scale")]
        public int CurrencyRateScale { get; set; }

        [Display(Name = "Symbol")]
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [Display(Name = "Cut off Time")]
        [JsonProperty("cut_off_time")]
        public string PaymentCutoffTime { get; set; }

        [Display(Name = "Days to Add")]
        [JsonProperty("day_to_add")]
        public int SettlementDaysToAdd { get; set; }
        
        [Display(Name="FavoriteCurrency")]
        [JsonProperty("is_favorite_currency")]
        public bool IsFavorite { get; set; }

    }
}