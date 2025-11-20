using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Swashbuckle.Swagger;
using Swashbuckle.Swagger.Annotations;
using Tsg.UI.Main.ApiMethods.UserInfoMethods;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.APIModels.UserInformation;
using TSG.Models.ServiceModels;
using TSG.ServiceLayer.Users;

namespace Tsg.UI.Main.APIControllers.UserInfo
{
    [ApiFilter]
    public class ApiGetWelcomeMessageController : ApiController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IUsersServiceMethods _usersServiceMethods;
        public ApiGetWelcomeMessageController(IUsersServiceMethods usersServiceMethods)
        {
            _usersServiceMethods = usersServiceMethods;
        }

        /// <summary>
        /// Get welcome message for user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get()
        {
            UserRepository userRepository = new UserRepository();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out var ui))
                {
                    var checkUserRes = _usersServiceMethods.InsertOrUpdateInfo(ui);
                    return Ok(new StandartResponse<TSG.Models.ServiceModels.User>(checkUserRes.Obj, checkUserRes.Success, checkUserRes.Message));
                }
            }
            catch (ApiException apiException)
            {
                _logger.Error(apiException);
                return Content(HttpStatusCode.BadRequest, new StandartResponse(false, "Error", apiException.Message));
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Content(HttpStatusCode.BadRequest, new StandartResponse(false, "Error", e.Message));
            }
            return Content(HttpStatusCode.BadRequest, new StandartResponse(false, "Undefined error"));
        }

        /// <summary>
        /// Close welome message
        /// </summary>
        /// <returns></returns>
        [Route("ApiHideWelcomeMessage", Name = "", Order = 1)]
        [SwaggerOperation(Tags = new[] { "ApiHideWelcomeMessage" })]
        [HttpPost]
        public IHttpActionResult Post()
        {
            try
            {
                UserRepository userRepository = new UserRepository();
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out var ui))
                {
                    var res = userRepository.ChangeWmStatuses(ui, false);
                    return Ok(new StandartResponse(res, res ? "Ok" : "Error in ChangeWmStatuses method"));
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Content(HttpStatusCode.BadRequest, new StandartResponse(false, "Error", e.Message));
            }
            return Content(HttpStatusCode.BadRequest, new StandartResponse(false, "Undefined error"));
        }
    }
}
