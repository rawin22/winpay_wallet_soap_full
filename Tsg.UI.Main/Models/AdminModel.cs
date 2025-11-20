using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tsg.UI.Main.Models
{
    public class AdminModel
    {
        public int Id { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; }

        [Display(Name = "UserPassword")]
        public string UserPassword { get; set; }
        
        [Display(Name = "UserPassword")]
        public string UserRetPassword { get; set; }
        
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Display(Name = "Creation Date")]
        public DateTime CreationDate { get; set; }
        [Display(Name = "Activation Date")]
        public DateTime ActivationDate { get; set; }

        public int LinkStatus { get; set; }
        public string LinkText { get; set; }
        public string MailAddress { get; set; }

        public bool IsNeedResetPassword { get; set; }
    }
}