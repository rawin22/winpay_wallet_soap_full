using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO.SuperUserModels
{
    [Dapper.Contrib.Extensions.Table("SharedAdminLink")]

    public class SharedAdminLinkDto
    {
        [ExplicitKey]
        [Required(ErrorMessage = "ID is required")]
        public int ID { get; set; } // int
        [MaxLength(50)]
        [Required(ErrorMessage = "User Name is required")]
        public string UserName { get; set; } // nvarchar(50)
        [MaxLength(50)]
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; } // nvarchar(50)
        [MaxLength(50)]
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; } // nvarchar(50)
        [MaxLength(100)]
        public string Email { get; set; } // nvarchar(100)
        public DateTime? CreationDate { get; set; } // datetime
        [MaxLength(150)]
        public string LinkAddress { get; set; } // nvarchar(150)
        public int? StatusLink { get; set; } // int
        public DateTime? ActivationDate { get; set; } // datetime
    }
}