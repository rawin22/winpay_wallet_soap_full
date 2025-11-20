using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.ServiceModels.LimitPayment;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsServiceMethods;
using WinstantPay.Common.Object;

namespace Tsg.UI.Main.APIControllers.DelegatedAuthority.DelegatedAuthority
{
    [ApiFilter]
    public class ApiDelegatedAuthorutyGetAllDevicesByUserController : ApiController
    {

        readonly static log4net.ILog _logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IDaPayLimitsServiceMethods _daForPayLimitsMethodsService;

        public ApiDelegatedAuthorutyGetAllDevicesByUserController(IDaPayLimitsServiceMethods daForPayLimitsMethodsService)
        {
            _daForPayLimitsMethodsService = daForPayLimitsMethodsService;
        }
        /// <summary>
        /// Get all device for User
        /// </summary>
        /// <returns>Standart Response + Objects[]</returns>
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
                    var query = _daForPayLimitsMethodsService.GetAllDaByUserName(ui.UserName);
                    var lists = query.Obj?.Where(w =>
                        !w.DaPayLimits_IsDeleted && ((w.DaPayLimits_DateOfExpire > DateTime.Now) || w.DaPayLimits_DateOfExpire == null) && !w.DaPayLimits_IsTransfered).OrderByDescending(ob=>ob.DaPayLimits_CreationDate).ToList() ?? new List<DaPayLimitsSo>();

                    return Ok(new StandartResponse<List<DaPayLimitsSo>>(lists, query.Success, query.Message));
                }
                return Content(HttpStatusCode.Unauthorized, new StandartResponse("Undefined user"));
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Content(HttpStatusCode.BadRequest, new StandartResponse { InfoBlock = new InfoBlock() { UserMessage = GlobalRes.ShopingCardController_UnspecError, DeveloperMessage = e.Message } });
            }
        }
    }
}