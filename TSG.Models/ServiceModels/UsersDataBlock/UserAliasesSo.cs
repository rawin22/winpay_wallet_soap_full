using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSG.Models.ServiceModels.UsersDataBlock
{
    public class UserAliasesSo
    {
        [Required(ErrorMessage = "User Id is required")]
        [Display(Name = "User Id")]
        public Guid Wpay_UserId { get; set; } // uniqueidentifier

        [MaxLength(150)]
        [Required(ErrorMessage = "User Name is required")]
        [Display(Name = "User Name")]
        public string Wpay_UserName { get; set; } // nvarchar(150)

        [MaxLength]
        [Required(ErrorMessage = "User Aliases is required")]
        [Display(Name = "User Aliases")]
        public List<string> Wpay_Ids { get; set; } // nvarchar(max)
    }
}
