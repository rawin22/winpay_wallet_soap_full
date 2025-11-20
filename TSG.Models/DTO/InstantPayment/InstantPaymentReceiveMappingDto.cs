using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.InstantPayment
{
    [Table("InstantPaymentReceiveMapping")]
    public class InstantPaymentReceiveMappingDto
    {
        [Dapper.Contrib.Extensions.ExplicitKey]
        public Guid Id { get; set; } // uniqueidentifier, not null
        public Guid InstantPaymentReceiveId { get; set; } // uniqueidentifier, not null
        public Guid InstantPaymentId { get; set; } // uniqueidentifier, not null
        public DateTime CreatedDate { get; set; } // datetime, not null
    }
}