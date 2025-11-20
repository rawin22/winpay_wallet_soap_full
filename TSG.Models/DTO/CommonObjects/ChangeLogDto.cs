using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [ChangeLog] This table contains ui change information about new features in system
    /// </summary>
    [Dapper.Contrib.Extensions.Table("ChangeLog")]
    public class ChangeLogDto
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(500)] public string LogTitle { get; set; }
        [MaxLength] public string LogInfo { get; set; }
        [MaxLength(50)] public string VersionInfo { get; set; }
        public bool IsUiVersion { get; set; }
    }
}