using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.KycIntegrationModel
{
    public class CustomerUserCreateResponseModel : StandartResponse
    {
        [JsonProperty("user_id")] public string UserId { get; set; }
        [JsonProperty("new_password")] public string NewPassword { get; set; }
    }
}