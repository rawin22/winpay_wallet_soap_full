using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Serialization;
using TSG.Models.APIModels;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Attributes;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using Crypto = WinstantPay.Common.CryptDecriptInfo.Crypto;

namespace Tsg.UI.Main.APIControllers
{

//UserInfo ui;
//if(userRepository.IsVerificatedUser(result.Data.TokenData.Token, out ui))
//Console.WriteLine("Ok");
    [ApiFilter]
    public class LoginController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post([FromBody]LoginPageModelRequest value)
        {
            var result = new LoginPageModelResponse { Success = false, InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty}, Data = new SendedDataByUser()};
            LoginPageModelRequest bodyValue = value;
            try
            {
                if (String.IsNullOrEmpty(bodyValue.Login) || String.IsNullOrEmpty(bodyValue.Password))
                    throw new ApiException(new InfoBlock() { DeveloperMessage = "Login or password can't be empty", UserMessage = "Login or password can't be empty", Code = ApiErrors.ErrorCodeState.LoginOrPasswordIsEmpty });
                
                UserLoginModel model = new UserLoginModel();
                model.Username = bodyValue.Login;
                // TODO: Need to add logic by password decode
                //...........................................
                model.Password = bodyValue.Password;
                var decodedPassword = Crypto.ApiDecript(model.Password);
                //...........................................

                #region token generation procedure
                var generateTokenData = new ExtendedLoginParameters();
                #endregion
                
                UserRepository userRepository = new UserRepository();
                var apiservRes = model.ApiServiceLogin();
                
                if (apiservRes.IsSuccess)
                    generateTokenData.UserInformation = apiservRes.UserInfo;
                var apilocalRes = model.ApiLocalUserLogin();
                if (apilocalRes.IsSuccess)
                    generateTokenData.UserInformation = apilocalRes.UserInfo;
                if (generateTokenData.UserInformation == null || (generateTokenData.UserInformation != null && generateTokenData.UserInformation.Role == UserRoleType.Admin))
                    throw new ApiException(new InfoBlock() { DeveloperMessage = "Authorization Error. Password or login for user does not match.", UserMessage = "Invalid login or password", Code = ApiErrors.ErrorCodeState.AuthorizationFailed });
                generateTokenData.CreationDate = DateTime.Now;
                generateTokenData.TokenId = Guid.NewGuid().ToString();
                generateTokenData.UserInformation.Password = Crypto.Encrypt(decodedPassword, generateTokenData.TokenId);
                generateTokenData.TokenKey = Crypto.Encrypt(generateTokenData.UserInformation.Password, generateTokenData.TokenId);
                if(!userRepository.CreateUserToken(ref generateTokenData) || generateTokenData.ExpiredDate == default(DateTime))
                    throw new ApiException(new InfoBlock() { DeveloperMessage = "DB get error when generated token.", UserMessage = "Service error, please try later.", Code = ApiErrors.ErrorCodeState.SqlTokenGenerationError });
                result.Data = new SendedDataByUser(){UserName = generateTokenData.UserInformation.UserName, Role = generateTokenData.UserInformation.RoleName,
                    FullName = String.Format("{0} {1}", generateTokenData.UserInformation.FirstName, generateTokenData.UserInformation.LastName), LastLogInDate = generateTokenData.UserInformation.LastLoginDate.ToString("F"),
                    TokenData = new UserTokenData() { ExpiredDate = generateTokenData.ExpiredDate,  Token = generateTokenData.TokenKey }
                };
                result.InfoBlock = new InfoBlock(){Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK", UserMessage = "Login and password correct"};
                result.Success = true;
            }
            catch (ApiException apiException)
            {
                result.InfoBlock = apiException.CustomInfoBlock;
            }
            catch (Exception e)
            {
                result.InfoBlock = new InfoBlock(){ Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = e.Message };
            }

            return Ok(result);
        }
    }
}
