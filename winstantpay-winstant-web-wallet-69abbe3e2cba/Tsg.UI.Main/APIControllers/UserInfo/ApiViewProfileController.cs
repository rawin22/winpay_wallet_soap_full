using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tsg.UI.Main.ApiMethods.UserInfoMethods;
using TSG.Models.APIModels;
using TSG.Models.APIModels.UserInformation;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Repository;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTabServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTypeServiceMethods;
using TSG.Models.ServiceModels.LimitPayment;
using TSG.ServiceLayer.LimitPayment.DaUserWPayIDSettingServiceMethods;

namespace Tsg.UI.Main.APIControllers.UserInfo
{
    /// <summary>
    /// View profile info
    /// </summary>
    [ApiFilter]
    public class ApiViewProfileController : ApiController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IDaPayLimitsTabServiceMethods _daPayLimitsTabServiceMethods;
        private readonly IDaPayLimitsTypeServiceMethods _daPayLimitsTypeServiceMethods;
        private readonly IDaUserWPayIDSettingServiceMethods _daUserWPayIDSettingServiceMethods;

        public ApiViewProfileController(IDaPayLimitsTabServiceMethods daPayLimitsTabServiceMethods, IDaPayLimitsTypeServiceMethods daPayLimitsTypeServiceMethods, IDaUserWPayIDSettingServiceMethods daUserWPayIDSettingServiceMethods)
        {
            _daPayLimitsTabServiceMethods = daPayLimitsTabServiceMethods;
            _daPayLimitsTypeServiceMethods = daPayLimitsTypeServiceMethods;
            _daUserWPayIDSettingServiceMethods = daUserWPayIDSettingServiceMethods;
        }
        
