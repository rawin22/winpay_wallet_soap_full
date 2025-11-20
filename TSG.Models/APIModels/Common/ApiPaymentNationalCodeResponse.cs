using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSG.Models.APIModels.Common
{
    public class ApiPaymentNationalCodeResponse : StandartResponse
    {
        public IList<ApiPaymentNationalCodeDetailsModel> NationalCodesList { get; set; } = new List<ApiPaymentNationalCodeDetailsModel>();
    }
}
