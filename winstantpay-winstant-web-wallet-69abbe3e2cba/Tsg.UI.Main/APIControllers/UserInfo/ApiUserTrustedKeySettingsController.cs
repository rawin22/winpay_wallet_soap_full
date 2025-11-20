using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using Tsg.UI.Main.ApiMethods.UserInfoMethods;
using TSG.Models.APIModels;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Repository;
using TSG.ServiceLayer.LimitPayment.DaUserWPayIDSettingServiceMethods;
using System.Net;
using Tsg.UI.Main.App_LocalResources;
using TSG.Models.ServiceModels.LimitPayment;

namespace Tsg.UI.Main.APIControllers.UserInfo
{
    [ApiFilter]
    public class ApiUserTrustedKeySettingsController : ApiController
    {
        private readonly IDaUserWPayIDSettingServiceMethods _daUserWPayIDSettingServiceMethods;

        readonly static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public ApiUserTrustedKeySettingsController(IDaUserWPayIDSettingServiceMethods daUserWPayIDSettingServiceMethods)
        {            
            _daUserWPayIDSettingServiceMethods = daUserWPayIDSettingServiceMethods;
        }

        [System.Web.Http.HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                TSG.Models.APIModels.UserInfo ui;
                UserRepository userRepository = new UserRepository();
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    var query = _daUserWPayIDSettingServiceMethods.GetAll();
                    var lists = query.Obj?.Where(w =>
                        !w.IsDeleted).OrderByDescending(ob => ob.CreationDate).ToList() ?? new List<DaUserWPayIDSettingSo>();

                    return Ok(new StandartResponse<List<DaUserWPayIDSettingSo>>(lists, query.Success, query.Message));
                }
                return Content(HttpStatusCode.Unauthorized, new StandartResponse("Undefined user"));
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Content(HttpStatusCode.BadRequest, new StandartResponse { InfoBlock = new InfoBlock() { UserMessage = GlobalRes.ShopingCardController_UnspecError, DeveloperMessage = e.Message } });
            }
        }

        [System.Web.Http.HttpGet]
        public IHttpActionResult Get(string wpayId)
        {
            try
            {
                TSG.Models.APIModels.UserInfo ui;
                UserRepository userRepository = new UserRepository();
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    var query = _daUserWPayIDSettingServiceMethods.GetAll();
                        // .Obj?.Where(s => s.WPayId == wpayId).FirstOrDefault() ?? new DaUserWPayIDSettingSo();
                    var setting = query.Obj?.Where(s =>
                        !s.IsDeleted && s.WPayId==wpayId).OrderByDescending(ob => ob.CreationDate).FirstOrDefault() ?? new DaUserWPayIDSettingSo();

                    return Ok(new StandartResponse<DaUserWPayIDSettingSo>(setting, query.Success, query.Message));
                }
                return Content(HttpStatusCode.Unauthorized, new StandartResponse("Undefined user"));
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Content(HttpStatusCode.BadRequest, new StandartResponse { InfoBlock = new InfoBlock() { UserMessage = GlobalRes.ShopingCardController_UnspecError, DeveloperMessage = e.Message } });
            }
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult Post(string aliasName)
        {
            var result = new StandartResponse
            {
                Success = false,
                InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty },
            };

            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    var userInfoMethods = new UserInfoMethods(ui);
                    result.Success = userInfoMethods.AddNewAlias(aliasName, out var res);
                    if(result.Success)
                        result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK", UserMessage = "WPayId successfully added" };
                    else 
                        result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = $"Error {res}", UserMessage = "This Account Number or WPayId already exists" };
                }
                else
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.CheckUserError, DeveloperMessage = "Data for user not correct", UserMessage = "Data for user is not correct" };
                }
            }
            catch (ApiException apiException)
            {
                result.InfoBlock = apiException.CustomInfoBlock;
            }
            catch (Exception e)
            {
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
            }
            return Ok(result);
        }
    }
}