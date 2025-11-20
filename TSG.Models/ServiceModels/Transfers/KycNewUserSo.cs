using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.Transfers
{
    public class KycNewUserSo
    {
        public Guid KycNewUser_Id { get; set; } // uniqueidentifier, not null
        [MaxLength(150)]
        public string KycNewUser_KycUserEmail { get; set; } // nvarchar(150), not null
        [MaxLength(150)]
        public string KycNewUser_KycUserFirstName { get; set; } // nvarchar(150), not null
        [MaxLength(150)]
        public string KycNewUser_KycUserLastName { get; set; } // nvarchar(150), not null
        [MaxLength(150)]
        public string KycNewUser_UserInitiator { get; set; } // nvarchar(150), not null
        public DateTime KycNewUser_CreationDate { get; set; } // datetime, not null
        public bool KycNewUser_IsCreated { get; set; } // bit, not null
    }
}