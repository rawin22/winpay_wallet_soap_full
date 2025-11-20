using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.Business.Model.TSGgpwsbeta;

namespace Tsg.UI.Main.Models
{
    public class ExchangeModel
    {
        [Display(Name = "From Account")]
        public string FromAccountId { get; set; }

        [Display(Name = "To Account")]
        public string ToAccountId { get; set; }

        [Display(Name = "QuoteType")]
        [Required(ErrorMessage = "Quote type must be selected")]
        public string QuoteType { get; set; }

        [Display(Name = "Buy")]
        public decimal BuyAmount { get; set; }

        [Display(Name = "SellCurrency")]
        public string SellCurrency { get; set; }        

        [Display(Name = "WindowOpenDate")]
        public string WindowOpenDate { get; set; }

        [Display(Name = "FinalValueDate")]
        public string FinalValueDate { get; set; }

        public int TotalSeconds { get; set; }

        public IEnumerable<SelectListItem> SelectedCurrency { get; set; }

        public FXDealQuoteCreateResponse FxDealQuoteResult { get; set; }
        public FXDealQuoteBookAndInstantDepositResponse FxDealQuoteBookResult { get; set; }

        public bool IsError { get; set; }
        public string Message { get; set; }
        public string CustomerBaseCurrencyCode { get; set; }
    }
}