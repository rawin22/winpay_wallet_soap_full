using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Newtonsoft.Json;
using TSG.Models.App_LocalResources;

namespace TSG.Models.ServiceModels.Shop
{
    public class ShopInfo
    {
        public ShopInfo()
        {
            this.ShopInfo_ShopProducts = new List<ShopProducts>();
        }

        [Required]
        [Display(Name = "CommonObjects_ID", ResourceType = typeof(GlobalModel))]
        public Guid ShopInfo_ID { get; set; } // uniqueidentifier, not null

        [StringLength(128)]
        [Required]
        [Display(Name = "ShopInfo_Name", ResourceType = typeof(GlobalModel))]
        public string ShopInfo_Name { get; set; } // nvarchar(128), not null

        [StringLength(50)]
        [Required]
        [Display(Name = "ShopInfo_OwnerWPayID", ResourceType = typeof(GlobalModel))]
        public string ShopInfo_OwnerWpayId { get; set; } // nvarchar(50), not null

        [StringLength(255)]
        [Required]
        [Display(Name = "ShopInfo_ShopLogo", ResourceType = typeof(GlobalModel))]
        public string ShopInfo_LogoPath { get; set; } // nvarchar(255), not null

        [Required]
        [Display(Name = "CommonObjects_Published", ResourceType = typeof(GlobalModel))]
        public bool ShopInfo_IsPublished { get; set; } // bit, not null
        [Required]
        [Display(Name = "CommonObjects_Deleted", ResourceType = typeof(GlobalModel))]
        public bool ShopInfo_IsDeleted { get; set; } // bit, not null

        // dbo.shop_Products.ShopID -> dbo.shop_Shops.ID (FK_Products_ShopID)
        public virtual List<ShopProducts> ShopInfo_ShopProducts { get; set; }
        [JsonIgnore] public HttpPostedFileBase ShopInfo_PostedFile { get; set; }

    }
}
