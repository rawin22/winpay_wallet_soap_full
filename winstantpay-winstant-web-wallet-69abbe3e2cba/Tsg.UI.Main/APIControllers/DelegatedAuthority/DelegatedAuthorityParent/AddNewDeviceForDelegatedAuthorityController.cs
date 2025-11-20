using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Http;
using Tsg.UI.Main.ApiMethods.Payments;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.Enums;
using TSG.Models.ServiceModels.LimitPayment;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsSourceTypeServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTabServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTypeServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaUserWPayIDSettingServiceMethods;
using WinstantPay.Common.CryptDecriptInfo;

namespace Tsg.UI.Main.APIControllers.DelegatedAuthority
{
    [ApiFilter]
    public class AddNewDeviceForDelegatedAuthorityController : ApiController
    {
        readonly static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IDaPayLimitsServiceMethods _daForPayLimitsMethodsService;
        private readonly IDaPayLimitsTabServiceMethods _daPayLimitsTabServiceMethods;
        private readonly IDaPayLimitsTypeServiceMethods _daPayLimitsTypeServiceMethods;
        private readonly IDaPayLimitsSourceTypeServiceMethods _daPayLimitsSourceTypeServiceMethods;
        private readonly IDaUserWPayIDSettingServiceMethods _daUserWPayIDSettingServiceMethods;

        public AddNewDeviceForDelegatedAuthorityController(IDaPayLimitsServiceMethods payLimitsMethodsService,
            IDaPayLimitsTypeServiceMethods daPayLimitsTypeServiceMethods,
            IDaPayLimitsTabServiceMethods daPayLimitsTabServiceMethods, IDaPayLimitsSourceTypeServiceMethods daPayLimitsSourceTypeServiceMethods, IDaUserWPayIDSettingServiceMethods daUserWPayIDSettingServiceMethods)
        {
            _daPayLimitsSourceTypeServiceMethods = daPayLimitsSourceTypeServiceMethods;
            _daForPayLimitsMethodsService = payLimitsMethodsService;
            _daPayLimitsTypeServiceMethods = daPayLimitsTypeServiceMethods;
            _daPayLimitsTabServiceMethods = daPayLimitsTabServiceMethods;
            _daUserWPayIDSettingServiceMethods = daUserWPayIDSettingServiceMethods;
        }

        // GET
        [System.Web.Http.HttpGet]
        [Route("api/TabsDeviceForDelegatedAuthority/{sourceId?}")]
        public IHttpActionResult Get(string sourceId = "")
        {
            try
            {
                //if (String.IsNullOrEmpty(sourceId) || !Guid.TryParse(sourceId, out var sourceGuid))
                //    return Content(HttpStatusCode.BadRequest, new StandartResponse() { Success = false, InfoBlock = new InfoBlock() { UserMessage = "Not found source", DeveloperMessage = "Not found source" } });
                TSG.Models.APIModels.UserInfo ui;
                UserRepository userRepository = new UserRepository();
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    var listOfBalances = UiHelper.ApiGetAccountBalancesForDa(ui, true).ToList();
                    if (listOfBalances.Count == 0)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse() { Success = false, InfoBlock = new InfoBlock() { UserMessage = "Empty balances", DeveloperMessage = "List with user balances is null or empty" } });
                    if (String.IsNullOrEmpty(sourceId) || !Guid.TryParse(sourceId, out var sourceGuid))
                    {
                        return Ok(new DaPayLimitsSo() { ListOfBalances = listOfBalances, Success = true, InfoBlock = new InfoBlock() { UserMessage = "Ok", DeveloperMessage = "Ok" } });
                    }
                    var sourceQuery = _daPayLimitsSourceTypeServiceMethods.GetById(sourceGuid);
                    if (!sourceQuery.Success || sourceQuery.Obj == null)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse() { Success = false, InfoBlock = new InfoBlock() { UserMessage = "Not found source", DeveloperMessage = sourceQuery.Message } });
                    return Ok(new DaPayLimitsSo() { ListOfBalances = listOfBalances, DaPayLimits_SourceType = sourceGuid, Success = true, InfoBlock = new InfoBlock() { UserMessage = "Ok", DeveloperMessage = "Ok" } });
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

