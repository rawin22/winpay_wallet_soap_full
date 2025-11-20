using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [WelcomeMessage] Table contains welcome message for users in WinstantPay system
    /// </summary>
    [Dapper.Contrib.Extensions.Table("WelcomeMessage")]
    public class WelcomeMessageDto
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int wlcMsgId { get; set; }

        [MaxLength] public string wlcMsgText { get; set; }
        public bool? isDefault { get; set; }
        [MaxLength] public string wlcMsgTextRu { get; set; }
        [MaxLength] public string wlcMsgTextFr { get; set; }
        [MaxLength] public string wlcMsgTextPh { get; set; }
        [MaxLength] public string wlcMsgTextTh { get; set; }
        [MaxLength] public string wlcMsgTextAe { get; set; }
        [MaxLength] public string wlcMsgTextKh { get; set; }
        [MaxLength] public string wlcMsgTextCn { get; set; }
    }
}