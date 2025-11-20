using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [OrdersHistory] This table contains information about all orders manipulation in WinstantPayment
    /// </summary>
    [Dapper.Contrib.Extensions.Table("OrdersHistory")]
    public class OrdersHistoryDto
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public Guid TokenKey { get; set; }
        public DateTime DateOfChanging { get; set; }
        public int StatusState { get; set; }
        [MaxLength] public string Comment { get; set; }
        public int MerchantId { get; set; }
        [MaxLength] public string AliasForUser { get; set; }
    }
}