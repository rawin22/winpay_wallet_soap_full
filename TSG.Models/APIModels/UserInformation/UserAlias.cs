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
    public class UserAlias : StandartResponse
    {
        /// <summary>
        /// User Alias (WPayID)
        /// </summary>
        public string Alias { get; set; }        
        public List<DaPayLimitsTabSo> LimitsTabs { get; set; } = new List<DaPayLimitsTabSo>();
        public List<DaPayLimitsTypeSo> LimitsTypes { get; set; } = new List<DaPayLimitsTypeSo>();
    }
}