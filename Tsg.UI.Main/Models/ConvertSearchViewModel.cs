using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.Models.Security;

namespace Tsg.UI.Main.Models
{
    /// <summary>
    /// Convert and Exchange search view model
    /// </summary>
    public class ConvertSearchViewModel : BaseViewModel
    {        
        /// <summary>
        /// Convert and exchange method
        /// </summary>
        public ConvertSearchViewModel()
        {            
            this.Converts = new List<ConvertViewModel>();
            this.AvailableStatus = new List<SelectListItem>();
            this.AvailableSortby = new List<SelectListItem>();
            this.AvailableSortDirection = new List<SelectListItem>();
        }

        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// FX deal reference
        /// </summary>
        [Display(Name = "FXDealReference")]
        public string FXDealReference { get; set; }

        /// <summary>
        /// FX deal date type
        /// </summary>
        [Display(Name = "DateType")]
        public FXDealSearchDateType DateType { get; set; }

        /// <summary>
        /// Account number
        /// </summary>
        [Display(Name = "AccountNumber")]
        public string AccountNumber { get; set; }

        [Display(Name = "Amount Min.")]
        public decimal AmountMin { get; set; }

        [Display(Name = "Amount Max.")]
        public decimal AmountMax { get; set; }

        [Display(Name = "Date Min.")]
        [DataType(DataType.DateTime)]
        public DateTime DateMin { get; set; } = new DateTime(2000, 1, 1);

        [Display(Name = "Date Max.")]
        [DataType(DataType.DateTime)]
        public DateTime DateMax { get; set; } = new DateTime(2099, 1, 1);

        [Display(Name = "Sort By")]
        public string SortBy { get; set; }

        [Display(Name = "Sort Direction")]
        public SortDirection SortDirection { get; set; }

        [Display(Name = "Page Size")]
        public int PageSize { get; set; }

        [Display(Name = "Page Index")]
        public int PageIndex { get; set; }

        [Display(Name = "Record Count")]
        public int RecordCount { get; set; }

        [Display(Name = "Total Records")]
        public int TotalRecords { get; set; }

        public IList<ConvertViewModel> Converts { get; set; }
        public IList<SelectListItem> AvailableStatus { get; set; }
        public IList<SelectListItem> AvailableSortby { get; set; }
        public IList<SelectListItem> AvailableSortDirection { get; set; }

        /// <summary>
        /// Prepare FX deals list
        /// </summary>
        public void PrepareConverts()
        {
            var response = Service.FXDealSearch(100000000);
            _logger.Debug("response: " + JsonConvert.SerializeObject(response));
            if (!response.ServiceResponse.HasErrors)
            {
                foreach (FXDealSearchData data in response.FXDeals)
                {
                    ConvertViewModel convert = new ConvertViewModel()
                    {
                        FXDealId = new Guid(data.FXDealId),
                        FXDealReference = data.FXDealReference,
                        DealDate = data.DealDate,
                        FinalValueDate = data.FinalValueDate,
                        BuyAmount = data.BuyAmount,
                        BuyAmountTextBare = data.BuyAmountTextBare,
                        BuyCurrencyCode = data.BuyCurrencyCode,
                        BuyCurrencyScale = data.BuyCurrencyScale,
                        BookedTime = data.BookedTime,
                        BookedRate = data.BookedRate,
                        BookedRateTextWithCurrencyCodes = data.BookedRateTextWithCurrencyCodes,
                        BookedRateTextBare = data.BookedRateTextBare,                        
                        SellCurrencyCode = data.SellCurrencyCode,
                        SellAmount = data.SellAmount,
                        SellAmountTextBare = data.SellAmountTextBare,
                        SellCurrencyScale = data.SellCurrencyScale,
                        SellAmountTextWithCurrencyCode = data.SellAmountTextWithCurrencyCode,
                    };
                    this.Converts.Add(convert);                    
                }
            }
        }       

    }

    //public enum Status : int
    //{
    //    All = 0,
    //    Created = 1,
    //    Posted = 2,
    //    Voided = 3,
    //}

    //public enum SortDirection : int
    //{
    //    Ascending = 0,
    //    Descending = 1,
    //}
}