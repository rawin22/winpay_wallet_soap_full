using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tsg.UI.Main.Models
{
    public class InstantPaymentViewModel
    {
        [Display(Name = "Payment Id")]
        public Guid PaymentId;
        [Display(Name = "Payment Reference")]
        public string PaymentReference;
        [Display(Name = "Status")]
        public string Status;
        [Display(Name = "From Customer WpayId")]
        public string FromCustomerAlias;
        [Display(Name = "To Customer WpayId")]
        public string ToCustomerAlias;
        [Display(Name = "From Customer Name")]
        public string FromCustomerName;
        [Display(Name = "To Customer Name")]
        public string ToCustomerName;
        [Display(Name = "Amount")]
        public decimal Amount;
        [Display(Name = "Currency")]
        public string Currency;
        [Display(Name = "Value Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime ValueDate;
        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedTime;
        public bool IsIncome;
    }
}