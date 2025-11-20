using System;
using System.Collections.Generic;

namespace TSG.Models.ServiceModels
{
    public class Conversion
    {
        public int Conversion_ConvId { get; set; }
        public int? Conversion_FromCcyId { get; set; }
        public int? Conversion_ToCcyId { get; set; }
        public decimal? Conversion_ConvAmt { get; set; }
        public DateTime? Conversion_ConvDate { get; set; }
        public decimal? Conversion_ExchangeRate { get; set; }

        // dbo.Conversion.fromCcyId -> dbo.Currency.ccyId (FK_Conversion_FromCurrency)
        public virtual Currency Currency { get; set; }
        // dbo.Conversion.toCcyId -> dbo.Currency.ccyId (FK_Conversion_ToCurrency)
        public virtual Currency Currency1 { get; set; }
    }
}
