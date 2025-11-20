using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [Fundings] This table contains information about all funds by all payment systems in system
    /// </summary>
    [Table("Fundings")]
    public class FundingsDto
    {
        [Dapper.Contrib.Extensions.Key] public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
        [MaxLength(50)] public string Username { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        public Guid SourceType { get; set; }
        public int? LastActivityId { get; set; }
        public bool IsDeleted { get; set; }

        [Write(false)]
        public int StatusByFund { get; set; }
    }
}