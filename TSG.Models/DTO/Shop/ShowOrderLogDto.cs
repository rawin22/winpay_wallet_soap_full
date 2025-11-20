using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.Shop
{
    [System.ComponentModel.DataAnnotations.Schema.Table("shop_OrderLog")]
    public class ShopOrderLogDto
    {
        [ExplicitKey]
        public Guid ID { get; set; } // uniqueidentifier, not null
        public Guid OrderID { get; set; } // uniqueidentifier, not null
        public int Status { get; set; } // int, not null
        public long Timestamp { get; set; } // bigint, not null
        public string Reason { get; set; }
    }
}
