using Newtonsoft.Json;

namespace TSG.Models.APIModels.InstantPayment
{
    public class ApiInstantPaymentAfterPost : StandartResponse
    {
        [JsonProperty("image_link")]public string ImageLink { get; set; }
    }
}