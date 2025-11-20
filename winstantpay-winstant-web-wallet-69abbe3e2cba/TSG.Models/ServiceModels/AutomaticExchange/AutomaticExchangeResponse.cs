using System.Collections.Generic;
using TSG.Models.APIModels;

namespace TSG.Models.ServiceModels.AutomaticExchange
{
    public class AutomaticExchangeResponse : StandartResponse
    {
        public List<AutomaticExchangeChekingModel> AutomaticExchngeListCcy { get; set; } = new List<AutomaticExchangeChekingModel>();

        public bool IsAutomaticExchange { get; set; }
        public bool IsNeedManualExchange { get; set; }
    }
}