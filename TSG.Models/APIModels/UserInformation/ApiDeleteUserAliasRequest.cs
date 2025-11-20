using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TSG.Models.ServiceModels.LimitPayment;

namespace TSG.Models.APIModels.UserInformation
{
    /// <summary>
    /// Request of user alias deletion
    /// </summary>
    public class ApiDeleteUserAliasRequest
    {
        /// <summary>
        /// User Alias (WPayID)
        /// </summary>
        public string Alias { get; set; }        
    }
}