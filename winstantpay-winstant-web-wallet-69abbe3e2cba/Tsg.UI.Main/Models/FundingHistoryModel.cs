using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tsg.UI.Main.Models
{
    public class FundingHistoryModel
    {
        public int FundingId { get; set; }

        public string AdminName { get; set; }
        public string StatusOld { get; set; }
        public string StatusNew { get; set; }
        public DateTime ChangedDateTime { get; set; }
        public string AdminNote { get; set; }
    }
}