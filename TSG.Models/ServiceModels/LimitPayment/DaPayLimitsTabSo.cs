using System;
using System.ComponentModel.DataAnnotations;
using TSG.Models.App_LocalResources;
using TSG.Models.APIModels;

namespace TSG.Models.ServiceModels.LimitPayment
{
    public class DaPayLimitsTabSo : StandartResponse
    {
        [Required]
        [Display(Name = "CommonObjects_ID", ResourceType = typeof(GlobalModel))]
        public Guid DaPayLimitsTab_ID { get; set; } // uniqueidentifier, not null
        [Required]
        [Display(Name = "DaForPaymentLimitLog_ParentId", ResourceType = typeof(GlobalModel))]
        public Guid DaPayLimitsTab_ParentDaPayId { get; set; } // uniqueidentifier, not null
        [Required]
        [Display(Name = "QrForPaymentLimitTab_TypeOfLimit", ResourceType = typeof(GlobalModel))]
        public Guid DaPayLimitsTab_TypeOfLimit { get; set; } // uniqueidentifier, not null
        [Required]
        [Display(Name = "CommonObjects_Amount", ResourceType = typeof(GlobalModel))]
        public decimal DaPayLimitsTab_Amount { get; set; } // decimal(35,8), not null
        public DateTime? DaPayLimitsTab_ExpireDate { get; set; } // datetime, null
        public bool DaPayLimitsTab_IsDeleted { get; set; } // bit, not null
        [Required]
        [Display(Name = "CommonObjects_ID", ResourceType = typeof(GlobalModel))]
        public Guid DaPayLimitsTab_UserId { get; set; } // uniqueidentifier, not null
        [Required]
        [Display(Name = "CommonObjects_ID", ResourceType = typeof(GlobalModel))]
        public string DaPayLimitsTab_WPayId { get; set; }

        // dbo.da_PayLimitsTab.ParentDaPayId -> dbo.da_PayLimits.ID (FK_DaPaymentLimit_Tab_DaPayLimits)
        //public virtual DaPayLimitsSo DaPayLimitsTab_DaPayLimit { get; set; }
        // dbo.da_PayLimitsTab.TypeOfLimit -> dbo.da_PayLimitsType.ID (FK_DaPaymentLimit_Tab_DaPaymentLimits)
        public virtual DaPayLimitsTypeSo DaPayLimitsTab_DaPayLimitsType { get; set; }
    }
}
