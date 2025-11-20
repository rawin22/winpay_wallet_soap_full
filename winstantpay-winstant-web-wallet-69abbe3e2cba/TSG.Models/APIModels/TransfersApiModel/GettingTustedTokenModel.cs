using System;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.TransfersApiModel
{
    public class GettingTustedTokenModel
    {
        [JsonProperty("transfered_record_id")]public Guid TransferedRecordId { get; set; }
        [JsonProperty("action")]public bool Action { get; set; }
        [JsonProperty("rejection_note")]public string RejectionNote { get; set; }
    }
}