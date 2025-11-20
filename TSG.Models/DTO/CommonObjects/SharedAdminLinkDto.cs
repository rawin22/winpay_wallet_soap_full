using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSG.Models.DTO
{
    // TODO: SharedAdminLink add notification
    /// <summary>
    /// Table [SharedAdminLink]
    /// </summary>
    [Table("SharedAdminLink")]
    public class SharedAdminLinkDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(50)] public string UserName { get; set; }
        [MaxLength(50)] public string FirstName { get; set; }
        [MaxLength(50)] public string LastName { get; set; }
        [MaxLength(100)] public string Email { get; set; }
        public DateTime? CreationDate { get; set; }
        [MaxLength(150)] public string LinkAddress { get; set; }
        public int? StatusLink { get; set; }
        public DateTime? ActivationDate { get; set; }
    }
}