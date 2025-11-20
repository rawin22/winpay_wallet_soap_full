using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TSG.Models.APIModels.InstantPayment;
using TSG.Models.APIModels.Payment;

namespace TSG.Models.APIModels
{
    public class ApiPaymentModel
    {
        public class ApiEditPaymentResponse : StandartResponse
        {
            public ApiPaymentDetailsModel PaymentDetails { get; set; }
        }
    }
}