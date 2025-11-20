using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.InstantPayment
{
    public class PaymentHistoryInfoModel : StandartResponse
    {
        [JsonProperty("payment_details")] public ApiInstantPaymentDetailsViewModel PaymentDetails { get; set; }
    }

    public class ApiInstantPaymentDetailsViewModel
    {
        public Guid PaymentId { get; set; }
        public string PaymentReference { get; set; }
        public string FromCustomerAlias { get; set; }
        public string ToCustomerAlias { get; set; }
        public string FromCustomerName { get; set; }
        public string FromCustomerId { get; set; }
        public string ToCustomerName { get; set; }
        public string ToCustomerId { get; set; }
        public string PaymentStatus { get; set; }

        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public decimal Amount { get; set; }
        public int AmountCurrencyScale { get; set; }
        public string Currency { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime ValueDate { get; set; }
        public string ProcessingBranchName { get; set; }
        public string ProcessingBranchCode { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? PostedTime { get; set; }
        public string PostedByName { get; set; }
        public bool IsDeleted { get; set; }
        public string BankMemo { get; set; }
        public string Invoice { get; set; }
        public string ReasonForPayment { get; set; }
        public string ImageUrl { get; set; }
    }
}