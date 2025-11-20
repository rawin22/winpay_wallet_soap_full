using Newtonsoft.Json;

namespace TSG.Models.APIModels.KycIntegrationModel
{
    public class CustomerCreateFromTemplateRepsonseModel : StandartResponse
    {
        [JsonProperty("customerId")] public string CustomerId { get; set; }
    }
}