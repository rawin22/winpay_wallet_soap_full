using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.Business.Model.TSGgpwsbeta;

namespace Tsg.Business.Model.Classes
{
    public class IgpCurrencyExchange
    {
        private static readonly string CallerId = ConfigurationManager.AppSettings["callerId"].ToString();
        //private const string CUSTOMER_ID = "315622a5-fa28-e211-96b3-002590067f61";
        private string LoginId { get; set; }
        private string Password { get; set; }
        private string CustomerId { get; set; }

        private readonly GPWebServiceSoapClient _exchanceService;

        public IgpCurrencyExchange()
        {
            _exchanceService = new GPWebServiceSoapClient();
        }

        public IgpCurrencyExchange(string username, string password, string customerId)
        {
            this.LoginId = username;
            this.Password = password;
            this.CustomerId = customerId;
            _exchanceService = new GPWebServiceSoapClient();
        }

        public void SetUserCredentials(string username, string password, string customerId)
        {
            this.LoginId = username;
            this.Password = password;
            this.CustomerId = customerId;
        }

        private ServiceCallerIdentity GetServiceCallerIdentity()
        {
            return new ServiceCallerIdentity()
            {
                LoginId = LoginId,
                Password = Password,
                ServiceCallerId = CallerId
            };
        }

        private ServiceCallerIdentity GetServiceCallerIdentity(string login, string password)
        {
            this.LoginId = login;
            this.Password = password;
            return GetServiceCallerIdentity();
        }

        public Guid GetCustomerIdGuid
        {
            get
            {
                return new Guid(CustomerId);
            }
        }

        public GetPaymentCurrenciesResult GetFxCurrenciesList()
        {
            var res = new GetPaymentCurrenciesResult();
            try
            {
                res = _exchanceService.GetPaymentCurrencies(LoginId, Password);
            }
            catch (Exception e)
            {
            }

            return res;
        }

        public GetFXDealBuyCurrenciesResult GetFxDealBuyCurrencies()
        {
            var res = new GetFXDealBuyCurrenciesResult();
            try
            {
                res = _exchanceService.GetFXDealBuyCurrencies(LoginId, Password);
            }
            catch (Exception e)
            {
            }

            return res;
        }

        public GetFXDealSellCurrenciesResult GetFxDealSellCurrencies()
        {
            var res = new GetFXDealSellCurrenciesResult();
            try
            {
                res = _exchanceService.GetFXDealSellCurrencies(LoginId, Password);
            }
            catch (Exception e)
            {
            }
            return res;
        }


        public GetFXDealQuoteResult GetFxDealQuoteBasic(string buyCurrencyCode, string sellCurrencyCode, decimal amount, string amountCurrencyCode, bool isForCurrencyCalculator, string customerReference = "371380249582007")
        {
            GetFXDealQuoteResult result = new GetFXDealQuoteResult();
            try
            {
                result = _exchanceService.GetFXDealQuoteBasic(LoginId, Password, customerReference, buyCurrencyCode, sellCurrencyCode, amount, sellCurrencyCode, false);
            }
            catch (Exception e)
            {
                result.ServiceErrorList.Add(new ServiceError(){ErrorTitle = "Ewallet exchange service error", ErrorCode = 800, ErrorMessage = e.Message});   
            }

            return result;
        }
        public BookFXDealResult BookFxDeal(string quotaId)
        {
            BookFXDealResult result = new BookFXDealResult();
            try
            {
                result = _exchanceService.BookFXDeal(LoginId, Password, quotaId);
            }
            catch (Exception e)
            {
                result.ServiceErrorList.Add(new ServiceError() { ErrorTitle = "Ewallet exchange service error", ErrorCode = 801, ErrorMessage = e.Message });
            }

            return result;
        }
    }
}
