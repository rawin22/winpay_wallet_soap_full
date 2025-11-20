using System;
using System.ComponentModel.DataAnnotations;
using TSG.Models.App_LocalResources;

namespace TSG.Models.ServiceModels.LimitPayment
{
    public class DaPayLimitsTypeSo
    {
        [Display(Name = "CommonObjects_ID", ResourceType = typeof(GlobalModel))]
        public Guid DaPayLimitsType_ID { get; set; } // uniqueidentifier, not null
        [MaxLength(500)]
        [Display(Name = "PaymentLimit_NameOfPaymentLimit", ResourceType = typeof(GlobalModel))]
        public string DaPayLimitsType_NameOfPaymentLimit { get; set; } // nvarchar(500), null
        [MaxLength(100)]
        [Required]
        [Display(Name = "PaymentLimit_SysNameOfPaymentLimit", ResourceType = typeof(GlobalModel))]
        public string DaPayLimitsType_SysNameOfPaymentLimit { get; set; } // nvarchar(100), not null
        [Required]
        [Display(Name = "PaymentLimit_LimitType", ResourceType = typeof(GlobalModel))]
        public int DaPayLimitsType_LimitType { get; set; } // int, not null
        public bool DaPayLimitsType_IsDeleted { get; set; } // bit, not null
    }
}
