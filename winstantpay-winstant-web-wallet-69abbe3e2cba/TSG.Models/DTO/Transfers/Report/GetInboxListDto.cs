using System;

namespace TSG.Models.DTO.Transfers.Report
{
    public class GetInboxListDto
    {
        public Guid Id { get; set; } // uniqueidentifier, not null
        public string TransferParent { get; set; } // nvarchar(500), not null
        public string TransferRecipient { get; set; } // nvarchar(500), not null
        public DateTime CreatedDate { get; set; } // datetime, not null
        public DateTime? AcceptedDate { get; set; } // datetime, null
        public bool IsKycCreated { get; set; } // bit, not null
        public Guid? KycLinkId { get; set; } // bit, not null
        public int SourceType { get; set; } // int, not null
        public string Source { get; set; } // nvarchar(50), null
        public Guid? LinkToSourceRow { get; set; } // uniqueidentifier, null
        public bool? IsRejected { get; set; } // bit, null
        public int TypeRecBySource { get; set; } // int, not null
        public string MediaNameByTransferToken { get; set; } // nvarchar(150), not null
        public int Status { get; set; } // int, not null
        public string Info { get; set; } // nvarchar(200), null
    }
}