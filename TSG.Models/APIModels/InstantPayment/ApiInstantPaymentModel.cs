using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TSG.Models.APIModels
{
    public class ApiInstantPaymentModel
    {

        public class InstantPaymentPageModelResponse : StandartResponse
        {
            public SendedDataByInstantPayment Data { get; set; }
        }

        public class SendedDataByInstantPayment
        {
            public IList<string> ToAlias { get; set; }
            public IList<string> CurrencyList { get; set; }
            public IList<string> FromAlias { get; set; }
        }

        public class ApiInstantPaymentResponse
        {
            public string PaymentId { get; set; }
            [Required]
            public string FromCustomer { get; set; }
            [Required]
            public string ToCustomer { get; set; }
            [Required]
            public decimal Amount { get; set; }
            [Required]
            public string CurrencyCode { get; set; }
            public string Memo { get; set; }
            public string Invoice { get; set; }
            public string ReasonForPayment { get; set; }
        }

        public class CreatePaymentResponse : StandartResponse
        {
            public ApiInstantPaymentResponse CreationInfo { get; set; }
        }
    }
}