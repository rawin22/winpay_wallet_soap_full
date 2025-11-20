using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TSG.Models.Enums;

namespace TSG.Models.ServiceModels.Transfers
{
    public class TransfersSo
    {
        public Guid Transfers_Id { get; set; } // uniqueidentifier, not null
        [MaxLength(500)]
        public string Transfers_TransferParent { get; set; } // nvarchar(500), not null
        [MaxLength(500)]
        public string Transfers_TransferRecipient { get; set; } // nvarchar(500), not null
        public DateTime Transfers_CreatedDate { get; set; } // datetime, not null
        public DateTime? Transfers_AcceptedDate { get; set; } // datetime, null
        public bool Transfers_IsKycCreated { get; set; } // bit, not null
        public Guid? Transfers_KycLinkId { get; set; } // uniqueidentifier, null
        public bool? Transfers_IsRejected { get; set; } // bit, not null
        public TransfersSourceTypeEnum Transfers_SourceType { get; set; } // enum, not null
        [MaxLength(50)]
        public string Transfers_Source { get; set; } // nvarchar(50), null
        [MaxLength(50)]
        public string Transfers_Info { get; set; } // nvarchar(50), null
        public Guid? Transfers_LinkToSourceRow { get; set; } // uniqueidentifier, null

        public virtual bool Transfers_IsInput { get; set; }
        public string Transfers_StatusByTokenString { get; set; }
    }

    public class TransfersTokenSo : TransfersSo
    {
        public int DaPaymentLimitSourceType_EnumKey { get; set; }
        public string DaPaymentLimitSourceType_EnumName { get; set; }
        public string DaPayLimits_MediaName { get; set; }
    }

    public class HistorycalDataByTokenSo : TransfersTokenSo
    {
        public TransferedTokenStatusesEnum StatusByToken { get; set; }
    }

}