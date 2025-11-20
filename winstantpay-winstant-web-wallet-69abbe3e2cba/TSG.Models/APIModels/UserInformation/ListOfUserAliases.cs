using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TSG.Models.ServiceModels.LimitPayment;

namespace TSG.Models.APIModels.UserInformation
{
    /// <summary>
    /// Result for get user aliases method
    /// </summary>
    public class ListOfUserAliases : StandartResponse
    {
        /// <summary>
        /// List Of Aliases
        /// </summary>
        //public List<UserAlias> UserAliases { get; set; } = new List<UserAlias>();
        public List<string> UserAliases { get; set; } = new List<string>();
        public List<DaPayLimitsTabSo> LimitsTabs { get; set; } = new List<DaPayLimitsTabSo>();
        public List<DaUserWPayIDSettingSo> DaUserWPayIDSettings { get; set; } = new List<DaUserWPayIDSettingSo>();
        public List<DaPayLimitsTypeSo> LimitsTypes { get; set; } = new List<DaPayLimitsTypeSo>();

    }
}