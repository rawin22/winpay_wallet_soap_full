using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [TaskList] Table contains all user requested sended from menu Contact Us in WinstantPay system
    /// </summary>
    [Dapper.Contrib.Extensions.Table("TaskList")]
    public class TaskListDto
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(50)] public string Code { get; set; }
        public int MailId { get; set; }
        public int? Status { get; set; }
        public DateTime? LastDateEdit { get; set; }
    }
}