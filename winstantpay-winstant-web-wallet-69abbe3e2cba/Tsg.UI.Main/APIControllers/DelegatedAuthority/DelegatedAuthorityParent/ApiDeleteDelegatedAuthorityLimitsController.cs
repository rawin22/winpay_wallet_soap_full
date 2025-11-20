using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Tsg.UI.Main.ApiMethods.DelegatedAuthorityMethods;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.APIModels.DelegatedAuthority;
using TSG.Models.ServiceModels.LimitPayment;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTabServiceMethods;

namespace Tsg.UI.Main.APIControllers.DelegatedAuthority
{
    public class ApiDeleteDelegatedAuthorityLimitsController : ApiController
    {
        readonly static log4net.ILog _logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IDaPayLimitsServiceMethods _daForPayLimitsMethodsService;
        private readonly IDaPayLimitsTabServiceMethods _daPayLimitsTabServiceMethods;

        public ApiDeleteDelegatedAuthorityLimitsController(IDaPayLimitsServiceMethods daForPayLimitsMethodsService, IDaPayLimitsTabServiceMethods daPayLimitsTabServiceMethods)
        {
            _daForPayLimitsMethodsService = daForPayLimitsMethodsService;
            _daPayLimitsTabServiceMethods = daPayLimitsTabServiceMethods;
        }      

        [HttpPost]
        [Route("api/DelegatedAuthority/Delete/{id}")]
        public IHttpActionResult Delete(Guid id)
        {
            try
            {
                TSG.Models.APIModels.UserInfo ui;
                UserRepository userRepository = new UserRepository();
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    if (id == default)
                        return Content(HttpStatusCode.NotFound,
                            new StandartResponse()
                            {
                                InfoBlock = new InfoBlock()
                                    { UserMessage = "Not found by this ID", DeveloperMessage = "Not found by this ID" }
                            });
                    var getById = _daForPayLimitsMethodsService.GetById(id);
                    if (!getById.Success || getById.Obj == null)
                        return Content(HttpStatusCode.NotFound,
                            new StandartResponse()
                            {
                                InfoBlock = new InfoBlock()
                                {
                                    UserMessage = "Not found by this ID",
                                    DeveloperMessage = "Not found or empty by this ID"
                                }
                            });
                    if (getById.Obj.DaPayLimits_UserName != ui.UserName)
                        return Content(HttpStatusCode.BadRequest,new StandartResponse("Device is not found"));

                    if(getById.Obj.DaPayLimits_IsDeleted)
                        return Ok(new StandartResponse(true, "OK"));

                    if (getById.Obj.DaPayLimits_DateOfExpire.HasValue && getById.Obj.DaPayLimits_DateOfExpire.Value.ToLocalTime() < DateTime.Now)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("You can not change expired limitation"));

                    var setIdDeletedQRes = _daForPayLimitsMethodsService.Delete(id);

                    if (!setIdDeletedQRes.Success)
                        return Content(HttpStatusCode.BadRequest,
                            new StandartResponse("This trusted token not deleted", setIdDeletedQRes.Message));
                    return Ok(new StandartResponse(setIdDeletedQRes.Success, String.IsNullOrEmpty(setIdDeletedQRes.Message) ? "OK" : setIdDeletedQRes.Message));
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