using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TSG.Models.App_LocalResources;

namespace TSG.Models.ServiceModels.Shop
{
    public class ShopOrderLog
    {
        [Required]
        [Display(Name = "CommonObjects_ID", ResourceType = typeof(GlobalModel))]
        [Key]
        public Guid ShopOrderLog_ID { get; set; } // uniqueidentifier, not null
        [Required]
        [Display(Name = "ShopOrdersItem_OrderId", ResourceType = typeof(GlobalModel))]
        public Guid ShopOrderLog_OrderID { get; set; } // uniqueidentifier, not null
        [Required]
        [Display(Name = "CommonObjects_Status", ResourceType = typeof(GlobalModel))]
        public int ShopOrderLog_Status { get; set; } // int, not null
        [Required]
        [Display(Name = "CommonObjects_Timestamp", ResourceType = typeof(GlobalModel))]
        public long ShopOrderLog_Timestamp { get; set; } // bigint, not null
        
        public string ShopOrderLog_Reason { get; set; } // string, not null

        // dbo.shop_OrderLog.OrderID -> dbo.shop_Orders.ID (FK_Order_Log_OrderID)
        public virtual ShopOrders ShopOrderLog_ShopOrder { get; set; }
    }
}
