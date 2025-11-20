using System;

namespace TSG.Models.APIModels.DelegatedAuthority
{
    public class JoinedLimitations
    {
        public Guid LimitId { get; set; }

        public Guid LimitTypeGuid { get; set; }

        public int LimitType { get; set; }

        public decimal MaxAmountByLimitation { get; set; }
        public decimal AmountByOrder { get; set; }
        public bool IsDeletedLimitation { get; set; }
        public bool IsSysLimitationDeleted { get; set; }

        public string CurrencyCode { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}