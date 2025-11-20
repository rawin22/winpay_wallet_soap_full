using System.ComponentModel.DataAnnotations;
using TSG.Models.App_LocalResources;

namespace TSG.Models.Enums
{
    public enum TransferedTokenStatusesEnum
    {
        [Display(Name = "TransferedTokenStatuses_TokenTransfered", ResourceType = typeof(GlobalModel))]
        TokenTransfered = 1,
        [Display(Name = "TransferedTokenStatuses_TokenPending", ResourceType = typeof(GlobalModel))]
        TokenPending = 2,
        [Display(Name = "TransferedTokenStatuses_TokenDeclined", ResourceType = typeof(GlobalModel))]
        TokenDeclined = 3
    }
}