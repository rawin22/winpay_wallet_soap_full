using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tsg.UI.Main.Models
{
    public class BankDepositModel
    {
        [Display(Name = "Deposit Id")]
        public int DepositId { get; set; }

        //[Required]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate { get; set; }

        [Display(Name ="Status")]
        public string Status { get; set; }

        [Display(Name = "Approved Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime ApprovedDate { get; set; }

        public string Username { get; set; }
        public int BankCurrencyId { get; set; }
        public int BankId { get; set; }
        public string Bank { get; set; }
        public string CurrencyName { get; set; }
        public IList<SelectListItem> AvailableBankCurrencies { get; set; }
        public IList<SelectListItem> AvailableStatuses { get; set; }
        public int ProofDocId { get; set; }
        public string PathToFile { get; set; }
        [AllowHtml]
        public string WireInstruction { get; set; }
        public string Notes { get; set; }

        public string ClientSendingBankName { get; set; }
        public string ClientAccountNumber { get; set; }
        public string CustomerName { get; set; }
        public string ClientOtherInfo { get; set; }


        public override string ToString()
        {
            return "[Deposit Id=" + DepositId + "],[Amount=" + Amount + "],[Created Date=" + CreatedDate + "],[Status=" + Status + "]";
        }
    }

    public class ListOfBankModel{
        public List<BankDepositModel> EnumBankDepositModel { get; set; }
        public BankFilterModel FilterModel { get; set; }
    }
        
}