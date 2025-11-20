using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.LimitPayment
{
    [Table("da_PayLimits")]
    public class DaPayLimitsDto
    {
        [ExplicitKey]
        public Guid ID { get; set; } // uniqueidentifier, not null
        public DateTime CreationDate { get; set; } // datetime, not null
        [MaxLength]
        public string LimitCodeInitialization { get; set; } // nvarchar(max), not null
        [MaxLength]
        public string UserName { get; set; } // nvarchar(max), not null
        [MaxLength]
        public string UserData { get; set; } // nvarchar(max), not null
        public DateTime? LastModifiedDate { get; set; } // datetime, null
        public int StatusByLimit { get; set; } // int, not null
        public DateTime? DateOfExpire { get; set; } // datetime, null
        [MaxLength(5)]
        public string CcyCode { get; set; } // nvarchar(5), not null
        [MaxLength(150)]
        public string PinCode { get; set; } // nvarchar(150), not null
        public bool IsDeleted { get; set; } // bit, not null
        public bool IsTransfered { get; set; } // bit, not null
        public bool IsPinProtected { get; set; } // bit, not null
        public Guid SourceType { get; set; } // uniqueidentifier, not null
        public Guid SecretCode { get; set; } // uniqueidentifier, not null
        public string MediaName { get; set; } // string, not null, required
        public string WPayId { get; set; } // string, null
        public decimal RequiredPinAmount { get; set; } // decimal(35,8), not null
        public bool IsRestrictedToSelectedCurrency { get; set; } // bit, not null

    }
}
