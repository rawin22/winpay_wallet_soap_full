using System;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO
{
    /// <summary>
    /// Table [Conversion] This table contains information about conversation
    /// INFO: But now is empty
    /// </summary>
    [Table("Conversion")]
    public class ConversionDto
    {
        [Key] public int convId { get; set; }
        public int? fromCcyId { get; set; }
        public int? toCcyId { get; set; }
        public decimal? convAmt { get; set; }
        public DateTime? convDate { get; set; }
        public decimal? exchangeRate { get; set; }
    }
}