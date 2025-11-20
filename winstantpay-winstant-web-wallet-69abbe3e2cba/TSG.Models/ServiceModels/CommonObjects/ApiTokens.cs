using System;
using System.Collections.Generic;

namespace TSG.Models.ServiceModels
{
    public class ApiTokens
    {
        public string ApiTokens_UserName { get; set; }
        public string ApiTokens_Password { get; set; }
        public string ApiTokens_UserId { get; set; }
        public string ApiTokens_TokenKey { get; set; }
        public DateTime? ApiTokens_CreationDate { get; set; }
        public DateTime? ApiTokens_ExpiredDate { get; set; }
        public string ApiTokens_TokenId { get; set; }
    }
}
