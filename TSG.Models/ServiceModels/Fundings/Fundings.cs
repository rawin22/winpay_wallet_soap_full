using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TSG.Models.ServiceModels
{
    public class Fundings
    {
        [JsonProperty("funding_id")] public Guid Fundings_Id { get; set; }
        [JsonProperty("funding_createDate")] public DateTime Fundings_CreateDate { get; set; }
        [JsonProperty("funding_username")] public string Fundings_Username { get; set; }
        [JsonProperty("funding_amount")] public decimal Fundings_Amount { get; set; }
        [JsonIgnore] public int Fundings_CurrencyId { get; set; }
        [JsonIgnore] public Guid Fundings_SourceType { get; set; }
        [JsonIgnore] public int? Fundings_LastActivityId { get; set; }
        [JsonProperty("funding_is_deleted")] public bool Fundings_IsDeleted { get; set; }
        [JsonIgnore] public int Fundings_StatusByFund { get; set; }
        // dbo.Fundings.Username -> dbo.User.username (FK_Fundings_User)
        [JsonIgnore] public virtual User Fundings_User { get; set; }
        // dbo.Fundings.CurrencyId -> dbo.Currency.ccyId (FK_Fundings_Currency)
        [JsonProperty("funding_currency_block")] public virtual Currency Fundings_Currency { get; set; }
        // dbo.Fundings.SourceType -> dbo.FundingSources.ID (FK_Fundings_SourceFund)
        [JsonProperty("funding_source_block")] public virtual FundingSources Fundings_FundingSource { get; set; }
        // dbo.Fundings.LastActivityId -> dbo.FundChanges.fndStatChangeId (FK_Fundings_FundActivity)
        [JsonProperty("funding_last_change")] public virtual FundChanges Fundings_FundChange { get; set; }
    }
}
