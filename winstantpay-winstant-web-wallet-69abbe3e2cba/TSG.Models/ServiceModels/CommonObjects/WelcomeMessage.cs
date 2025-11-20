using System;
using System.Collections.Generic;

namespace TSG.Models.ServiceModels
{
    public class WelcomeMessage
    {
        public int WelcomeMessage_WlcMsgId { get; set; }
        public string WelcomeMessage_WlcMsgText { get; set; }
        public bool? WelcomeMessage_IsDefault { get; set; }
        public string WelcomeMessage_WlcMsgTextRu { get; set; }
        public string WelcomeMessage_WlcMsgTextFr { get; set; }
        public string WelcomeMessage_WlcMsgTextPh { get; set; }
        public string WelcomeMessage_WlcMsgTextTh { get; set; }
        public string WelcomeMessage_WlcMsgTextAe { get; set; }
        public string WelcomeMessage_WlcMsgTextKh { get; set; }
        public string WelcomeMessage_WlcMsgTextCn { get; set; }

        //// dbo.User.wlcMsgId -> dbo.WelcomeMessage.wlcMsgId (FK_User_WelcomeMessage)
        //public virtual List<User> Users { get; set; }
    }
}
