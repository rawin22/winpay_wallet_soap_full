using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.Shop
{
    [Table("shop_Orders")]
    public class ShopOrdersDto
    {
        [ExplicitKey]
        public Guid ID { get; set; } // uniqueidentifier, not null
        [MaxLength(50)]
        public string BuyerWpayId { get; set; } // nvarchar(50), not null
        public Guid? LastOrderHistoryRec { get; set; }
        [Write(false)]
        public int OrderCounter { get; set; }
        [Write(false)]
        public DateTime CreateDate { get; set; }
    }
}
