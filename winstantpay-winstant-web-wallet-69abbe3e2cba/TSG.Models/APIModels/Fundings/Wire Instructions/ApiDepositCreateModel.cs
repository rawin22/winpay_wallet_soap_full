using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.Wire_Instructions
{
    public class ApiDepositCreateRequest
    {
        public string CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string FXDealId { get; set; }
        public string AmountCurrencyCode { get; set; }
        public decimal FeeAmount { get; set; }
        public string FeeAmountCurrencyCode { get; set; }
        public string ValueDate { get; set; }
        public string Reference { get; set; }
        public string CustomerMemo { get; set; }
        public string BankMemo { get; set; }
    }

    public class ApiCreatedDepositInformation
    {
        public string DepositId { get; set; }
        public string DepositReference { get; set; }
        public string Timestamp { get; set; }
    }


    public class ApiDepositCreateResponse : StandartResponse
    {
        public ApiCreatedDepositInformation DepositInformation { get; set; }
    }
}