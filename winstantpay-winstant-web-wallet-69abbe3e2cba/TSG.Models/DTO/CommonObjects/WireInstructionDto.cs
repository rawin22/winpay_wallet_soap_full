using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [WireInstruction] Table contains wire instruction for deposit and linked with bank and currency
    /// </summary>
    [Dapper.Contrib.Extensions.Table("WireInstruction")]
    public class WireInstructionDto
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int wireInstId { get; set; }

        [MaxLength] public string wireInstText { get; set; }
    }
}