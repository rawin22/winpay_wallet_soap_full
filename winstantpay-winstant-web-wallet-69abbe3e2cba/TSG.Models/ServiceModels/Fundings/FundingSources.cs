using System;
using System.Collections.Generic;

namespace TSG.Models.ServiceModels
{
    public class FundingSources
    {
        public Guid FundingSources_ID { get; set; }
        public string FundingSources_SourceName { get; set; }
        public string FundingSources_DesignName { get; set; }
        public bool FundingSources_IsDeleted { get; set; }
    }
}
