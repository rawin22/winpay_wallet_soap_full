using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.KycIntegrationModel
{
    public class CustomerUserCreateModel
    {
        [JsonProperty("customer_id")] [JsonRequired] public string CustomerId { get; set; }
        [JsonProperty("user_name")] [JsonRequired] public string UserName { get; set; }
        [JsonProperty("email_address")] [JsonRequired] public string EmailAddress { get; set; }
        [JsonProperty("first_name")] [JsonRequired] public string FirstName { get; set; }
        [JsonProperty("last_name")] [JsonRequired] public string LastName { get; set; }

        [JsonProperty("password")] [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,20}$")] public string Password { get; set; }
        [JsonProperty("is_approved")] public bool IsApproved { get; set; }
        [JsonProperty("user_must_change_password")] public bool UserMustChangePassword { get; set; }
        [JsonProperty("email_password_to_user")] public bool EmailPasswordToUser { get; set; }
    }
}