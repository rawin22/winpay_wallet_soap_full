using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.LimitPayment
{
    [Table("da_PayLimitsLog")]
    public class DaPayLimitsLogDto
    {
        [ExplicitKey]
        public Guid ID { get; set; } // uniqueidentifier, not null
        public Guid DaPayParentId { get; set; } // uniqueidentifier, not null
        [MaxLength]
        public string Info { get; set; } // nvarchar(max), null

        [StringLength(50)]
        public string CurrencyCode { get; set; } // nvarchar(50), not null
        public string UserName { get; set; } // nvarchar(50), not null

        public decimal Amount { get; set; } // decimal(35,8), not null
        public DateTime CreateDate { get; set; } // datetime, not null
        public decimal AmountInLimitsCurrency { get; set; } // decimal(35,8), not null
        public string LimitsCurrencyCode { get; set; } // nvarchar(50), not null

    }
}
