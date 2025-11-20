using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.KycIntegrationModel
{
    public class MiniKycAddNewUser
    {
        [JsonRequired]
        [JsonProperty("email_address")]
        [EmailAddress]
        //[RegularExpression(@"^(?()(.+?(?<!\\)@)|(([0-9a-z]((\.(?!\.))|[-!#\\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$", ErrorMessage = "The field EmailAddress must be valid")]
        public string EmailAddress { get; set; }

        [JsonRequired]
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonRequired]
        [JsonProperty("last_name")]
        public string LastName { get; set; }
    }
}
