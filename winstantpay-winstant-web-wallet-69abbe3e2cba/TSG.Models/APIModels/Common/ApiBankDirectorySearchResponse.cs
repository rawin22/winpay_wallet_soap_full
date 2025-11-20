using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSG.Models.APIModels.Common
{
    public class ApiBankDirectorySearchResponse : StandartResponse
    {
        public ApiBankDetailsModel BankInformation { get; set; }
    }
}
