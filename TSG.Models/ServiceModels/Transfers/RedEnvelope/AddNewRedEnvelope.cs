using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace TSG.Models.ServiceModels.Transfers.RedEnvelope
{
    public class AddNewRedEnvelope
    {
        public string WpayIdFrom { get; set; }
        public string WpayIdTo { get; set; }
        public decimal Amount { get; set; }
        public string Memo { get; set; }
        public string CurrencyCode { get; set; }
        public bool IsKycCreated { get; set; }
        public Guid? KycId { get; set; }
        public string KycUserName { get; set; }
        public IList<SelectListItem> AccountAliases { get; set; }
        public IList<SelectListItem> AvailableCurrencies { get; set; }
        public IList<SelectListItem> PriorUsedAliases { get; set; }

        [JsonIgnore] public HttpPostedFileBase RedEnvelope_PostedFile { get; set; }

    }
}
