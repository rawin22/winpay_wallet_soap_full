using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.Payment
{
    public class ApiPaymentsListResponse : StandartResponse
    {
       public IList<ApiPaymentDetailsModel> PayoutsList { get; set; } = new List<ApiPaymentDetailsModel>();
    }    
}