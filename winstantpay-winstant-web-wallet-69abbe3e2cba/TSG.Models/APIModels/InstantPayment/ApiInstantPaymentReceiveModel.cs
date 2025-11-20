using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TSG.Models.APIModels.InstantPayment;

namespace TSG.Models.APIModels
{
    public class ApiInstantPaymentReceiveModel
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

        public class ApiCreateInstantPaymentReceiveRequest
        {
            public string Alias { get; set; }
            public string Currency { get; set; }
            public decimal Amount { get; set; }
            public string Invoice { get; set; }
            public string Memo { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string Email { get; set; }
            public string KycId { get; set; }
        }

        public class ApiInstantPaymentReceiveResponse
        {
            public Guid InstantPaymentReceiveId { get; set; }
            public string Alias { get; set; }
            public string Currency { get; set; }
            public decimal Amount { get; set; }
            public string Invoice { get; set; }
            public string Memo { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string Email { get; set; }
            public string ShortenedUrl { get; set; }


        }
        public class ApiCreateInstantPaymentReceiveResponse : StandartResponse
        {
            public ApiInstantPaymentReceiveDetailsViewModel InstantPaymentReceiveDetails { get; set; }
        }

        public class ApiEditInstantPaymentReceiveRequest
        {
            public Guid InstantPaymentReceiveId { get; set; }
            public string Alias { get; set; }
            public string Currency { get; set; }
            public decimal Amount { get; set; }
            public string Invoice { get; set; }
            public string Memo { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string Email { get; set; }
            public string KycId { get; set; }
        }

        public class ApiEditInstantPaymentReceiveResponse : StandartResponse
        {
            public ApiInstantPaymentReceiveDetailsViewModel InstantPaymentReceiveDetails { get; set; }
        }
    }
}