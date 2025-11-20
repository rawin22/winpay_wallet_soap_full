using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TSG.Models.App_LocalResources;

namespace TSG.Models.ServiceModels.Shop
{
    public class ShopOrderItems
    {
        [Required]
        [Display(Name = "CommonObjects_ID", ResourceType = typeof(GlobalModel))]
        [Key]
        public Guid ShopOrderItems_ID { get; set; } // uniqueidentifier, not null
        [Required]
        [Display(Name = "ShopOrdersItem_ProductId", ResourceType = typeof(GlobalModel))]
        public Guid ShopOrderItems_ProductId { get; set; } // uniqueidentifier, not null
        [Required]
        [Display(Name = "ShopOrdersItem_OrderId", ResourceType = typeof(GlobalModel))]
        public Guid ShopOrderItems_OrderId { get; set; } // uniqueidentifier, not null
        [Required]
        [Display(Name = "CommonObjects_Price", ResourceType = typeof(GlobalModel))]
        public decimal ShopOrderItems_Price { get; set; } // decimal(35,8), not null
        [MaxLength(3)]
        [Required]
        [Display(Name = "CommonObjects_CurrencyCode", ResourceType = typeof(GlobalModel))]
        public string ShopOrderItems_CurrencyCode { get; set; } // nvarchar(3), not null
        [Required]
        [Display(Name = "CommonObjects_Quantity", ResourceType = typeof(GlobalModel))]
        public int ShopOrderItems_Quantity { get; set; } // int, not null
        [MaxLength(128)]
        [Required]
        [Display(Name = "CommonObjects_Position", ResourceType = typeof(GlobalModel))]
        public string ShopOrderItems_Position { get; set; } // nvarchar(128), not null
        [Required]
        [Display(Name = "CommonObjects_Timestamp", ResourceType = typeof(GlobalModel))]
        public long ShopOrderItems_Timestamp { get; set; } // bigint, not null

        public virtual bool ShopOrderItems_IsPayed { get; set; }

        // dbo.shop_OrderItems.ProductId -> dbo.shop_Products.ID (FK_Order_Items_ProductID)
        public virtual ShopProducts ShopOrderItems_ShopProduct { get; set; }
        // dbo.shop_OrderItems.OrderId -> dbo.shop_Orders.ID (FK_Order_Items_OrderID)
        public virtual ShopOrders ShopOrderItems_ShopOrder { get; set; }
    }
}
