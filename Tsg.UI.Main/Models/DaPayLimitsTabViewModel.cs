using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TSG.Models.App_LocalResources;
using TSG.Models.ServiceModels.LimitPayment;

namespace Tsg.UI.Main.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class DaPayLimitsTabViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "CommonObjects_ID", ResourceType = typeof(GlobalModel))]
        public Guid ID { get; set; } // uniqueidentifier, not null
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "DaForPaymentLimitLog_ParentId", ResourceType = typeof(GlobalModel))]
        public Guid ParentDaPayId { get; set; } // uniqueidentifier, not null
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "QrForPaymentLimitTab_TypeOfLimit", ResourceType = typeof(GlobalModel))]
        public Guid TypeOfLimit { get; set; } // uniqueidentifier, not null
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "CommonObjects_Amount", ResourceType = typeof(GlobalModel))]
        public decimal Amount { get; set; } // decimal(35,8), not null
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ExpireDate { get; set; } // datetime, null
        /// <summary>
        /// 
        /// </summary>
        public bool IsDeleted { get; set; } // bit, not null

        // dbo.da_PayLimitsTab.ParentDaPayId -> dbo.da_PayLimits.ID (FK_DaPaymentLimit_Tab_DaPayLimits)
        /// <summary>
        /// 
        /// </summary>
        public virtual DaPayLimitsSo DaPayLimitsTab_DaPayLimit { get; set; }
        // dbo.da_PayLimitsTab.TypeOfLimit -> dbo.da_PayLimitsType.ID (FK_DaPaymentLimit_Tab_DaPaymentLimits)
        /// <summary>
        /// 
        /// </summary>
        public virtual DaPayLimitsTypeSo DaPayLimitsTab_DaPayLimitsType { get; set; }

        public DaPayLimitsTabSo PrepareDaPayTabSo()
        {
            return new DaPayLimitsTabSo
            {
                DaPayLimitsTab_ID = ID,
                DaPayLimitsTab_ParentDaPayId = ParentDaPayId,
                DaPayLimitsTab_TypeOfLimit = TypeOfLimit,
                DaPayLimitsTab_Amount = Amount,
                DaPayLimitsTab_IsDeleted = IsDeleted
            };
        }
    }
}