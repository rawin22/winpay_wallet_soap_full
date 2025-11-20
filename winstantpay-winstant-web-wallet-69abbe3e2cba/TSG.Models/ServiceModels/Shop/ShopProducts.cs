using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using Newtonsoft.Json;
using TSG.Models.App_LocalResources;

namespace TSG.Models.ServiceModels.Shop
{
    public class ShopProducts
    {
        public ShopProducts()
        {
            this.ShopProducts_ShopOrderItems = new List<ShopOrderItems>();
            this.ShopProducts_ShopProductImages = new List<ShopProductImages>();
        }

        [Required]
        [Display(Name = "CommonObjects_ID", ResourceType = typeof(GlobalModel))]
        [Key]
        public Guid ShopProducts_ID { get; set; } // uniqueidentifier, not null
        [Required]
        [Display(Name = "ShopProduct_Parent", ResourceType = typeof(GlobalModel))]
        public Guid ShopProducts_ShopID { get; set; } // uniqueidentifier, not null
        [Required]
        [Display(Name = "ShopProduct_CategoryId", ResourceType = typeof(GlobalModel))]
        public Guid ShopProducts_CategoryID { get; set; } // uniqueidentifier, not null
        [StringLength(128)]
        [Required]
        [Display(Name = "ShopProduct_Name", ResourceType = typeof(GlobalModel))]
        public string ShopProducts_Name { get; set; } // nvarchar(128), not null
        [Display(Name = "ShopProduct_ShortDescription", ResourceType = typeof(GlobalModel))]
        [AllowHtml]
        public string ShopProducts_ShortDescription { get; set; } // nvarchar(max), null
        [Display(Name = "ShopProduct_FullDescription", ResourceType = typeof(GlobalModel))]
        [AllowHtml]
        public string ShopProducts_FullDescription { get; set; } // nvarchar(max), null
        [Required]
        [Display(Name = "CommonObjects_Price", ResourceType = typeof(GlobalModel))]
        public decimal ShopProducts_Price { get; set; } // decimal(35,8), not null
        [StringLength(3)]
        [Required]
        [Display(Name = "CommonObjects_CurrencyCode", ResourceType = typeof(GlobalModel))]
        public string ShopProducts_CurrencyCode { get; set; } // nvarchar(3), not null
        [Required]
        [Display(Name = "CommonObjects_Published", ResourceType = typeof(GlobalModel))]
        public bool ShopProducts_IsPublished { get; set; } // bit, not null
        [Required]
        [Display(Name = "CommonObjects_Deleted", ResourceType = typeof(GlobalModel))]
        public bool ShopProducts_IsDeleted { get; set; } // bit, not null

        [JsonIgnore]
        public string ShopingProducts_Action { get; set; } // string

        [JsonIgnore]
        [DefaultValue(typeof(int), "1")]
        public int ShopingProducts_Count { get; set; } // string


        // dbo.shop_Products.ShopID -> dbo.shop_Shops.ID (FK_Products_ShopID)
        public virtual ShopInfo ShopProducts_ShopInfo { get; set; }
        // dbo.shop_Products.CategoryID -> dbo.shop_Categories.ID (FK_Products_CategoryID)
        public virtual ShopCategories ShopProducts_ShopCategory { get; set; }
        // dbo.shop_OrderItems.ProductId -> dbo.shop_Products.ID (FK_Order_Items_ProductID)
        public virtual List<ShopOrderItems> ShopProducts_ShopOrderItems { get; set; }
        // dbo.shop_ProductImages.ProductID -> dbo.shop_Products.ID (FK_Product_Images_ProductID)
        public virtual List<ShopProductImages> ShopProducts_ShopProductImages { get; set; }
    }
}
