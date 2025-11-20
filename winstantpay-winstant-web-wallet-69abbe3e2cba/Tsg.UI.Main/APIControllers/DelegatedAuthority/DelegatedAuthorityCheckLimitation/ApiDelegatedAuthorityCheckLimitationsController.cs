using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.ServiceModels.LimitPayment;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsServiceMethods;

namespace Tsg.UI.Main.APIControllers.DelegatedAuthority
{
    public class ApiDelegatedAuthorityCheckLimitationsController : ApiController
    {
        readonly static log4net.ILog _logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IDaPayLimitsServiceMethods _daForPayLimitsMethodsService;

        public ApiDelegatedAuthorityCheckLimitationsController(IDaPayLimitsServiceMethods daForPayLimitsMethodsService)
        {
            _daForPayLimitsMethodsService = daForPayLimitsMethodsService;
        }

        /// <summary>
        /// Check limitation in Db
        /// </summary>
        /// <returns>Standart Response</returns>
        [HttpGet]
        public IHttpActionResult Get(string limitations)
        {
            try
            {
                if (String.IsNullOrEmpty(limitations)) return Content(HttpStatusCode.BadRequest, new StandartResponse("Please send correct limitation for check"));

                TSG.Models.APIModels.UserInfo ui;
                UserRepository userRepository = new UserRepository();
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    var listByCode = _daForPayLimitsMethodsService.GetAllDaByDeviceCode(
                        Newtonsoft.Json.JsonConvert.SerializeObject(
                            new LimitationString()
                            {
                                DelegatedAuthorityCode = limitations
                            }));
                    if (!listByCode.Success || listByCode.Obj == null)
                        return Content(HttpStatusCode.BadRequest,
                            new StandartResponse("You can not use this limitation"));

                    if (listByCode.Obj.Any(w => !w.DaPayLimits_IsDeleted && w.DaPayLimits_DateOfExpire > DateTime.Now))
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("You can not use this limitation", "We have activated device"));

                    return Ok(new StandartResponse(true, "Ok"));
                }

                return Content(HttpStatusCode.BadRequest, new StandartResponse("Undefined user"));
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Content(HttpStatusCode.BadRequest,
                    new StandartResponse
                    {
                        InfoBlock = new InfoBlock()
                        { UserMessage = GlobalRes.ShopingCardController_UnspecError, DeveloperMessage = e.Message }
                    });
            }
        }

        
    }
}