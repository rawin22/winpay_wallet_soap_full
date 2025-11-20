using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.LimitPayment
{
    [Table("da_PaymentLimitSourceType")]
    public class DaPaymentLimitSourceTypeDto
    {
        [ExplicitKey]
        public Guid ID { get; set; } // uniqueidentifier, not null
        [MaxLength(50)]
        public string SysName { get; set; } // nvarchar(50), not null
        public int EnumNumber { get; set; } // int, not null
        public bool IsAcceptableOnWeb { get; set; } // bit, not null
        public bool IsAcceptableOnMobDevice { get; set; } // bit, not null
        public bool IsDeleted { get; set; } // bit, not null
    }
}