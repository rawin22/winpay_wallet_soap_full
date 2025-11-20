using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TSG.Models.Enums;

namespace Tsg.UI.Main.Models
{
    public class ProofDocModel
    {
        public int? ProofDocId { get; set; }
        public int BankCurrencyId { get; set; }
        public IList<SelectListItem> AvailableBankCurrencies { get; set; }

        [Display(Name = "Proof Document Name")]
        public string ProofDocName { get; set; }

        [Display(Name = "Proof Document Path")]
        public string ProofDocPath { get; set; }

        [Display(Name = "Payment Date")]
        public string PaymentDateString { get; set; }
        
        public DateTime PaymentDate { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [Display(Name = "Last four digits")]
        public string LastFourDigits { get; set; }

        [Display(Name = "Other Information")]
        public string OtherInfo { get; set; }

        public HttpPostedFileBase PostedFile { get; set; }
        public int DepositId { get; set; }
        [Required]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        public string FundingStatus { get; set; }
        public FundingStatus FundingEnumStatus { get; set; }
        public override string ToString()
        {
            return "[Proof Document Name=" + ProofDocName +
                "],[Proof Document Path=" + ProofDocPath +
                "],[Customer Name=" + CustomerName +
                "],[Bank Name=" + BankName +
                "],[Last four digits=" + LastFourDigits +
                "],[Other Information=" + OtherInfo + "]";
        }
    }
}