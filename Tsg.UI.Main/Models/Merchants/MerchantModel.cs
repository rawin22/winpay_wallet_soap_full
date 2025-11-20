using System.Collections.Generic;
using Tsg.UI.Main.Models.MessagesObjects;

namespace Tsg.UI.Main.Models.Merchants
{
    public class MerchantModel : AcMessageable
    {
        public int Id { get; set; }
        public string MerchantGuid { get; set; }
        public string MerchantName { get; set; }
        public string MerchantAlias { get; set; }
        public string MerchantApi { get; set; }
        public string MerchantPathForJs { get; set; }
        public string MerchantPhone { get; set; }
        public string MerchantAddress { get; set; }
        /// <summary>
        /// MerchantUserId is username
        /// </summary>
        public string MerchantUserId { get; set; }
        
        /// <summary>
        /// MerchantUserGuid is guid key in tsg system
        /// </summary>
        public string MerchantUserGuid { get; set; }

        /// <summary>
        /// MerchantIsSandbox - use for test merchant functional
        /// </summary>
        public bool MerchantIsSandbox { get; set; }
        
        /// <summary>
        /// MerchantPublicTokenKey - use for identify merchant
        /// </summary>
        public string MerchantPublicTokenKey { get; set; }

        /// <summary>
        /// MerchantPrivateTokenKey - use for identify merchant
        /// </summary>
        public string MerchantPrivateTokenKey { get; set; }
        
        public string MerchantWebSiteCallBackAddress { get; set; }

        public bool IntegrateJsIntoSite { get; set; } = false;

        public string MerchantParentAddress { get; set; }

        //public IList<MerchantModel> ListOfMerchants { get; set; }
        //public IList<SelectListItem> AccountAliases { get; set; }
    }

    public class MercantsList : AcMessageable
    {
        public IList<MerchantModel> ListOfMerchants { get; set;  }
    }

}