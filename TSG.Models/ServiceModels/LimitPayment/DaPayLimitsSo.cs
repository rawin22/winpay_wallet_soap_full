using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Newtonsoft.Json;
using TSG.Models.App_LocalResources;
using TSG.Models.APIModels;
using TSG.Models.CustomValidation;

namespace TSG.Models.ServiceModels.LimitPayment
{
    public class DaPayLimitsSo : StandartResponse
    {
        [Required]
        [Display(Name = "CommonObjects_ID", ResourceType = typeof(GlobalModel))]
        public Guid DaPayLimits_ID { get; set; } // uniqueidentifier, not null
        [Required]
        [Display(Name = "CommonObjects_CreationDate", ResourceType = typeof(GlobalModel))]
        public DateTime DaPayLimits_CreationDate { get; set; } // datetime, not null

        [Required]
        [Display(Name = "QrForPaymentLimit_QrCode", ResourceType = typeof(GlobalModel))]
        [MaxLength(500)]
        public string DaPayLimits_LimitCodeInitialization { get; set; } // nvarchar(max), not null

        [MaxLength]
        [Required]
        [Display(Name = "QrForPaymentLimit_UserName", ResourceType = typeof(GlobalModel))]
        [JsonIgnore]
        public string DaPayLimits_UserName { get; set; } // nvarchar(max), not null
        [MaxLength]
        [Required]
        [Display(Name = "QrForPaymentLimit_UserData", ResourceType = typeof(GlobalModel))]
        [JsonIgnore]
        public string DaPayLimits_UserData { get; set; } // nvarchar(max), not null
        [Display(Name = "QrForPaymentLimit_LastModifiedDate", ResourceType = typeof(GlobalModel))]
        public DateTime? DaPayLimits_LastModifiedDate { get; set; } // datetime, null
        [Required]
        [Display(Name = "QrForPaymentLimit_StatusByLimit", ResourceType = typeof(GlobalModel))]
        public int DaPayLimits_StatusByLimit { get; set; } // int, not null
        [Display(Name = "CommonObjects_DateOfExpire", ResourceType = typeof(GlobalModel))]
        public DateTime? DaPayLimits_DateOfExpire { get; set; } // datetime, null
        public string DaPayLimits_DateOfExpireString { get; set; } // string->datetime, null
        public string DaPayLimits_TimeOfExpireString { get; set; } // string->datetime, null
        [Display(Name = "QrForPaymentLimit_SourceType", ResourceType = typeof(GlobalModel))]
        public Guid DaPayLimits_SourceType { get; set; } // uniqueidentifier, not null

        //[MaxLength(5)]
        //[Required(AllowEmptyStrings = false)]
        //[Display(Name = "CommonObjects_CurrencyCode", ResourceType = typeof(GlobalModel))]
        //public string DaPayLimits_CcyCode { get; set; } // nvarchar(5), not null

        [MaxLength(500)]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "CommonObjects_CurrencyCode", ResourceType = typeof(GlobalModel))]
        public string DaPayLimits_MediaName { get; set; } // nvarchar(500), not null

        
        //[MaxLength(150)]
        //[Display(Name = "QrForPaymentLimit_SecretCode", ResourceType = typeof(GlobalModel))]
        //public string DaPayLimits_PinCode { get; set; } // nvarchar(150), not null

        public bool DaPayLimits_IsDeleted { get; set; } // bool, not null
        public bool DaPayLimits_IsTransfered { get; set; } // bool, not null
        public bool DaPayLimits_IsPinProtected { get; set; } // bool, not null
        [JsonIgnore]
        public Guid DaPayLimits_SecretCode { get; set; } // uniqueidentifier, not null

        [MaxLength(500)]
        public string DaPayLimits_WPayId { get; set; } // nvarchar(500), not null
        //public decimal DaPayLimits_RequiredPinAmount { get; set; } // decimal(35,8), not null
        //public bool DaPayLimits_IsRestrictedToSelectedCurrency { get; set; } // bool, not null
        //public bool DaPayLimits_IsNoPin { get; set; } // bool, not null

        // dbo.da_PayLimits.SourceType -> dbo.da_PaymentLimitSourceType.ID (FK_da_PayLimits_da_PaymentLimitSourceType)
        //public virtual List<DaPayLimitsTypeSo> TotalListOfLimits { get; set; } = new List<DaPayLimitsTypeSo>();
        public virtual DaPaymentLimitSourceTypeSo DaPaymentLimitSourceType { get; set; } = new DaPaymentLimitSourceTypeSo();
        // dbo.da_PayLimitsLog.DaPayParentId -> dbo.da_PayLimits.ID (FK_DaPayLimit_log_DaPayLimits)
        public virtual List<DaPayLimitsLogSo> DaPayLimitsLogs { get; set; } = new List<DaPayLimitsLogSo>();
        // dbo.da_PayLimitsTab.ParentDaPayId -> dbo.da_PayLimits.ID (FK_DaPaymentLimit_Tab_DaPayLimits)
        //public virtual List<DaPayLimitsTabSo> DaPayLimitsTabs { get; set; } = new List<DaPayLimitsTabSo>();

        public virtual List<SelectListItem> ListOfBalances { get; set; } = new List<SelectListItem>();

        public IList<SelectListItem> AccountWPayIds { get; set; }        
    }
}
