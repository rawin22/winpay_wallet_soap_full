using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [Merchants] This table contains information about all merchants in WinstantPayment
    /// INFO: Used with 3-d party services for integration with WinstantPay 
    /// </summary>
    [Dapper.Contrib.Extensions.Table("Merchants")]
    public class MerchantsDto
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public Guid UniqueID { get; set; }
        [MaxLength(500)] public string Name { get; set; }
        [MaxLength(500)] public string Address { get; set; }
        [MaxLength(100)] public string Phone { get; set; }
        [MaxLength(500)] public string UserId { get; set; }
        [MaxLength(500)] public string UserGuid { get; set; }
        public DateTime? CreationDate { get; set; }
        [MaxLength(500)] public string CallBackAddress { get; set; }
        public bool? IsSandBox { get; set; }
        [MaxLength(50)] public string PublicTokenKey { get; set; }
        [MaxLength(50)] public string PrivateToken { get; set; }
        [MaxLength(100)] public string MerchantAlias { get; set; }
    }
}