        [System.Web.Http.HttpPost]
        public IHttpActionResult Post([FromBody] DaPayLimitsSo model)
        {
            try
            {
                TSG.Models.APIModels.UserInfo ui;
                UserRepository userRepository = new UserRepository();
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    if (model.DaPayLimits_ID != default)
                        return Content(HttpStatusCode.BadRequest,
                            new DaPayLimitsSo()
                            {
                                InfoBlock = new InfoBlock()
                                { UserMessage = GlobalRes.DelegatedAuthority_CreatedEarly },
                                Success = false
                            });
                    // Check if pin setted but not sended
                    //if(model.DaPayLimits_IsPinProtected && String.IsNullOrEmpty(model.DaPayLimits_PinCode))
                    //    return Content(HttpStatusCode.BadRequest,
                    //        new DaPayLimitsSo()
                    //        {
                    //            InfoBlock = new InfoBlock()
                    //                { UserMessage = "PIN code not setted", DeveloperMessage = "PIN code not setted" },
                    //            Success = false
                    //        });


                    // Check selected cureency
                    //if (String.IsNullOrEmpty(model.DaPayLimits_CcyCode))
                    //    return Content(HttpStatusCode.BadRequest,
                    //        new DaPayLimitsSo()
                    //        {
                    //            InfoBlock = new InfoBlock()
                    //            { UserMessage = GlobalRes.DelegatedAuthority_CurrencyValidationError },
                    //            Success = false
                    //        });
                    // Check media name
                    if (String.IsNullOrEmpty(model.DaPayLimits_MediaName) && String.IsNullOrWhiteSpace(model.DaPayLimits_MediaName))
                        return Content(HttpStatusCode.BadRequest,
                            new DaPayLimitsSo()
                            {
                                InfoBlock = new InfoBlock()
                                { UserMessage = GlobalRes.DelegatedAuthority_MediaNameValidationError},
                                Success = false
                            });
                    if (String.IsNullOrEmpty(model.DaPayLimits_LimitCodeInitialization) && String.IsNullOrWhiteSpace(model.DaPayLimits_LimitCodeInitialization))
                        return Content(HttpStatusCode.BadRequest,
                            new DaPayLimitsSo()
                            {
                                InfoBlock = new InfoBlock()
                                { UserMessage = "DaPayLimits_LimitCodeInitialization cannot be blank or null" },
                                Success = false
                            });

                    if (!model.DaPayLimits_DateOfExpire.HasValue && DateTime.TryParseExact(model.DaPayLimits_DateOfExpireString, "dd/MM/yyyy",
                                                                         CultureInfo.CurrentCulture, DateTimeStyles.None, out var date) &&
                                                                     TimeSpan.TryParse(model.DaPayLimits_TimeOfExpireString, out var time))
                        model.DaPayLimits_DateOfExpire = date.Add(time);
                    //else if (!model.DaPayLimits_DateOfExpire.HasValue)
                    //{
                    //    return Content(HttpStatusCode.BadRequest,
                    //        new DaPayLimitsSo()
                    //        {
                    //            InfoBlock = new InfoBlock()
                    //            { UserMessage = "DaPayLimits_DateOfExpire no value" },
                    //            Success = false
                    //        });
                    //}
                    //else if (!DateTime.TryParseExact(model.DaPayLimits_DateOfExpireString, "dd/MM/yyyy",                                                                      CultureInfo.CurrentCulture, DateTimeStyles.None, out var date1))
                    //{
                    //    return Content(HttpStatusCode.BadRequest,
                    //        new DaPayLimitsSo()
                    //        {
                    //            InfoBlock = new InfoBlock()
                    //            { UserMessage = "DaPayLimits_DateOfExpireString converting issue" },
                    //            Success = false
                    //        });
                    //}
                    //else if (!TimeSpan.TryParse(model.DaPayLimits_TimeOfExpireString, out var time1))
                    //{
                    //    return Content(HttpStatusCode.BadRequest,
                    //        new DaPayLimitsSo()
                    //        {
                    //            InfoBlock = new InfoBlock()
                    //            { UserMessage = "DaPayLimits_TimeOfExpireString converting issue" },
                    //            Success = false
                    //        });
                    //}
                    //else if (!model.DaPayLimits_DateOfExpire.HasValue)
                    //{
                    //    return Content(HttpStatusCode.BadRequest,
                    //        new DaPayLimitsSo()
                    //        {
                    //            InfoBlock = new InfoBlock()
                    //            { UserMessage = GlobalRes.DelegatedAuthority_ExpDateValidationError },
                    //            Success = false
                    //        });
                    //}

                    if (model.DaPayLimits_DateOfExpire!= null && model.DaPayLimits_DateOfExpire.Value.ToLocalTime() <= DateTime.Now)
                        return Content(HttpStatusCode.BadRequest,
                            new DaPayLimitsSo()
                            {
                                InfoBlock = new InfoBlock()
                                { UserMessage = GlobalRes.DelegatedAuthority_ExpDateValidationError },
                                Success = false
                            });
                    
                    
                    

                    // Check by limitation
                    // -- You can not connect early connected device or device with unexpired Trusted Token limitation
                    var checkIfNfcLimit = _daForPayLimitsMethodsService.GetAllDaByDeviceCode(Newtonsoft.Json.JsonConvert.SerializeObject(
                        new LimitationString()
                        {
                            DelegatedAuthorityCode = model.DaPayLimits_LimitCodeInitialization
                        }));
                    checkIfNfcLimit.Obj.Where(w=>w.DaPaymentLimitSourceType.DaPaymentLimitSourceType_EnumNumber == (int)DelegatedAuthorirySourceLimitationTypeEnum.Nfc).ToList().ForEach(
                        f => { _daForPayLimitsMethodsService.Delete(f.DaPayLimits_ID); }
                    );

                    var checkLimit = _daForPayLimitsMethodsService.GetAllDaByDeviceCode(Newtonsoft.Json.JsonConvert.SerializeObject(
                        new LimitationString()
                        {
                            DelegatedAuthorityCode = model.DaPayLimits_LimitCodeInitialization
                        }));

                    if (!checkLimit.Success || checkLimit.Obj?.Where(w => ((w.DaPayLimits_DateOfExpire!=null && w.DaPayLimits_DateOfExpire > DateTime.Now) || w.DaPayLimits_DateOfExpire == null) && !w.DaPayLimits_IsDeleted).Count() > 0)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("Sorry. This code is taken. Try again.", checkLimit.Message));

                    // We can add check to balance

                    // Check ccy code
                    var balancesQueryRes = new NewInstantPaymentMethods(ui).PrepareAccountBalances();
                    if (balancesQueryRes.ServiceResponse.HasErrors)
                        return Content(HttpStatusCode.BadRequest,
                            new StandartResponse()
                            {
                                InfoBlock = new InfoBlock()
                                {
                                    UserMessage =
                                        $"{balancesQueryRes.ServiceResponse.Responses[0].MessageDetails} \r\n {balancesQueryRes.ServiceResponse.Responses[0].MessageDetails}"
                                },
                                Success = false
                            });
                    //var daUserSetting = _daUserWPayIDSettingServiceMethods.GetByWPayId(model.DaPayLimits_WPayId);

                    //if (daUserSetting == null)
                    //    return Content(HttpStatusCode.BadRequest,
                    //        new StandartResponse()
                    //        {
                    //            InfoBlock = new InfoBlock()
                    //            {
                    //                UserMessage = "daUserSetting is null"
                    //            },
                    //            Success = false
                    //        });
                    //var wpayIdCcy = AppSecurity.CurrentUser.BaseCurrencyCode;
                    //if (wpayIdCcy == null)
                    //    return Content(HttpStatusCode.BadRequest,
                    //        new StandartResponse()
                    //        {
                    //            InfoBlock = new InfoBlock()
                    //            {
                    //                UserMessage = "wpayIdCcy is null"
                    //            },
                    //            Success = false
                    //        });
                    //if (daUserSetting.Success)
                    //{
                    //    wpayIdCcy = daUserSetting.Obj.CcyCode;
                    //}

                    //return Content(HttpStatusCode.BadRequest,
                    //    new StandartResponse()
                    //    {
                    //        InfoBlock = new InfoBlock()
                    //        {
                    //            UserMessage = "wpayIdCcy" + wpayIdCcy
                    //        },
                    //        Success = false
                    //    });

                    //else
                    //{
                    //    return Content(HttpStatusCode.BadRequest,
                    //        new StandartResponse()
                    //        {
                    //            InfoBlock = new InfoBlock()
                    //            {
                    //                UserMessage = "daUserSetting failed"
                    //            },
                    //            Success = false
                    //        });
                    //}

                    //var selectedCcy = 
                    //    balancesQueryRes.Balances.FirstOrDefault(w => w.CCY == wpayIdCcy);
                    //if (selectedCcy == null)
                    //    return Content(HttpStatusCode.BadRequest, new StandartResponse()
                    //    { InfoBlock = new InfoBlock() { UserMessage = "Please select currency" }, Success = false });
                    //else
                    //{
                    //    return Content(HttpStatusCode.BadRequest,
                    //        new StandartResponse()
                    //        {
                    //            InfoBlock = new InfoBlock()
                    //            {
                    //                UserMessage = "selectedCcy " + Newtonsoft.Json.JsonConvert.SerializeObject(selectedCcy)
                    //            },
                    //            Success = false
                    //        });
                    //}
                    // Check source type
                    var sourceType = _daPayLimitsSourceTypeServiceMethods.GetById(model.DaPayLimits_SourceType);
                    if (model.DaPayLimits_SourceType == default || !sourceType.Success || sourceType.Obj == null)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse() { InfoBlock = new InfoBlock() { UserMessage = "Please set correct source type" }, Success = false });


