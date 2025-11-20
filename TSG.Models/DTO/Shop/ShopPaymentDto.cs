using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.Shop
{
    [Table("shop_PaymentOrder")]
    public class ShopPaymentDto
    {
        [ExplicitKey]
        public Guid ID { get; set; } // uniqueidentifier, not null
        public Guid OrderId { get; set; } // uniqueidentifier, not null
        public Guid OrderItemId { get; set; } // uniqueidentifier, not null
        public Guid? PaymentId { get; set; } // uniqueidentifier, null
        [MaxLength(500)]
        public string PaymentNumber { get; set; } // string, null
        public int? Status { get; set; } // int, null
        [MaxLength(50)]
        public string Reason { get; set; } // nvarchar(50), null
        public DateTime PaymentAttemptDate { get; set; } // datetime, not null
    }
}