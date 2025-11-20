using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tsg.UI.Main.Models
{
    public class InstantPaymentReceiveViewModel
    {
        [Display(Name = "Payment Id")]
        public Guid InstantPaymentReceiveId;
        [Display(Name = "Alias")]
        public string Alias;
        [Display(Name = "Currency")]
        public string Currency;
        [Display(Name = "Amount")]
        public decimal Amount;
        [Display(Name = "Invoice")]
        public string Invoice;
        [Display(Name = "Memo")]
        public string Memo;
        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate;
    }
}