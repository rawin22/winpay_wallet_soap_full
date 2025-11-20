using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.Transfers
{
    [Table("kyc_NewUserByKyc")]
    public class KycNewUserDto
    {
        [Dapper.Contrib.Extensions.ExplicitKey]
        public Guid Id { get; set; } // uniqueidentifier, not null
        [MaxLength(150)]
        public string KycUserEmail { get; set; } // nvarchar(150), not null
        [MaxLength(150)]
        public string KycUserFirstName { get; set; } // nvarchar(150), not null
        [MaxLength(150)]
        public string KycUserLastName { get; set; } // nvarchar(150), not null
        [MaxLength(150)]
        public string UserInitiator { get; set; } // nvarchar(150), not null
        [Write(false)]
        public DateTime CreationDate { get; set; } // datetime, not null
        public bool IsCreated { get; set; } // bit, not null
    }
}