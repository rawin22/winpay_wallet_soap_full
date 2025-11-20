using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Tsg.Business.Model.Classes;
using Tsg.UI.Main.ApiMethods.DelegatedAuthorityMethods;
using Tsg.UI.Main.ApiMethods.Payments;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Models.Security;
using TSG.Models.APIModels;
using TSG.Models.APIModels.DelegatedAuthority;
using TSG.Models.Enums;
using TSG.Models.ServiceModels.LimitPayment;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsSourceTypeServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTabServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTypeServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaUserWPayIDSettingServiceMethods;
using Crypto = WinstantPay.Common.CryptDecriptInfo.Crypto;

namespace Tsg.UI.Main.Controllers
{
    public class DaPaymentLimitsListController : Controller
    {
        
        private readonly IDaPayLimitsServiceMethods _daForPayLimitsMethodsService;
        private readonly IDaPayLimitsTabServiceMethods _daPayLimitsTabServiceMethods;
        private readonly IDaPayLimitsTypeServiceMethods _daPayLimitsTypeServiceMethods;
        private readonly IDaPayLimitsSourceTypeServiceMethods _daPayLimitsSourceTypeServiceMethods;
        readonly static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected IgpService Service = new IgpService(AppSecurity.CurrentUser.UserName, AppSecurity.CurrentUser.Password, AppSecurity.CurrentUser.UserId);
        private static readonly string cryptoCurrency = ConfigurationManager.AppSettings["cryptoCurrencies"];
        private readonly IDaUserWPayIDSettingServiceMethods _daUserWPayIDSettingServiceMethods;


        public DaPaymentLimitsListController(IDaPayLimitsServiceMethods payLimitsMethodsService,
            IDaPayLimitsTypeServiceMethods daPayLimitsTypeServiceMethods,
            IDaPayLimitsTabServiceMethods daPayLimitsTabServiceMethods,
            IDaPayLimitsSourceTypeServiceMethods daPayLimitsSourceTypeServiceMethods, IDaUserWPayIDSettingServiceMethods daUserWPayIDSettingServiceMethods)
        {
            _daForPayLimitsMethodsService = payLimitsMethodsService;
            _daPayLimitsTypeServiceMethods = daPayLimitsTypeServiceMethods;
            _daPayLimitsTabServiceMethods = daPayLimitsTabServiceMethods;
            _daPayLimitsSourceTypeServiceMethods = daPayLimitsSourceTypeServiceMethods;
            _daUserWPayIDSettingServiceMethods = daUserWPayIDSettingServiceMethods;
        }

        // GET: DaPaymentLimitsList
        public ActionResult Index()
        {
            return View(_daForPayLimitsMethodsService.GetAll().Obj.Where(w=>!w.DaPayLimits_IsDeleted && !w.DaPayLimits_IsTransfered && 
                                                                            (w.DaPayLimits_DateOfExpire.HasValue && w.DaPayLimits_DateOfExpire.Value > DateTime.Now || !w.DaPayLimits_DateOfExpire.HasValue)));
        }

        // GET: DaPaymentLimitsList/Create
        public ActionResult Create(Guid? sourceType)
        {
            if (!sourceType.HasValue || sourceType.Value == default)
                return RedirectToAction("Index", "DelegatedAuthority");

            var sourceQuery = _daPayLimitsSourceTypeServiceMethods.GetById(sourceType.Value);
            if (!sourceQuery.Success || sourceQuery.Obj == null)
                return RedirectToAction("Index", "DelegatedAuthority");
            var model = new DaPayLimitsSo()
            {
                DaPayLimits_SourceType = sourceType.Value,
                DaPaymentLimitSourceType = sourceQuery.Obj,
                AccountWPayIds = PrepareAccountAliases()
            };

            return View(model);
        }

