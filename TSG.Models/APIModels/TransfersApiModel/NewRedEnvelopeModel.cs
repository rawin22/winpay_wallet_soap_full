using System;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.TransfersApiModel
{
    public class NewRedEnvelopeModel
    {
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("wpay_id_to")]
        public string WpayIdTo { get; set; }

        [JsonProperty("wpay_id_from")]
        public string WpayIdFrom { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("is_kyc_created")]
        public bool IsKycCreated { get; set; }
        
        [JsonProperty("kyc_id")]
        public Guid? KycId { get; set; }
        [JsonProperty("kyc_username")]
        public string KycUserName { get; set; }
    }
}