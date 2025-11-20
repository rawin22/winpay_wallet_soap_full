using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.Shop
{
    [Table("shop_Products")]
    public class ShopProductsDto
    {
        [ExplicitKey]
        public Guid ID { get; set; } // uniqueidentifier, not null
        public Guid ShopID { get; set; } // uniqueidentifier, not null
        public Guid CategoryID { get; set; } // uniqueidentifier, not null
        [MaxLength(128)]
        public string Name { get; set; } // nvarchar(128), not null
        [MaxLength]
        [AllowHtml]
        public string ShortDescription { get; set; } // nvarchar(max), null
        [AllowHtml]
        [MaxLength]
        public string FullDescription { get; set; } // nvarchar(max), null
        public decimal Price { get; set; } // decimal(35,8), not null
        [MaxLength(3)]
        public string CurrencyCode { get; set; } // nvarchar(3), not null
        public bool IsPublished { get; set; } // bit, not null
        public bool IsDeleted { get; set; } // bit, not null
    }
}
