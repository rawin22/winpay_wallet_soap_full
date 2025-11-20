using Newtonsoft.Json;

namespace TSG.Models.APIModels.KycIntegrationModel
{
    public class CustomerCreateFromTemplateModel
    {
        [JsonProperty("first_name")] [JsonRequired] public string FirstName { get; set; }
        [JsonProperty("last_name")] [JsonRequired] public string LastName { get; set; }
        [JsonProperty("address_line1")] [JsonRequired] public string AddressLine1 { get; set; }
        [JsonProperty("city")] [JsonRequired] public string City { get; set; }
        [JsonProperty("postal_code")] [JsonRequired] public string PostalCode { get; set; }
        [JsonProperty("phone")] [JsonRequired] public string Phone { get; set; }
        [JsonProperty("account_number")] [JsonRequired] public string AccountNumber { get; set; }
        [JsonProperty("customer_template_id")] [JsonRequired] public string CustomerTemplateId { get; set; }
        [JsonProperty("country_code")] [JsonRequired] public string CountryCode { get; set; }
        [JsonProperty("account_representative_id")] [JsonRequired] public string AccountRepresentativeId { get; set; }

        [JsonProperty("customer_name")] public string CustomerName { get; set; }
        [JsonProperty("customer_type_id")] public int CustomerTypeID { get; set; }
        [JsonProperty("middle_name")] public string MiddleName { get; set; }
        [JsonProperty("address_line2")] public string AddressLine2 { get; set; }
        [JsonProperty("state")] public string State { get; set; }
        [JsonProperty("fax")] public string Fax { get; set; }
        [JsonProperty("tax_id")] public string TaxId { get; set; }
    }
}