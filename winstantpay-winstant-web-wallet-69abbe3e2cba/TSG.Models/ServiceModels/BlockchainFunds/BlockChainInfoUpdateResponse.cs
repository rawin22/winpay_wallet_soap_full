using Newtonsoft.Json;

namespace TSG.Models.ServiceModels
{
    public class BlockChainInfoUpdateResponse
    {
        [JsonProperty("id")]    public int IndexId { get; set; }
        [JsonProperty("addr")]  public string Address { get; set; }
        [JsonProperty("op")]  public string Operation { get; set; }
        [JsonProperty("confs")]  public int Confirmations { get; set; }
        [JsonProperty("callback")] public string CallBack { get; set; }
        [JsonProperty("onNotification")] public string OnNotification { get; set; }
        [JsonProperty("message")]  public string Message { get; set; }
    }
}