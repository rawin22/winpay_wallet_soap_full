using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.AutomaticExchanges
{
    [Table("ae_LiquidOverDraftUserList")]
    public class LiquidOverDraftUserListDto
    {
        [ExplicitKey]
        public Guid Id { get; set; } // uniqueidentifier, not null
        public Guid UserId { get; set; } // uniqueidentifier, not null
        [MaxLength(50)]
        public string UserName { get; set; } // nvarchar(50), not null
        [Write(false)]
        [Computed]
        public string FullName { get; set; } // nvarchar(50), not null
        public string AccountRep { get; set; } // nvarchar(50), not null
        //public Guid LiquidCurrencyId { get; set; } // uniqueidentifier, not null
        //public decimal Amount { get; set; } // decimal(35,8), not null
        public DateTime CreationDate { get; set; } // datetime, not null
    }
}