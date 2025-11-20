using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSG.Models.ServiceModels.Shop
{
    public class ShopPayment
    {
        public Guid ShopPayment_ID { get; set; } // uniqueidentifier, not null
        public Guid ShopPayment_OrderId { get; set; } // uniqueidentifier, not null
        public Guid ShopPayment_OrderItemId { get; set; } // uniqueidentifier, not null
        public Guid? ShopPayment_PaymentId { get; set; } // uniqueidentifier, null
        public string ShopPayment_PaymentNumber { get; set; } // uniqueidentifier, null
        public int? ShopPayment_Status { get; set; } // int, null
        public string ShopPayment_Reason { get; set; } // nvarchar(50), null
        public DateTime ShopPayment_PaymentAttemptDate { get; set; } // datetime, not null
        public long ShopPayment_PaymentCounter { get; set; } // bigint, not null
    }
}
