using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tsg.UI.Main.Models
{
    internal static class SettingClass
    {
        private static string _sysNotificationEmailInUploadProof;
        
        public static string SysNotificationEmailInUploadProof => _sysNotificationEmailInUploadProof;

        internal static bool SetSysEmailUploadProof(string email)
        {
            _sysNotificationEmailInUploadProof = email.Trim();
            return !string.IsNullOrEmpty(email.Trim());
        }

    }
}