using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [FundChanges] This table contains information about history operations for all Funds Operation in system
    /// </summary>
    [Dapper.Contrib.Extensions.Table("FundChanges")]
    public class FundChangesDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int fndStatChangeId { get; set; }
        public DateTime? changedDate { get; set; }
        [MaxLength(50)]
        public string changedBy { get; set; }
        [MaxLength(1000)]
        public string notes { get; set; }
        public Guid FundingId { get; set; }
        public int? FundingToStatus { get; set; }
        public int? FundingFromStatus { get; set; }
    }
}