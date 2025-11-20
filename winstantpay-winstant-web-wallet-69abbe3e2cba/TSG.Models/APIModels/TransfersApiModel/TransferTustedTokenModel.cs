using System;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.TransfersApiModel
{
    public class TransferTustedTokenModel
    {
        [JsonProperty("token_id")]public Guid TokenId { get; set; }
        [JsonProperty("alias_to_user")]public string AliasToUser { get; set; }
    }
}