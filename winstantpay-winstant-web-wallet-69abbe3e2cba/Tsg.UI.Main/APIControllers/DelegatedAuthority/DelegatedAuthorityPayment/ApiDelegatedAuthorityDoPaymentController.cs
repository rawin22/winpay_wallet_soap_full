using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Http;
using Tsg.UI.Main.ApiMethods.AutomaticExchange;
using Tsg.UI.Main.ApiMethods.DelegatedAuthorityMethods;
using Tsg.UI.Main.ApiMethods.Payments;
using Tsg.UI.Main.ApiMethods.UserInfoMethods;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.APIModels.DelegatedAuthority;
using TSG.Models.APIModels.InstantPayment;
using TSG.Models.Enums;
using TSG.Models.ServiceModels.AutomaticExchange;
using TSG.Models.ServiceModels.LimitPayment;
using TSG.ServiceLayer.AutomaticExchange.DependencyLiquidForUserServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidCcyListServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidOverDraftUserServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsLogServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsSourceTypeServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTabServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTypeServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaUserWPayIDSettingServiceMethods;
using WinstantPay.Common.CryptDecriptInfo;
using WinstantPay.Common.Extension;

namespace Tsg.UI.Main.APIControllers.DelegatedAuthority.DelegatedAuthority
{
    [ApiFilter]
    public class ApiDelegatedAuthorityDoPaymentController : ApiController
    {
        readonly static log4net.ILog _logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string cryptoCurrency = ConfigurationManager.AppSettings["cryptoCurrencies"];

        private readonly IDaPayLimitsServiceMethods _daForPayLimitsMethodsService;
        private readonly IDaPayLimitsTabServiceMethods _daPayLimitsTabServiceMethods;
        private readonly IDaPayLimitsTypeServiceMethods _daPayLimitsTypeServiceMethods;
        private readonly IDaPayLimitsSourceTypeServiceMethods _daPayLimitsSourceTypeServiceMethods;
        private readonly IDaPayLimitsLogServiceMethods _daPayLimitsLogServiceMethods;
        private readonly IDependencyLiquidForUserServiceMethods _dependencyLiquidForUserService;
        private readonly ILiquidCcyListServiceMethods _liquidCcyListServiceMethods;
        private readonly ILiquidOverDraftUserServiceMethods _liquidOverDraftUserServiceMethods;
        private readonly IDaUserWPayIDSettingServiceMethods _daUserWPayIDSettingServiceMethods;


