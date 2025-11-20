using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.ExchangeModels
{
    public class ApiFxQuoteBookRequest
    {
        public string QuoteId { get; set; }
    }

    public class apiFxQuoteBookFxDeal
    {
        public string FXDealId { get; set; }
        public string FXDealReference { get; set; }
        public string FXDealSequenceNumber { get; set; }

    }
    public class ApiFxQuoteBookResponse : StandartResponse
    {
        public apiFxQuoteBookFxDeal FxDeal { get; set; }
    }
}