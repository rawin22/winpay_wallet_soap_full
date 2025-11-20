using System.ComponentModel.DataAnnotations.Schema;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [BankCurrency] This table linked Bank id, Currency Id and WiredInstruction Id
    /// </summary>
    [Dapper.Contrib.Extensions.Table("BankCurrency")]
    public class BankCurrencyDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int bankCcyId { get; set; }

        public int? bankId { get; set; }
        public int? ccyId { get; set; }
        public int? wireInstId { get; set; }
        public bool? IsDeleted { get; set; }
    }
}