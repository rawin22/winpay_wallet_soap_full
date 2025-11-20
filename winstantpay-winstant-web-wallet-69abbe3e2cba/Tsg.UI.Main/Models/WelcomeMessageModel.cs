using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tsg.UI.Main.Models
{
    public class WelcomeMessageModel : ICloneable
    {
        [Display(Name = "Welcome Message Id")]
        public int WelcomeMessageId { get; set; }

        [Display(Name = "Welcome Message")]
        [AllowHtml]
        public string WelcomeMessage { get; set; }

        [AllowHtml]
        public string WelcomeMessageFr { get; set; }
        [AllowHtml]
        public string WelcomeMessageRu { get; set; }
        [AllowHtml]
        public string WelcomeMessagePh { get; set; }
        [AllowHtml]
        public string WelcomeMessageAe { get; set; }
        [AllowHtml]
        public string WelcomeMessageTh { get; set; }
        public string WelcomeMessageKh { get; set; }
        public string WelcomeMessageCn { get; set; }


        [Display(Name = "Is Default")]
        public bool IsDefault { get; set; }

        public string Username { get; set; }

        public override string ToString()
        {
            return "[Welcome Message Id=" + WelcomeMessageId +
                "],[Welcome Message=" + WelcomeMessage +
                "],[IsDefault=" + IsDefault + "]";
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}