                    var dateRec = DateTime.Now;
                    var secretCode = Guid.NewGuid();
                    var newPayLimit = new DaPayLimitsSo()
                    {
                        //DaPayLimits_CcyCode = selectedCcy.CCY,
                        DaPayLimits_CreationDate = dateRec,
                        DaPayLimits_IsDeleted = false,
                        DaPayLimits_LimitCodeInitialization =
                            Newtonsoft.Json.JsonConvert.SerializeObject(
                                new LimitationString()
                                {
                                    DelegatedAuthorityCode = model.DaPayLimits_LimitCodeInitialization
                                }),
                        DaPayLimits_LastModifiedDate = dateRec,
                        DaPayLimits_SourceType = model.DaPayLimits_SourceType,
                        DaPayLimits_StatusByLimit = 0,
                        DaPayLimits_SecretCode = secretCode,
                        DaPayLimits_UserData = Crypto.Encrypt(Newtonsoft.Json.JsonConvert.SerializeObject(
                            new UserData()
                            {
                                UserId = ui.UserId,
                                UserName = ui.UserName,
                                Password = ui.Password
                            }
                        ), secretCode.ToString()),
                        DaPayLimits_UserName = ui.UserName,
                        DaPayLimits_DateOfExpire = model.DaPayLimits_DateOfExpire?.ToLocalTime(),
                        //DaPayLimits_PinCode = String.IsNullOrEmpty(model.DaPayLimits_PinCode)? string.Empty : model.DaPayLimits_PinCode,
                        DaPayLimits_IsPinProtected = model.DaPayLimits_IsPinProtected,
                        DaPayLimits_MediaName = model.DaPayLimits_MediaName,
                        DaPayLimits_WPayId = model.DaPayLimits_WPayId
                    };
                    
