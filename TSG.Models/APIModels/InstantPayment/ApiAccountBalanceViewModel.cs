using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TSG.Models.APIModels.InstantPayment
{
    
    public class ApiAccountBalanceViewModel
    {
        [Display(Name = "Account ID")]
        public Guid AccountId { get; set; }

        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Display(Name = "Currency")]
        public string Currency { get; set; }

        [Display(Name = "Balance")]
        public decimal Balance { get; set; }

        [Display(Name = "Active Holds")]
        public decimal ActiveHoldsTotal { get; set; }

        [Display(Name = "Available Balance")]
        public decimal BalanceAvailable { get; set; }

        [Display(Name = "Base Currency")]
        public string BaseCurrency { get; set; }

        [Display(Name = "Available Balance in Base")]
        public decimal BalanceAvailableBase { get; set; }
    }
}