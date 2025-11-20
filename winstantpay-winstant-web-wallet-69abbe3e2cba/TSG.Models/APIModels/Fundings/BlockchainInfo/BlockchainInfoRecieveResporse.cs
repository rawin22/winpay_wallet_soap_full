using Newtonsoft.Json;

namespace TSG.Models.APIModels.Fundings.BlockchainInfo
{
    public class BlockchainInfoRecieveResporse
    {
        [JsonProperty("transaction_hash")] public string TransactionHash { get; set; }      // The payment transaction hash.
        [JsonProperty("address")] public string Address { get; set; }                       // The destination bitcoin address (part of your xPub account).
        [JsonProperty("confirmations")] public int Confirmations { get; set; }              // The number of confirmations of this transaction.
        [JsonProperty("value")] public long Value { get; set; }                             // The value of the payment received (in satoshi, so divide by 100,000,000 to get the value in BTC).
        [JsonProperty("custom_parameter")] public string CustomParameter { get; set; }      // Any parameters included in the callback URL will be passed back to the callback URL in the notification.
    }
}