        // POST: DaPaymentLimitsList/Create
        [HttpPost]
        public ActionResult Create(DaPayLimitsSo model)
        {
            try
            {
                if (model.DaPayLimits_ID != default)
                    return Json(
                        new DaPayLimitsSo()
                        {
                            InfoBlock = new InfoBlock()
                            { UserMessage = GlobalRes.DelegatedAuthority_CreatedEarly },
                            Success = false
                        }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);

                //Check source type
                var sourceType = _daPayLimitsSourceTypeServiceMethods.GetById(model.DaPayLimits_SourceType);
                if (model.DaPayLimits_SourceType == default || !sourceType.Success || sourceType.Obj == null)
                    return Json(
                        new DaPayLimitsSo()
                        {
                            InfoBlock = new InfoBlock()
                            { UserMessage = "Source not found" },

                            Success = false
                        }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                // Check media name
                if (String.IsNullOrEmpty(model.DaPayLimits_MediaName) && String.IsNullOrWhiteSpace(model.DaPayLimits_MediaName))
                    return Json(
                        new DaPayLimitsSo()
                        { InfoBlock = new InfoBlock(GlobalRes.DelegatedAuthority_MediaNameValidationError),

                            Success = false
                        }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);

                //if (model.DaPayLimits_IsPinProtected && String.IsNullOrEmpty(model.DaPayLimits_PinCode))
                //    return Json(new StandartResponse("Please insert PIN code or disable checkbox \"I want protect payment with PIN\""));

                var checkLimit = _daForPayLimitsMethodsService.GetAllDaByDeviceCode(Newtonsoft.Json.JsonConvert.SerializeObject(
                    new LimitationString()
                    {
                        DelegatedAuthorityCode = model.DaPayLimits_LimitCodeInitialization
                    }));

                // We can add check to balance

                // Check selected cureency
                //if (String.IsNullOrEmpty(model.DaPayLimits_CcyCode))
                //    return Json(
                //        new DaPayLimitsSo()
                //        {
                //            InfoBlock = new InfoBlock()
                //            { UserMessage = GlobalRes.DelegatedAuthority_CurrencyValidationError },
                //            Success = false
                //        }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);

                if (DateTime.TryParseExact(model.DaPayLimits_DateOfExpireString, "dd/MM/yyyy",
                        CultureInfo.CurrentCulture, DateTimeStyles.None, out var date) &&
                    TimeSpan.TryParse(model.DaPayLimits_TimeOfExpireString, out var time))
                    model.DaPayLimits_DateOfExpire = date.Add(time);
                // Task-0041
                //else
                //{
                //    return Json(
                //        new DaPayLimitsSo()
                //        {
                //            InfoBlock = new InfoBlock()
                //            { UserMessage = GlobalRes.DelegatedAuthority_ExpDateValidationError },
                //            Success = false
                //        }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                //}

                if (model.DaPayLimits_DateOfExpire!=null && model.DaPayLimits_DateOfExpire.Value <= DateTime.Now)
                    return Json(
                        new DaPayLimitsSo()
                        {
                            InfoBlock = new InfoBlock()
                            { UserMessage = GlobalRes.DelegatedAuthority_ExpDateValidationError },
                            Success = false
                        }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);

                // Check limits
                if (!checkLimit.Success || checkLimit.Obj?.Where(w => ((w.DaPayLimits_DateOfExpire != null && w.DaPayLimits_DateOfExpire > DateTime.Now) || w.DaPayLimits_DateOfExpire == null) && !w.DaPayLimits_IsDeleted).Count() > 0)
                    return Json(
                        new DaPayLimitsSo()
                        {
                            InfoBlock = new InfoBlock()
                                { UserMessage = "Sorry. This code is taken. Try again." },

                            Success = false
                        }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);


                // We can add check to balance

                // Check ccy code
                var balancesQueryRes = new NewInstantPaymentMethods(AppSecurity.CurrentUser).PrepareAccountBalances();
                if (balancesQueryRes.ServiceResponse.HasErrors)
                    return Json(
                        new StandartResponse()
                        {
                            InfoBlock = new InfoBlock()
                            {
                                UserMessage =
                                    $"{balancesQueryRes.ServiceResponse.Responses[0].MessageDetails} \r\n {balancesQueryRes.ServiceResponse.Responses[0].MessageDetails}"
                            },
                            Success = false
                        });

                var daUserSetting = _daUserWPayIDSettingServiceMethods.GetByWPayId(model.DaPayLimits_WPayId);
                var wpayIdCcy = AppSecurity.CurrentUser.BaseCurrencyCode;
                if (daUserSetting.Success)
                {
                    wpayIdCcy = daUserSetting.Obj.CcyCode;
                }

                var selectedCcy =
                    balancesQueryRes.Balances.FirstOrDefault(w => w.CCY == wpayIdCcy);


                if (selectedCcy == null)
                    return Json(new StandartResponse()
                    { InfoBlock = new InfoBlock() { UserMessage = "Please select currency" }, Success = false });

                var dateRec = DateTime.Now;
                //if (!String.IsNullOrEmpty(model.DaPayLimits_PinCode) && model.DaPayLimits_PinCode.Length != 4)
                //    return Json(new StandartResponse()
                //    { InfoBlock = new InfoBlock() { UserMessage = "Please set correct Pin code" }, Success = false });

                if (sourceType.Obj.DaPaymentLimitSourceType_EnumNumber == (int)DelegatedAuthorirySourceLimitationTypeEnum.SecretCode && !String.IsNullOrEmpty(model.DaPayLimits_LimitCodeInitialization) && model.DaPayLimits_LimitCodeInitialization.Length < 6)
                    return Json(new StandartResponse()
                    { InfoBlock = new InfoBlock() { UserMessage = "At least 6 characters, may contain lowercase letters & numbers" }, Success = false });



                string delegatedAuth = model.DaPayLimits_LimitCodeInitialization;
                if (sourceType.Obj.DaPaymentLimitSourceType_EnumNumber == (int)DelegatedAuthorirySourceLimitationTypeEnum.Qr)
                    delegatedAuth = Guid.NewGuid().ToString();
                if(sourceType.Obj.DaPaymentLimitSourceType_EnumNumber == (int)DelegatedAuthorirySourceLimitationTypeEnum.BarCode)
                    delegatedAuth = DateTime.Now.Ticks.ToString();
                
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
                                DelegatedAuthorityCode = delegatedAuth
                            }),
                    DaPayLimits_LastModifiedDate = dateRec,
                    DaPayLimits_SourceType = model.DaPayLimits_SourceType,
                    DaPayLimits_StatusByLimit = 0,
                    //DaPayLimits_IsPinProtected = !String.IsNullOrEmpty(model.DaPayLimits_PinCode),
                    DaPayLimits_SecretCode = secretCode,
                    //DaPayLimits_PinCode = String.IsNullOrEmpty(model.DaPayLimits_PinCode) ? string.Empty : model.DaPayLimits_PinCode,
                    DaPayLimits_UserData = Crypto.Encrypt(Newtonsoft.Json.JsonConvert.SerializeObject(
                        new UserData()
                        {
                            UserId = AppSecurity.CurrentUser.UserId,
                            UserName = AppSecurity.CurrentUser.UserName,
                            Password = AppSecurity.CurrentUser.Password
                        }
                    ), secretCode.ToString()),
                    DaPayLimits_UserName = AppSecurity.CurrentUser.UserName,
                    DaPayLimits_DateOfExpire = model.DaPayLimits_DateOfExpire,
                    DaPayLimits_MediaName = model.DaPayLimits_MediaName,
                    DaPayLimits_WPayId = model.DaPayLimits_WPayId,
                    //DaPayLimits_RequiredPinAmount = model.DaPayLimits_RequiredPinAmount,
                    //DaPayLimits_IsRestrictedToSelectedCurrency = model.DaPayLimits_IsRestrictedToSelectedCurrency,

                };

