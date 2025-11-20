using System;
using System.Collections.Generic;

namespace TSG.Models.ServiceModels
{
    public class AnonimMail
    {
        public int AnonimMail_ID { get; set; }
        public string AnonimMail_MailFrom { get; set; }
        public string AnonimMail_MailSubject { get; set; }
        public string AnonimMail_MailNotes { get; set; }
        public string AnonimMail_UserIpInRequest { get; set; }
        public DateTime AnonimMail_DateSending { get; set; }
        public string AnonimMail_BrowserName { get; set; }
        public string AnonimMail_BrowserVersion { get; set; }
        public bool? AnonimMail_IsMobile { get; set; }
    }
}
