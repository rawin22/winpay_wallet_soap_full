using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [SharedPhoto] Table contains links for shared photo for InstantPayment
    /// </summary>
    [Dapper.Contrib.Extensions.Table("SharedPhoto")]
    public class SharedPhotoDto
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(100)] public string PaymentId { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public bool IsViewed { get; set; }
        [MaxLength(100)] public string SharedPhotoId { get; set; }
        [MaxLength(500)] public string Comment { get; set; }
    }
}