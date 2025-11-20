using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.Shop
{
    [Table("shop_Categories")]
    public class ShopCategoriesDto
    {
        //[Dapper.Contrib.Extensions.Key]
        [ExplicitKey]
        public Guid ID { get; set; } // uniqueidentifier, not null
        [MaxLength(128)]
        public string Name { get; set; } // nvarchar(128), not null
        public Guid? Parent { get; set; } // uniqueidentifier, null
        public bool IsPublished { get; set; } // bit, not null
        public bool IsDeleted { get; set; } // bit, not null
        [Write(false)]
        public int Order { get; set; } // bit, not null
    }
}