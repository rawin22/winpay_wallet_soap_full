using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tsg.UI.Main.Models
{
    public class UserModel
    {
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Display(Name = "UserPassword")]
        public string UserPassword { get; set; }
        
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Display(Name = "LastName")]
        public string LastName { get; set; }

        public bool DefWlcMsgAssigned { get; set; }
        public bool IsLocal { get; set; }
        public bool IsBlockLocal { get; set; }
        
        [Display(Name = "Last Login Date")]
        public DateTime LastLoginDate { get; set; }

        [Display(Name = "RoleId")]
        public int RoleId { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }

        [Display(Name = "RoleName")]
        public string RoleName { get; set; }

        public int WelcomeMessageId { get; set; }
        public string WelcomeMessageText { get; set; }
        public string UserMail { get; set; }

        public bool IsNeedToShowWm { get; set; }
        public override string ToString()
        {
            return "[Username=" + Username + "],[Last Login Date=" + LastLoginDate + "],[Role=" + Role + "]";
        }
    }
}