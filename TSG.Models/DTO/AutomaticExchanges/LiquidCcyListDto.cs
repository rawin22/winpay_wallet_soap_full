using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.AutomaticExchanges
{
    [Dapper.Contrib.Extensions.Table("ae_LiquidCcyList")]
    public class LiquidCcyListDto
    {
        [ExplicitKey]
        public Guid Id { get; set; } // uniqueidentifier, not null
        public int CurrencyId { get; set; } // int, not null
        public bool IsLiquidCurrency { get; set; } // bit, not null
        public int LiquidOrder { get; set; } // int, not null
        [MaxLength(50)]
        public string CurrencyCode { get; set; } // nvarchar(50), not null
    }
}