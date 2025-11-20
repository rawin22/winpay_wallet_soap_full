using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Tsg.UI.Main.ApiMethods.DelegatedAuthorityMethods;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.APIModels.DelegatedAuthority;
using TSG.Models.ServiceModels.LimitPayment;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTabServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTypeServiceMethods;

namespace Tsg.UI.Main.APIControllers.DelegatedAuthority
{
    [ApiFilter]
    [MvcApplication.LogExceptionFilterAttribute]
    public class ApiTabsDeviceForDelegatedAuthorityController : ApiController
    {
        readonly static log4net.ILog _logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IDaPayLimitsServiceMethods _daForPayLimitsMethodsService;
        private readonly IDaPayLimitsTabServiceMethods _daPayLimitsTabServiceMethods;
        private readonly IDaPayLimitsTypeServiceMethods _daPayLimitsTypeServiceMethods;

        public ApiTabsDeviceForDelegatedAuthorityController(IDaPayLimitsServiceMethods payLimitsMethodsService,
            IDaPayLimitsTypeServiceMethods daPayLimitsTypeServiceMethods,
            IDaPayLimitsTabServiceMethods daPayLimitsTabServiceMethods)
        {
            _daForPayLimitsMethodsService = payLimitsMethodsService;
            _daPayLimitsTypeServiceMethods = daPayLimitsTypeServiceMethods;
            _daPayLimitsTabServiceMethods = daPayLimitsTabServiceMethods;
        }

