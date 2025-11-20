using System.Collections.Generic;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.Fundings.Wire_Instructions
{
    public class DepositHistoryModel : StandartResponse
    {
        [JsonProperty("fundings")] public List<ServiceModels.Fundings> Fundings { get; set; }
    }
}