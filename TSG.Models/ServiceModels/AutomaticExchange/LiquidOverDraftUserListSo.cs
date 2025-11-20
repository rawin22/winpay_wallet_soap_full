using System;
using System.ComponentModel.DataAnnotations;

namespace TSG.Models.ServiceModels.AutomaticExchange
{
    public class LiquidOverDraftUserListSo
    {
        [Required]
        [Display(Name = "Id")]
        public Guid LiquidOverDraftUserList_Id { get; set; } // uniqueidentifier, not null
        [Required]
        [Display(Name = "User Id")]
        public Guid LiquidOverDraftUserList_UserId { get; set; } // uniqueidentifier, not null
        [MaxLength(50)]
        [Required]
        [Display(Name = "User Name")]
        public string LiquidOverDraftUserList_UserName { get; set; } // nvarchar(50), not null

        [Display(Name = "Full Name")]
        public string LiquidOverDraftUserList_FullName { get; set; } // nvarchar(50), not null

        [Display(Name = "User Name")]
        public string LiquidOverDraftUserList_AccountRep { get; set; } // nvarchar(50), not null
        //[Required]
        //[Display(Name = "Liquid Currency Id")]
        //public Guid LiquidOverDraftUserList_LiquidCurrencyId { get; set; } // uniqueidentifier, not null
        //[Required]
        //[Display(Name = "Amount")]
        //public decimal LiquidOverDraftUserList_Amount { get; set; } // decimal(35,8), not null
        [Required]
        [Display(Name = "Creation Date")]
        public DateTime LiquidOverDraftUserList_CreationDate { get; set; } // datetime, not null

        //public virtual LiquidCcyListSo LiquidOverDraftUserList_LiquidCcyList { get; set; }
    }

}