using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSG.Models.APIModels.Payment
{
    public class ApiCreatedPayment
    {
        public string PaymentId { get; set; }
        public string PaymentReference { get; set; }
        public string Timestamp { get; set; }
    }

    public class ApiCreatePaymentResponse : StandartResponse
    {
        public ApiCreatedPayment Payment { get; set; }
    }
}
