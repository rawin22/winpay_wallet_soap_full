using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [DepartamentMails] This table contains information about Email addresses for WinstantPay Departments
    /// </summary>
    [Dapper.Contrib.Extensions.Table("DepartmentMails")]
    public class DepartmentMailsDto
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(100)] public string DepartmentName { get; set; }
        [MaxLength(100)] public string DepartmentMail { get; set; }
        public int DepartmentWeight { get; set; }
    }
}