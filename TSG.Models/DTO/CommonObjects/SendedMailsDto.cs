using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSG.Models.DTO
{
    // TODO: Needs to update in single model with AnonimMailDto model
    /// <summary>
    /// Table [SendedMails] This table contains information about sended mail inside system in WinstantPayment
    /// </summary>
    [Dapper.Contrib.Extensions.Table("SendedMails")]
    public class SendedMailsDto
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int DepID { get; set; }
        [MaxLength(500)] public string MailFrom { get; set; }
        [MaxLength] public string MailText { get; set; }
        [MaxLength] public string MailSubject { get; set; }
        [MaxLength] public byte[] MailAttachment { get; set; }
        [MaxLength(500)] public string SiteCategory { get; set; }
    }
}