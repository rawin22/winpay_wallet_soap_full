using System;
using TSG.Models.Enums;

namespace TSG.Models.ServiceModels.Transfers.Reports
{
    public class GetInboxListSo
    {
        public Guid GetInboxList_Id { get; set; } // uniqueidentifier, not null
        public string GetInboxList_TransferParent { get; set; } // nvarchar(500), not null
        public string GetInboxList_TransferRecipient { get; set; } // nvarchar(500), not null
        public DateTime GetInboxList_CreatedDate { get; set; } // datetime, not null
        public DateTime? GetInboxList_AcceptedDate { get; set; } // datetime, null
        public bool GetInboxList_IsKycCreated { get; set; } // bit, not null
        public Guid? GetInboxList_KycLinkId { get; set; } // bit, not null
        public TransfersSourceTypeEnum GetInboxList_SourceType { get; set; } // int, not null
        public string GetInboxList_Source { get; set; } // nvarchar(50), null
        public Guid? GetInboxList_LinkToSourceRow { get; set; } // uniqueidentifier, null
        public bool? GetInboxList_IsRejected { get; set; } // bit, null
        public DelegatedAuthorirySourceLimitationTypeEnum GetInboxList_TypeRecBySource { get; set; } // int, not null
        public string GetInboxList_TypeRecBySourceEnumString { get; set; } // int, not null
        public string GetInboxList_MediaNameByTransferToken { get; set; } // nvarchar(150), not null
        public TransferStatusesEnum GetInboxList_Status { get; set; } // int, not null
        public string GetInboxList_StatusEnumString { get; set; } // int, not null
        public string GetInboxList_Info { get; set; } // nvarchar(200), null
    }
}