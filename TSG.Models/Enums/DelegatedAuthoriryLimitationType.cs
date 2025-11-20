using System.ComponentModel.DataAnnotations;
using TSG.Models.App_LocalResources;

namespace TSG.Models.Enums
{
    public enum DelegatedAuthoriryLimitationType
    {
        [Display(Name = "DelegatedAuthoriryLimitationType_Transaction", ResourceType = typeof(GlobalModel))]
        SingleTransaction = 1,
        [Display(Name = "DelegatedAuthoriryLimitationType_Daily", ResourceType = typeof(GlobalModel))]
        DailyLimit = 2,
        [Display(Name = "DelegatedAuthoriryLimitationType_Weekly", ResourceType = typeof(GlobalModel))]
        WeekLimit = 3,
        [Display(Name = "DelegatedAuthoriryLimitationType_Montly", ResourceType = typeof(GlobalModel))]
        MonthLimit = 4,
        [Display(Name = "DelegatedAuthoriryLimitationType_Yearly", ResourceType = typeof(GlobalModel))]
        YearLimit = 5,
        [Display(Name = "DelegatedAuthoriryLimitationType_AllTime", ResourceType = typeof(GlobalModel))]
        AllTime = 6,
    }
}