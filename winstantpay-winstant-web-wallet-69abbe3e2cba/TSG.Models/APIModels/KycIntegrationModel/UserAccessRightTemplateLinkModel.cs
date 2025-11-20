using Newtonsoft.Json;

namespace TSG.Models.APIModels.KycIntegrationModel
{
    public class UserAccessRightTemplateLinkModel 
    {
        [JsonProperty("user_id")] [JsonRequired] public string UserId { get; set; }
        [JsonProperty("access_right_template_id")] [JsonRequired] public string AccessRightTemplateId { get; set; }
    }
}