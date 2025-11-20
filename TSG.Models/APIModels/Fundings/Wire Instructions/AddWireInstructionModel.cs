using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TSG.Models.ServiceModels;

namespace TSG.Models.APIModels.Fundings.Wire_Instructions
{
    public class BankCurrencyData
    {
        [JsonProperty("bank_ccy_id")]public int Id { get; set; }
        [JsonProperty("bank_ccy_name")]public string BankCcyName { get; set; }
    }


    public class AddWireInstructionModel : StandartResponse
    {
        [JsonProperty("list_of_linked_bank_and_ccy")] public List<BankCurrencyData> ListOfLinkedBankAndCcy { get; set; }
        [JsonProperty("wire_instruction")] public string WireInstruction { get; set; }
    }

    public class NewWireTransferInfo : StandartResponse
    {
        [JsonProperty("list_of_linked_bank_and_ccy")] public List<BankCurrencyData> ListOfLinkedBankAndCcy { get; set; }

        [JsonProperty("full_name")] public string FullName { get; set; }
    }

    public class WireDetails : StandartResponse
    {
        [JsonProperty("funding_tab_id")] public int WireDetails_ProofDocId { get; set; }
        [JsonProperty("funding_tab_customer_name")] public string WireDetails_CustName { get; set; }
        [JsonProperty("funding_tab_bank_name")] public string WireDetails_BankName { get; set; }
        [JsonProperty("funding_tab_last_four_digits")] public string WireDetails_LastFourDigits { get; set; }
        [JsonProperty("funding_tab_notes")] public string WireDetails_Other { get; set; }
        [JsonProperty("funding_tab_file_name")] public string WireDetails_FileName { get; set; }
        [JsonProperty("funding_tab_filePath")] public string WireDetails_FilePath { get; set; }
        [JsonProperty("funding_tab_bank_currency_id")] public int? WireDetails_BankCcyId { get; set; }
        [JsonProperty("funding_tab_date_value")] public string WireDetails_PaymentDateString { get; set; }
        [JsonProperty("funding_tab_parent_id")] public Guid WireDetails_ParentId { get; set; }
        [JsonProperty("funding_tab_amount")] public decimal WireDetails_Amount { get; set; }
        [JsonProperty("image_base64")] public string ImageInBase64 { get; set; }
        [JsonProperty("funding_status")] public int WireDetails_Status { get; set; }
        [JsonProperty("funding_is_deleted")] public bool WireDetails_IsDeleted { get; set; }
        [JsonProperty("funding_currency_block")] public virtual Currency Fundings_Currency { get; set; }
        [JsonProperty("funding_last_change")] public virtual FundChanges Fundings_FundChange { get; set; }
        [JsonProperty("list_of_linked_bank_and_ccy")] public List<BankCurrencyData> ListOfLinkedBankAndCcy { get; set; } = new List<BankCurrencyData>();
    }
}