                var res = _daForPayLimitsMethodsService.Insert(newPayLimit);

                if (!res.Success)
                    return Json(
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
                //newPayLimit.DaPayLimitsTabs = new List<DaPayLimitsTabSo>();
                //return RedirectToAction("Edit", new {id=newPayLimit.DaPayLimits_ID});
                //return PartialView("~/Views/DaPaymentLimitsList/QrLimitsPaymentTab/_QrLimiTabView.cshtml", newPayLimit);
                newPayLimit.Success = true;
                return Json(newPayLimit);
            }
            catch (Exception e)
            {
                _logger.Error(e);

                return Json(
                    new StandartResponse()
                    {
                        InfoBlock = new InfoBlock()
                        {
                            UserMessage = GlobalRes.ShopingCardController_UnspecError
                        },
                        Success = false
                    });
            }
        }

        // GET: DaPaymentLimitsList/Edit/5
        public ActionResult Edit(Guid id)
        {
            var resObjSo = _daForPayLimitsMethodsService.GetById(id);
            if (!resObjSo.Success || resObjSo.Obj == null)
                return RedirectToAction("Index", "DelegatedAuthority");

            resObjSo.Obj.DaPayLimits_LimitCodeInitialization = Newtonsoft.Json.JsonConvert
                .DeserializeObject<LimitationString>(resObjSo.Obj.DaPayLimits_LimitCodeInitialization)
                .DelegatedAuthorityCode;
            resObjSo.Obj.DaPayLimits_WPayId = String.IsNullOrEmpty(resObjSo.Obj.DaPayLimits_WPayId) ? "" : resObjSo.Obj.DaPayLimits_WPayId;      
            resObjSo.Obj.AccountWPayIds = PrepareAccountAliases();
            //resObjSo.Obj.DaPayLimits_RequiredPinAmount = cryptoCurrency.Contains(resObjSo.Obj.DaPayLimits_CcyCode)
            //                ? Decimal.Round(resObjSo.Obj.DaPayLimits_RequiredPinAmount, 8, MidpointRounding.AwayFromZero)
            //                : Decimal.Round(resObjSo.Obj.DaPayLimits_RequiredPinAmount, 2, MidpointRounding.AwayFromZero);
            return View(resObjSo.Obj);
        }

