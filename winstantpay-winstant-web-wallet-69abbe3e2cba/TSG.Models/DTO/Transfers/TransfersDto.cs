using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.Transfers
{
    [Table("tr_Transfers")]
    public class TransfersDto
    {
        [Dapper.Contrib.Extensions.Key]
        public Guid Id { get; set; } // uniqueidentifier, not null
        [MaxLength(500)]
        public string TransferParent { get; set; } // nvarchar(500), not null
        [MaxLength(500)]
        public string TransferRecipient { get; set; } // nvarchar(500), not null
        [Write(false)]
        public DateTime CreatedDate { get; set; } // datetime, not null
        public DateTime? AcceptedDate { get; set; } // datetime, null
        public bool IsKycCreated { get; set; } // bit, not null
        public Guid? KycLinkId { get; set; } // uniqueidentifier, null
        public bool? IsRejected { get; set; } // bit, not null
        public int SourceType { get; set; } // bit, not null
        [MaxLength(50)]
        public string Source { get; set; } // nvarchar(50), null
        public Guid? LinkToSourceRow { get; set; } // uniqueidentifier, null
        [Write(false)]
        public string Info { get; set; } // nvarchar(500), null
    }

}