using Newtonsoft.Json;

namespace TSG.Models.ServiceModels
{
    public class BlockChainInfoReceiveResponse
    {
        [JsonProperty("index")]    public int Index { get; set; }
        [JsonProperty("address")]  public string Address { get; set; }
        [JsonProperty("callback")] public string CallBack { get; set; }
        [JsonProperty("message")]  public string Message { get; set; }
    }
}