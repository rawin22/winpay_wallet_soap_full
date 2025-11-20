using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSG.Models.APIModels.Common
{
    public class ApiPaymentCountriesResponse : StandartResponse
    {
        public IList<ApiPaymentCountryDetailsModel> CountriesList { get; set; } = new List<ApiPaymentCountryDetailsModel>();
    }
}
