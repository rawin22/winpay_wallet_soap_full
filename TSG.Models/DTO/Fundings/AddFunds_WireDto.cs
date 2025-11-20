using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [AddFunds_Wire] This table contains wire transfer details (Proof Docs)
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("AddFunds_Wire")]
    public class AddFunds_WireDto
    {
        [System.ComponentModel.DataAnnotations.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int proofDocId { get; set; }
        [MaxLength(100)]
        public string custName { get; set; }
        [MaxLength(100)]
        public string bankName { get; set; }
        [MaxLength(50)]
        public string lastFourDigits { get; set; }
        [MaxLength(500)]
        public string other { get; set; }
        [MaxLength(100)]
        public string fileName { get; set; }
        [MaxLength(500)]
        public string filePath { get; set; }
        public int? bankCcyId { get; set; }
        public DateTime paymentDate { get; set; }
        public Guid ParentId { get; set; }

        [Write(false)]
        public decimal Amount { get; set; }
    }
}
