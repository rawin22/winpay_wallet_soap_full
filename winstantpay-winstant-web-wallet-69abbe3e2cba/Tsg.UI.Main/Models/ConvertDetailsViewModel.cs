using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using TSG.Models.ServiceModels.Shop;

namespace Tsg.UI.Main.Models
{
    /// <summary>
    /// Convert and exchange (FX) details view model
    /// </summary>
    public class ConvertDetailsViewModel : BaseViewModel
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ConvertDetailsViewModel()
        {
            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fxDealId">FX deal ID</param>
        public ConvertDetailsViewModel(Guid fxDealId)
        {
            FXDealId = fxDealId;            
        }

        [Display(Name = "Account Representative Id")]
        public Guid AccountRepresentativeId { get; set; }
        [Display(Name = "AmountCurrencyCode")]
        public Guid AmountCurrencyCode { get; set; }
        [Display(Name = "Ask")]
        public decimal Ask { get; set; }
        [Display(Name = "Bid")]
        public decimal Bid { get; set; }
        [Display(Name = "BookedByUserId")]
        public Guid BookedByUserId { get; set; }
        [Display(Name = "BookedByUserName")]
        public string BookedByUserName { get; set; }
        [Display(Name = "BookedForCustomerId")]
        public Guid BookedForCustomerId { get; set; }
        [Display(Name = "BookedForCustomerName")]
        public string BookedForCustomerName { get; set; }
        [Display(Name = "BookedRate")]
        public decimal BookedRate { get; set; }
        [Display(Name = "BookedRateScale")]
        public int BookedRateScale { get; set; }
        [Display(Name = "BookedRateTextBare")]
        public string BookedRateTextBare { get; set; }
        [Display(Name = "BookedRateTextWithCurrencyCodes")]
        public string BookedRateTextWithCurrencyCodes { get; set; }
        [Display(Name = "BookedTime")]
        public string BookedTime { get; set; }
        [Display(Name = "BranchCode")]
        public string BranchCode { get; set; }
        [Display(Name = "BranchName")]
        public string BranchName { get; set; }
        [Display(Name = "BuyAmount")]
        public decimal BuyAmount { get; set; }
        [Display(Name = "BuyAmountTextBare")]
        public string BuyAmountTextBare { get; set; }
        [Display(Name = "BuyAmountTextWithCurrencyCode")]
        public string BuyAmountTextWithCurrencyCode { get; set; }
        [Display(Name = "BuyCurrencyAsk")]
        public decimal BuyCurrencyAsk { get; set; }
        [Display(Name = "BuyCurrencyBid")]
        public decimal BuyCurrencyBid { get; set; }
        [Display(Name = "BuyCurrencyCode")]
        public string BuyCurrencyCode { get; set; }
        [Display(Name = "BuyCurrencyScale")]
        public int BuyCurrencyScale { get; set; }
        [Display(Name = "CoverRate")]
        public decimal CoverRate { get; set; }
        [Display(Name = "CoverRateTextBare")]
        public string CoverRateTextBare { get; set; }
        [Display(Name = "CoverRateTextWithCurrencyCodes")]
        public string CoverRateTextWithCurrencyCodes { get; set; }
        [Display(Name = "DealBaseCurrencyCode")]
        public string DealBaseCurrencyCode { get; set; }
        [Display(Name = "DealCounterCurrencyCode")]
        public string DealCounterCurrencyCode { get; set; }
        [Display(Name = "DealDate")]
        public string DealDate { get; set; }
        [Display(Name = "FXDealId")]
        public Guid FXDealId { get; set; }
        [Display(Name = "FXDealQuoteId")]
        public Guid FXDealQuoteId { get; set; }
        [Display(Name = "FXDealReference")]
        public string FXDealReference { get; set; }
        [Display(Name = "FXDealTypeId")]
        public int FXDealTypeId { get; set; }
        [Display(Name = "FXDealTypeName")]
        public string FXDealTypeName { get; set; }
        [Display(Name = "FeeTemplateFeeAmount")]
        public decimal FeeTemplateFeeAmount { get; set; }
        [Display(Name = "FeeTemplateFeeAmountCurrencyCode")]
        public string FeeTemplateFeeAmountCurrencyCode { get; set; }
        [Display(Name = "FeeTemplateFeeAmountCurrencyScale")]
        public int FeeTemplateFeeAmountCurrencyScale { get; set; }
        [Display(Name = "FeeTemplateFeeAmountTextBare")]
        public string FeeTemplateFeeAmountTextBare { get; set; }
        [Display(Name = "FeeTemplateFeeAmountTextWithCurrencyCode")]
        public string FeeTemplateFeeAmountTextWithCurrencyCode { get; set; }
        [Display(Name = "FinalForwardRate")]
        public decimal FinalForwardRate { get; set; }
        [Display(Name = "FinalForwardRateTextBare")]
        public string FinalForwardRateTextBare { get; set; }
        [Display(Name = "FinalForwardRateTextWithCurrencyCodes")]
        public string FinalForwardRateTextWithCurrencyCodes;
        [Display(Name = "FinalSpotRate")]
        public decimal FinalSpotRate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "FinalValueDate")]
        public string FinalValueDate { get; set; }
        [Display(Name = "ForwardPoints")]
        public decimal ForwardPoints { get; set; }
        [Display(Name = "IsDeleted")]
        public bool IsDeleted { get; set; }
        [Display(Name = "MarketForwardRate")]
        public decimal MarketForwardRate { get; set; }
        [Display(Name = "MarketForwardRateTextBare")]
        public string MarketForwardRateTextBare { get; set; }
        [Display(Name = "MarketForwardRateTextWithCurrencyCodes")]
        public string MarketForwardRateTextWithCurrencyCodes { get; set; }
        [Display(Name = "MarketSpotRate")]
        public decimal MarketSpotRate { get; set; }
        [Display(Name = "MarketSpotRateTextBare")]
        public string MarketSpotRateTextBare { get; set; }
        [Display(Name = "MarketSpotRateTextWithCurrencyCodes")]
        public string MarketSpotRateTextWithCurrencyCodes { get; set; }
        [Display(Name = "PricingTemplateFeeAmount")]
        public string PricingTemplateFeeAmount { get; set; }
        [Display(Name = "PricingTemplateFeeAmountCurrencyCode")]
        public string PricingTemplateFeeAmountCurrencyCode { get; set; }
        [Display(Name = "PricingTemplateFeeAmountCurrencyScale")]
        public decimal PricingTemplateFeeAmountCurrencyScale { get; set; }
        [Display(Name = "PricingTemplateFeeAmountTextBare")]
        public string PricingTemplateFeeAmountTextBare { get; set; }
        [Display(Name = "PricingTemplateFeeAmountTextWithCurrencyCode")]
        public string PricingTemplateFeeAmountTextWithCurrencyCode { get; set; }
        [Display(Name = "ProviderQuoteId")]
        public string ProviderQuoteId { get; set; }
        [Display(Name = "QuoteProviderName")]
        public string QuoteProviderName { get; set; }
        [Display(Name = "RateFeedProviderId")]
        public string RateFeedProviderId { get; set; }
        [Display(Name = "RateFormat")]
        public string RateFormat { get; set; }
        [Display(Name = "SellAmount")]
        public decimal SellAmount { get; set; }
        [Display(Name = "SellAmountTextBare")]
        public string SellAmountTextBare { get; set; }
        [Display(Name = "SellAmountTextWithCurrencyCode")]
        public string SellAmountTextWithCurrencyCode { get; set; }
        [Display(Name = "SellCurrencyAsk")]
        public decimal SellCurrencyAsk { get; set; }
        [Display(Name = "SellCurrencyBid")]
        public decimal SellCurrencyBid { get; set; }
        [Display(Name = "SellCurrencyCode")]
        public string SellCurrencyCode { get; set; }
        [Display(Name = "SellCurrencyScale")]
        public int SellCurrencyScale { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "SpotValueDate")]
        public string SpotValueDate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "WindowOpenDate")]
        public string WindowOpenDate { get; set; }

        public UserMailModel MailModel { get; set; }


        /// <summary>
        /// Prepare convert and exchange (FX) deal details
        /// </summary>
        public void PrepareDetails()
        {
            FXDealGetSingleResponse response = Service.GetFXDealDetails(this.FXDealId);

            if (!response.ServiceResponse.HasErrors)
            {
                
                this.FXDealId = new Guid(response.FXDeal.FXDealId);
                this.FXDealReference = response.FXDeal.FXDealReference;
                this.FXDealTypeName = response.FXDeal.FXDealTypeName;
                this.FeeTemplateFeeAmountTextWithCurrencyCode = response.FXDeal.FeeTemplateFeeAmountTextWithCurrencyCode;
                this.BuyAmount = response.FXDeal.BuyAmount;
                this.BuyAmountTextBare = response.FXDeal.BuyAmountTextBare;
                this.BuyAmountTextWithCurrencyCode = response.FXDeal.BuyAmountTextWithCurrencyCode;
                this.BuyCurrencyCode = response.FXDeal.BuyCurrencyCode;
                this.BuyCurrencyScale = response.FXDeal.BuyCurrencyScale;
                this.SellAmount = response.FXDeal.SellAmount;
                this.SellAmountTextBare = response.FXDeal.SellAmountTextBare;
                this.SellAmountTextWithCurrencyCode = response.FXDeal.SellAmountTextWithCurrencyCode;
                this.SellCurrencyCode = response.FXDeal.SellCurrencyCode;
                this.BookedRate = response.FXDeal.BookedRate;
                this.BookedRateTextWithCurrencyCodes = response.FXDeal.BookedRateTextWithCurrencyCodes;
                this.BookedByUserName = response.FXDeal.BookedByUserName;
                this.BookedForCustomerName = response.FXDeal.BookedForCustomerName;
                //this.DealDate = Convert.ToDateTime(response.FXDeal.DealDate);
                this.DealDate = response.FXDeal.DealDate;
                this.FinalValueDate = response.FXDeal.FinalValueDate;
                this.SpotValueDate = response.FXDeal.SpotValueDate;
                this.PricingTemplateFeeAmountTextWithCurrencyCode = response.FXDeal.PricingTemplateFeeAmountTextWithCurrencyCode;
                this.BranchName = response.FXDeal.BranchName;

            }
        }

        /// <summary>
        /// Prepare convert and exchange (FX) deal details
        /// </summary>
        public void PrepareDetailsByReference(string reference)
        {
            var SearchResponse = Service.FXDealSearchByReference(reference);

            if (!SearchResponse.ServiceResponse.HasErrors)
            {
                this.FXDealId = new Guid(SearchResponse.FXDeals[0].FXDealId);
                this.PrepareDetails();
            }
        }
    }
}