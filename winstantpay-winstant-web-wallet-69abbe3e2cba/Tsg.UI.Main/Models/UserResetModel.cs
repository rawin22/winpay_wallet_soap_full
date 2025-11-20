using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Security;
using Tsg.Business.Model.Classes;
using Tsg.UI.Main.Models.Security;
using System.Security.Cryptography;
using Tsg.UI.Main.Repository;
using System.Text;
using Tsg.UI.Main.Models.Attributes;

namespace Tsg.UI.Main.Models
{
    public class UserResetModel
    {
        public string UserId { get; set; }

        [Required]
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime LastLogOnDate { get; set; }
        public string BranchOrCustomerName { get; set; }
        public string Email { get; set; } 
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }
        public bool? IsSuccessResForAction { get; set; }
        public string ActionResultText { get; set; }
        public List<UserResetModel> UserList { get; set; }
     }
}