using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Newtonsoft.Json;
using TSG.Models.App_LocalResources;

namespace TSG.Models.ServiceModels.Shop
{
    public class ShopProductImages
    {
        [Required]
        [Display(Name = "CommonObjects_ID", ResourceType = typeof(GlobalModel))]
        [Key]
        public Guid ShopProductImages_ID { get; set; } // uniqueidentifier, not null
        [Required]
        [Display(Name = "ShopOrdersItem_ProductId", ResourceType = typeof(GlobalModel))]
        public Guid ShopProductImages_ProductID { get; set; } // uniqueidentifier, not null
        [MaxLength(255)]
        [Required]
        [Display(Name = "ShopProductImages_Path", ResourceType = typeof(GlobalModel))]
        public string ShopProductImages_Path { get; set; } // nvarchar(255), not null

        // dbo.shop_ProductImages.ProductID -> dbo.shop_Products.ID (FK_Product_Images_ProductID)
        public virtual ShopProducts ShopProductImages_ShopProduct { get; set; }

        [JsonIgnore] public HttpPostedFileBase ShopProductImage_PostedFile { get; set; }

    }
}
