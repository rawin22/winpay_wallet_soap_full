using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Tsg.UI.Main.Models
{
    public class BankFilterModel
    {
        public int? BankId { get; set; }
        public DateTime ? BeginDate { get; set; }
        public DateTime ? EndDate { get; set; }
        public string Currency { get; set; }
        public int? CurrencyId { get; set; }
        public decimal ? AmountMin { get; set; }   
        public decimal ? AmountMax { get; set; }
        public string NameOfSender { get; set; }
        public string NameOfSendingBank { get; set; }
        public string AccountNumber { get; set; }
        public string OtherInformation { get; set; }
    }
}