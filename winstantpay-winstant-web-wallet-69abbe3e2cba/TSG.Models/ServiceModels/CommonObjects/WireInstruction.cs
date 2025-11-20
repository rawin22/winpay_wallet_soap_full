using System;
using System.Collections.Generic;

namespace TSG.Models.ServiceModels
{
    public class WireInstruction
    {
        public int WireInstruction_WireInstId { get; set; }
        public string WireInstruction_WireInstText { get; set; }

        //// dbo.BankCurrency.wireInstId -> dbo.WireInstruction.wireInstId (FK_BankCurrency_WireInstruction)
        //public virtual List<BankCurrency> BankCurrencies { get; set; }
    }
}
