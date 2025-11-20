using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.InstantPayment
{
    [Table("InstantPaymentReceive")]
    public class InstantPaymentReceiveDto
    {
        [Dapper.Contrib.Extensions.ExplicitKey]
        public Guid Id { get; set; } // uniqueidentifier, not null
        public Guid TsgUserGuId { get; set; } // uniqueidentifier, not null
        [MaxLength(150)]
        public string Alias { get; set; } // nvarchar(150), null
        [MaxLength(5)]
        public string Currency { get; set; } // nvarchar(5), null
        public decimal Amount { get; set; } // nvarchar(150), null
        [MaxLength(150)]
        public string Invoice { get; set; } // nvarchar(150), null
        [MaxLength(4000)]
        public string Memo { get; set; } // nvarchar(4000), null
        public string ShortenedUrl { get; set; }
        public DateTime CreatedDate { get; set; } // datetime, not null
        public DateTime LastModifiedDate { get; set; } // datetime, not null
        public byte[] AttachedFile { get; set; }
        public string AttachedFileName { get; set; }
        public string AttachedFileExtension { get; set; }
        public string AttachedFileContentType { get; set; }
    }
}