                    var res = _daForPayLimitsMethodsService.Insert(newPayLimit);

                    if (!res.Success)
                        return Content(HttpStatusCode.BadRequest,
                            new StandartResponse()
                            {
                                InfoBlock = new InfoBlock()
                                {
                                    UserMessage = GlobalRes.ShopingCardController_UnspecError,
                                    DeveloperMessage = res.Message
                                },
                                Success = false
                            });
                    //newPayLimit.TotalListOfLimits = _daPayLimitsTypeServiceMethods.GetAll()?.Obj?.Where(w => !w.DaPayLimitsType_IsDeleted).ToList() ?? new List<DaPayLimitsTypeSo>();
                    //var newTabs = from types in newPayLimit.TotalListOfLimits
                    //              join tabs in newPayLimit.DaPayLimitsTabs
                    //        on types.DaPayLimitsType_ID equals tabs.DaPayLimitsTab_TypeOfLimit into ps
                    //              from tabs in ps.DefaultIfEmpty()
                    //              select new DaPayLimitsTabSo()
                    //              {
                    //                  DaPayLimitsTab_ID = tabs?.DaPayLimitsTab_ID ?? Guid.Empty,
                    //                  DaPayLimitsTab_Amount = tabs?.DaPayLimitsTab_Amount ?? 0,
                    //                  DaPayLimitsTab_TypeOfLimit = types.DaPayLimitsType_ID,
                    //                  DaPayLimitsTab_IsDeleted = tabs == null || tabs.DaPayLimitsTab_ID == default,
                    //                  DaPayLimitsTab_ParentDaPayId = newPayLimit.DaPayLimits_ID,
                    //                  DaPayLimitsTab_DaPayLimitsType = types
                    //              };
                    //newPayLimit.DaPayLimitsTabs = newTabs.ToList();
                    newPayLimit.Success = true;
                    newPayLimit.InfoBlock = new InfoBlock() { UserMessage = "Success", DeveloperMessage = "Ok" };
                    return Ok(newPayLimit);
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