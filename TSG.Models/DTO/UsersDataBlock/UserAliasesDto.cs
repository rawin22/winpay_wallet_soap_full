using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.UsersDataBlock
{
    [System.ComponentModel.DataAnnotations.Schema.Table("usr_UserAliases")]
    public class UserAliasesDto
    {
        [ExplicitKey]
        public Guid UserId { get; set; } // uniqueidentifier
        [MaxLength(150)]
        public string UserName { get; set; } // nvarchar(150)
        [MaxLength]
        public string UserAliases { get; set; } // nvarchar(max)
    }
}
