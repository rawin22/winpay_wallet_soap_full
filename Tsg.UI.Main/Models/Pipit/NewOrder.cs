using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Tsg.UI.Main.App_LocalResources;

namespace Tsg.UI.Main.Models.Pipit
{
    public class PipitNewOrder
    {
        [JsonProperty("vendorName")] public string VendorName { get; set; }
        [JsonProperty("vendorOrderReference")] public string VendorOrderReference { get; set; }
        [JsonProperty("orderCurrencyCode")]
        [Display(Name = "Shared_Pipit_CreateFieldCurrency", ResourceType = typeof(GlobalRes))]
        public string OrderCurrencyCode { get; set; }
        [JsonProperty("orderValue")]
        [Display(Name = "Shared_Pipit_CreateFieldAmount", ResourceType = typeof(GlobalRes))]
        public decimal OrderValue { get; set; }
        [JsonProperty("customerEmail")]
        [Display(Name = "Shared_Pipit_CreateFieldEmail", ResourceType = typeof(GlobalRes))]
        public string CustomerEmail { get; set; }
        [Display(Name = "Shared_Pipit_CreateFieldMobileNumber", ResourceType = typeof(GlobalRes))]
        [JsonProperty("customerPhone")] public string CustomerPhone { get; set; }

        [Display(Name = "HomeCheckoutIndexPage_ChooseAlias", ResourceType = typeof(GlobalRes))]
        [JsonProperty("selected_alias")] public string Alias { get; set; }

    }
}