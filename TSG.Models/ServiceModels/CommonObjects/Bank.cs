using System;
using System.Collections.Generic;

namespace TSG.Models.ServiceModels
{
    public class Bank
    {
        public int Bank_BankId { get; set; }
        public string Bank_BankName { get; set; }
        public int? Bank_BankCountry { get; set; }
        public bool? Bank_IsDeleted { get; set; }

        // dbo.Bank.bankId -> dbo.Country.ID (FK_Bank_Country)
        public virtual Country Bank_Country { get; set; }
    }
}
