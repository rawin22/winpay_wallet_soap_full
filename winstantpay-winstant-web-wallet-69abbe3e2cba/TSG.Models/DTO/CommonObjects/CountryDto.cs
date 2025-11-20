using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [Country] This table contains information about Countries in system
    /// </summary>
    [Table("Country")]
    public class CountryDto
    {
        [Dapper.Contrib.Extensions.Key] public int ID { get; set; }
        [MaxLength(2)] public string ISO { get; set; }
        [MaxLength(80)] public string Name { get; set; }
        [MaxLength(80)] public string Fullname { get; set; }
        [MaxLength(3)] public string ISO3 { get; set; }
        public short? numcode { get; set; }
        public int phonecode { get; set; }
    }
}