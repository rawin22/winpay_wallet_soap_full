using Newtonsoft.Json;
using TSG.Models.APIModels.UserInformation;

namespace TSG.Models.APIModels.ReferalLinksModel
{
    public class ReferalLinksModel : ListOfUserAliases
    {
        [JsonProperty("referal_link")] public string ReferalLink { get; set; } = string.Empty;
    }
}