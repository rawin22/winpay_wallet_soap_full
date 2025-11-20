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
    public class DaUserWPayIDSettingSo : StandartResponse
    {
        [Required]
        [Display(Name = "CommonObjects_ID", ResourceType = typeof(GlobalModel))]
        public Guid ID { get; set; } // uniqueidentifier, not null
        [Required]
        [Display(Name = "CommonObjects_ID", ResourceType = typeof(GlobalModel))]
        public Guid UserID { get; set; } // uniqueidentifier, not null
        [MaxLength(500)]
        public string WPayId { get; set; } // nvarchar(500), not null


        [MaxLength(5)]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "CommonObjects_CurrencyCode", ResourceType = typeof(GlobalModel))]
        public string CcyCode { get; set; } // nvarchar(5), not null

        [MaxLength(150)]
        [Display(Name = "QrForPaymentLimit_SecretCode", ResourceType = typeof(GlobalModel))]
        public string PinCode { get; set; } // nvarchar(150), not null

        public bool IsPinProtected { get; set; } // bool, not null

        public decimal ExceedingAmount { get; set; } // decimal(35,8), not null
        public bool IsRestrictedToSelectedCurrency { get; set; } // bool, not null
        public bool IsPinRequired { get; set; } // bool, not null      

        [Required]
        [Display(Name = "CommonObjects_CreationDate", ResourceType = typeof(GlobalModel))]
        public DateTime CreationDate { get; set; } // datetime, not null

        [Display(Name = "QrForPaymentLimit_LastModifiedDate", ResourceType = typeof(GlobalModel))]
        public DateTime? LastModifiedDate { get; set; } // datetime, null
        public bool IsDeleted { get; set; } // bit, not null


    }
}
