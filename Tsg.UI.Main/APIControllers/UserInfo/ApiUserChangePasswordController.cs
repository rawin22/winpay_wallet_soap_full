using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Http;
using Tsg.Business.Model.Classes;
using Tsg.UI.Main.ApiMethods.UserInfoMethods;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;

namespace Tsg.UI.Main.APIControllers.UserInfo
{
    /// <summary>
    /// Api method for change password by user
    /// </summary>
    [ApiFilter]
    public class ApiUserChangePasswordController : ApiController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpPost]
        public IHttpActionResult Post([FromBody] UserLoginModel userLoginModel)
        {
            try
            {
                UserRepository userRepository = new UserRepository();
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out var ui))
                {
                    if (userLoginModel.OldPassword != ui.Password)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse(GlobalRes.User_UserController_ChangePassword_PassNotMatched));
                    userLoginModel.Username = ui.UserName;
                    if (userLoginModel.Password.Equals(userLoginModel.RepeatPassword))
                    {
                        string hashedPassword = UserLoginModel.GetSha1(userLoginModel.Password);
                        UserRepository userRepo = new UserRepository();
                        var user = userRepo.GetUsers().FirstOrDefault(f => f.Username.ToLower() == ui.UserName.ToLower() /*&& f.UserPassword == UserLoginModel.GetSha1(AppSecurity.CurrentUser.Password)*/);
                        if (user != null && user.IsLocal)
                        {
                            if (userRepo.ChangePassword(userLoginModel, hashedPassword))
                            {
                                return Ok(new StandartResponse(true, GlobalRes.User_UserController_ChangePassword_Success));
                            }
                            return Content(HttpStatusCode.BadRequest, new StandartResponse(GlobalRes.User_UserController_ChangePassword_UnSuccess));
                        }
                        else if (user != null)
                        {
                            UserInfoMethods uiMethods = new UserInfoMethods(ui);
                            var changePasswordServRes = uiMethods.ChangePassword(userLoginModel.OldPassword, userLoginModel.Password);
                            if (!changePasswordServRes.ServiceResponse.HasErrors)
                            {
                                if (userRepo.ChangePassword(userLoginModel, hashedPassword))
                                {
                                    return Ok(new StandartResponse(true, GlobalRes.User_UserController_ChangePassword_Success));
                                }
                                return Content(HttpStatusCode.BadRequest, new StandartResponse(GlobalRes.User_UserController_ChangePassword_UnSuccess));
                            }
                            else
                            {
                                return Content(HttpStatusCode.BadRequest, new StandartResponse($"{changePasswordServRes.ServiceResponse.Responses[0].MessageDetails}"));
                            }
                        }
                        else
                        {
                            UserInfoMethods uiMethods = new UserInfoMethods(ui);
                            var changePasswordServRes = uiMethods.ChangePassword(userLoginModel.OldPassword, userLoginModel.Password);
                            if (!changePasswordServRes.ServiceResponse.HasErrors)
                            {
                                    return Ok(new StandartResponse(true, GlobalRes.User_UserController_ChangePassword_Success));
                            }
                            return Content(HttpStatusCode.BadRequest, new StandartResponse(GlobalRes.User_UserController_ChangePassword_UnSuccess));
                        }
                    }
                    else
                    {
                        return Content(HttpStatusCode.BadRequest, new StandartResponse(GlobalRes.User_UserController_ChangePassword_UnSuccess));
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Content(HttpStatusCode.BadRequest, new StandartResponse("We have problem with your request", e.Message));
            }
            return Content(HttpStatusCode.BadRequest, new StandartResponse(false, "Undefined error"));
        }
    }
}