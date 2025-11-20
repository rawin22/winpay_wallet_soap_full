using System.ComponentModel.DataAnnotations;

namespace Tsg.UI.Main.Models
{
    public class BankCurrencyModel
    {
        [Display(Name = "BankCurrency Id")]
        public int BankCurrencyId { get; set; }

        [Display(Name = "BankCurrency Name")]
        public string BankCurrencyName { get; set; }
    }
}