        // POST: DaPaymentLimitsList/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid id, DaPayLimitsSo model)
        {
            try
            {
                // TODO: Add update logic here
                var currId = _daForPayLimitsMethodsService.GetById(id);
                if (!currId.Success && currId.Obj == null)
                    return Json(new StandartResponse("Object not found"));

                if (DateTime.TryParseExact(model.DaPayLimits_DateOfExpireString, "dd/MM/yyyy",
                        CultureInfo.CurrentCulture, DateTimeStyles.None, out var date) &&
                    TimeSpan.TryParse(model.DaPayLimits_TimeOfExpireString, out var time))
                    model.DaPayLimits_DateOfExpire = date.Add(time);
                // Task-0041
                if (model.DaPayLimits_DateOfExpire != null && model.DaPayLimits_DateOfExpire.Value <= DateTime.Now)
                    return Json(
                        new StandartResponse(GlobalRes.DelegatedAuthority_ExpDateValidationError), 
                        "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                
                var sourceType = _daPayLimitsSourceTypeServiceMethods.GetById(model.DaPayLimits_SourceType);
                if (model.DaPayLimits_SourceType == default || !sourceType.Success || sourceType.Obj == null)
                    return Json(
                        new DaPayLimitsSo()
                        {
                            InfoBlock = new InfoBlock()
                            { UserMessage = "Source not found" },

                            Success = false
                        }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);

                //if (!String.IsNullOrEmpty(model.DaPayLimits_PinCode) && model.DaPayLimits_PinCode.Length != 4)
                //    return Json(new StandartResponse()
                //    { InfoBlock = new InfoBlock() { UserMessage = "Please set correct Pin code" }, Success = false });

                if (sourceType.Obj.DaPaymentLimitSourceType_EnumNumber == (int)DelegatedAuthorirySourceLimitationTypeEnum.SecretCode && !String.IsNullOrEmpty(model.DaPayLimits_LimitCodeInitialization) && model.DaPayLimits_LimitCodeInitialization.Length < 6)
                    return Json(new StandartResponse()
                    { InfoBlock = new InfoBlock() { UserMessage = "At least 6 characters, may contain lowercase letters & numbers" }, Success = false });

                //currId.Obj.DaPayLimits_CcyCode = model.DaPayLimits_CcyCode.Substring(0, 3);
                currId.Obj.DaPayLimits_DateOfExpire = model.DaPayLimits_DateOfExpire;
                //currId.Obj.DaPayLimits_PinCode = String.IsNullOrEmpty(model.DaPayLimits_PinCode)
                //    ? string.Empty : model.DaPayLimits_PinCode;
                //currId.Obj.DaPayLimits_IsPinProtected = !String.IsNullOrEmpty(model.DaPayLimits_PinCode);
                currId.Obj.DaPayLimits_WPayId = model.DaPayLimits_WPayId;
                //currId.Obj.DaPayLimits_RequiredPinAmount = model.DaPayLimits_RequiredPinAmount;
                //currId.Obj.DaPayLimits_IsRestrictedToSelectedCurrency = model.DaPayLimits_IsRestrictedToSelectedCurrency;
                
                var updateResult = _daForPayLimitsMethodsService.Update(currId.Obj);
                if (!updateResult.Success)
                    return Json(new StandartResponse("Delegeted authority does not updated successifully", updateResult.Message));


                //var limits = model.DaPayLimitsTabs.Where(w => !w.DaPayLimitsTab_IsDeleted).Join(currId.Obj.TotalListOfLimits,
                //    so => so.DaPayLimitsTab_TypeOfLimit, tl => tl.DaPayLimitsType_ID, (so, tl) => new CheckingLimitations
                //    {
                //        IsDeletedLimitation = so.DaPayLimitsTab_IsDeleted,
                //        IsSysLimitationDeleted = so.DaPayLimitsTab_IsDeleted,
                //        LimitType = tl.DaPayLimitsType_LimitType,
                //        LimitId = so.DaPayLimitsTab_ID,
                //        LimitTypeGuid = tl.DaPayLimitsType_ID,
                //        CurrencyCode = currId.Obj.DaPayLimits_CcyCode,
                //        AmountByLimitation = so.DaPayLimitsTab_Amount
                //    }).ToList();


                //var checkByLimits = CheckingLimitation.CheckTypesByAmount(model.DaPayLimitsTabs.Where(w=>!w.DaPayLimitsTab_IsDeleted).ToList(), limits.Where(w => !w.IsDeletedLimitation && !w.IsSysLimitationDeleted).ToList());
                //if (checkByLimits.Any(a => !a.Success))
                //{
                //    var res = checkByLimits.FirstOrDefault(f => !f.Success)?.InfoBlock.UserMessage ?? "Exceeded limitation";
                //    return Json(new StandartResponse(res));
                //}

                //foreach (var daPayLimitsTabSo in model.DaPayLimitsTabs)
                //{
                //    if (!daPayLimitsTabSo.DaPayLimitsTab_IsDeleted && daPayLimitsTabSo.DaPayLimitsTab_Amount <= 0)
                //    {
                //        daPayLimitsTabSo.Success = false; daPayLimitsTabSo.InfoBlock = new InfoBlock()
                //        {
                //            UserMessage = "Insert correct amount",
                //            DeveloperMessage = "Negative amount or amount equals 0"
                //        };
                //        continue;
                //    }
                //    if (daPayLimitsTabSo.DaPayLimitsTab_ID == default)
                //    {
                //        if (currId.Obj.DaPayLimitsTabs.Select(s => s.DaPayLimitsTab_TypeOfLimit)
                //            .Contains(daPayLimitsTabSo.DaPayLimitsTab_TypeOfLimit))
                //        {
                //            daPayLimitsTabSo.InfoBlock = new InfoBlock() { UserMessage = "Record alwayls exist", DeveloperMessage = "Record alwayls exist by current source type, enter Id for limitation " };
                //            continue;
                //        }
                //        var resInsert = _daPayLimitsTabServiceMethods.Insert(daPayLimitsTabSo);
                //        daPayLimitsTabSo.Success = resInsert.Success;
                //        daPayLimitsTabSo.InfoBlock =
                //            new InfoBlock() { UserMessage = resInsert.Success ? "Success" : "Error", DeveloperMessage = String.IsNullOrEmpty(resInsert.Message) ? "OK" : resInsert.Message };
                //    }
                //    else
                //    {
                //        var currRecTab = currId.Obj.DaPayLimitsTabs.FirstOrDefault(f =>
                //            f.DaPayLimitsTab_ID == daPayLimitsTabSo.DaPayLimitsTab_ID);
                //        if (currRecTab != null)
                //        {
                //            //if (getById.Obj.DaPayLimitsTabs.Select(s => s.DaPayLimitsTab_ID)
                //            //        .Contains(daPayLimitsTabSo.DaPayLimitsTab_ID) && getById.Obj.DaPayLimitsTabs.Select(s => s.DaPayLimitsTab_TypeOfLimit)
                //            //        .Contains(daPayLimitsTabSo.DaPayLimitsTab_TypeOfLimit))
                //            //{
                //            //}
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

                //if (model.DaPayLimitsTabs.Any(a => !a.Success))
                //{
                //    return Json(new StandartResponse(false, model.DaPayLimitsTabs.FirstOrDefault(f => !f.Success)?.InfoBlock.UserMessage));
                //}
                return Json(new StandartResponse(updateResult.Success, String.IsNullOrEmpty(updateResult.Message) ? "All data will be save successfully" : updateResult.Message));

            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Json(new StandartResponse(GlobalRes.ShopingCardController_UnspecError, e.Message));
            }
        }

        [HttpPost]
        public ActionResult SaveLimitTabItem(DaPayLimitsTabSo model)
        {
            // check if model is null
            if (model == null)
                return Json(new StandartResponse { Success = false, InfoBlock = new InfoBlock { UserMessage = "Data corrupted" } }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            // Check limit type
            var limitType = _daPayLimitsTypeServiceMethods.GetById(model.DaPayLimitsTab_TypeOfLimit);
            if (!limitType.Success || limitType.Obj == null)
                return Json(new DaPayLimitsTabSo { Success = false, InfoBlock = new InfoBlock { UserMessage = "Undefined limit" } }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            //Check parent for tabs 
            if (model.DaPayLimitsTab_ParentDaPayId == default)
                return Json(new StandartResponse { Success = false, InfoBlock = new InfoBlock { UserMessage = "Undefined parent id" } }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            var parentDaLimit = _daForPayLimitsMethodsService.GetById(model.DaPayLimitsTab_ParentDaPayId);
            if (!parentDaLimit.Success || parentDaLimit.Obj == null)
                return Json(new StandartResponse { Success = false, InfoBlock = new InfoBlock { UserMessage = "Trusted Token not found" } }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            //var tab = parentDaLimit.Obj.DaPayLimitsTabs?.FirstOrDefault(f => f.DaPayLimitsTab_ID == model.DaPayLimitsTab_ID);
            if (model.DaPayLimitsTab_Amount <= 0)
                return Json(new StandartResponse { Success = false, InfoBlock = new InfoBlock { UserMessage = "Amount must be positive" } }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            //if (model.DaPayLimitsTab_ID != default && tab == null)
            //    return Json(new StandartResponse { Success = false, InfoBlock = new InfoBlock { UserMessage = "Limitation type blocked or not found" } }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);

            //var testPaymentBeforeInsert = ApiMethods.DelegatedAuthorityMethods.CheckingLimitation.CheckByTypeBeforeInsert(parentDaLimit.Obj.DaPayLimitsTabs, model, (DelegatedAuthoriryLimitationType)limitType.Obj.DaPayLimitsType_LimitType);
            //if (!testPaymentBeforeInsert.Success)
            //    return Json(new StandartResponse { Success = false,
            //        InfoBlock = testPaymentBeforeInsert.InfoBlock }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);


            if (model.DaPayLimitsTab_ID == default)
            {
                var insertRes = _daPayLimitsTabServiceMethods.Insert(model);
                model = insertRes.Obj;
                return Json(new { DaPayLimitsTab_ID = model.DaPayLimitsTab_ID, Success = insertRes.Success, InfoBlock = new InfoBlock { UserMessage = insertRes.Success ? "Success saved" : "Error saved" } }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }

            var updateRes = _daPayLimitsTabServiceMethods.Update(model);
            return Json(new { DaPayLimitsTab_ID = model.DaPayLimitsTab_ID, Success = updateRes.Success, InfoBlock = new InfoBlock { UserMessage = updateRes.Success ? "Success saved" : "Error saved" } }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }

        // POST: DaPaymentLimitsList/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid daId)
        {
            try
            {
                if (daId == default)
                    return Json(new StandartResponse("Not found by this ID"));
                var getById = _daForPayLimitsMethodsService.GetById(daId);
                if (!getById.Success || getById.Obj == null)
                    return Json(new StandartResponse("Not found by this ID"));
                if (getById.Obj.DaPayLimits_UserName != AppSecurity.CurrentUser.UserName)
                    return Json(new StandartResponse("Device is not found"));

                if (getById.Obj.DaPayLimits_IsDeleted)
                    //return RedirectToAction("Index", "DelegatedAuthority");
                    return Json(new StandartResponse(true, "OK"));

                if (getById.Obj.DaPayLimits_DateOfExpire < DateTime.Now)
                    return Json(new StandartResponse("Device expired"));
                    
                var setIdDeletedQRes = _daForPayLimitsMethodsService.Delete(daId);

                if (!setIdDeletedQRes.Success)
                    return Json(new StandartResponse("This trusted token not deleted", setIdDeletedQRes.Message));
                return Json(new StandartResponse(setIdDeletedQRes.Success, String.IsNullOrEmpty(setIdDeletedQRes.Message) ? "OK" : setIdDeletedQRes.Message));
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Json(new StandartResponse(GlobalRes.ShopingCardController_UnspecError, e.Message));
            }
        }

        [NonAction]
        private IList<SelectListItem> PrepareAccountAliases()
        {
            var aliasListItem = new List<SelectListItem>();
            var aliases = Service.GetAccountAliases();

            aliasListItem.Add
            (
                new SelectListItem
                {
                    Text = "",
                    Value = ""
                }
            );

            if (!aliases.ServiceResponse.HasErrors)
            {
                foreach (var item in aliases.Aliases)
                {
                    aliasListItem.Add
                    (
                        new SelectListItem
                        {
                            Text = item.AccountAlias,
                            Value = item.AccountAlias
                        }
                    );
                }
            }
            return aliasListItem;
        }
    }
}