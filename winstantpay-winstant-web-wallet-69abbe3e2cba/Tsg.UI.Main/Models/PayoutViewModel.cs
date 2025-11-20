using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tsg.UI.Main.Models
{
    public class PayoutViewModel
    {
        /// <summary>
        /// Payment GUID
        /// </summary>
        [Display(Name = "Payment Id")]
        public Guid PaymentId;

        /// <summary>
        /// Payment reference
        /// </summary>
        [Display(Name = "Payment Reference")]
        public string PaymentReference;

        /// <summary>
        /// Payment status
        /// </summary>
        [Display(Name = "Status")]
        public string Status;
        /// <summary>
        /// Payment ordering customer name
        /// </summary>
        [Display(Name = "Customer Name")]
        public string CustomerName;

        /// <summary>
        /// Payment amount
        /// </summary>
        [Display(Name = "Amount")]
        public decimal Amount;

        /// <summary>
        /// Payment amount with currency
        /// </summary>
        [Display(Name = "AmountTextWithCurrencyCode")]
        public string AmountTextWithCurrencyCode;

        /// <summary>
        /// Payment amount currency
        /// </summary>
        [Display(Name = "Amount Currency")]
        public string AmountCurrency;

        /// <summary>
        /// Payment amount currency
        /// </summary>
        [Display(Name = "Beneficiary Name")]
        public string BeneficiaryName;

        /// <summary>
        /// Payment value date
        /// </summary>
        [Display(Name = "Value Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime ValueDate;
        
        /// <summary>
        /// Payment created date
        /// </summary>
        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedTime;

        /// <summary>
        /// Payment sumitted date
        /// </summary>
        [Display(Name = "Posted Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? SumittedTime;
    }
}