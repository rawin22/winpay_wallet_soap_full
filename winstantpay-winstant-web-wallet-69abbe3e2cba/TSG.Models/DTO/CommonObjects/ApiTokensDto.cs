using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [ApiTokens] This table contains all user tokens for Mobile API
    /// </summary>
    [Table("ApiTokens")]
    public class ApiTokensDto
    {
        [MaxLength(50)]
        public string UserName { get; set; }
        [MaxLength(500)]
        public string Password { get; set; }
        [MaxLength(50)]
        public string UserId { get; set; }
        [MaxLength]
        public string TokenKey { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        [MaxLength(50)]
        public string TokenId { get; set; }
    }
}
