using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [Role] This table contains information about all roles in WinstantPayment
    /// </summary>
    [Dapper.Contrib.Extensions.Table("Role")]
    public class RoleDto
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int roleId { get; set; }

        [MaxLength(50)] public string roleName { get; set; }
        [MaxLength(100)] public string roleDesc { get; set; }
    }
}