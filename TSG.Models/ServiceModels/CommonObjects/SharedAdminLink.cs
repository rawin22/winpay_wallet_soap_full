using System;
using System.Collections.Generic;

namespace TSG.Models.ServiceModels
{
    public class SharedAdminLink
    {
        public int SharedAdminLink_ID { get; set; }
        public string SharedAdminLink_UserName { get; set; }
        public string SharedAdminLink_FirstName { get; set; }
        public string SharedAdminLink_LastName { get; set; }
        public string SharedAdminLink_Email { get; set; }
        public DateTime? SharedAdminLink_CreationDate { get; set; }
        public string SharedAdminLink_LinkAddress { get; set; }
        public int? SharedAdminLink_StatusLink { get; set; }
        public DateTime? SharedAdminLink_ActivationDate { get; set; }
    }
}
