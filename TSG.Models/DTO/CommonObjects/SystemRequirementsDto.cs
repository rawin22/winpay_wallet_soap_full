using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [SystemRequirements] Table contains system properties for WinstantPay system
    /// </summary>
    [Dapper.Contrib.Extensions.Table("SystemRequirements")]
    public class SystemRequirementsDto
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(500)] public string PropertyName { get; set; }
        [MaxLength(500)] public string PropertyValue { get; set; }
    }
}