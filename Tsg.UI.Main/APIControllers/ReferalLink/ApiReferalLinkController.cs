using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Tsg.UI.Main.ApiMethods.ReferalLinks;
using Tsg.UI.Main.ApiMethods.UserInfoMethods;
using TSG.Models.APIModels;
using TSG.Models.APIModels.ReferalLinksModel;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Repository;

namespace Tsg.UI.Main.APIControllers.ReferalLink
{
    /// <summary>
    /// Get referal link
    /// </summary>
    [ApiFilter]
    public class ApiReferalLinkController : ApiController
    {

        readonly log4net.ILog _logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Method gets referal link and aliases list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get()
        {
            var result = new ReferalLinksModel();
            UserRepository userRepository = new UserRepository();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out var ui))
                {
                    var userInfoMethods = new UserInfoMethods(ui);
                    var getReferalLink = new ReferalLinksMethods();
                    result = getReferalLink.GetReferalLinks();
                    result.UserAliases = userInfoMethods.GetUserAliases();
                    result.UserAliases = userInfoMethods.GetUserAliases();
                    return Ok(result);
                }
                return Content(HttpStatusCode.Unauthorized, "User doesn't recognized");
            }
            catch (ApiException apiException)
            {
                result.InfoBlock = apiException.CustomInfoBlock;
                _logger.Error(apiException);
            }
            catch (Exception e)
            {
                result.InfoBlock = new InfoBlock()
                {
                    Code = ApiErrors.ErrorCodeState.UnspecifiedError,
                    DeveloperMessage = e.InnerException?.ToString() ?? e.Message,
                    UserMessage = "We have some problem with your query. Please try again"
                };
                _logger.Error(e);
            }

            return Ok(result);
        }
    }
}