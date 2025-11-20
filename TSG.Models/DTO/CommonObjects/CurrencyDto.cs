using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [Currency] This table contains information about Currencies in system
    /// </summary>
    [Dapper.Contrib.Extensions.Table("Currency")]
    public class CurrencyDto
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ccyId { get; set; }

        [MaxLength(5)] public string ccyCode { get; set; }
        [MaxLength(50)] public string ccyName { get; set; }
        [MaxLength(50)] public string ccySymbol { get; set; }
    }
}