using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [FundingSources] This table contains information about all payment systems in WinstantPayment such as [WinstantPay, PipIt and etc]
    /// </summary>
    [Table("FundingSources")]
    public class FundingSourcesDto
    {
        [System.ComponentModel.DataAnnotations.Key]
        public Guid ID { get; set; }
        [MaxLength(500)] public string SourceName { get; set; }
        [MaxLength(500)] public string DesignName { get; set; }
        public bool IsDeleted { get; set; }
    }
}