using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.Shop
{
    [Table("shop_Shops")]
    public class ShopInfoDto
    {
        [ExplicitKey]
        public Guid ID { get; set; } // uniqueidentifier, not null
        [MaxLength(128)]
        public string Name { get; set; } // nvarchar(128), not null
        [MaxLength(50)]
        public string OwnerWpayId { get; set; } // nvarchar(50), not null
        [MaxLength(255)]
        public string LogoPath { get; set; } // nvarchar(255), not null
        public bool IsPublished { get; set; } // bit, not null
        public bool IsDeleted { get; set; } // bit, not null
    }
}
