using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TSG.Models.ServiceModels
{
    public class AddFundsPipit
    {
        [JsonProperty("id")] public int AddFundsPipit_Id { get; set; }
        [JsonProperty("barcode")] public string AddFundsPipit_BarCode { get; set; }
        [JsonProperty("reference")] public string AddFundsPipit_Reference { get; set; }
        [JsonProperty("vendorReference")] public string AddFundsPipit_VendorReference { get; set; }
        [JsonProperty("orderValue")] public decimal AddFundsPipit_OrderValue { get; set; }
        [JsonProperty("totalValue")] public decimal AddFundsPipit_TotalValue { get; set; }
        [JsonProperty("createdDate")] public DateTime AddFundsPipit_CreatedDate { get; set; }
        [JsonProperty("expiryDate")] public DateTime AddFundsPipit_ExpiredDate { get; set; }
        [JsonProperty("status")] public string AddFundsPipit_Status { get; set; }
        [JsonProperty("parent_id")]public Guid AddFundsPipit_ParentId { get; set; }
        [JsonProperty("payer_alias")] public string AddFundsPipit_Alias { get; set; }
        [JsonProperty("payment_date")] public DateTime? AddFundsPipit_PaymentDate { get; set; }
        [JsonProperty("currencyCode")] public string AddFundsPipit_CurrencyCode { get; set; }

        // dbo.Funding.proofDocId -> dbo.AddFunds_Wire.proofDocId (FK_Fundings_ProofDoc)
        public virtual Fundings AddFundsWire_Fundings { get; set; }
    }
}
