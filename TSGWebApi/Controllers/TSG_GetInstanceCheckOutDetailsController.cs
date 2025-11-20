using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using TSGWebApi.Models;

namespace TSGWebApi.Controllers
{
    public class TSG_GetInstanceCheckOutDetailsController : ApiController
    {
        /// <summary>
        /// Get instance check out details
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Remarks: Get instance check out details
        /// </remarks>
        /// <response code="200"></response>
        [ResponseType(typeof(CustomerDetails))]
        public CustomerDetails Post([FromBody]string tokenId)
        {
            if (String.IsNullOrEmpty(tokenId))
                return new CustomerDetails() { ResultAnswer = new ResultAnswer(){ResultCode = HttpStatusCode.BadRequest, ResultDate = DateTime.Now, ResultText = "String can't be empty"}};
            return new CustomerDetails()
            {
                ResultAnswer = new ResultAnswer(){ResultDate = DateTime.Now, ResultText = "OK", ResultCode = HttpStatusCode.OK },
                CustomerAccountings = new List<Accounting>() { new Accounting(){CurrencyName = "Dollar", CurrencyQuantity = 100, CurrencyValute = Models.Currency.Dollar }, new Accounting(){CurrencyName = "Euro", CurrencyQuantity = 120, CurrencyValute = TSGWebApi.Models.Currency.Euro }},
                CustomerCountry = "USA",
                CustomerName = "Test Test Test",
                ReturnUrl = "www.google.ru" 
            }; 
        }
    }
}