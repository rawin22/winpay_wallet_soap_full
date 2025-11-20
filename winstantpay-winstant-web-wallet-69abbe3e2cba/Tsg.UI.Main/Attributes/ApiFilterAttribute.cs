using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using Tsg.UI.Main.APIControllers;
using Tsg.UI.Main.Repository;

namespace Tsg.UI.Main.Attributes
{
    /// <summary>
    /// Filter atribute for Mobile API
    /// </summary>
    public class ApiFilterAttribute : AuthorizationFilterAttribute
    {
        private static readonly string ApiKey = ConfigurationManager.AppSettings["x-api-key"];

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                IEnumerable<string> outerApiKey;
                var isKeyContains = actionContext.Request.Headers.TryGetValues("X-API-KEY", out outerApiKey);
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = actionContext.Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                var apiKey = outerApiKey as string[] ?? outerApiKey.ToArray();
                UserRepository userRepository = new UserRepository();
                if (isKeyContains && apiKey.First() == ApiKey && actionContext.ControllerContext.Controller is ApiLoginController)
                    base.OnAuthorization(actionContext);
                else if (!(actionContext.ControllerContext.Controller is ApiLoginController) && isKeyContains && apiKey.First() == ApiKey && isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First()))
                    base.OnAuthorization(actionContext);
                else { actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized); }
            }
            catch (Exception e)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, e.Message);
            }
        }
    }
}