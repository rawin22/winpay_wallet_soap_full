using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TSG.Models.App_LocalResources;

namespace TSG.Models.ServiceModels.Shop
{
    public class ShopOrders
    {
        public ShopOrders()
        {
            this.ShopOrders_ShopOrderItems = new List<ShopOrderItems>();
            this.ShopOrders_ShopOrderLogs = new List<ShopOrderLog>();
        }

        [Key]
        [Required]
        [Display(Name = "CommonObjects_ID", ResourceType = typeof(GlobalModel))]
        public Guid ShopOrders_ID { get; set; } // uniqueidentifier, not null
        [MaxLength(50)]
        [Required]
        [Display(Name = "ShopOrders_BuyerWpayId", ResourceType = typeof(GlobalModel))]
        public string ShopOrders_BuyerWpayId { get; set; } // nvarchar(50), not null
        
        public Guid? ShopOrders_LastLogItem { get; set; } // uniqueidentifier, not null
        
        public virtual ShopOrderLog ShopOrders_LastShopOrderLog { get; set; }

        // dbo.shop_OrderItems.OrderId -> dbo.shop_Orders.ID (FK_Order_Items_OrderID)
        public virtual List<ShopOrderItems> ShopOrders_ShopOrderItems { get; set; }
        // dbo.shop_OrderLog.OrderID -> dbo.shop_Orders.ID (FK_Order_Log_OrderID)
        public virtual List<ShopOrderLog> ShopOrders_ShopOrderLogs { get; set; }
        public virtual List<ShopPayment> ShopOrders_ShopPaymentByOrder { get; set; } = new List<ShopPayment>();
        public int ShopOrders_OrderCounter { get; set; }
        public DateTime ShopOrders_CreateDate { get; set; }
    }
}
