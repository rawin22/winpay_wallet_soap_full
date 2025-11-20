using System;
using System.ComponentModel.DataAnnotations;

namespace TSG.Models.ServiceModels.LimitPayment
{
    public class DaPaymentLimitSourceTypeSo
    {
        public Guid DaPaymentLimitSourceType_ID { get; set; } // uniqueidentifier, not null
        [MaxLength(50)]
        public string DaPaymentLimitSourceType_SysName { get; set; } // nvarchar(50), not null
        public int DaPaymentLimitSourceType_EnumNumber { get; set; } // int, not null
        public string DaPaymentLimitSourceType_Label { get; set;  }  // string LabelName
        public bool DaPaymentLimitSourceType_IsAcceptableOnWeb { get; set; } // bit, not null
        public bool DaPaymentLimitSourceType_IsAcceptableOnMobDevice { get; set; } // bit, not null
        public bool DaPaymentLimitSourceType_IsDeleted { get; set; } // bit, not null
    }
}