using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSG.Models.ServiceModels.AutomaticExchange
{
    public class AutomaticExchangeChekingModel
    {
        public string CurrencyCode { get; set; }
        public string PrintString { get; set; }
        public decimal CurrencyAmount { get; set; }
        public bool IsPaymentableCurrency { get; set; }
        public bool IsLiquidCurrency { get; set; }
        public decimal AmountForExchange { get; set; }
        public string Rate { get; set; }
        public string QuoteId { get; set; }
        
    }
}
