using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TSG.Models.ServiceModels.AutomaticExchange
{
    public class LiquidCcyListSo
    {

        public LiquidCcyListSo()
        {
            this.LiquidCcyList_DependencyLiquidForUsers = new List<DependencyLiquidForUserSo>();
            this.LiquidCcyList_LiquidOverDraftUserLists = new List<LiquidOverDraftUserListSo>();
        }

        [Required]
        [Display(Name = "Id")]
        public Guid LiquidCcyList_Id { get; set; } // uniqueidentifier, not null
        //[Required]
        //[Display(Name = "Currency Id")]
        //public int LiquidCcyList_CurrencyId { get; set; } // int, not null
        [Required]
        [Display(Name = "Is Liquid Currency")]
        public bool LiquidCcyList_IsLiquidCurrency { get; set; } // bit, not null
        [Required]
        [Display(Name = "Liquid Order")]
        public int LiquidCcyList_LiquidOrder { get; set; } // int, not null
        [MaxLength(50)]
        [Required]
        [Display(Name = "Currency Code")]
        public string LiquidCcyList_CurrencyCode { get; set; } // nvarchar(50), not null

        public virtual List<DependencyLiquidForUserSo> LiquidCcyList_DependencyLiquidForUsers { get; set; }
        public virtual List<LiquidOverDraftUserListSo> LiquidCcyList_LiquidOverDraftUserLists { get; set; }
    }
}