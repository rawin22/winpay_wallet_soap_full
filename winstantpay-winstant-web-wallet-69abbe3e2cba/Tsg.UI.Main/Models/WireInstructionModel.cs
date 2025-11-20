using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tsg.UI.Main.Models
{
    public class WireInstructionModel
    {
        [Display(Name = "Wire Instruction Id")]
        public int WireInstructionId { get; set; }

        [Display(Name = "Wire Instrction")]
        [Required(ErrorMessage = "Wire Instruction is requried.")]
        [AllowHtml]
        public string WireInstruction { get; set; }

        public int BankCurrencyId { get; set; }
        public string BankCurrencyName { get; set; }
        public IList<SelectListItem> AvailableBankCurrencies { get; set; }

        public override string ToString()
        {
            return "[Wire Instruction Id=" + WireInstructionId +
                "],[Wire Instruction=" + WireInstruction + "]";
        }
    }
}