        // GET
        [HttpGet]
        [Route("api/TabsDeviceForDelegatedAuthority/{parentId}")]
        public IHttpActionResult Get(Guid parentId)
        {
            try
            {
                TSG.Models.APIModels.UserInfo ui;
                UserRepository userRepository = new UserRepository();
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    var daLimitByIdQuery = _daForPayLimitsMethodsService.GetById(parentId);
                    if (!daLimitByIdQuery.Success || daLimitByIdQuery.Obj == null)
                        throw new Exception("Not found trusted token");
                    var limitationTypesQuery = _daPayLimitsTypeServiceMethods.GetAll();
                    if (!limitationTypesQuery.Success || limitationTypesQuery.Obj == null)
                        throw new Exception("Not found types by trusted token");

                    //daLimitByIdQuery.Obj.DaPayLimitsTabs = _daPayLimitsTabServiceMethods.GetAllLimitsByKey(parentId).Obj;

                    //var newTabs = from types in limitationTypesQuery.Obj.OrderBy(ob => ob.DaPayLimitsType_LimitType).Where(w => !w.DaPayLimitsType_IsDeleted)
                    //              join tabs in daLimitByIdQuery.Obj.DaPayLimitsTabs
                    //                  on types.DaPayLimitsType_ID equals tabs.DaPayLimitsTab_TypeOfLimit into ps
                    //              from tabs in ps.DefaultIfEmpty()
                    //              select new DaPayLimitsTabSo()
                    //              {
                    //                  DaPayLimitsTab_ID = tabs?.DaPayLimitsTab_ID ?? Guid.Empty,
                    //                  DaPayLimitsTab_Amount = tabs?.DaPayLimitsTab_Amount ?? 0,
                    //                  DaPayLimitsTab_TypeOfLimit = types.DaPayLimitsType_ID,
                    //                  DaPayLimitsTab_IsDeleted = tabs == null || tabs.DaPayLimitsTab_ID == default || tabs.DaPayLimitsTab_IsDeleted,
                    //                  DaPayLimitsTab_ParentDaPayId = tabs?.DaPayLimitsTab_ParentDaPayId ?? Guid.Empty,
                    //                  DaPayLimitsTab_DaPayLimitsType = types
                    //              };
                    //daLimitByIdQuery.Obj.DaPayLimitsTabs = newTabs.ToList();

                    daLimitByIdQuery.Obj.Success = true;
                    daLimitByIdQuery.Obj.InfoBlock = new InfoBlock() {UserMessage = "Ok", DeveloperMessage = "Ok"};
                    return Ok(daLimitByIdQuery);
                }

                return Content(HttpStatusCode.BadRequest, new StandartResponse("Undefined user"));
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Content(HttpStatusCode.BadRequest,
                    new StandartResponse()
                    {
                        Success = false,
                        InfoBlock = new InfoBlock()
                            {UserMessage = @GlobalRes.ShopingCardController_UnspecError, DeveloperMessage = e.Message}
                    });
            }
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] List<DaPayLimitsTabSo> model)
        {
            try
            {
                if (model == null || model.Count == 0)
                    throw new Exception("Model is null or empty");
                TSG.Models.APIModels.UserInfo ui;
                UserRepository userRepository = new UserRepository();
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                bool isTotalSuccess = true;
                var listOfsource = _daPayLimitsTypeServiceMethods.GetAll().Obj.Where(w => !w.DaPayLimitsType_IsDeleted);

                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    foreach (var parentId  in model.Select(s=>s.DaPayLimitsTab_ParentDaPayId).OrderBy(ob=>ob))
                    {
                        var parentQuery = _daForPayLimitsMethodsService.GetById(parentId);
                        if (!parentQuery.Success || parentQuery.Obj == null)
                        {
                            model.ForEach(f =>
                            {
                                if (f.DaPayLimitsTab_ParentDaPayId == parentId)
                                {
                                    f.Success = false;
                                    f.InfoBlock = new InfoBlock(){UserMessage = "Not found parent by id", DeveloperMessage = "Not found parent by id" };
                                }

                            });
                            break;
                        }

                        //var limits = model.Where(w=>w.DaPayLimitsTab_ParentDaPayId == parentId && !w.DaPayLimitsTab_IsDeleted).Join(parentQuery.Obj.TotalListOfLimits,
                        //    so => so.DaPayLimitsTab_TypeOfLimit, tl => tl.DaPayLimitsType_ID, (so, tl) => new CheckingLimitations
                        //    {
                        //        IsDeletedLimitation = so.DaPayLimitsTab_IsDeleted,
                        //        IsSysLimitationDeleted = tl.DaPayLimitsType_IsDeleted,
                        //        LimitType = tl.DaPayLimitsType_LimitType,
                        //        LimitId = so.DaPayLimitsTab_ID,
                        //        LimitTypeGuid = tl.DaPayLimitsType_ID,
                        //        CurrencyCode = parentQuery.Obj.DaPayLimits_CcyCode,
                        //        AmountByLimitation = so.DaPayLimitsTab_Amount
                        //    }).ToList();


                        //var checkByLimits = CheckingLimitation.CheckTypesByAmount(limits.Where(w => !w.IsDeletedLimitation && !w.IsSysLimitationDeleted).ToList());
                        //if (checkByLimits.Any(a => !a.Success))
                        //{
                        //    var res = checkByLimits.FirstOrDefault(f => !f.Success)?.InfoBlock.UserMessage ?? "Exceeded limitation";
                        //    return Content(HttpStatusCode.BadRequest, new StandartResponse(res));
                        //}

                        //foreach (var daPayLimitsTabSo in model.Where(w=>w.DaPayLimitsTab_ParentDaPayId == parentId))
                        //{
                        //    if (daPayLimitsTabSo.DaPayLimitsTab_ID == default)
                        //    {
                        //        //if (parentQuery.Obj.DaPayLimitsTabs.Select(s => s.DaPayLimitsTab_TypeOfLimit)
                        //        //    .Contains(daPayLimitsTabSo.DaPayLimitsTab_TypeOfLimit))
                        //        //{
                        //        //    daPayLimitsTabSo.InfoBlock = new InfoBlock(){UserMessage = "Record alwayls exist", DeveloperMessage = "Record alwayls exist by current source type, enter Id for limitation " };
                        //        //    continue;
                        //        //}
                        //        var resInsert = _daPayLimitsTabServiceMethods.Insert(daPayLimitsTabSo);
                        //        daPayLimitsTabSo.Success = resInsert.Success;
                        //        daPayLimitsTabSo.InfoBlock = new InfoBlock(){UserMessage = resInsert.Success ? "Success":"Error", DeveloperMessage = String.IsNullOrEmpty(resInsert.Message) ? "OK": resInsert.Message};
                        //    }
                        //    else
                        //    {
                        //        if (parentQuery.Obj.DaPayLimitsTabs.Select(s => s.DaPayLimitsTab_ID)
                        //            .Contains(daPayLimitsTabSo.DaPayLimitsTab_ID) && parentQuery.Obj.DaPayLimitsTabs.Select(s => s.DaPayLimitsTab_TypeOfLimit)
                        //                .Contains(daPayLimitsTabSo.DaPayLimitsTab_TypeOfLimit))
                        //        {
                        //            if (daPayLimitsTabSo.DaPayLimitsTab_Amount <= 0)
                        //            {
                        //                daPayLimitsTabSo.Success = false; daPayLimitsTabSo.InfoBlock = new InfoBlock()
                        //                {
                        //                    UserMessage = "Insert correct amount",
                        //                    DeveloperMessage = "Negative amount or amount equals 0"
                        //                };
                        //                continue;
                        //            }

                        //            var updateQuery = _daPayLimitsTabServiceMethods.Update(daPayLimitsTabSo);
                        //            daPayLimitsTabSo.Success = updateQuery.Success;
                        //            daPayLimitsTabSo.InfoBlock = new InfoBlock() { UserMessage = updateQuery.Success ? "Success" : "Error", DeveloperMessage = String.IsNullOrEmpty(updateQuery.Message) ? "OK" : updateQuery.Message };
                        //        }
                        //        else
                        //        {
                        //            daPayLimitsTabSo.Success = false;
                        //            daPayLimitsTabSo.InfoBlock = new InfoBlock()
                        //            {
                        //                UserMessage = "Not updated",
                        //                DeveloperMessage = "Record not found. Update record unavaliable"
                        //            };
                        //        }
                        //    }
                        //}
                    }
                    return Ok(model);
                }
                return Content(HttpStatusCode.BadRequest, new StandartResponse("Undefined user"));
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Content(HttpStatusCode.BadRequest,
                    new StandartResponse()
                    {
                        Success = false,
                        InfoBlock = new InfoBlock()
                            {UserMessage = @GlobalRes.ShopingCardController_UnspecError, DeveloperMessage = e.Message}
                    });
            }
        }

        [HttpDelete]
        public IHttpActionResult Delete(Guid limitId)
        {
            try
            {
                TSG.Models.APIModels.UserInfo ui;
                UserRepository userRepository = new UserRepository();
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    //var listOfBalances = UiHelper.ApiGetAccountBalances(ui).ToList();
                    var daLimitByIdQuery = _daPayLimitsTabServiceMethods.GetById(limitId);
                    if (!daLimitByIdQuery.Success || daLimitByIdQuery.Obj == null)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("Not found trusted token Limit"));
                    var limitationTypesQuery = _daPayLimitsTypeServiceMethods.GetById(daLimitByIdQuery.Obj.DaPayLimitsTab_TypeOfLimit);
                    if (!limitationTypesQuery.Success || limitationTypesQuery.Obj == null)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("Not found types by trusted token Limit"));
                    var daLimitParent = _daForPayLimitsMethodsService.GetById(daLimitByIdQuery.Obj.DaPayLimitsTab_ParentDaPayId);

                    if (!daLimitParent.Success || daLimitParent.Obj == null)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("Not found types by trusted token parent"));

                    if (daLimitParent.Obj.DaPayLimits_DateOfExpire < DateTime.Now)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("You can not change expired limitation"));
                    
                    var deleteQueryRes = _daPayLimitsTabServiceMethods.Delete(daLimitByIdQuery.Obj.DaPayLimitsTab_ID);
                    if (!deleteQueryRes.Success)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("Limit does not deleted"));
                    return Ok(new StandartResponse(deleteQueryRes.Success, !String.IsNullOrEmpty(deleteQueryRes.Message) ? deleteQueryRes.Message : "Successfully deleted"));
                }

                return Content(HttpStatusCode.BadRequest, new StandartResponse("Undefined user"));
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Content(HttpStatusCode.BadRequest,
                    new StandartResponse()
                    {
                        Success = false,
                        InfoBlock = new InfoBlock()
                        { UserMessage = @GlobalRes.ShopingCardController_UnspecError, DeveloperMessage = e.Message }
                    });
            }
        }


    }
}