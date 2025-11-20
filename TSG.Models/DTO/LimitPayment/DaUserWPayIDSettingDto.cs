using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.LimitPayment
{
    [Table("da_UserWPayIDSettings")]
    public class DaUserWPayIDSettingDto
    {
        [ExplicitKey]
        public Guid ID { get; set; } // uniqueidentifier, not null
        public Guid UserId { get; set; } // uniqueidentifier, not null
        public string WPayId { get; set; } // string, null

        [MaxLength(5)]
        public string CcyCode { get; set; } // nvarchar(5), not null
        public bool IsPinRequired { get; set; } // bit, not null

        [MaxLength(150)]
        public string PinCode { get; set; } // nvarchar(150), not null

        public bool IsPinProtected { get; set; } // bit, not null

        public decimal ExceedingAmount { get; set; } // decimal(35,8), not null

        public bool IsRestrictedToSelectedCurrency { get; set; } // bit, not null

        public DateTime CreationDate { get; set; } // datetime, not null
        [MaxLength]
        public DateTime? LastModifiedDate { get; set; } // datetime, null
        public bool IsDeleted { get; set; } // bit, not null


    }
}
