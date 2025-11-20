using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tsg.UI.Main.Models
{
    public class UserTaskModel : UserMailModel
    {
        public string UserTaskNumber { get; set; }
        public string DepartmentName { get; set; }
        public string CompanyName { get; set; }
    }
}