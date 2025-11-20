using System;
using System.Collections.Generic;

namespace TSG.Models.ServiceModels
{
    public class SendedMails
    {
        public int SendedMails_ID { get; set; }
        public int SendedMails_DepID { get; set; }
        public string SendedMails_MailFrom { get; set; }
        public string SendedMails_MailText { get; set; }
        public string SendedMails_MailSubject { get; set; }
        public byte[] SendedMails_MailAttachment { get; set; }
        public string SendedMails_SiteCategory { get; set; }
    }
}
