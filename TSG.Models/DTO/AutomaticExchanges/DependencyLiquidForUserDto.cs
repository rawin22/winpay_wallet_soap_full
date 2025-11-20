using System;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.AutomaticExchanges
{
    [Table("ae_DependencyLiquidForUser")]
    public class DependencyLiquidForUserDto
    {
        [ExplicitKey]
        public Guid Id { get; set; } // uniqueidentifier, not null
        public Guid UserId { get; set; } // uniqueidentifier, not null
        public Guid LiquidCcyId { get; set; } // uniqueidentifier, not null
        public int LiquidOrder { get; set; } // int, not null
    }
}