        public ApiDelegatedAuthorityDoPaymentController(IDaPayLimitsServiceMethods daForPayLimitsMethodsService,
            IDaPayLimitsTabServiceMethods daPayLimitsTabServiceMethods,
            IDaPayLimitsTypeServiceMethods daPayLimitsTypeServiceMethods,
            IDaPayLimitsSourceTypeServiceMethods daPayLimitsSourceTypeServiceMethods,
            IDaPayLimitsLogServiceMethods daPayLimitsLogServiceMethods,
            IDependencyLiquidForUserServiceMethods dependencyLiquidForUserService,
            ILiquidCcyListServiceMethods liquidCcyListServiceMethods,
            ILiquidOverDraftUserServiceMethods liquidOverDraftUserServiceMethods, IDaUserWPayIDSettingServiceMethods daUserWPayIDSettingServiceMethods)
        {   
            _daForPayLimitsMethodsService = daForPayLimitsMethodsService;
            _daPayLimitsTabServiceMethods = daPayLimitsTabServiceMethods;
            _daPayLimitsTypeServiceMethods = daPayLimitsTypeServiceMethods;
            _daPayLimitsSourceTypeServiceMethods = daPayLimitsSourceTypeServiceMethods;
            _daPayLimitsLogServiceMethods = daPayLimitsLogServiceMethods;
            
            _dependencyLiquidForUserService = dependencyLiquidForUserService;
            _liquidCcyListServiceMethods = liquidCcyListServiceMethods;
            _liquidOverDraftUserServiceMethods = liquidOverDraftUserServiceMethods;
            _daUserWPayIDSettingServiceMethods = daUserWPayIDSettingServiceMethods;
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] NewPaymentForDa model)
        {
            

            try
            {
                TSG.Models.APIModels.UserInfo ui;
                UserRepository userRepository = new UserRepository();
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    var userInfoMethods = new UserInfoMethods(ui);                    
                }
                else
                {
                    //result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.CheckUserError, DeveloperMessage = "Data for user not correct", UserMessage = "Data for user is not correct" };
                    return Content(HttpStatusCode.Unauthorized, new StandartResponse { InfoBlock = new InfoBlock() { UserMessage = "Data for user is not correct", DeveloperMessage = "Data for user not correct" } });
                }

                if (String.IsNullOrEmpty(model.DelegatedAuthorityCode))
                    return Content(HttpStatusCode.BadRequest, new StandartResponse { InfoBlock = new InfoBlock() { UserMessage = "Code undefined", DeveloperMessage = "Code string is null or empty" } });

                if (String.IsNullOrEmpty(model.CurrencyCode))
                    return Content(HttpStatusCode.BadRequest, new StandartResponse { InfoBlock = new InfoBlock() { UserMessage = "Currency undefined", DeveloperMessage = "Currency string is null or empty" } });

                if (String.IsNullOrEmpty(model.MerchantAlias.Trim()))
                    return Content(HttpStatusCode.BadRequest, new StandartResponse(GlobalRes.DelegatedAuthority_MediaNameValidationError));

                if (model.Amount <= 0 )
                    return Content(HttpStatusCode.BadRequest, new StandartResponse { InfoBlock = new InfoBlock() { UserMessage = "Amount cannot be 0 or negative", DeveloperMessage = "Amount is not be negative or 0" } });

                var delegatetAllList =
                    _daForPayLimitsMethodsService.GetAllDaByDeviceCode(Newtonsoft.Json.JsonConvert.SerializeObject(
                        new LimitationString()
                        {
                            DelegatedAuthorityCode = model.DelegatedAuthorityCode
                        })).Obj ?? new List<DaPayLimitsSo>();
                var delegatetList = delegatetAllList.Where(w => ((w.DaPayLimits_DateOfExpire > DateTime.Now) || w.DaPayLimits_DateOfExpire == null) && !w.DaPayLimits_IsDeleted).ToList() ?? new List<DaPayLimitsSo>();

                if(delegatetList.Count == 0)
                    return Content(HttpStatusCode.BadRequest, new StandartResponse { InfoBlock = new InfoBlock() { UserMessage = "Device/code not recognized or expired", DeveloperMessage = $"Not found device in system or active date is expired. Total device by code {model.DelegatedAuthorityCode} : {delegatetAllList.Count}" } });

                if (delegatetList.Count > 1)
                    return Content(HttpStatusCode.BadRequest, new StandartResponse { InfoBlock = new InfoBlock() { UserMessage = "Finded one more active device, payment unavaliable", DeveloperMessage = $"Finded one more active device. Total device by code {model.DelegatedAuthorityCode} : {delegatetAllList.Count}" } });

                var currentUser = _daForPayLimitsMethodsService.GetById(delegatetList[0].DaPayLimits_ID);
                var getPayerInfoBy = new UserData();
                try
                {
                    getPayerInfoBy = Newtonsoft.Json.JsonConvert.DeserializeObject<UserData>(
                        Crypto.Decrypt(currentUser.Obj.DaPayLimits_UserData,
                            currentUser.Obj.DaPayLimits_SecretCode.ToString()));
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                    return Content(HttpStatusCode.BadRequest, new StandartResponse { InfoBlock = new InfoBlock() { UserMessage = "Payer data is corrupt", DeveloperMessage = e.Message } });
                }

                AutomaticExchangeCommonMethods automaticExchangeCommonMethods = new AutomaticExchangeCommonMethods(new TSG.Models.APIModels.UserInfo() { UserName = getPayerInfoBy.UserName, UserId = getPayerInfoBy.UserId, Password = getPayerInfoBy.Password });
                decimal amountInDaLimit = model.Amount;

                var daUserSetting = _daUserWPayIDSettingServiceMethods.GetByWPayId(currentUser.Obj.DaPayLimits_WPayId);
                var wpayIdCcy = "";
                if (daUserSetting.Success)
                {
                    wpayIdCcy = daUserSetting.Obj.CcyCode;
                }
                else
                {
                    //var wpayIdSetting = PrepareNewDaUserWPayIDSettingSo(new Guid(ui.UserId), currentUser.Obj.DaPayLimits_WPayId, ui.BaseCurrencyCode);
                    var wpayIdSetting = PrepareNewDaUserWPayIDSettingSo(new Guid(ui.UserId), currentUser.Obj.DaPayLimits_WPayId, "USD");                    
                    
                    var settingInsert = _daUserWPayIDSettingServiceMethods.Insert(wpayIdSetting);
                    
                    if (settingInsert.Success)
                    {
                        daUserSetting = _daUserWPayIDSettingServiceMethods.GetByWPayId(currentUser.Obj.DaPayLimits_WPayId);
                        if (daUserSetting.Success)
                        {
                            wpayIdCcy = daUserSetting.Obj.CcyCode;
                        }
                    }
                }

                if (wpayIdCcy != model.CurrencyCode)
                {
                    var convertInBase = automaticExchangeCommonMethods.CreateQuoteForExchange(
                        model.CurrencyCode, wpayIdCcy, model.CurrencyCode, model.Amount);

                    if (convertInBase.ServiceResponse.HasErrors)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse($"Can't convert selected currency ({model.CurrencyCode}) " +
                                                                                       $"in currency by token ({wpayIdCcy})"));
                    try
                    {
                        // model.CurrencyCode = delegatetList[0]?.DaPayLimits_CcyCode;
                        // model.Amount = Convert.ToDecimal(convertInBase.Quote.SellAmount.Replace(convertInBase.Quote.SellCurrencyCode, "").Trim());
                        amountInDaLimit = Convert.ToDecimal(convertInBase.Quote.SellAmount.Replace(convertInBase.Quote.SellCurrencyCode, "").Trim());
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e);
                        return Content(HttpStatusCode.BadRequest, new StandartResponse($"Can't convert selected currency ({model.CurrencyCode}) " +
                                                                                       $"in currency by token ({wpayIdCcy})"));
                    }
                    
