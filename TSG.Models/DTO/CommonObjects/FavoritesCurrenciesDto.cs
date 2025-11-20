using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [FavoritesCurrencies] This table contains linked information about user and favorite currency for this user
    /// </summary>
    [Table("FavoritesCurrencies")]
    public class FavoritesCurrenciesDto
    {
        [MaxLength(100)] public string IDUser { get; set; }
        [MaxLength(100)] public string IDCurrency { get; set; }
    }
}