using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [ReferalLinksList] This table contains information about all referal link address in WinstantPayment
    /// </summary>
    [Dapper.Contrib.Extensions.Table("ReferalLinksList")]
    public class ReferalLinksListDto
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(500)] public string LinkText { get; set; }
        public bool IsDefault { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? ExpiredDate { get; set; }
    }
}