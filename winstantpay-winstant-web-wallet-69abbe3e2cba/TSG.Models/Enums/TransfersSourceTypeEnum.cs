using System.ComponentModel.DataAnnotations;
using TSG.Models.App_LocalResources;

namespace TSG.Models.Enums
{
    public enum TransfersSourceTypeEnum
    {
        [Display(Name = "TransfersSourceType_TokenTransfers", ResourceType = typeof(GlobalModel))]
        TokenTransfers = 1,
        [Display(Name = "TransfersSourceType_RedEnvelope", ResourceType = typeof(GlobalModel))]
        RedEnvelope = 2,
    }
}