using System;
using System.ComponentModel.DataAnnotations;

namespace TSG.Models.ServiceModels.AutomaticExchange
{
    public class DependencyLiquidForUserSo
    {
        [Required][Display(Name = "Id")]
        public Guid DependencyLiquidForUser_Id { get; set; } // uniqueidentifier, not null

        [Required] [Display(Name = "User Id")] public Guid DependencyLiquidForUser_UserId { get; set; } // uniqueidentifier, not null
        
        [Required][Display(Name = "Liquid Ccy Id")]
        public Guid DependencyLiquidForUser_LiquidCcyId { get; set; } // uniqueidentifier, not null

        [Required][Display(Name = "Liquid Order")]
        public int DependencyLiquidForUser_LiquidOrder { get; set; } // int, not null

        public virtual LiquidCcyListSo DependencyLiquidForUser_LiquidCcyList { get; set; }
    }
}