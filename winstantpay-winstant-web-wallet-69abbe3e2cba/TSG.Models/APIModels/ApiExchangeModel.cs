using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TSG.Models.APIModels
{
    public class ApiExchangeModel
    {
        public class CurrencyAndAccAmount
        {
            public string AccountId { get; set; }
            public string AccountText { get; set; }
        }

        public class GetAccountValue : StandartResponse
        {
            public IList<CurrencyAndAccAmount> Data { get; set; }
        }
    }
}