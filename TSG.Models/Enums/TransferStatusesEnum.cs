using System.ComponentModel.DataAnnotations;
using TSG.Models.App_LocalResources;

namespace TSG.Models.Enums
{
    public enum TransferStatusesEnum
    {
        [Display(Name = "TransferStatuses_TokenNotClaimed", ResourceType = typeof(GlobalModel))]
        TokenNotClaimed = 1,
        [Display(Name = "TransferStatuses_TokenClaimed", ResourceType = typeof(GlobalModel))]
        TokenClaimed = 2,
        [Display(Name = "TransferStatuses_TokenDeclined", ResourceType = typeof(GlobalModel))]
        TokenDeclined = 3,
        
        [Display(Name = "TransferStatuses_RedEnvelopeWaiting", ResourceType = typeof(GlobalModel))]
        RedEnvelopeWaiting = 4,
        [Display(Name = "TransferStatuses_RedEnvelopeAccepted", ResourceType = typeof(GlobalModel))]
        RedEnvelopeAccepted = 5,
        [Display(Name = "TransferStatuses_RedEnvelopeDeclined", ResourceType = typeof(GlobalModel))]
        RedEnvelopeDeclined = 6
    }
}