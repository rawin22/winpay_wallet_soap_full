using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.Models.Helpers;
using Tsg.UI.Main.Models.Security;

namespace Tsg.UI.Main.Models
{
    public class ReferalLinks
    {
        public int Id { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public String LinkText { get; set; }
        public DateTime ExparedDate { get; set; }
        public string UserAlias { get; set; }
    }
}