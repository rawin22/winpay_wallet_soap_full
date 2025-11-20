using System;
using System.Web.Helpers;
using Newtonsoft.Json;

namespace TSG.Models.APIModels
{
    public class UserTokenData
    {
        public string Token { get; set; }
        public DateTime ExpiredDate { get; set; }
    }
    public class LiquidCurrency
    {
        public string ids { get; set; }
    }
    public class LoginPageModelRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string MerchantAppIdentificator { get; set; }
    }

    public class LoginPageModelResponse : StandartResponse
    {
        public SendedDataByUser Data { get; set; }
    }

    public class UserData
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class LimitationString
    {
        [JsonProperty("DelegatedAuthorityCode")]public string DelegatedAuthorityCode { get; set; }
    }

    public class SendedDataByUser
    {
        public UserTokenData TokenData { get; set; }
        public string Role { get; set; }
        public string LastLogInDate { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string OrganizationId { get; set; }
        public string UserEmail { get; set; }
    }

    public class ExtendedLoginParameters
    {
        public UserInfo UserInformation { get; set; }
        public string UserId  { get; set; }
        public string TokenId { get; set; }
        public string TokenKey { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string MerchatAppIdentificator { get; set; }
    }
}