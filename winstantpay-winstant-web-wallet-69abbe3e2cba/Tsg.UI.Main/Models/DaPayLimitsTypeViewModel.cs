using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TSG.Models.App_LocalResources;

namespace Tsg.UI.Main.Models
{
    public class DaPayLimitsTypeViewModel
    {
        [Display(Name = "CommonObjects_ID", ResourceType = typeof(GlobalModel))]
        public Guid ID { get; set; } // uniqueidentifier, not null
        [MaxLength(500)]
        [Display(Name = "PaymentLimit_NameOfPaymentLimit", ResourceType = typeof(GlobalModel))]
        public string NameOfPaymentLimit { get; set; } // nvarchar(500), null
        [MaxLength(100)]
        [Required]
        [Display(Name = "PaymentLimit_SysNameOfPaymentLimit", ResourceType = typeof(GlobalModel))]
        public string SysNameOfPaymentLimit { get; set; } // nvarchar(100), not null
        [Required]
        [Display(Name = "PaymentLimit_LimitType", ResourceType = typeof(GlobalModel))]
        public int LimitType { get; set; } // int, not null
        public bool IsDeleted { get; set; } // bit, not null

    }
}