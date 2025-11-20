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
    public class CheckoutModel
    {
        public string MerchantName { get; set; }
        public string CustomerName { get; set; }
        public IList<SelectListItem> AliasesList { get; set; }
        public string Account { get; set; }
        public string Alias { get; set; }
        public Guid OrderToken { get; set; }
        public string ReturnUrl { get; set; }
        public IList<SelectListItem> AvailableAccounts { get; set; }
    }
}