using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tsg.UI.Main.Models
{
    public class InstantPaymentReceiveMemoViewModel
    {
        [Display(Name = "Memo")]
        public string Memo;
        [Display(Name = "Name")]
        public string Name;
        [Display(Name = "Address")]
        public string Address;
        [Display(Name = "Email")]
        public string Email;
        public string KycId;
    }
}