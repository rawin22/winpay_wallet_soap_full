using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tsg.UI.Main.Models
{
    public class ReferralModel
    {
        [Display(Name = "From")]
        public string From { get; set; }

        [Display(Name = "To")]
        public string To { get; set; }

        [Display(Name = "Message")]
        public string Message { get; set; }

        public bool IsError { get; set; }
    }
}