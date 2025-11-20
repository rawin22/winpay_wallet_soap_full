using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Tsg.UI.Main.Models.Attributes;
using TSG.Models.APIModels;

namespace Tsg.UI.Main.Models.Security
{
    public static class AppSecurity
    {
        private static string _keyAuthUserInfo = "UZ_AUTH_USER_INFO";

        public static UserInfo CurrentUser
        {
            get
            {
                if (HttpContext.Current.Session[_keyAuthUserInfo] == null)
                {
                    return null;
                }
                return (UserInfo) HttpContext.Current.Session[_keyAuthUserInfo];
            }
            set { HttpContext.Current.Session[_keyAuthUserInfo] = value; }
        }
    }

    
}