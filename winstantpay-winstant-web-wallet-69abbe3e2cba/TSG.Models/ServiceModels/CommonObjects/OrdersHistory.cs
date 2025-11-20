using System;
using System.Collections.Generic;

namespace TSG.Models.ServiceModels
{
    public class OrdersHistory
    {
        public int OrdersHistory_ID { get; set; }
        public Guid OrdersHistory_TokenKey { get; set; }
        public DateTime OrdersHistory_DateOfChanging { get; set; }
        public int OrdersHistory_StatusState { get; set; }
        public string OrdersHistory_Comment { get; set; }
        public int OrdersHistory_MerchantId { get; set; }
        public string OrdersHistory_AliasForUser { get; set; }

        // dbo.OrdersHistory.TokenKey -> dbo.TokenKeysForOrders.CustomerOrderId (FK_OrdersHistory_TokenKeysForOrders)
        public virtual TokenKeysForOrders OrdersHistory_TokenKeysForOrder { get; set; }
        // dbo.OrdersHistory.StatusState -> dbo.EnumOfOrderStatuses.ID (FK_OrdersHistory_EnumOfOrderStatuses)
        public virtual EnumOfOrderStatuses OrdersHistory_EnumOfOrderStatus { get; set; }
        // dbo.OrdersHistory.MerchantId -> dbo.Merchants.ID (FK_OrdersHistory_Merchants)
        public virtual Merchants OrdersHistory_Merchant { get; set; }
    }
}
