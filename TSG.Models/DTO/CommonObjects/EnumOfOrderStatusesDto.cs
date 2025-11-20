using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [EnumOfOrderStatuses] This table contains information about weight of Order Statuses in system
    /// </summary>
    [Dapper.Contrib.Extensions.Table("EnumOfOrderStatuses")]
    public class EnumOfOrderStatusesDto
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(500)] public string StatusName { get; set; }
        [MaxLength(500)] public string StatusDesignName { get; set; }
    }
}