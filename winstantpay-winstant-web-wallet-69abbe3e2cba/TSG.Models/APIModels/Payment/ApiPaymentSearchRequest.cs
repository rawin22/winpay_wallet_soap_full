using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSG.Models.APIModels.Payment
{
    public class ApiPaymentSearchRequest
    {
        /// <summary>
        /// Page Index
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// Page Size
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Sort By
        /// </summary>
        public string SortBy { get; set; }
        /// <summary>
        /// Sort Direction
        /// </summary>
        public string SortDirection { get; set; }
        /// <summary>
        /// Search Options
        /// </summary>
        public apiPayoutSearchOptions SearchOptions { get; set; }

    }

    public class apiPayoutSearchOptions
    {
        /// <summary>
        /// Account Number
        /// </summary>
        public string AccountNumber { get; set; }
        /// <summary>
        /// Amount Max
        /// </summary>
        public decimal AmountMax { get; set; }
        /// <summary>
        /// Amount Min
        /// </summary>
        public string AmountMin { get; set; }
        /// <summary>
        /// Branch Name
        /// </summary>
        public string BranchName { get; set; }
        /// <summary>
        /// Currency Code
        /// </summary>
        public string CurrencyCode { get; set; }
        /// <summary>
        /// Customer Name
        /// </summary>
        public decimal CustomerName { get; set; }
        /// <summary>
        /// Payment Id
        /// </summary>
        public string PaymentId { get; set; }
        /// <summary>
        /// Payment Reference
        /// </summary>
        public string PaymentReference { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public decimal Status { get; set; }
        /// <summary>
        /// Value Date Max
        /// </summary>
        public string ValueDateMax { get; set; }
        /// <summary>
        /// Value Date Min
        /// </summary>
        public string ValueDateMin { get; set; }
    }
}
