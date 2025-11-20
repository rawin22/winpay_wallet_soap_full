using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.Shop
{
    [Table("shop_OrderItems")]
    public class ShopOrderItemsDto
    {
        [ExplicitKey]
        public Guid ID { get; set; } // uniqueidentifier, not null
        public Guid ProductId { get; set; } // uniqueidentifier, not null
        public Guid OrderId { get; set; } // uniqueidentifier, not null
        public decimal Price { get; set; } // decimal(35,8), not null
        [MaxLength(3)]
        public string CurrencyCode { get; set; } // nvarchar(3), not null
        public int Quantity { get; set; } // int, not null
        [MaxLength(128)]
        public string Position { get; set; } // nvarchar(128), not null
        public long Timestamp { get; set; } // bigint, not null
    }

}