        [HttpGet]
        public IHttpActionResult Get()
        {
            var result = new ViewProfileModel();
            UserRepository userRepository = new UserRepository();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out var ui))
                {
                    var userInfoMethods = new UserInfoMethods(ui);
                    userInfoMethods.GetUserData(result);
                    
                    result.UserAliases = userInfoMethods.GetUserAliases();
                    result.LimitsTabs = GetLimitTabs(new Guid(ui.UserId));
                    result.LimitsTypes = GetLimitTypes();
                    result.DaUserWPayIDSettings = GetDaUserWPayIDSettings(new Guid(ui.UserId));                    
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, UserMessage = string.Empty, DeveloperMessage = "Ok" };
                    result.Success = true;
                }
            }
            catch (ApiException apiException)
            {
                result.InfoBlock = apiException.CustomInfoBlock;
                _logger.Error(apiException);
            }
            catch (Exception e)
            {
                result.InfoBlock = new InfoBlock()
                {
                    Code = ApiErrors.ErrorCodeState.UnspecifiedError,
                    DeveloperMessage = e.InnerException?.ToString() ?? e.Message,
                    UserMessage = "We have some problem with your query. Please try again"
                };
                _logger.Error(e);
            }

            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult Post(ViewProfileModel model)
        {
            UserRepository userRepository = new UserRepository();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out var ui))
                {
                    SaveDaUserWPayIdSetting(model.DaUserWPayIDSettings);

                    foreach (var daPayLimitsTab in model.LimitsTabs)
                    {
                        //model.Message += "</br>Tabs ID" + daPayLimitsTab.ID + ", IsDelete: " + daPayLimitsTab.IsDeleted + ", Amount: " + daPayLimitsTab.Amount;
                        if (!daPayLimitsTab.DaPayLimitsTab_IsDeleted && daPayLimitsTab.DaPayLimitsTab_Amount <= 0)
                        {
                            //daPayLimitsTab.Success = false; daPayLimitsTabSo.InfoBlock = new InfoBlock()
                            //{
                            //    UserMessage = "Insert correct amount",
                            //    DeveloperMessage = "Negative amount or amount equals 0"
                            //};
                            continue;
                        }
                        if (daPayLimitsTab.DaPayLimitsTab_ID == default)
                        {

                            //if ( currId.Obj.DaPayLimitsTabs.Select(s => s.DaPayLimitsTab_TypeOfLimit)
                            //    .Contains(daPayLimitsTabSo.DaPayLimitsTab_TypeOfLimit))
                            //{
                            //    daPayLimitsTabSo.InfoBlock = new InfoBlock() { UserMessage = "Record alwayls exist", DeveloperMessage = "Record alwayls exist by current source type, enter Id for limitation " };
                            //    continue;
                            //}
                            // var TabSo = daPayLimitsTab.PrepareDaPayTabSo();
                            //daPayLimitsTab.DaPayLimitsTab_UserId = new Guid(ui.UserId);
                            //daPayLimitsTab.DaPayLimitsTab_WPayId = daPayLimitsTab.WPayId;



                            var resInsert = _daPayLimitsTabServiceMethods.Insert(daPayLimitsTab);
                            daPayLimitsTab.Success = resInsert.Success;

                            daPayLimitsTab.InfoBlock =
                                new InfoBlock() { UserMessage = resInsert.Success ? "Success" : "Error", DeveloperMessage = String.IsNullOrEmpty(resInsert.Message) ? "OK" : resInsert.Message };
                            //model.Message += "<br/> daPayLimitsTab.ID == default, Insert TabSo: " + JsonConvert.SerializeObject(TabSo);
                        }
                        else
                        {
                            var currRecTab = _daPayLimitsTabServiceMethods.GetAll().Obj.FirstOrDefault(t => t.DaPayLimitsTab_ID == daPayLimitsTab.DaPayLimitsTab_ID);
                            if (currRecTab != null)
                            {
                                //if (getById.Obj.DaPayLimitsTabs.Select(s => s.DaPayLimitsTab_ID)
                                //        .Contains(daPayLimitsTabSo.DaPayLimitsTab_ID) && getById.Obj.DaPayLimitsTabs.Select(s => s.DaPayLimitsTab_TypeOfLimit)
                                //        .Contains(daPayLimitsTabSo.DaPayLimitsTab_TypeOfLimit))
                                //{
                                //}
                                currRecTab.DaPayLimitsTab_Amount = daPayLimitsTab.DaPayLimitsTab_Amount;
                                currRecTab.DaPayLimitsTab_IsDeleted = daPayLimitsTab.DaPayLimitsTab_IsDeleted;

                                var updateQuery = _daPayLimitsTabServiceMethods.Update(currRecTab);
                                currRecTab.Success = updateQuery.Success;
                                currRecTab.InfoBlock = new InfoBlock() { UserMessage = updateQuery.Success ? "Success" : "Error", DeveloperMessage = String.IsNullOrEmpty(updateQuery.Message) ? "OK" : updateQuery.Message };
                                //model.Message += "<br/> daPayLimitsTab.ID != default, Update TabSo: " + JsonConvert.SerializeObject(currRecTab);

                            }
                            else
                            {
                                //daPayLimitsTabSo.Success = false;
                                //daPayLimitsTabSo.InfoBlock = new InfoBlock()
                                //{
                                //    UserMessage = "Not updated",
                                //    DeveloperMessage = "Record not found. Update record unavaliable"
                                //};
                                //model.Message += "<br/> currRecTab is null";
                            }
                        }
                    }
                }

            }
            catch (ApiException apiException)
            {
                model.InfoBlock = apiException.CustomInfoBlock;
                _logger.Error(apiException);
            }
            catch (Exception e)
            {
                model.InfoBlock = new InfoBlock()
                {
                    Code = ApiErrors.ErrorCodeState.UnspecifiedError,
                    DeveloperMessage = e.InnerException?.ToString() ?? e.Message,
                    UserMessage = "We have some problem saving user profile. Please try again"
                };
                _logger.Error(e);
            }

            ////model.Message += "<br/>Before DaPayLimitsTabs: " + JsonConvert.SerializeObject(model.LimitTabs);
            //model.UserID = new Guid(AppSecurity.CurrentUser.UserId);
            //model.LimitTypes = PrepareDaLimitsTypes();
            ////model.WPayIds = PrepareWPayIds();
            //model.IsCrypto = cryptoCurrency.Contains(model.CcyCode);
            //model = PrepareDaUserWPayIdSetting(model);
            //model.LimitTabs = PrepareWPayIdDaLimitsTabs(model.WPayId, model.IsCrypto);
            ////model.Message += "<br/>After DaPayLimitsTabs: " + JsonConvert.SerializeObject(model.LimitTabs);

            // return View(model);
            return Ok(model);


        }

        [NonAction]
        private List<DaPayLimitsTabSo> GetLimitTabs(Guid userId, string wPayId)
        {
            //var tabs = _daPayLimitsTabServiceMethods.GetAll().Obj?.Where(w => !w.DaPayLimitsTab_IsDeleted && w.DaPayLimitsTab_WPayId == wPayId).ToList();

            return GetLimitTabs(userId).Where(t => t.DaPayLimitsTab_WPayId == wPayId).ToList();
        }

        [NonAction]
        private List<DaPayLimitsTabSo> GetLimitTabs(Guid userId)
        {
            var tabs = _daPayLimitsTabServiceMethods.GetAll().Obj?.Where(w => !w.DaPayLimitsTab_IsDeleted && w.DaPayLimitsTab_UserId==userId).ToList();

            return tabs;
        }

        [NonAction]
        private List<DaPayLimitsTypeSo> GetLimitTypes()
        {
            var types = _daPayLimitsTypeServiceMethods.GetAll().Obj;

            return types;
        }

        [NonAction]
        private List<DaUserWPayIDSettingSo> GetDaUserWPayIDSettings(Guid userId)
        {
            var settings = _daUserWPayIDSettingServiceMethods.GetAll().Obj?.Where(w => !w.IsDeleted && w.UserID == userId).ToList();

            return settings;
        }

        [NonAction]
        private void SaveDaUserWPayIdSetting(List<DaUserWPayIDSettingSo> settings)
        {
            foreach (var setting in settings)
            {
                if (setting.ID == default)
                {
                    setting.ID = Guid.NewGuid();
                    //model.Message += "<br />model.DaUserWPayIDSettingID == default";

                    // var daSettingSo = model.PrepareDaUserWPayIDSettingSo();
                    // daSettingSo.ID = Guid.NewGuid();

                    setting.IsDeleted = false;
                    var resInsert = _daUserWPayIDSettingServiceMethods.Insert(setting);
                    //model.Message += "<br /> resInsert.Message: " + resInsert.Message;

                }
                else
                {
                    //model.Message += "<br />model.DaUserWPayIDSettingID != default";
                    var daUserSettingSo = _daUserWPayIDSettingServiceMethods.GetById(setting.ID).Obj;
                    if (daUserSettingSo != null)
                    {
                        var recUpdate = _daUserWPayIDSettingServiceMethods.Update(setting);                        
                    }
                }
            }
        }
    }
}
