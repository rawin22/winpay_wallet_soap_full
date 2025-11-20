using System;

namespace TSG.Models.APIModels.DelegatedAuthority
{
    public class CheckingLimitations
    {
        public Guid LimitId { get; set; }

        public Guid LimitTypeGuid { get; set; }

        public int LimitType { get; set; }

        public decimal AmountByLimitation { get; set; }
        public bool IsDeletedLimitation { get; set; }
        public bool IsSysLimitationDeleted { get; set; }

        public string CurrencyCode { get; set; }
    }
}