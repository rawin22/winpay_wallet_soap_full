using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tsg.UI.Main.Models
{
    public class ConvertViewModel
    {
        [Display(Name = "AmountCurrencyCode")]
        public Guid AmountCurrencyCode;
        [Display(Name = "Ask")]
        public decimal Ask;
        [Display(Name = "Bid")]
        public decimal Bid;
        [Display(Name = "BookedForCustomerId")]
        public Guid BookedForCustomerId;
        [Display(Name = "BookedForCustomerName")]
        public string BookedForCustomerName;
        [Display(Name = "BookedRate")]
        public decimal BookedRate;
        [Display(Name = "BookedRateScale")]
        public int BookedRateScale;
        [Display(Name = "BookedRateTextBare")]
        public string BookedRateTextBare;
        [Display(Name = "BookedRateTextWithCurrencyCodes")]
        public string BookedRateTextWithCurrencyCodes;
        [Display(Name = "BookedTime")]
        public string BookedTime;
        [Display(Name = "BranchCode")]
        public string BranchCode;
        [Display(Name = "BranchName")]
        public string BranchName;
        [Display(Name = "BuyAmount")]
        public decimal BuyAmount;
        [Display(Name = "BuyAmountTextBare")]
        public string BuyAmountTextBare;
        [Display(Name = "BuyAmountTextWithCurrencyCode")]
        public string BuyAmountTextWithCurrencyCode;
        [Display(Name = "BuyCurrencyAsk")]
        public decimal BuyCurrencyAsk;
        [Display(Name = "BuyCurrencyBid")]
        public decimal BuyCurrencyBid;
        [Display(Name = "BuyCurrencyCode")]
        public string BuyCurrencyCode;
        [Display(Name = "BuyCurrencyScale")]
        public int BuyCurrencyScale;
        [Display(Name = "CoverRate")]
        public decimal CoverRate;
        [Display(Name = "CoverRateTextBare")]
        public string CoverRateTextBare;
        [Display(Name = "CoverRateTextWithCurrencyCodes")]
        public string CoverRateTextWithCurrencyCodes;
        [Display(Name = "DealBaseCurrencyCode")]
        public string DealBaseCurrencyCode;
        [Display(Name = "DealCounterCurrencyCode")]
        public string DealCounterCurrencyCode;
        [Display(Name = "DealDate")]
        public string DealDate;
        [Display(Name = "FXDealId")]
        public Guid FXDealId;
        [Display(Name = "FXDealQuoteId")]
        public Guid FXDealQuoteId;
        [Display(Name = "FXDealReference")]
        public string FXDealReference;
        [Display(Name = "FXDealTypeId")]
        public int FXDealTypeId;
        [Display(Name = "FXDealTypeName")]
        public string FXDealTypeName;
        [Display(Name = "FinalForwardRate")]
        public decimal FinalForwardRate;
        [Display(Name = "FinalForwardRateTextBare")]
        public string FinalForwardRateTextBare;
        [Display(Name = "FinalForwardRateTextWithCurrencyCodes")]
        public string FinalForwardRateTextWithCurrencyCodes;
        [Display(Name = "FinalSpotRate")]
        public decimal FinalSpotRate;
        [DataType(DataType.Date)]
        [Display(Name = "FinalValueDate")]
        public string FinalValueDate;
        [Display(Name = "ForwardPoints")]
        public decimal ForwardPoints;
        [Display(Name = "IsDeleted")]
        public bool IsDeleted;
        [Display(Name = "MarketForwardRate")]
        public decimal MarketForwardRate;
        [Display(Name = "MarketForwardRateTextBare")]
        public string MarketForwardRateTextBare;
        [Display(Name = "MarketForwardRateTextWithCurrencyCodes")]
        public string MarketForwardRateTextWithCurrencyCodes;
        [Display(Name = "MarketSpotRate")]
        public decimal MarketSpotRate;
        [Display(Name = "MarketSpotRateTextBare")]
        public string MarketSpotRateTextBare;
        [Display(Name = "MarketSpotRateTextWithCurrencyCodes")]
        public string MarketSpotRateTextWithCurrencyCodes;
        [Display(Name = "ProviderQuoteId")]
        public string ProviderQuoteId;
        [Display(Name = "QuoteProviderName")]
        public string QuoteProviderName;
        [Display(Name = "RateFeedProviderId")]
        public string RateFeedProviderId;
        [Display(Name = "RateFormat")]
        public string RateFormat;
        [Display(Name = "SellAmount")]
        public decimal SellAmount;
        [Display(Name = "SellAmountTextBare")]
        public string SellAmountTextBare;
        [Display(Name = "SellAmountTextWithCurrencyCode")]
        public string SellAmountTextWithCurrencyCode;
        [Display(Name = "SellCurrencyAsk")]
        public decimal SellCurrencyAsk;
        [Display(Name = "SellCurrencyBid")]
        public decimal SellCurrencyBid;
        [Display(Name = "SellCurrencyCode")]
        public string SellCurrencyCode;
        [Display(Name = "SellCurrencyScale")]
        public int SellCurrencyScale;
        [DataType(DataType.Date)]
        [Display(Name = "SpotValueDate")]
        public string SpotValueDate;
        [DataType(DataType.Date)]
        [Display(Name = "WindowOpenDate")]
        public string WindowOpenDate;
    }
}