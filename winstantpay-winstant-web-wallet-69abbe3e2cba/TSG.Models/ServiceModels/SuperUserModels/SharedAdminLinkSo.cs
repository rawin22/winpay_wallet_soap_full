using System;
using System.ComponentModel.DataAnnotations;

namespace TSG.Models.ServiceModels.SuperUserModels
{
    public class SharedAdminLinkSo
    {
        [Required(ErrorMessage = "ID is required")]
        [Display(Name = "ID")]
        public int SharedAdminLink_ID { get; set; } // int
        [MaxLength(50)]
        [Required(ErrorMessage = "User Name is required")]
        [Display(Name = "User Name")]
        public string SharedAdminLink_UserName { get; set; } // nvarchar(50)
        [MaxLength(50)]
        [Required(ErrorMessage = "First Name is required")]
        [Display(Name = "First Name")]
        public string SharedAdminLink_FirstName { get; set; } // nvarchar(50)
        [MaxLength(50)]
        [Required(ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        public string SharedAdminLink_LastName { get; set; } // nvarchar(50)
        [MaxLength(100)]
        [Display(Name = "Email")]
        public string SharedAdminLink_Email { get; set; } // nvarchar(100)
        [Display(Name = "Creation Date")]
        public DateTime? SharedAdminLink_CreationDate { get; set; } // datetime
        [MaxLength(150)]
        [Display(Name = "Link Address")]
        public string SharedAdminLink_LinkAddress { get; set; } // nvarchar(150)
        [Display(Name = "Status Link")]
        public int? SharedAdminLink_StatusLink { get; set; } // int
        [Display(Name = "Activation Date")]
        public DateTime? SharedAdminLink_ActivationDate { get; set; } // datetime
    }
}