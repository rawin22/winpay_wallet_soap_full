using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace TSG.Models.APIModels
{
    public class ApiInstantPaymentHistoryModel
    {
        public class HistoryData
        {
            [JsonProperty("payment_id")]public string PaymentId{ get; set; }
            [JsonProperty("payment_ref")] public string PaymentRef { get; set; }
            [JsonProperty("is_sent_payment")]public bool IsSentPayment { get; set; }
            [JsonProperty("creation_date")]public long CreationDate { get; set; }
            [JsonProperty("to")]public string To { get; set; }
            [JsonProperty("from")] public string From { get; set; }
            [JsonProperty("text_info")] public string TextInfo { get; set; }
            [JsonProperty("status")] public string Status { get; set; }
            [JsonProperty("payment_data")]public ApiCommonModel.BalanceData HistoryBalanceData { get; set; }
        }
        
        public class MainPageModel: StandartResponse
        {
            [JsonProperty("balances")] public List<ApiCommonModel.BalanceData> BalanceDatasList { get; set; }
            [JsonProperty("history")] public IList<HistoryData> PaymentHistory { get; set; }
        }
    }
}