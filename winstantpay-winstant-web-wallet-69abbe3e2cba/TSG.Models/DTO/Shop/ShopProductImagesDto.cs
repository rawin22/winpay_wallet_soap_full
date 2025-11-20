using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.Shop
{
    [System.ComponentModel.DataAnnotations.Schema.Table("shop_ProductImages")]
    public class ShopProductImagesDto
    {
        [ExplicitKey]
        public Guid ID { get; set; } // uniqueidentifier, not null
        public Guid ProductID { get; set; } // uniqueidentifier, not null
        [MaxLength(255)]
        public string Path { get; set; } // nvarchar(255), not null
    }
}
