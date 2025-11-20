using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsSourceTypeServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTabServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTypeServiceMethods;

namespace Tsg.UI.Main.APIControllers.DelegatedAuthority.DelegatedAuthorityParent
{
    [ApiFilter]
    public class ApiSecretCodeGenCheckController : ApiController
    {
        readonly static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IDaPayLimitsServiceMethods _daForPayLimitsMethodsService;
        private readonly IDaPayLimitsTabServiceMethods _daPayLimitsTabServiceMethods;
        private readonly IDaPayLimitsTypeServiceMethods _daPayLimitsTypeServiceMethods;
        private readonly IDaPayLimitsSourceTypeServiceMethods _daPayLimitsSourceTypeServiceMethods;

        public ApiSecretCodeGenCheckController(IDaPayLimitsServiceMethods payLimitsMethodsService,
            IDaPayLimitsTypeServiceMethods daPayLimitsTypeServiceMethods,
            IDaPayLimitsTabServiceMethods daPayLimitsTabServiceMethods, IDaPayLimitsSourceTypeServiceMethods daPayLimitsSourceTypeServiceMethods)
        {
            _daPayLimitsSourceTypeServiceMethods = daPayLimitsSourceTypeServiceMethods;
            _daForPayLimitsMethodsService = payLimitsMethodsService;
            _daPayLimitsTypeServiceMethods = daPayLimitsTypeServiceMethods;
            _daPayLimitsTabServiceMethods = daPayLimitsTabServiceMethods;
        }

        [HttpGet]
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
                    var res = _daForPayLimitsMethodsService.GetRandomWordBySecretPhrase();

                    return Content(res.Success ? HttpStatusCode.OK: HttpStatusCode.BadRequest, res);
                }

                return Content(HttpStatusCode.BadRequest, new StandartResponse("Undefined user"));
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Content(HttpStatusCode.BadRequest, new StandartResponse(GlobalRes.ShopingCardController_UnspecError, e.Message));
            }
        }
    }
}
