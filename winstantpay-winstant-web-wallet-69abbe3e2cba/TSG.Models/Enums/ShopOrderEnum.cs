using System.ComponentModel.DataAnnotations;

namespace TSG.Models.Enums
{
    public enum ShopOrderEnum
    {
        [Display(Name = "Undefined")]
        Undefined = 0,
        [Display(Name = "Created")]
        Created,
        [Display(Name = "Opened")]
        Opened,
        [Display(Name = "Fail to pay")]
        PayedFail,
        [Display(Name = "Payed")]
        SuccessifullyPayed = 100
    }
}