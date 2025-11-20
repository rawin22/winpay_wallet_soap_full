using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;

namespace TSG.Models.ServiceModels.Transfers.RedEnvelope
{
    public class RedEnvelopeSo
    {
        public Guid RedEnvelope_Id { get; set; } // uniqueidentifier, not null
        [MaxLength(50)]
        public string RedEnvelope_CurrencyCode { get; set; } // nvarchar(50), not null
        public decimal RedEnvelope_Amount { get; set; } // decimal(35,8), not null
        [MaxLength(500)]
        public string RedEnvelope_Note { get; set; } // nvarchar(500), null
        [MaxLength(500)]
        public string RedEnvelope_FilePath { get; set; } // nvarchar(500), null
        public bool? RedEnvelope_IsSuccessTransferToRedEnvelopeAcc { get; set; } // bit, null
        public bool? RedEnvelope_IsNeedToNotifyByEmail { get; set; } // bit, null
        public Guid? RedEnvelope_RedEnvelopePaymentId { get; set; } // uniqueidentifier, null
        public Guid? RedEnvelope_RecipientPaymentId { get; set; } // uniqueidentifier, null
        public bool? RedEnvelope_IsSuccessTransferToRecipient { get; set; } // bit, null
        public DateTime? RedEnvelope_DateTransferedToRedEnvelope { get; set; } // datetime, null
        public DateTime? RedEnvelope_DateTransferedToRecipient { get; set; } // datetime, null
        [MaxLength(150)]
        public string RedEnvelope_RecipientUserName { get; set; } // nvarchar(150), null
        [MaxLength(500)]
        public string RedEnvelope_RejectionNote { get; set; } // nvarchar(500), null
        [MaxLength(150)]
        public string RedEnvelope_WPayIdTo { get; set; } // nvarchar(150), null
        [MaxLength(150)]
        public string RedEnvelope_WPayIdFrom { get; set; } // nvarchar(150), null

        [JsonIgnore]
        public virtual TransfersSo RedEnvelope_TransferSo { get; set; }
    }
}