using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [AnonimMail] This table contains all sended mails for all form
    /// </summary>
    [Dapper.Contrib.Extensions.Table("AnonimMail")]
    public class AnonimMailDto
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(100)] public string MailFrom { get; set; }
        [MaxLength(500)] public string MailSubject { get; set; }
        [MaxLength(1000)] public string MailNotes { get; set; }
        [MaxLength(100)] public string UserIpInRequest { get; set; }
        public DateTime DateSending { get; set; }
        [MaxLength(100)] public string BrowserName { get; set; }
        [MaxLength(50)] public string BrowserVersion { get; set; }
        public bool? IsMobile { get; set; }
    }
}