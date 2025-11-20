using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace TSG.Models.ServiceModels
{
    public class User
    {
        public string User_Username { get; set; }
        public string User_Password { get; set; }
        public string User_KYCLink { get; set; }
        public int? User_WlcMsgId { get; set; }
        public DateTime? User_LastLoginDate { get; set; }
        public bool? User_IsLocal { get; set; }
        public DateTime? User_CurLoginDate { get; set; }
        public int? User_RoleId { get; set; }
        public Guid? User_UserIdByTSG { get; set; }
        public string User_FirstName { get; set; }
        public string User_LastName { get; set; }
        public string User_UserMail { get; set; }
        public bool User_NeedToSearchWelcomeMessage { get; set; }
        public string User_UserUiVersion { get; set; }
        public bool User_IsViewedChangeLog { get; set; }

        [Write(false)]
        [Computed]
        public bool User_IsNewUser { get; set; }

        // dbo.User.wlcMsgId -> dbo.WelcomeMessage.wlcMsgId (FK_User_WelcomeMessage)
        public virtual WelcomeMessage User_WelcomeMessage { get; set; }
        // dbo.User.roleId -> dbo.Role.roleId (FK_User_Role)
        public virtual Role User_Role { get; set; }
    }
}
