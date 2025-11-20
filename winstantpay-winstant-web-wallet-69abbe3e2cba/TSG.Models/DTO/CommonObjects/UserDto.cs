using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [User] Table contains all user information for WinstantPay system
    /// </summary>
    [Dapper.Contrib.Extensions.Table("User")]
    public class UserDto
    {
        [Dapper.Contrib.Extensions.Key]
        [MaxLength(50)]
        public string username { get; set; }
        [MaxLength(50)] public string password { get; set; }
        [MaxLength(100)] public string KYCLink { get; set; }
        public int? wlcMsgId { get; set; }
        public DateTime? lastLoginDate { get; set; }
        public bool? isLocal { get; set; }
        public DateTime? curLoginDate { get; set; }
        public int? roleId { get; set; }
        public Guid? userIdByTSG { get; set; }
        [MaxLength(50)] public string firstName { get; set; }
        [MaxLength(50)] public string lastName { get; set; }
        [MaxLength(500)] public string userMail { get; set; }
        public bool needToSearchWelcomeMessage { get; set; }
        [MaxLength(50)] public string UserUiVersion { get; set; }
        public bool IsViewedChangeLog { get; set; }
    }
}