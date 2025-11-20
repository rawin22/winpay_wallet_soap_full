using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.ExchangeModels
{
    public class ApiFxQuoteCreateRequest
    {
        public string BuyCCY { get; set; }
        public string CustomerId { get; set; }
        public string SellCCY { get; set; }
        public decimal Amount { get; set; }
        public string AmountCCY { get; set; }
        public string DealType { get; set; }
        public string WindowOpenDate { get; set; }
        public string FinalValueDate { get; set; }
        public bool IsForCurrencyCalculator { get; set; }
    }

    public class ApiFxQuote
    {
        public string QuoteId { get; set; }
        public string QuoteReference { get; set; }
        public string QuoteSequenceNumber { get; set; }
        public string CustomerAccountNumber { get; set; }
        public string DealType { get; set; }
        public string BuyAmount { get; set; }
        public string BuyCurrencyCode { get; set; }
        public string SellAmount { get; set; }
        public string SellCurrencyCode { get; set; }
        public string Rate { get; set; }
        public string RateFormat { get; set; }
        public string DealDate { get; set; }
        public string ValueDate { get; set; }
        public string QuoteTime { get; set; }
        public string ExpirationTime { get; set; }
        public bool IsForCurrencyCalculator { get; set; }
    }


    public class ApiFxQuoteCreateResponse : StandartResponse
    {
        public ApiFxQuote Quote { get; set; }
    }
}