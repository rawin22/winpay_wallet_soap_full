using System;
using System.Net;

namespace TSG.Models.APIModels
{
    public class TsgApiModels
    {
        public class Merchant
        {
            public string MerchantUniqueKey { get; set; }
            public string MerchantPublicKeyToken { get; set; }
            public string MerchantWebSiteCallBackAddress { get; set; }
        }

        public class TokenGuid
        {
            public string KeyOfTokenGuid { get; set; }
            public ResultAnswer ResultAnswer { get; set; }
        }

        public class AnswerForOrder : TokenGuid
        {
            public bool IsSuccesifull { get; set; }
        }

        public class LoginResult : TokenGuid
        {
            public string ReturnUrl { get; set; }
        }

        public class LoginStruct : Merchant
        {
            public string KeyOfTokenGuid { get; set; }
            public string Source { get; set; }
            public string Login { get; set; }
            public string Password { get; set; }
        }

        //public class UserInfo
        //{
        //    public string UserName { get; set; }
        //    public string OrderGuid { get; set; }
        //    public string Stoke { get; set; }
        //    public string UserTokenInTsgSystem { get; set; }
        //}

        public class ResultAnswer
        {
            public DateTime ResultDate { get; set; }
            public string ResultText { get; set; }
            public HttpStatusCode ResultCode { get; set; }
        }

        public class Accounting
        {
            public int CurrencyValute { get; set; }
            public string CurrencyName { get; set; }
            public double CurrencyQuantity { get; set; }
        }

        public class CustomerDetails
        {
            public string CustomerAlias { get; set; }

            public string CustomerCountry { get; set; }

            //public List<Accounting> CustomerAccountings { get; set; }
            public ResultAnswer ResultAnswer { get; set; }
        }

        public class GetOrderFromMerchant
        {
            /*
             OrderToken: orderToken, IsPay:true,
                MerchantUniqueKey: "8FCEAEFB-CA0F-461E-A407-C479A389EF24", Quantity: quantity}
             */
            public string OrderToken { get; set; }

            public string IsPay { get; set; }
            public string UserUniqueKey { get; set; }
            public string MerchantUniqueKey { get; set; }
            public string Quantity { get; set; }
            public string OrderCurrency { get; set; }
        }
    }
}