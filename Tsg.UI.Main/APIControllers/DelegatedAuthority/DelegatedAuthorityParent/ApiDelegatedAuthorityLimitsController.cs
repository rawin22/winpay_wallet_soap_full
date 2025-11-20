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
    public class ApiDelegatedAuthorityLimitsController : ApiController
    {
        readonly static log4net.ILog _logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IDaPayLimitsServiceMethods _daForPayLimitsMethodsService;
        private readonly IDaPayLimitsTabServiceMethods _daPayLimitsTabServiceMethods;

        public ApiDelegatedAuthorityLimitsController(IDaPayLimitsServiceMethods daForPayLimitsMethodsService, IDaPayLimitsTabServiceMethods daPayLimitsTabServiceMethods)
        {
            _daForPayLimitsMethodsService = daForPayLimitsMethodsService;
            _daPayLimitsTabServiceMethods = daPayLimitsTabServiceMethods;
        }

        /// <summary>
        /// Get limitation by ID
        /// </summary>
        /// <returns>Standart Response + Objects</returns>
        [HttpGet]
        public IHttpActionResult Get(Guid id)
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
                    var listOfBalances = UiHelper.ApiGetAccountBalancesForDa(ui, true).ToList();
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
                    //getById.Obj.DaPayLimitsTabs = _daPayLimitsTabServiceMethods.GetAllLimitsByKey(id).Obj;

                    //var newTabs = from types in getById.Obj.TotalListOfLimits.OrderBy(ob => ob.DaPayLimitsType_LimitType).Where(w => !w.DaPayLimitsType_IsDeleted)
                    //    join tabs in getById.Obj.DaPayLimitsTabs
                    //        on types.DaPayLimitsType_ID equals tabs.DaPayLimitsTab_TypeOfLimit into ps
                    //    from tabs in ps.DefaultIfEmpty()
                    //    select new DaPayLimitsTabSo()
                    //    {
                    //        DaPayLimitsTab_ID = tabs?.DaPayLimitsTab_ID ?? Guid.Empty,
                    //        DaPayLimitsTab_Amount = tabs?.DaPayLimitsTab_Amount ?? 0,
                    //        DaPayLimitsTab_TypeOfLimit = types.DaPayLimitsType_ID,
                    //        DaPayLimitsTab_IsDeleted = tabs == null || tabs.DaPayLimitsTab_ID == default || tabs.DaPayLimitsTab_IsDeleted,
                    //        DaPayLimitsTab_ParentDaPayId = getById.Obj.DaPayLimits_ID,
                    //        DaPayLimitsTab_DaPayLimitsType = types
                    //    };
                    //getById.Obj.DaPayLimitsTabs = newTabs.ToList();


                    getById.Obj.ListOfBalances = listOfBalances;
                    getById.Obj.Success = getById.Success;
                    getById.Obj.InfoBlock = new InfoBlock(String.IsNullOrEmpty(getById.Message) ? "OK" : getById.Message);

                    return Ok(getById.Obj);
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


        [HttpPost]
        public IHttpActionResult Post(DaPayLimitsSo model)
        {
            try
            {
                TSG.Models.APIModels.UserInfo ui;
                UserRepository userRepository = new UserRepository();
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    if (model.DaPayLimits_ID == default)
                        return Content(HttpStatusCode.NotFound,
                            new StandartResponse()
                            {
                                InfoBlock = new InfoBlock()
                                { UserMessage = "Not found by this ID", DeveloperMessage = "Not found by this ID" }
                            });
                    var getById = _daForPayLimitsMethodsService.GetById(model.DaPayLimits_ID);
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


                    if (getById.Obj.DaPayLimits_DateOfExpire.HasValue && getById.Obj.DaPayLimits_DateOfExpire.Value.ToLocalTime() < DateTime.Now  && model.DaPayLimits_DateOfExpire.HasValue &&  model.DaPayLimits_DateOfExpire.Value.ToLocalTime() < DateTime.Now)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("You can not change, limitation date expired"));
                    
                    //model.DaPayLimits_CcyCode = model.DaPayLimits_CcyCode.Substring(0, 3);

                    getById.Obj.DaPayLimits_DateOfExpire = model.DaPayLimits_DateOfExpire;
                    getById.Obj.DaPayLimits_UserName =  ui.UserName;
                    getById.Obj.DaPayLimits_WPayId =  model.DaPayLimits_WPayId;
                    getById.Obj.DaPayLimits_LastModifiedDate =  DateTime.UtcNow;

                    //getById.Obj.DaPayLimits_CcyCode = model.DaPayLimits_CcyCode;
                    //getById.Obj.DaPayLimits_PinCode = model.DaPayLimits_PinCode;
                    //getById.Obj.DaPayLimits_IsPinProtected = 
                    //    model.DaPayLimits_IsPinProtected || !String.IsNullOrEmpty(model.DaPayLimits_PinCode);
                    
                    var updateResult = _daForPayLimitsMethodsService.Update(getById.Obj);
                    if (!updateResult.Success)
                        return Content(HttpStatusCode.NotFound, new StandartResponse("Delegeted authority does not updated successifully", updateResult.Message));


                    //var limits = model.DaPayLimitsTabs.Where(w => !w.DaPayLimitsTab_IsDeleted).Join(getById.Obj.TotalListOfLimits,
                    //    so => so.DaPayLimitsTab_TypeOfLimit, tl => tl.DaPayLimitsType_ID, (so, tl) => new CheckingLimitations
                    //    {
                    //        IsDeletedLimitation = so.DaPayLimitsTab_IsDeleted,
                    //        IsSysLimitationDeleted = tl.DaPayLimitsType_IsDeleted,
                    //        LimitType = tl.DaPayLimitsType_LimitType,
                    //        LimitId = so.DaPayLimitsTab_ID,
                    //        LimitTypeGuid = tl.DaPayLimitsType_ID,
                    //        CurrencyCode = getById.Obj.DaPayLimits_CcyCode,
                    //        AmountByLimitation = so.DaPayLimitsTab_Amount,
                    //    }).ToList();


                    //var checkByLimits = CheckingLimitation.CheckTypesByAmount(model.DaPayLimitsTabs.Where(w => !w.DaPayLimitsTab_IsDeleted).ToList(), limits.Where(w=>!w.IsDeletedLimitation && !w.IsSysLimitationDeleted).ToList());
                    //if (checkByLimits.Any(a => !a.Success))
                    //{
                    //    var res = checkByLimits.FirstOrDefault(f => !f.Success)?.InfoBlock.UserMessage ?? "Exceeded limitation";
                    //    return Content(HttpStatusCode.BadRequest, new StandartResponse(res));
                    //}

                    
                    //foreach (var daPayLimitsTabSo in model.DaPayLimitsTabs)
                    //{
                    //    if (daPayLimitsTabSo.DaPayLimitsTab_ID == default)
                    //    {
                    //        if (getById.Obj.DaPayLimitsTabs.Select(s => s.DaPayLimitsTab_TypeOfLimit)
                    //            .Contains(daPayLimitsTabSo.DaPayLimitsTab_TypeOfLimit))
                    //        {
                    //            daPayLimitsTabSo.InfoBlock = new InfoBlock() { UserMessage = "Record alwayls exist", DeveloperMessage = "Record alwayls exist by current source type, enter Id for limitation " };
                    //        }
                    //        else
                    //        {
                    //            var resInsert = _daPayLimitsTabServiceMethods.Insert(daPayLimitsTabSo);
                    //            daPayLimitsTabSo.Success = resInsert.Success;
                    //            daPayLimitsTabSo.InfoBlock =
                    //                new InfoBlock()
                    //                {
                    //                    UserMessage = resInsert.Success ? "Success" : "Error",
                    //                    DeveloperMessage = String.IsNullOrEmpty(resInsert.Message)
                    //                        ? "OK"
                    //                        : resInsert.Message
                    //                };
                    //        }
                    //    }
                    //    else
                    //    {
                    //        var currRecTab = getById.Obj.DaPayLimitsTabs.FirstOrDefault(f =>
                    //            f.DaPayLimitsTab_ID == daPayLimitsTabSo.DaPayLimitsTab_ID);
                    //        if (currRecTab != null)
                    //        {
                    //            //if (getById.Obj.DaPayLimitsTabs.Select(s => s.DaPayLimitsTab_ID)
                    //            //        .Contains(daPayLimitsTabSo.DaPayLimitsTab_ID) && getById.Obj.DaPayLimitsTabs.Select(s => s.DaPayLimitsTab_TypeOfLimit)
                    //            //        .Contains(daPayLimitsTabSo.DaPayLimitsTab_TypeOfLimit))
                    //            //{
                    //            //}
                                
                    //            if (daPayLimitsTabSo.DaPayLimitsTab_Amount <= 0)
                    //            {
                    //                daPayLimitsTabSo.Success = false; daPayLimitsTabSo.InfoBlock = new InfoBlock()
                    //                {
                    //                    UserMessage = "Insert correct amount",
                    //                    DeveloperMessage = "Negative amount or amount equals 0"
                    //                };
                    //                continue;
                    //            }
                    //            currRecTab.DaPayLimitsTab_Amount = daPayLimitsTabSo.DaPayLimitsTab_Amount;
                    //            currRecTab.DaPayLimitsTab_IsDeleted = daPayLimitsTabSo.DaPayLimitsTab_IsDeleted;

                    //            var updateQuery = _daPayLimitsTabServiceMethods.Update(currRecTab);
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

                    //return Ok(new StandartResponse(updateResult.Success, String.IsNullOrEmpty(updateResult.Message) ? "OK" : updateResult.Message, string.Join("; ", model.DaPayLimitsTabs.Select(s=>s.InfoBlock.DeveloperMessage))));
                    return Ok(new StandartResponse(updateResult.Success, String.IsNullOrEmpty(updateResult.Message) ? "OK" : updateResult.Message ));
                }
                return Content(HttpStatusCode.BadRequest, new StandartResponse("Undefined user"));
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Content(HttpStatusCode.BadRequest, new StandartResponse(GlobalRes.ShopingCardController_UnspecError, e.Message));
            }
        }

        [HttpPatch]
        public IHttpActionResult Patch(DaPayLimitsSo model)
        {
            try
            {
                TSG.Models.APIModels.UserInfo ui;
                UserRepository userRepository = new UserRepository();
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    if (model.DaPayLimits_ID == default)
                        return Content(HttpStatusCode.NotFound,
                            new StandartResponse()
                            {
                                InfoBlock = new InfoBlock()
                                { UserMessage = "Not found by this ID", DeveloperMessage = "Not found by this ID" }
                            });
                    var getById = _daForPayLimitsMethodsService.GetById(model.DaPayLimits_ID);
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
                    if (getById.Obj.DaPayLimits_DateOfExpire.HasValue && getById.Obj.DaPayLimits_DateOfExpire.Value.ToLocalTime() < DateTime.Now && model.DaPayLimits_DateOfExpire.HasValue && model.DaPayLimits_DateOfExpire.Value.ToLocalTime() < DateTime.Now)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("You can not change, limitation date expired"));

                    //model.DaPayLimits_CcyCode = model.DaPayLimits_CcyCode.Substring(0, 3);
                    getById.Obj.DaPayLimits_DateOfExpire = model.DaPayLimits_DateOfExpire;
                    getById.Obj.DaPayLimits_UserName = ui.UserName;
                    getById.Obj.DaPayLimits_WPayId = model.DaPayLimits_WPayId;
                    getById.Obj.DaPayLimits_LastModifiedDate = DateTime.UtcNow;

                    var updateResult = _daForPayLimitsMethodsService.Update(getById.Obj);
                    if (!updateResult.Success)
                        return Content(HttpStatusCode.NotFound, new StandartResponse("Delegeted authority does not updated successifully", updateResult.Message));

                    //foreach (var daPayLimitsTabSo in model.DaPayLimitsTabs)
                    //{
                    //    if (daPayLimitsTabSo.DaPayLimitsTab_ID == default)
                    //    {
                    //        if (getById.Obj.DaPayLimitsTabs.Select(s => s.DaPayLimitsTab_TypeOfLimit)
                    //            .Contains(daPayLimitsTabSo.DaPayLimitsTab_TypeOfLimit))
                    //        {
                    //            daPayLimitsTabSo.InfoBlock = new InfoBlock() { UserMessage = "Record alwayls exist", DeveloperMessage = "Record alwayls exist by current source type, enter Id for limitation " };
                    //            continue;
                    //        }
                    //        var resInsert = _daPayLimitsTabServiceMethods.Insert(daPayLimitsTabSo);
                    //        daPayLimitsTabSo.Success = resInsert.Success;
                    //        daPayLimitsTabSo.InfoBlock = new InfoBlock() { UserMessage = resInsert.Success ? "Success" : "Error", DeveloperMessage = String.IsNullOrEmpty(resInsert.Message) ? "OK" : resInsert.Message };
                    //    }
                    //    else
                    //    {
                    //        if (getById.Obj.DaPayLimitsTabs.Select(s => s.DaPayLimitsTab_ID)
                    //            .Contains(daPayLimitsTabSo.DaPayLimitsTab_ID) && getById.Obj.DaPayLimitsTabs.Select(s => s.DaPayLimitsTab_TypeOfLimit)
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


                    return Ok(new StandartResponse(updateResult.Success, String.IsNullOrEmpty(updateResult.Message) ? "OK" : updateResult.Message));
                }
                return Content(HttpStatusCode.BadRequest, new StandartResponse("Undefined user"));
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Content(HttpStatusCode.BadRequest, new StandartResponse(GlobalRes.ShopingCardController_UnspecError, e.Message));
            }
        }



        [HttpDelete]
        public IHttpActionResult Delete(Guid daId)
        {
            try
            {
                TSG.Models.APIModels.UserInfo ui;
                UserRepository userRepository = new UserRepository();
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    if (daId == default)
                        return Content(HttpStatusCode.NotFound,
                            new StandartResponse()
                            {
                                InfoBlock = new InfoBlock()
                                    { UserMessage = "Not found by this ID", DeveloperMessage = "Not found by this ID" }
                            });
                    var getById = _daForPayLimitsMethodsService.GetById(daId);
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

                    var setIdDeletedQRes = _daForPayLimitsMethodsService.Delete(daId);

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