using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSG.Models.ServiceModels
{
    public class BitcoinManagerSenderApiResponse
    {
        [JsonProperty("receiverBTCAddress")] public string receiverBTCAddress { get; set; }
    }
}
