using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.ExchangeModels
{
    public class UpdateTokenExchangeModel : RateModel
    {
        [JsonProperty("from_acc_id")] [Required] public string FromAccountId { get; set; }
        [JsonProperty("to_acc_id")] [Required] public string ToAccountId { get; set; }
        [JsonProperty("buy_amount")] [Required] public decimal BuyAmount { get; set; }
        [JsonProperty("sell_currency")] [Required] public string SellCurrency { get; set; }
        [JsonProperty("quota_id")] [Required] public string QuotaId { get; set; }
        [JsonProperty("quote_time")] public long QuotaTime { get; set; }
        [JsonProperty("quote_created_utctime")] public long CreatedTime { get; set; }
        [JsonProperty("quote_expired_utctime")] public long ExpirationTime { get; set; }
    }

    public class UpdateTokenResponse : StandartResponse
    {
        [JsonProperty("quota_data")] public UpdateTokenExchangeModel Data { get; set; }
    }
}