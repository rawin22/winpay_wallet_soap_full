using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSG.Models.APIModels.Payment
{
    public class ApiDeletePaymentResquest 
    {
        public string PaymentId { get; set; }
        public string Timestamp { get; set; }
    }
}
