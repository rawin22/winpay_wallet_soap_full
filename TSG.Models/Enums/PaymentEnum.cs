using System.ComponentModel.DataAnnotations;

namespace TSG.Models.Enums
{
    public enum FundingStatus
    {
        [Display(Name = "Pending")]
        Pending,
        [Display(Name = "In Process")]
        InProcess,
        [Display(Name = "Credited")]
        Credited,
        [Display(Name = "Canceled")]
        Canceled
    }
}