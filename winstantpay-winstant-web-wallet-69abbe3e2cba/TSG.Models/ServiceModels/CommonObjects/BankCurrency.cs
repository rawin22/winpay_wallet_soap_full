using System;
using System.Collections.Generic;

namespace TSG.Models.ServiceModels
{
    public class BankCurrency
    {
        public int BankCurrency_BankCcyId { get; set; }
        public int? BankCurrency_BankId { get; set; }
        public int? BankCurrency_CcyId { get; set; }
        public int? BankCurrency_WireInstId { get; set; }
        public bool? BankCurrency_IsDeleted { get; set; }

        // dbo.BankCurrency.bankId -> dbo.Bank.bankId (FK_BankCurrency_Bank)
        public virtual Bank BankCurrency_Bank { get; set; }
        // dbo.BankCurrency.ccyId -> dbo.Currency.ccyId (FK_BankCurrency_Currency)
        public virtual Currency BankCurrency_Currency { get; set; }
        // dbo.BankCurrency.wireInstId -> dbo.WireInstruction.wireInstId (FK_BankCurrency_WireInstruction)
        public virtual WireInstruction BankCurrency_WireInstruction { get; set; }
    }
}
