using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [TokenKeysForOrders] Table contains Orders information for 3-d party services for payment in WinstantPay system
    /// </summary>
    [Table("TokenKeysForOrders")]
    public class TokenKeysForOrdersDto
    {
        [Dapper.Contrib.Extensions.Key] public Guid CustomerOrderId { get; set; }
        public int CustomerOrderStatus { get; set; }
        public int? MerchantId { get; set; }
        public Guid? CustomerId { get; set; }
        [MaxLength(50)] public string Currency { get; set; }
        public decimal? Quantity { get; set; }
        [MaxLength(500)] public string MerchantWebSite { get; set; }
        public int? CustomerAccounting { get; set; }
        public DateTime? OrderExpiredDate { get; set; }
        [MaxLength(1000)] public string Stoke { get; set; }
        public Guid? PaymentGuid { get; set; }
        public bool IsSanboxOrder { get; set; }
    }
}