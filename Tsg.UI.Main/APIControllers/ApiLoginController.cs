using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Tsg.UI.Main.ApiMethods.UserInfoMethods;
using TSG.Models.APIModels;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Attributes;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.ServiceModels.UsersDataBlock;
using TSG.ServiceLayer.Users;
using Crypto = WinstantPay.Common.CryptDecriptInfo.Crypto;

namespace Tsg.UI.Main.APIControllers
{
    [ApiFilter]
    public class ApiLoginController : ApiController
    {
        private readonly IUsersServiceMethods _usersServiceMethods;
        public ApiLoginController(IUsersServiceMethods usersServiceMethods)
        {
            _usersServiceMethods = usersServiceMethods;
        }


        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
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
                    throw new ApiException(new InfoBlock() { DeveloperMessage = "Authorization InfoBlock. Password or login for user does not match.", UserMessage = "Invalid login or password", Code = ApiErrors.ErrorCodeState.AuthorizationFailed });
                var checkUserRes = _usersServiceMethods.InsertOrUpdateInfo(generateTokenData.UserInformation);
                if (!checkUserRes.Obj.User_IsLocal.HasValue || !checkUserRes.Obj.User_IsLocal.Value)
                {
                    var uiMethod = new UserInfoMethods(generateTokenData.UserInformation);
                    var getUserWpayIds = uiMethod?.GetUserAliases();
                    if (getUserWpayIds.Count > 0)
                    {
                        _usersServiceMethods.SaveUserAliases(new UserAliasesSo()
                        {
                            Wpay_UserId = Guid.Parse(generateTokenData.UserInformation.UserId),
                            Wpay_UserName = generateTokenData.UserInformation.UserName,
                            Wpay_Ids = getUserWpayIds
                        });
                    }
                }

                generateTokenData.CreationDate = DateTime.Now;
                generateTokenData.TokenId = Guid.NewGuid().ToString();
                generateTokenData.UserId = apiservRes.UserInfo.UserId;
                generateTokenData.UserInformation.Password = Crypto.Encrypt(decodedPassword, generateTokenData.TokenId);
                generateTokenData.TokenKey = Crypto.Encrypt(JsonConvert.SerializeObject(apiservRes.UserInfo), generateTokenData.TokenId);
                generateTokenData.MerchatAppIdentificator = value.MerchantAppIdentificator;
                if (!userRepository.CreateUserToken(ref generateTokenData) || generateTokenData.ExpiredDate == default(DateTime))
                    throw new ApiException(new InfoBlock() { DeveloperMessage = "DB get infoBlock when generated token.", UserMessage = "Service infoBlock, please try later.", Code = ApiErrors.ErrorCodeState.SqlTokenGenerationError });
                result.Data = new SendedDataByUser(){
                    UserName = generateTokenData.UserInformation.UserName, 
                    Role = generateTokenData.UserInformation.RoleName,
                    FullName = String.Format("{0} {1}", generateTokenData.UserInformation.FirstName, generateTokenData.UserInformation.LastName),
                    LastLogInDate = generateTokenData.UserInformation.LastLoginDate.ToString("F"),
                    TokenData = new UserTokenData() { ExpiredDate = generateTokenData.ExpiredDate,  Token = generateTokenData.TokenKey }, 
                    UserId = generateTokenData.UserId,
                    OrganizationId = apiservRes.UserInfo?.OrganisationId, 
                    UserEmail = apiservRes.UserInfo?.EmailAddress
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
