using System;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.LimitPayment
{
    [Table("da_PayLimitsTab")]
    public class DaPayLimitsTabDto
    {
        [ExplicitKey]
        public Guid ID { get; set; } // uniqueidentifier, not null
        public Guid ParentDaPayId { get; set; } // uniqueidentifier, not null
        public Guid TypeOfLimit { get; set; } // uniqueidentifier, not null
        public decimal Amount { get; set; } // decimal(35,8), not null
        public DateTime? ExpireDate { get; set; } // datetime, null
        public bool IsDeleted { get; set; } // bit, not null
        public Guid UserId { get; set; } // uniqueidentifier, not null
        public string WPayId { get; set; }

    }
}
