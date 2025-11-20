using System;
using System.ComponentModel.DataAnnotations;
using TSG.Models.App_LocalResources;

namespace TSG.Models.ServiceModels.LimitPayment
{
    public class DaPayLimitsLogSo
    {
        [Required]
        [Display(Name = "CommonObjects_ID", ResourceType = typeof(GlobalModel))]
        public Guid DaPayLimitsLog_ID { get; set; } // uniqueidentifier, not null
        [Required]
        [Display(Name = "DaForPaymentLimitLog_ParentId", ResourceType = typeof(GlobalModel))]
        public Guid DaPayLimitsLog_DaPayParentId { get; set; } // uniqueidentifier, not null
        [MaxLength]
        [Display(Name = "CommonObjects_Info", ResourceType = typeof(GlobalModel))]
        public string DaPayLimitsLog_Info { get; set; } // nvarchar(max), null

        [Required]
        [StringLength(50)]
        public string DaPayLimitsLog_CurrencyCode { get; set; } // nvarchar(50), not null
        public string DaPayLimitsLog_UserName { get; set; } // nvarchar(MAX), not null

        [Display(Name = "CommonObjects_Amount", ResourceType = typeof(GlobalModel))]
        public decimal DaPayLimitsLog_Amount { get; set; } // decimal(35,8), not null
        [Display(Name = "CommonObjects_CreationDate", ResourceType = typeof(GlobalModel))]
        public DateTime DaPayLimitsLog_CreateDate { get; set; } // datetime, not null
        public decimal DaPayLimitsLog_AmountInLimitsCurrency { get; set; } // decimal(35,8), not null
        [StringLength(50)]
        public string DaPayLimitsLog_LimitsCurrencyCode { get; set; } // nvarchar(50), not null

        public virtual DaPayLimitsSo DaPayLimitsLog_DaPayLimitSo { get; set; }

    }
}
