using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO
{
    public class AddFunds_PipitDto
    {
        [System.ComponentModel.DataAnnotations.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(500)]
        public string BarCode { get; set; }
        [MaxLength(500)]
        public string Reference { get; set; }
        [MaxLength(500)]
        public string VendorReference { get; set; }
        public decimal OrderValue { get; set; }
        public decimal TotalValue { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        [MaxLength(500)]
        public string Status { get; set; }
        public Guid ParentId { get; set; }

        [MaxLength(500)]
        public string Alias { get; set; } 
        public DateTime? PaymentDate { get; set; } 

        [Write(false)]
        public string CurrencyCode { get; set; }

    }
}