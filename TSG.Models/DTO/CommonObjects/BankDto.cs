using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [Bank] This table contains Bank information in WinstantPayment system
    /// </summary>
    [Dapper.Contrib.Extensions.Table("Bank")]
    public class BankDto
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int bankId { get; set; }

        [MaxLength(100)] public string bankName { get; set; }
        public int? bankCountry { get; set; }
        public bool? IsDeleted { get; set; }
    }
}