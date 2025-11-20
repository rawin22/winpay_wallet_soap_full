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
    /// Payment search view model
    /// </summary>
    public class PayoutSearchViewModel : BaseViewModel
    {        
        /// <summary>
        /// constructor
        /// </summary>
        public PayoutSearchViewModel()
        {            
            this.Payments = new List<PayoutViewModel>();
            this.AvailableStatus = new List<SelectListItem>();
            this.AvailableSortby = new List<SelectListItem>();
            this.AvailableSortDirection = new List<SelectListItem>();
        }

        /// <summary>
        /// Searching payment reference
        /// </summary>
        [Display(Name = "Payment Reference")]
        public string PaymentReference { get; set; }

        /// <summary>
        /// Searching payment status 
        /// </summary>
        [Display(Name = "Status")]
        public PaymentStatus Status { get; set; }

        /// <summary>
        /// Searching customer name
        /// </summary>
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        /// <summary>
        /// Searching minimum amount
        /// </summary>
        [Display(Name = "Amount Min.")]
        public decimal AmountMin { get; set; }

        /// <summary>
        /// Searching maximum amount
        /// </summary>
        [Display(Name = "Amount Max.")]
        public decimal AmountMax { get; set; }

        /// <summary>
        /// Searching minimum value date
        /// </summary>
        [Display(Name = "Value Date Min.")]
        [DataType(DataType.DateTime)]
        public DateTime ValueDateMin { get; set; } = new DateTime(2000, 1, 1);

        /// <summary>
        /// Searching maximum value date
        /// </summary>
        [Display(Name = "Value Date Max.")]
        [DataType(DataType.DateTime)]
        public DateTime ValueDateMax { get; set; } = new DateTime(2099, 1, 1);

        /// <summary>
        /// Payment searching sorting by
        /// </summary>
        [Display(Name = "Sort By")]
        public string SortBy { get; set; }

        /// <summary>
        /// Payment search sorting direction
        /// </summary>
        [Display(Name = "Sort Direction")]
        public PaymentSortDirection SortDirection { get; set; }

        /// <summary>
        /// Payment search result page size
        /// </summary>
        [Display(Name = "Page Size")]
        public int PageSize { get; set; }

        /// <summary>
        /// Payment search result page index
        /// </summary>
        [Display(Name = "Page Index")]
        public int PageIndex { get; set; }

        /// <summary>
        /// Payment search result record count
        /// </summary>
        [Display(Name = "Record Count")]
        public int RecordCount { get; set; }

        /// <summary>
        /// Payment search result total records
        /// </summary>
        [Display(Name = "Total Records")]
        public int TotalRecords { get; set; }

        /// <summary>
        /// Payments search data
        /// </summary>
        public IList<PayoutViewModel> Payments { get; set; }

        /// <summary>
        /// Available search payment status
        /// </summary>
        public IList<SelectListItem> AvailableStatus { get; set; }

        /// <summary>
        /// Available payment search sort by
        /// </summary>
        public IList<SelectListItem> AvailableSortby { get; set; }

        /// <summary>
        /// Available payment search direction
        /// </summary>
        public IList<SelectListItem> AvailableSortDirection { get; set; }

        /// <summary>
        /// Prepare payment search result
        /// </summary>
        public void PreparePayouts()
        {
            var response = Service.PaymentSearch(100000000);

            if (!response.ServiceResponse.HasErrors)
            {
                // INFO: eWallet September bug edited --Payment History default is showing oldest transactions first 
                //foreach (InstantPaymentSearchData data in response.Payments.OrderBy(ob => ob.CreatedTime))
                var payments = response.Payments.OrderBy(p => p.PaymentStatusTypeName).OrderByDescending(p => p.PaymentReference).OrderByDescending(p => p.CreatedTime);

                foreach (PaymentSearchData data in payments)
                {
                    PayoutViewModel instantPayment = new PayoutViewModel()
                    {
                        PaymentId = new Guid(data.PaymentId),
                        PaymentReference = data.PaymentReference,
                        Status = data.PaymentStatusTypeName,
                        CustomerName = data.CustomerName,
                        Amount = data.Amount,
                        AmountCurrency = data.AmountCurrencyCode,
                        AmountTextWithCurrencyCode = data.AmountTextWithCurrencyCode,
                        BeneficiaryName = data.BeneficiaryName,
                        ValueDate = Convert.ToDateTime(data.ValueDate).Date,
                        CreatedTime = Convert.ToDateTime(data.CreatedTime),
                        SumittedTime = String.IsNullOrEmpty(data.SubmittedTime)? (DateTime?) null : Convert.ToDateTime(data.SubmittedTime)
                    };
                    this.Payments.Add(instantPayment);                    
                }
            }
        }       

    }


    /// <summary>
    /// Payment search status
    /// </summary>
    public enum PaymentStatus : int
    {
        /// <summary>
        /// All
        /// </summary>
        All = 0,

        /// <summary>
        /// Created
        /// </summary>
        Created = 1,

        /// <summary>
        /// Submitted
        /// </summary>
        Submitted = 2,

        /// <summary>
        /// Funds Approved
        /// </summary>
        FundsApproved = 3,

        /// <summary>
        /// Verified
        /// </summary>
        Verified = 4,

        /// <summary>
        /// Released
        /// </summary>
        Released = 5,

        /// <summary>
        /// Voided
        /// </summary>
        Voided = 6,
    }

    /// <summary>
    /// Payment sorting direction
    /// </summary>
    public enum PaymentSortDirection : int
    {
        /// <summary>
        /// Sorting payment search result in ascending order
        /// </summary>
        Ascending = 0,

        /// <summary>
        /// Sorting payment search result in descending order
        /// </summary>
        Descending = 1,
    }
}