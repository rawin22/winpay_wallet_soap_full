using System;
using Newtonsoft.Json;

namespace TSG.Models.ServiceModels
{
    public class AddFunds_BlockChainInfo
    {
        [JsonIgnore] public int AddFundsBlockChainInfo_Id { get; set; }
        [JsonProperty("alias")]public string AddFundsBlockChainInfo_Alias { get; set; } // User message for fundings
        [JsonProperty("message")]public string AddFundsBlockChainInfo_Message { get; set; } // User message for fundings
        [JsonProperty("blockchain_address")]public string AddFundsBlockChainInfo_BlockChainAddress { get; set; } // After 'Recieve' method response - Generated Address
        [JsonProperty("callback_address")]public string AddFundsBlockChainInfo_CallBackAddress { get; set; } // CallbackAddress for send on BlockChain.info API
        [JsonProperty("index")]public int AddFundsBlockChainInfo_Index { get; set; } // After 'Recieve' method response - Generated Index
        [JsonProperty("timestamp")]public Guid AddFundsBlockChainInfo_TimeStampPayment { get; set; } // Unique field for fix funding
        [JsonProperty("parent_id")]public Guid AddFundsBlockChainInfo_ParentId { get; set; }
        [JsonIgnore] public int AddFundsBlockChainInfo_CurrencyIndex { get; set; }
        [JsonProperty("currencyCode")] public string AddFundsBlockChainInfo_CurrencyCode { get; set; }
        [JsonIgnore] public decimal AddFundsBlockChainInfo_TotalValue { get; set; }

        [JsonProperty("user_url")] public string AddFundsBlockChainInfo_UserUrl { get; set; }
        

        [JsonIgnore] public string AddFundsBlockChainInfo_TransactionHash { get; set; }

        [JsonIgnore] public string AddFundsBlockChainInfo_DestinatedBitcoinAddress { get; set; }

        [JsonIgnore] public int? AddFundsBlockChainInfo_NumberOfConfirmation { get; set; }

        [JsonIgnore] public long? AddFundsBlockChainInfo_ValueInSatoshi { get; set; }

        [JsonIgnore] public string AddFundsBlockChainInfo_CustomParameter { get; set; }

        [JsonIgnore] public int? AddFundsBlockChainInfo_ConfirmatedTransaction { get; set; }

        [JsonIgnore] public long AddFundsBlockChainInfo_TransactionId { get; set; }


        [JsonIgnore] public string AddFundsBlockChainInfo_RequestUrl { get; set; }


        [JsonIgnore] public string AddFundsBlockChainInfo_Operation { get; set; }

        [JsonIgnore] public string AddFundsBlockChainInfo_CCY { get; set; }

        // dbo.Funding.proofDocId -> dbo.AddFunds_Wire.proofDocId (FK_Fundings_ProofDoc)
        public virtual Fundings AddFundsWire_Fundings { get; set; }
    }
}