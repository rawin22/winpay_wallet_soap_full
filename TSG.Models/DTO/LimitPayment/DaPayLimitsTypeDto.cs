using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.LimitPayment
{
    [Table("da_PayLimitsType")]
    public class DaPayLimitsTypeDto
    {
        [ExplicitKey]
        public Guid ID { get; set; } // uniqueidentifier, not null
        [MaxLength(500)]
        public string NameOfPaymentLimit { get; set; } // nvarchar(500), null
        [MaxLength(100)]
        public string SysNameOfPaymentLimit { get; set; } // nvarchar(100), not null
        [Write(false)]
        public int LimitType { get; set; } // int, not null
        public bool IsDeleted { get; set; } // bit, not null

    }
}
