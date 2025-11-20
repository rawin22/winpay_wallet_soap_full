using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Tsg.UI.Main.Models
{
    public class BankModel
    {
        [Display(Name = "Bank Id")]
        public int BankId { get; set; }

        [Display(Name = "Bank Name")]
        [Required(ErrorMessage = "Bank Name is required.")]
        public string BankName { get; set; }

        [Display(Name = "Bank Country")]
        [Required(ErrorMessage = "Bank Country is required.")]
        public int BankCountryId { get; set; }

        [Display(Name = "Bank Country")]
        public string BankCountry { get; set; }

        [Display(Name = "Currency Id")]
        //[Required(ErrorMessage = "Currency is required.")]
        public int CurrencyId { get; set; }
        public bool IsDeleted { get; set; }

        public string CurrencyCode { get; set; }

        public IList<SelectListItem> AvaliableCountries { get; set; }
        public IList<SelectListItem> AvailableCurrencies { get; set; }

        public override string ToString()
        {
            return "[Bank Id=" + BankId + "],[Bank Name=" + BankName + "],[Bank Country=" + BankCountry+"]";
        }
    }
}