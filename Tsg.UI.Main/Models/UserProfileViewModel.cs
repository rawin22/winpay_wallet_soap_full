using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using TSG.Models.App_LocalResources;
using TSG.Models.ServiceModels.LimitPayment;

namespace Tsg.UI.Main.Models
{
    /// <summary>
    /// View model for User/Profile
    /// </summary>
    public class UserProfileViewModel : BaseViewModel
    {        
        public UserProfileViewModel()
        {
            this.Data = Service.GetUserData();
            this.BaseCurrency = this.Data.UserSettings.BaseCurrencyCode;
            this.DefaultWPayId = GetDefaultWPayId();
        }

        public UserSettingsGetResponse Data { get; set; }

        
        public void PrepareDetails()
        {
            this.Data = Service.GetUserData();
            this.DefaultWPayId = GetDefaultWPayId();
        }

        public DaUserWPayIDSettingSo PrepareDaUserWPayIDSettingSo()
        {
            return new DaUserWPayIDSettingSo
            {
                ID = DaUserWPayIDSettingID,
                UserID = UserID,
                WPayId = WPayId,
                IsPinRequired = IsPinRequired,
                PinCode = PinCode,
                ExceedingAmount = ExceedingAmount,
                CcyCode = CcyCode,
                CreationDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
            };
        }

        /// <summary>
        /// Retrieve customer WPayID (alias)
        /// </summary>
        /// <returns></returns>
        public string GetDefaultWPayId()
        {
            var response = Service.GetAccountAliases();
            var defaultAlias = response.Aliases.Where(a => a.IsDefault).FirstOrDefault();
            if (defaultAlias != null)
            {
                return defaultAlias.AccountAlias;
            }
            
            return string.Empty;
        }

        //[Required]
        public Guid UserID { get; set; } // uniqueidentifier, not null
        //[Required]
        public Guid DaUserWPayIDSettingID { get; set; } // uniqueidentifier, not null

        //[MaxLength(500)]
        public string WPayId { get; set; } // nvarchar(500), not null

        //[MaxLength(5)]
        //[Required(AllowEmptyStrings = false)]
        //[Display(Name = "CommonObjects_CurrencyCode", ResourceType = typeof(GlobalModel))]
        public string CcyCode { get; set; } // nvarchar(5), not null

        /// <summary>
        /// List of trusted key limit types (Transaction, Daily, Weekly, Monthly, Yearly, All Time)
        /// </summary>
        public virtual IList<DaPayLimitsTypeViewModel> LimitTypes { get; set; } = new List<DaPayLimitsTypeViewModel>();
        
        /// <summary>
        /// List of trusted key limit details
        /// </summary>
        public virtual IList<DaPayLimitsTabViewModel> LimitTabs { get; set; } = new List<DaPayLimitsTabViewModel>();
        //public IList<SelectListItem> WPayIds { get; set; }

        public string Message { get; set; } // nvarchar(500), not null
        public decimal ExceedingAmount { get; set; } // decimal(35,8), not null
        public bool IsRestrictedToSelectedCurrency { get; set; } // bool, not null
        public bool IsPinRequired { get; set; } // bool, not null
        //[Display(Name = "QrForPaymentLimit_SecretCode", ResourceType = typeof(GlobalModel))]
        public string PinCode { get; set; } // nvarchar(150), not null
        public bool IsCrypto { get; set; } // bool, not null
        /// <summary>
        /// Default WPayID
        /// </summary>
        public string DefaultWPayId { get; set; }
        /// <summary>
        /// Customer base currency
        /// </summary>
        public string BaseCurrency { get; set; }

    }
}