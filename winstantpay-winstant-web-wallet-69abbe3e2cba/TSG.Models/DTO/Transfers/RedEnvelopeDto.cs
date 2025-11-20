using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.Transfers
{
    [Table("re_RedEnvelope")]
    public class RedEnvelopeDto
    {
        [Dapper.Contrib.Extensions.ExplicitKey]
        public Guid Id { get; set; } // uniqueidentifier, not null
        [MaxLength(50)]
        public string CurrencyCode { get; set; } // nvarchar(50), not null
        public decimal Amount { get; set; } // decimal(35,8), not null
        [MaxLength(500)]
        public string Note { get; set; } // nvarchar(500), null
        [MaxLength(500)]
        public string FilePath { get; set; } // nvarchar(500), null
        public bool? IsSuccessTransferToRedEnvelopeAcc { get; set; } // bit, null
        public bool? IsNeedToNotifyByEmail { get; set; } // bit, null
        public Guid? RedEnvelopePaymentId { get; set; } // uniqueidentifier, null
        public Guid? RecipientPaymentId { get; set; } // uniqueidentifier, null
        public bool? IsSuccessTransferToRecipient { get; set; } // bit, null
        public DateTime? DateTransferedToRedEnvelope { get; set; } // datetime, null
        public DateTime? DateTransferedToRecipient { get; set; } // datetime, null
        [MaxLength(150)]
        public string RecipientUserName { get; set; } // nvarchar(150), null
        [MaxLength(500)]
        public string RejectionNote { get; set; } // nvarchar(500), null
        [MaxLength(150)]
        public string WPayIdTo { get; set; } // nvarchar(500), null
        [MaxLength(150)]
        public string WPayIdFrom { get; set; } // nvarchar(500), null
    }
}