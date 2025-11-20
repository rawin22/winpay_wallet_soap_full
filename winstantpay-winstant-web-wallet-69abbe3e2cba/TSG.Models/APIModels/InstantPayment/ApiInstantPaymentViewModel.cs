using System;

namespace TSG.Models.APIModels.InstantPayment
{
    public class ApiInstantPaymentViewModel
    {
        public Guid PaymentId { get; set; }
        public string Result { get; set; }
    }
}