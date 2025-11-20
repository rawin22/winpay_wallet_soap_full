using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TSG.Models.ServiceModels.BlockchainFunds
{
    public class DashManagerSenderApiResponse
    {
        [JsonProperty("receiverAddress")] public string receiverAddress { get; set; }
    }
}
