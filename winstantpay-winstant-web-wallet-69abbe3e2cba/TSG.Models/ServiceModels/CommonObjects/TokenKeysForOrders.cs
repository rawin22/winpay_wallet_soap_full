using System;
using System.Collections.Generic;

namespace TSG.Models.ServiceModels
{
    public class TokenKeysForOrders
    {
        public Guid TokenKeysForOrders_CustomerOrderId { get; set; }
        public int TokenKeysForOrders_CustomerOrderStatus { get; set; }
        public int? TokenKeysForOrders_MerchantId { get; set; }
        public Guid? TokenKeysForOrders_CustomerId { get; set; }
        public string TokenKeysForOrders_Currency { get; set; }
        public decimal? TokenKeysForOrders_Quantity { get; set; }
        public string TokenKeysForOrders_MerchantWebSite { get; set; }
        public int? TokenKeysForOrders_CustomerAccounting { get; set; }
        public DateTime? TokenKeysForOrders_OrderExpiredDate { get; set; }
        public string TokenKeysForOrders_Stoke { get; set; }
        public Guid? TokenKeysForOrders_PaymentGuid { get; set; }
        public bool TokenKeysForOrders_IsSanboxOrder { get; set; }

        // dbo.TokenKeysForOrders.CustomerOrderStatus -> dbo.EnumOfOrderStatuses.ID (FK_TokenKeysForOrders_EnumOfOrderStatuses)
        public virtual EnumOfOrderStatuses TokenKeysForOrders_EnumOfOrderStatus { get; set; }
    }
}