                    //return Content(HttpStatusCode.BadRequest,
                    //    new StandartResponse
                    //    {
                    //        InfoBlock = new InfoBlock()
                    //        {
                    //            UserMessage = $"Currency doesn't equals.",
                    //            DeveloperMessage =
                    //                $"Currency does not equals {model.CurrencyCode} / {delegatetList[0]?.DaPayLimits_CcyCode}"
                    //        }
                    //    });
                }

                var limitationTypes = _daPayLimitsTypeServiceMethods.GetAll();

                if(!limitationTypes.Success || limitationTypes.Obj == null /*|| limitationTypes.Obj.Count == 0*/)
                    return Content(HttpStatusCode.BadRequest, new StandartResponse { InfoBlock = new InfoBlock() { UserMessage = "System does not have limitation", DeveloperMessage = "System does not have limitation" } });

                if (!currentUser.Success || currentUser.Obj == null)
                    return Content(HttpStatusCode.BadRequest, new StandartResponse { InfoBlock = new InfoBlock() { UserMessage = "Code undefined", DeveloperMessage = "Code string is null or empty" } });

                //currentUser.Obj.TotalListOfLimits = limitationTypes.Obj.Where(w => !w.DaPayLimitsType_IsDeleted).ToList();
                var totalListOfLimits = limitationTypes.Obj.Where(w => !w.DaPayLimitsType_IsDeleted).ToList();
                var DaPayLimitsTabs = _daPayLimitsTabServiceMethods.GetAll().Obj.Where(ta => ta.DaPayLimitsTab_WPayId == currentUser.Obj.DaPayLimits_WPayId);
                
                // if (currentUser.Obj.DaPayLimitsTabs == null /*|| currentUser.Obj.DaPayLimitsTabs?.Count == 0*/)
                if (DaPayLimitsTabs == null /*|| currentUser.Obj.DaPayLimitsTabs?.Count == 0*/)
                    return Content(HttpStatusCode.BadRequest, new StandartResponse { InfoBlock = new InfoBlock() { UserMessage = "User does not have limitation", DeveloperMessage = "User does not have limitation" } });

                var limits = DaPayLimitsTabs.Where(w => !w.DaPayLimitsTab_IsDeleted).Join(totalListOfLimits,
                    so => so.DaPayLimitsTab_TypeOfLimit, tl => tl.DaPayLimitsType_ID, (so, tl) => new JoinedLimitations
                    {
                        MaxAmountByLimitation = so.DaPayLimitsTab_Amount,
                        IsDeletedLimitation = so.DaPayLimitsTab_IsDeleted,
                        IsSysLimitationDeleted = so.DaPayLimitsTab_IsDeleted,
                        LimitType = tl.DaPayLimitsType_LimitType,
                        LimitId = so.DaPayLimitsTab_ID,
                        LimitTypeGuid = tl.DaPayLimitsType_ID,
                        ExpireDate = currentUser.Obj.DaPayLimits_DateOfExpire,
                        //CurrencyCode = currentUser.Obj.DaPayLimits_CcyCode,
                        AmountByOrder = model.Amount
                    }).ToList();
                if (limits.Count != 0)
                {
                    var checkByLimits = CheckingLimitation.CheckByTypes(currentUser.Obj.DaPayLimitsLogs, limits);
                    if (checkByLimits.Any(a => !a.Success))
                        return Content(HttpStatusCode.BadRequest, new StandartResponse
                        {
                            InfoBlock = new InfoBlock()
                            {
                                UserMessage = checkByLimits.FirstOrDefault(f => !f.Success)?.InfoBlock.UserMessage ??
                                              "Exceeded limitation",
                                DeveloperMessage =
                                    checkByLimits.FirstOrDefault(f => !f.Success)?.InfoBlock.DeveloperMessage ??
                                    "Exceeded limitation"
                            }
                        });
                }

                // if (daUserSetting.Obj.IsPinRequired && string.IsNullOrEmpty(model.PinCode))
                if (daUserSetting.Obj.IsPinRequired && (model.Amount >= daUserSetting.Obj.ExceedingAmount && string.IsNullOrEmpty(model.PinCode)))
                    return Ok(new StandartResponse
                    {
                        InfoBlock = new InfoBlock()
                        {
                            UserMessage = "Please enter PIN code for payment order",
                            DeveloperMessage = "Need to enter user PIN code",
                            Code = ApiErrors.ErrorCodeState.NeedToSetUpPin
                        }
                    });

                if (daUserSetting.Obj.IsPinRequired && (!string.IsNullOrEmpty(model.PinCode)) && (model.Amount >= daUserSetting.Obj.ExceedingAmount && model.PinCode != daUserSetting.Obj.PinCode))
                    return Ok(new StandartResponse
                    {
                        InfoBlock = new InfoBlock()
                        {
                            UserMessage = "PIN code is not correct",
                            DeveloperMessage = "Need to enter correct PIN code"
                        }
                    });
                // Check merchant for know user 
                //TSG.Models.APIModels.UserInfo ui;
                //UserRepository userRepository = new UserRepository();
                //IEnumerable<string> outerUserToken;
                //var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                //if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                //{
                var getAliasForMerchant = new UserInfoMethods(ui).GetUserAliases();
                    if(getAliasForMerchant.Count == 0)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse
                        {
                            InfoBlock = new InfoBlock()
                            {
                                UserMessage = "Merchant does not contains avaliable WpayId",
                                DeveloperMessage = "Merchant does not contains avaliable WpayId"
                            }
                        });

                    if (getAliasForMerchant.All(a => a != model.MerchantAlias.Trim()))
                        return Content(HttpStatusCode.BadRequest, new StandartResponse(GlobalRes.DelegatedAuthority_IncorrectMerchantName));
                    
                    
                    var paymentMethods = new NewInstantPaymentMethods(new TSG.Models.APIModels.UserInfo() {UserName = getPayerInfoBy.UserName, UserId = getPayerInfoBy.UserId, Password = getPayerInfoBy.Password });
                    var payersAliases = paymentMethods.PrepareAccountAliases();
                    if(payersAliases.Count == 0)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse
                        {
                            InfoBlock = new InfoBlock()
                            {
                                UserMessage = "Payer does not contains avaliable WpayId",
                                DeveloperMessage = "Payer does not contains avaliable WpayId"
                            }
                        });
                    ApiNewInstantPaymentViewModel apiNewInstantPayment = new ApiNewInstantPaymentViewModel();
                    apiNewInstantPayment.FromCustomer = currentUser.Obj.DaPayLimits_WPayId;
                    apiNewInstantPayment.ToCustomer = model.MerchantAlias.Trim();
                    apiNewInstantPayment.Amount = Convert.ToDecimal(model.Amount);
                    apiNewInstantPayment.CurrencyCode = model.CurrencyCode;
                    apiNewInstantPayment.ReasonForPayment = "Purchase of product";

                    //Paid using Swisspass QR, via QR code TrustedKey
                    var sourceText = EnumHelper<DelegatedAuthorirySourceLimitationTypeEnum>.GetDisplayValue((DelegatedAuthorirySourceLimitationTypeEnum)currentUser.Obj.DaPaymentLimitSourceType.DaPaymentLimitSourceType_EnumNumber);
                    
                    apiNewInstantPayment.Memo = $"Paid using {currentUser.Obj.DaPayLimits_WPayId} {sourceText}, via {sourceText} Trusted Key";
                    //apiNewInstantPayment.Memo = $"Trusted token payment via {EnumHelper<DelegatedAuthorirySourceLimitationTypeEnum>.GetDisplayValue((DelegatedAuthorirySourceLimitationTypeEnum)currentUser.Obj.DaPaymentLimitSourceType.DaPaymentLimitSourceType_EnumNumber)}";
                    apiNewInstantPayment.Invoice = "";

                    var getAllBalances = paymentMethods.PrepareAccountBalances();
                    var liquidCcyList = _dependencyLiquidForUserService.GetAllSoByUser(Guid.Parse(getPayerInfoBy.UserId)).Obj;
                    
                    var selectedBalanceByCcy = getAllBalances.Balances.FirstOrDefault(f => f.CCY == model.CurrencyCode);
                    var baseCcy = getAllBalances.Balances.FirstOrDefault(f => f.CCY == (getAllBalances.Balances.FirstOrDefault()?.BaseCCY ?? "USD"));
                    if (selectedBalanceByCcy != null)
                    {
                        var overdraftUser = _liquidOverDraftUserServiceMethods.GetAllSo().Obj.FirstOrDefault(f =>
                            f.LiquidOverDraftUserList_UserId == Guid.Parse(getPayerInfoBy.UserId));

                        model.Amount = cryptoCurrency.Contains(selectedBalanceByCcy.CCY)
                            ? Decimal.Round(model.Amount, 8, MidpointRounding.AwayFromZero)
                            : Decimal.Round(model.Amount, 2, MidpointRounding.AwayFromZero);

                        var insufficientAmount = selectedBalanceByCcy.Balance - model.Amount;
                        if (insufficientAmount >= 0 || overdraftUser != null)
                        {
                            //var check = automaticExchangeCommonMethods.CommonChekingTotalCurrencies(liquidCcyList, getAllBalances, baseCcy, selectedBalanceByCcy, model.Amount);
                            //if (!check.Success)
                            //    return Content(HttpStatusCode.BadRequest, (StandartResponse)check);
                        }
                        else
                        {
                            /********** Automatic Exhange **********/
                            var check = automaticExchangeCommonMethods.CommonChekingBeforeAutomaticExchange(liquidCcyList, getAllBalances, baseCcy, selectedBalanceByCcy, insufficientAmount);

                            if (check.Success && !check.IsWarning)
                            {
                                if (liquidCcyList.Count > 0)
                                {
                                    var overdraftUsers = _liquidOverDraftUserServiceMethods.GetAllSo().Obj.Where(f =>
                                        f.LiquidOverDraftUserList_UserId == Guid.Parse(getPayerInfoBy.UserId)).ToList();

                                    var checkedLiquids = automaticExchangeCommonMethods.CheckingPaymentSize(model.CurrencyCode,
                                        model.Amount, overdraftUsers, getAllBalances, liquidCcyList.OrderBy(ob => ob.DependencyLiquidForUser_LiquidOrder).ToList(), true);


                                    if (checkedLiquids.Success && checkedLiquids.IsAutomaticExchange)
                                    {
                                        var allExchangeCcy = checkedLiquids.AutomaticExchngeListCcy.Where(w => !w.IsPaymentableCurrency).ToList();
                                        _logger.Info(Newtonsoft.Json.JsonConvert.SerializeObject(allExchangeCcy));
                                        StringBuilder sb = new StringBuilder();
                                        string s = "";
                                        allExchangeCcy.ForEach(f =>
                                        {
                                            sb.Append(String.Format(" {0} {1} at a rate of {2}{3} per {0} and", f.CurrencyCode, f.CurrencyAmount, f.Rate, model.CurrencyCode));
                                        });

                                        if (sb.Length > 4)
                                            s = sb.ToString().Remove(sb.Length - 4, 4);
                                        if (!String.IsNullOrEmpty(s) && !String.IsNullOrWhiteSpace(s))
                                        {
                                            var message =
                                                $"There was not enough {model.CurrencyCode} in your account to complete the transaction " +
                                                $"so automatic FX exchanged {model.CurrencyCode} {(cryptoCurrency.Contains(selectedBalanceByCcy.CCY) ? model.Amount.ToString("N8") : model.Amount.ToString("N2"))}" +
                                                $", selling " +
                                                $"{s}.";

                                            apiNewInstantPayment.Memo += $"{Environment.NewLine} {message}";
                                        }
                                    }
                                    else
                                    {
                                        return Content(HttpStatusCode.BadRequest, (StandartResponse)checkedLiquids);
                                    }
                                }
                                else
                                {
                                    return Content(HttpStatusCode.BadRequest, new StandartResponse(false, "Sorry, you do not have sufficient funds based on your selected liquidation preferences in your user profile."));
                                }
                            }
                            else
                            {
                                return Content(HttpStatusCode.BadRequest, new StandartResponse(false, "Sorry, you do not have sufficient funds based on your selected liquidation preferences in your user profile."));
                            }
                            /***************************************/
                        }
                    }
                    
                    var createPayment = paymentMethods.Create(apiNewInstantPayment);
                    
                    if (createPayment.ServiceResponse.HasErrors)
                    {
                        return Content(HttpStatusCode.BadRequest, new StandartResponse
                        {
                            InfoBlock = new InfoBlock()
                            {
                                UserMessage = "Payment can not be created",
                                DeveloperMessage = createPayment.ServiceResponse.Responses[0].Message + "\r\n" + createPayment.ServiceResponse.Responses[0].MessageDetails
                            }
                        });
                    }
                    if(!Guid.TryParse(createPayment.PaymentInformation.PaymentId, out var paymentGuid))
                        return Content(HttpStatusCode.BadRequest, new StandartResponse
                        {
                            InfoBlock = new InfoBlock()
                            {
                                UserMessage = "Payment can not be post",
                                DeveloperMessage = "Payment ID is not GUID type"
                            }
                        });

                    var postPayment = paymentMethods.Post(paymentGuid);
                    if(postPayment.ServiceResponse.HasErrors)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse
                        {
                            InfoBlock = new InfoBlock()
                            {
                                UserMessage = $"Sorry. Your payment can't be processed. Error message {postPayment.ServiceResponse.Responses[0].Message}",
                                DeveloperMessage = postPayment.ServiceResponse.Responses[0].Message + "\r\n" + postPayment.ServiceResponse.Responses[0].MessageDetails
                            }
                        });
                    var insertedObjLog = currentUser.Obj.DaPayLimitsLogs.LastOrDefault();
                    var newloglist = new List<DaPayLimitsLogSo>();
                    if(insertedObjLog != null)
                        newloglist.Add(insertedObjLog);


                    var newObj  = new DaPayLimitsSo()
                    {
                        Success = currentUser.Obj.Success,
                        DaPayLimits_DateOfExpire = currentUser.Obj.DaPayLimits_DateOfExpire,
                        DaPayLimits_ID = currentUser.Obj.DaPayLimits_ID,
                        //DaPayLimits_CcyCode = currentUser.Obj.DaPayLimits_CcyCode,
                        //DaPayLimits_PinCode = currentUser.Obj.DaPayLimits_PinCode,
                        // DaPayLimitsTabs = currentUser.Obj.DaPayLimitsTabs,
                        InfoBlock = currentUser.Obj.InfoBlock,
                        DaPayLimits_LimitCodeInitialization = currentUser.Obj.DaPayLimits_LimitCodeInitialization,
                        DaPayLimits_IsPinProtected = currentUser.Obj.DaPayLimits_IsPinProtected,
                        DaPayLimits_SourceType = currentUser.Obj.DaPayLimits_SourceType,
                        DaPaymentLimitSourceType = currentUser.Obj.DaPaymentLimitSourceType,
                        DaPayLimits_IsDeleted = currentUser.Obj.DaPayLimits_IsDeleted,
                        DaPayLimitsLogs = newloglist,
                        DaPayLimits_CreationDate = currentUser.Obj.DaPayLimits_CreationDate,
                        DaPayLimits_SecretCode = currentUser.Obj.DaPayLimits_SecretCode,
                        DaPayLimits_UserName = currentUser.Obj.DaPayLimits_UserName,
                        //TotalListOfLimits = currentUser.Obj.TotalListOfLimits,
                        DaPayLimits_UserData= currentUser.Obj.DaPayLimits_UserData,
                        DaPayLimits_StatusByLimit = currentUser.Obj.DaPayLimits_StatusByLimit,
                        DaPayLimits_DateOfExpireString = currentUser.Obj.DaPayLimits_DateOfExpireString,
                        DaPayLimits_LastModifiedDate = currentUser.Obj.DaPayLimits_LastModifiedDate,
                        DaPayLimits_TimeOfExpireString = currentUser.Obj.DaPayLimits_TimeOfExpireString,
                        ListOfBalances = currentUser.Obj.ListOfBalances
                    };
                    

                    var insertInLog = _daPayLimitsLogServiceMethods.Insert(new DaPayLimitsLogSo()
                    {
                        DaPayLimitsLog_ID = Guid.NewGuid(),
                        DaPayLimitsLog_DaPayParentId = currentUser.Obj.DaPayLimits_ID,
                        DaPayLimitsLog_Amount = model.Amount,
                        DaPayLimitsLog_CreateDate = DateTime.Now,
                        DaPayLimitsLog_CurrencyCode = model.CurrencyCode,
                        DaPayLimitsLog_UserName = currentUser.Obj.DaPayLimits_UserName,
                        DaPayLimitsLog_AmountInLimitsCurrency = amountInDaLimit,
                        //DaPayLimitsLog_LimitsCurrencyCode = currentUser.Obj.DaPayLimits_CcyCode,
                        DaPayLimitsLog_Info = Newtonsoft.Json.JsonConvert.SerializeObject(newObj) // save all data by limitation
                    });
                    if(!insertInLog.Success)
                        _logger.Error(insertInLog.Message);

                    InstantPaymentMethods ipm = new InstantPaymentMethods(new TSG.Models.APIModels.UserInfo() { UserName = getPayerInfoBy.UserName, UserId = getPayerInfoBy.UserId, Password = getPayerInfoBy.Password });
                    var result = new ApiInstantPaymentDetailsViewModel();
                    result = ipm.PrepareDetails(paymentGuid);
                    if (result == null)
                        throw new Exception("Null object");
                    var photoId = ipm.GetImageLink(result, String.Empty, ui);
                    result.ImageUrl =
                        $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/SharedViewer/PaymentDetails?photoId={photoId}";

                    return Ok(new StandartResponse<ApiInstantPaymentDetailsViewModel>
                    {
                        Obj = result,
                        Success = true,
                        InfoBlock = new InfoBlock()
                        {
                            UserMessage = "Thank you. Your payment has been received",
                            DeveloperMessage = "Success payment." + (insertInLog.Success ? "Success Add to log": "Not added to log")
                        }
                    });
                //}
                //return Content(HttpStatusCode.BadRequest, new StandartResponse("Undefined user"));
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Content(HttpStatusCode.BadRequest, new StandartResponse {InfoBlock = new InfoBlock() {UserMessage = GlobalRes.ShopingCardController_UnspecError, DeveloperMessage = e.Message } }); 
            }
        }

        public DaUserWPayIDSettingSo PrepareNewDaUserWPayIDSettingSo(Guid userId, string wpayId, string currencyCode, bool isPinRequired=false, string pinCode="", decimal exceedingAmount=0)
        {
            return new DaUserWPayIDSettingSo
            {                
                ID = Guid.NewGuid(),
                UserID = userId,
                WPayId = wpayId,
                IsPinRequired = isPinRequired,
                PinCode = pinCode,
                ExceedingAmount = exceedingAmount,
                CcyCode = currencyCode,
                IsDeleted = false,
                CreationDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
            };
        }
    }
}