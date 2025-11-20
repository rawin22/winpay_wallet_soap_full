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
    public class TokenKeyOrderModel
    {
        public System.Guid CustomerOrderId { get; set; }
        public int CustomerOrderStatus { get; set; }
        public Nullable<int> MerchantId { get; set; }
        public Nullable<System.Guid> CustomerId { get; set; }
        public string Currency { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public string MerchantWebSite { get; set; }
        public string CustomerAccounting { get; set; }
        public Nullable<System.DateTime> OrderExpiredDate { get; set; }
        public string Stoke { get; set; }
    }
}