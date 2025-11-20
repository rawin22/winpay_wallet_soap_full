using System;
using System.Collections.Generic;

namespace TSG.Models.ServiceModels
{
    public class ReferalLinksList
    {
        public int ReferalLinksList_ID { get; set; }
        public string ReferalLinksList_LinkText { get; set; }
        public bool ReferalLinksList_IsDefault { get; set; }
        public bool? ReferalLinksList_IsActive { get; set; }
        public DateTime? ReferalLinksList_ExpiredDate { get; set; }
    }
}
