using System;
using System.Collections.Generic;
using System.Web;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace TSG.Models.ServiceModels
{
    public class AddFundsWire
    {
        [JsonProperty("funding_tab_id")] public int AddFundsWire_ProofDocId { get; set; }
        [JsonProperty("funding_tab_customer_name")] public string AddFundsWire_CustName { get; set; }
        [JsonProperty("funding_tab_bank_name")] public string AddFundsWire_BankName { get; set; }
        [JsonProperty("funding_tab_last_four_digits")] public string AddFundsWire_LastFourDigits { get; set; }
        [JsonProperty("funding_tab_notes")] public string AddFundsWire_Other { get; set; }
        [JsonProperty("funding_tab_file_name")] public string AddFundsWire_FileName { get; set; }
        [JsonProperty("funding_tab_filePath")] public string AddFundsWire_FilePath { get; set; }
        [JsonProperty("funding_tab_bank_currency_id")] public int? AddFundsWire_BankCcyId { get; set; }
        [JsonIgnore] public DateTime AddFundsWire_PaymentDate { get; set; }
        [JsonProperty("funding_tab_date_value")] public string AddFundsWire_PaymentDateString { get; set; }
        [JsonProperty("funding_tab_parent_id")] public Guid AddFundsWire_ParentId { get; set; }
        [JsonProperty("funding_tab_amount")] public decimal AddFundsWire_Amount { get; set; }
        [JsonProperty("image_base64")] public string ImageInBase64 { get; set; }
        [JsonIgnore] public string Commentaries { get; set; }
        [JsonIgnore] public int Status { get; set; }

        // dbo.Funding.proofDocId -> dbo.AddFunds_Wire.proofDocId (FK_Fundings_ProofDoc)
        [JsonProperty("funding")] public Fundings AddFundsWire_Fundings { get; set; }
        [JsonIgnore] public HttpPostedFileBase AddFundsWire_PostedFile { get; set; }
    }
}
