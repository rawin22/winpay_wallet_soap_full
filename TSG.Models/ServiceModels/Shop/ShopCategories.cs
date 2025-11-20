using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.ModelBinding;
using Newtonsoft.Json;
using TSG.Models.App_LocalResources;
using WinstantPay.Common.Extension;

namespace TSG.Models.ServiceModels.Shop
{
    public class ShopCategories
    {
        public ShopCategories()
        {
            this.ShopCategories_ShopCategoriesList = new List<ShopCategories>();
            this.ShopCategories_ShopProducts = new List<ShopProducts>();
        }

        [Required]
        [Display(Name = "CommonObjects_ID", ResourceType = typeof(GlobalModel))]
        public Guid ShopCategories_ID { get; set; } // uniqueidentifier, not null
        [MaxLength(128)]
        [Required]
        [Display(Name = "ShopCategories_Name", ResourceType = typeof(GlobalModel))]
        public string ShopCategories_Name { get; set; } // nvarchar(128), not null

        [Display(Name = "ShopCategories_Parent", ResourceType = typeof(GlobalModel))]
        [HtmlExtensions.NoValidate]
        public Guid? ShopCategories_Parent { get; set; } // uniqueidentifier, null
        [Required]
        [Display(Name = "CommonObjects_Published", ResourceType = typeof(GlobalModel))]
        public bool ShopCategories_IsPublished { get; set; } // bit, not null
        [Required]
        [Display(Name = "CommonObjects_Deleted", ResourceType = typeof(GlobalModel))]
        public bool ShopCategories_IsDeleted { get; set; } // bit, not null
        public int ShopCategories_Order { get; set; } // bit, not null

        // dbo.shop_Categories.Parent -> dbo.shop_Categories.ID (FK_Categories_Parent)
        public virtual ShopCategories ShopCategories_ParentShopCategory { get; set; }
        // dbo.shop_Categories.Parent -> dbo.shop_Categories.ID (FK_Categories_Parent)
        public virtual List<ShopCategories> ShopCategories_ShopCategoriesList { get; set; }
        // dbo.shop_Products.CategoryID -> dbo.shop_Categories.ID (FK_Products_CategoryID)
        public virtual List<ShopProducts> ShopCategories_ShopProducts { get; set; }
    }
}
