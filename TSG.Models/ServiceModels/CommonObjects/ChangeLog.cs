using System;
using System.Collections.Generic;

namespace TSG.Models.ServiceModels
{
    public class ChangeLog
    {
        public int ChangeLog_ID { get; set; }
        public string ChangeLog_LogTitle { get; set; }
        public string ChangeLog_LogInfo { get; set; }
        public string ChangeLog_VersionInfo { get; set; }
        public bool ChangeLog_IsUiVersion { get; set; }
    }
}
