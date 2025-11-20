using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Tsg.Business.Model.Classes;
using Tsg.UI.Main.ApiMethods.FavoriteCurrency;
using Tsg.UI.Main.ApiMethods.UserInfoMethods;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Attributes;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.ServiceModels.SuperUserModels;
using TSG.Models.ServiceModels.UsersDataBlock;
using TSG.ServiceLayer.AutomaticExchange.DependencyLiquidForUserServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidCcyListServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTabServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTypeServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaUserWPayIDSettingServiceMethods;
using TSG.ServiceLayer.SuperAdmin;
using TSG.ServiceLayer.Users;
using Crypto = WinstantPay.Common.CryptDecriptInfo.Crypto;

namespace Tsg.UI.Main.Controllers
{
	public class UserController : BaseController
	{
		readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private SqlConnection _con;
		protected IgpService Service;
		private readonly IUsersServiceMethods _usersServiceMethods;
		private readonly IDependencyLiquidForUserServiceMethods _dependencyLiquidForUserService;
		private readonly ILiquidCcyListServiceMethods _liquidCcyListServiceMethods;
		private readonly ISaSharedLinkForAdminServiceMethods _saSharedLinkForAdminServiceMethods;
		private readonly IDaPayLimitsTypeServiceMethods _daPayLimitsTypeServiceMethods;
		private readonly IDaPayLimitsTabServiceMethods _daPayLimitsTabServiceMethods;
		private readonly IDaUserWPayIDSettingServiceMethods _daUserWPayIDSettingServiceMethods;
		private static readonly string cryptoCurrency = ConfigurationManager.AppSettings["cryptoCurrencies"];

		// protected IgpService Service = new IgpService(AppSecurity.CurrentUser.UserName, AppSecurity.CurrentUser.Password, AppSecurity.CurrentUser.UserId);




		public UserController(IUsersServiceMethods usersServiceMethods, IDependencyLiquidForUserServiceMethods dependencyLiquidForUserService,
			ILiquidCcyListServiceMethods liquidCcyListServiceMethods,
			ISaSharedLinkForAdminServiceMethods saSharedLinkForAdminServiceMethods, IDaPayLimitsTypeServiceMethods daPayLimitsTypeServiceMethods, IDaPayLimitsTabServiceMethods daPayLimitsTabServiceMethods, IDaUserWPayIDSettingServiceMethods daUserWPayIDSettingServiceMethods)
		{
			_usersServiceMethods = usersServiceMethods;
			_dependencyLiquidForUserService = dependencyLiquidForUserService;
			_liquidCcyListServiceMethods = liquidCcyListServiceMethods;
			_saSharedLinkForAdminServiceMethods = saSharedLinkForAdminServiceMethods;
			_daPayLimitsTypeServiceMethods = daPayLimitsTypeServiceMethods;
			_daPayLimitsTabServiceMethods = daPayLimitsTabServiceMethods;
			_daUserWPayIDSettingServiceMethods = daUserWPayIDSettingServiceMethods;
		}

		private void Connection()
		{
			string conStr = ConfigurationManager.ConnectionStrings["ewalletDbCon"].ToString();
			_con = new SqlConnection(conStr);
		}

		public new ActionResult Profile(string wPayId)
		{

			UserProfileViewModel model = new UserProfileViewModel();
			//model.Message += "Profile GET";
			model.UserID = new Guid(AppSecurity.CurrentUser.UserId);
			model.LimitTypes = PrepareDaLimitsTypes();
			// model.CcyCode = AppSecurity.CurrentUser.BaseCurrencyCode;
			model.WPayId = wPayId;
			if (!string.IsNullOrEmpty(model.WPayId))
			{
				//model.Amount = cryptoCurrency.Contains(selectedBalanceByCcy.CCY)
				//            ? Decimal.Round(model.Amount, 8, MidpointRounding.AwayFromZero)
				//            : Decimal.Round(model.Amount, 2, MidpointRounding.AwayFromZero);
				model.CcyCode = AppSecurity.CurrentUser.BaseCurrencyCode;
				model.IsCrypto = cryptoCurrency.Contains(model.CcyCode);
				model.LimitTabs = PrepareWPayIdDaLimitsTabs(wPayId, model.IsCrypto);
			}
			//model.WPayIds = PrepareWPayIds();
			//model.Message += "<br/>Selected CCY: " + model.CcyCode;
			//model.Message += "<br/>Selected WPayId: " + model.WPayId;
			//model.Message += "<br/>WPayIds: " + JsonConvert.SerializeObject(model.WPayIds);          
			//model.Message += "<br/>DaPayLimitsTabs: " + JsonConvert.SerializeObject(model.LimitTabs);
			model = PrepareDaUserWPayIdSetting(model);
			//model.Message += "<br/>Model: " + JsonConvert.SerializeObject(model);
			return View(model);

			// return View();
		}

		[HttpPost]
		public new ActionResult Profile(UserProfileViewModel model)
		{
			// var DaLimitsTab = _daPayLimitsTabServiceMethods.GetAll().Obj.Where(t => t.DaPayLimitsTab_WPayId.Equals(model.WPayId)).ToList();
			//model.Message += "<br/>Profile POST";
			//model.Message += "</br>DaPayLimitsTabs.Count" + model.LimitTabs.Count;
			//model.Message += "<br/>CcyCode: " + model.CcyCode;
			if (model.IsPinRequired == true && (String.IsNullOrEmpty(model.PinCode.Trim()) || model.PinCode.Length < 4))
			{
				model.HasError = true;
				model.Errors.Add(new ErrorViewModel()
				{
					MessageDetails = "PIN must be at least 4 digits"
				});


				//return View(model);
			}
			else
			{
				SaveDaUserWPayIdSetting(model);

				foreach (var daPayLimitsTab in model.LimitTabs)
				{
					//model.Message += "</br>Tabs ID" + daPayLimitsTab.ID + ", IsDelete: " + daPayLimitsTab.IsDeleted + ", Amount: " + daPayLimitsTab.Amount;
					if (!daPayLimitsTab.IsDeleted && daPayLimitsTab.Amount <= 0)
					{
						//daPayLimitsTab.Success = false; daPayLimitsTabSo.InfoBlock = new InfoBlock()
						//{
						//    UserMessage = "Insert correct amount",
						//    DeveloperMessage = "Negative amount or amount equals 0"
						//};
						continue;
					}
					if (daPayLimitsTab.ID == default)
					{

						//if ( currId.Obj.DaPayLimitsTabs.Select(s => s.DaPayLimitsTab_TypeOfLimit)
						//    .Contains(daPayLimitsTabSo.DaPayLimitsTab_TypeOfLimit))
						//{
						//    daPayLimitsTabSo.InfoBlock = new InfoBlock() { UserMessage = "Record alwayls exist", DeveloperMessage = "Record alwayls exist by current source type, enter Id for limitation " };
						//    continue;
						//}
						var TabSo = daPayLimitsTab.PrepareDaPayTabSo();
						TabSo.DaPayLimitsTab_UserId = new Guid(AppSecurity.CurrentUser.UserId);
						TabSo.DaPayLimitsTab_WPayId = model.WPayId;



						var resInsert = _daPayLimitsTabServiceMethods.Insert(TabSo);
						TabSo.Success = resInsert.Success;

						TabSo.InfoBlock =
							new InfoBlock() { UserMessage = resInsert.Success ? "Success" : "Error", DeveloperMessage = String.IsNullOrEmpty(resInsert.Message) ? "OK" : resInsert.Message };
						//model.Message += "<br/> daPayLimitsTab.ID == default, Insert TabSo: " + JsonConvert.SerializeObject(TabSo);
					}
					else
					{
						var currRecTab = _daPayLimitsTabServiceMethods.GetAll().Obj.FirstOrDefault(t => t.DaPayLimitsTab_ID == daPayLimitsTab.ID);
						if (currRecTab != null)
						{
							//if (getById.Obj.DaPayLimitsTabs.Select(s => s.DaPayLimitsTab_ID)
							//        .Contains(daPayLimitsTabSo.DaPayLimitsTab_ID) && getById.Obj.DaPayLimitsTabs.Select(s => s.DaPayLimitsTab_TypeOfLimit)
							//        .Contains(daPayLimitsTabSo.DaPayLimitsTab_TypeOfLimit))
							//{
							//}
							currRecTab.DaPayLimitsTab_Amount = daPayLimitsTab.Amount;
							currRecTab.DaPayLimitsTab_IsDeleted = daPayLimitsTab.IsDeleted;

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
			//model.Message += "<br/>Before DaPayLimitsTabs: " + JsonConvert.SerializeObject(model.LimitTabs);
			model.UserID = new Guid(AppSecurity.CurrentUser.UserId);
			model.LimitTypes = PrepareDaLimitsTypes();
			//model.WPayIds = PrepareWPayIds();
			model.IsCrypto = cryptoCurrency.Contains(model.CcyCode);
			model = PrepareDaUserWPayIdSetting(model);
			model.LimitTabs = PrepareWPayIdDaLimitsTabs(model.WPayId, model.IsCrypto);
			// model.Message += "<br/>Model: " + JsonConvert.SerializeObject(model);
			model.Message += "<br/>DaUserWPayIDSettingID: " + model.DaUserWPayIDSettingID;

			//model.Message += "<br/>After DaPayLimitsTabs: " + JsonConvert.SerializeObject(model.LimitTabs);

			return View(model);
			// return PartialView("_UserDaLimits", model);
		}
		public new ActionResult GetWPayIdDaLimits(string wPayId)
		{
			UserProfileViewModel model = new UserProfileViewModel();
			//model.Message += "GetWPayIdDaLimits";
			model.UserID = new Guid(AppSecurity.CurrentUser.UserId);
			model.LimitTypes = PrepareDaLimitsTypes();
			model.CcyCode = AppSecurity.CurrentUser.BaseCurrencyCode;
			model.WPayId = wPayId;
			//model.WPayIds = PrepareWPayIds();
			model.IsCrypto = cryptoCurrency.Contains(model.CcyCode);
			model.LimitTabs = PrepareWPayIdDaLimitsTabs(wPayId, model.IsCrypto);
			model = PrepareDaUserWPayIdSetting(model);
			return PartialView("_UserDaLimits", model);
			// return View("Profile", model);

			// return View();
		}

		//[HttpPost]
		//public new ActionResult GetWPayIdDaLimits(UserProfileViewModel model)
		//{
		//    model.Message += "GetWPayIdDaLimits POST";
		//    foreach (var daPayLimitsTab in model.DaPayLimitsTabs)
		//    {
		//        if (!daPayLimitsTab.IsDeleted && daPayLimitsTab.Amount <= 0)
		//        {
		//            //daPayLimitsTab.Success = false; daPayLimitsTabSo.InfoBlock = new InfoBlock()
		//            //{
		//            //    UserMessage = "Insert correct amount",
		//            //    DeveloperMessage = "Negative amount or amount equals 0"
		//            //};
		//            continue;
		//        }
		//        if (daPayLimitsTab.ID == default)
		//        {

		//            //if ( currId.Obj.DaPayLimitsTabs.Select(s => s.DaPayLimitsTab_TypeOfLimit)
		//            //    .Contains(daPayLimitsTabSo.DaPayLimitsTab_TypeOfLimit))
		//            //{
		//            //    daPayLimitsTabSo.InfoBlock = new InfoBlock() { UserMessage = "Record alwayls exist", DeveloperMessage = "Record alwayls exist by current source type, enter Id for limitation " };
		//            //    continue;
		//            //}
		//            var TabSo = daPayLimitsTab.PrepareDaPayTabSo();
		//            TabSo.DaPayLimitsTab_UserId = model.UserID;
		//            TabSo.DaPayLimitsTab_WPayId = model.WPayId;

		//            var resInsert = _daPayLimitsTabServiceMethods.Insert(TabSo);
		//            //daPayLimitsTabSo.Success = resInsert.Success;
		//            //daPayLimitsTabSo.InfoBlock =
		//            //    new InfoBlock() { UserMessage = resInsert.Success ? "Success" : "Error", DeveloperMessage = String.IsNullOrEmpty(resInsert.Message) ? "OK" : resInsert.Message };
		//        }
		//        else
		//        {
		//            var currRecTab = _daPayLimitsTabServiceMethods.GetAll().Obj.FirstOrDefault(t => t.DaPayLimitsTab_ID == daPayLimitsTab.ID);
		//            if (currRecTab != null)
		//            {
		//                //if (getById.Obj.DaPayLimitsTabs.Select(s => s.DaPayLimitsTab_ID)
		//                //        .Contains(daPayLimitsTabSo.DaPayLimitsTab_ID) && getById.Obj.DaPayLimitsTabs.Select(s => s.DaPayLimitsTab_TypeOfLimit)
		//                //        .Contains(daPayLimitsTabSo.DaPayLimitsTab_TypeOfLimit))
		//                //{
		//                //}
		//                currRecTab.DaPayLimitsTab_Amount = daPayLimitsTab.Amount;
		//                currRecTab.DaPayLimitsTab_IsDeleted = daPayLimitsTab.IsDeleted;

		//                var updateQuery = _daPayLimitsTabServiceMethods.Update(currRecTab);
		//                //daPayLimitsTabSo.Success = updateQuery.Success;
		//                //daPayLimitsTabSo.InfoBlock = new InfoBlock() { UserMessage = updateQuery.Success ? "Success" : "Error", DeveloperMessage = String.IsNullOrEmpty(updateQuery.Message) ? "OK" : updateQuery.Message };

		//            }

		//            else
		//            {
		//                //daPayLimitsTabSo.Success = false;
		//                //daPayLimitsTabSo.InfoBlock = new InfoBlock()
		//                //{
		//                //    UserMessage = "Not updated",
		//                //    DeveloperMessage = "Record not found. Update record unavaliable"
		//                //};
		//            }
		//        }
		//    }

		//    return PartialView("_UserDaLimits", model);
		//}

		[HttpPost]
		public ActionResult CreateNewAlias(string newAliasName)
		{
			try
			{
				Service = new IgpService(AppSecurity.CurrentUser.AccessToken, AppSecurity.CurrentUser.UserId);
				var addNewAliasRes = Service.AddNewUserAliases(newAliasName);
				if (!addNewAliasRes.ServiceResponse.HasErrors)
				{
					return Json(new { success = true, message = GlobalRes.User_UserController_CreateNewAlias_AddSuccess }, JsonRequestBehavior.DenyGet);
				}
				else
				{
					var res = addNewAliasRes.ServiceResponse.Responses[0].MessageDetails;
					throw new Exception(res);
				}
			}
			catch (Exception e)
			{
				if (e.Message.Contains("already exists"))
				{
					ViewBag.ErrorMessage = GlobalRes.User_UserController_CreateNewAlias_ErrAdded;
					return Json(new { success = false, message = ViewBag.ErrorMessage }, JsonRequestBehavior.DenyGet);
				}
				else
				{
					ViewBag.ErrorMessage = e.Message;
					return Json(new { success = false, message = e.Message }, JsonRequestBehavior.DenyGet);
				}
			}
		}

		/// <summary>
		/// Delete user alias
		/// </summary>
		/// <param name="alias">User alias to be deleted</param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult DeleteAlias(string alias)
		{
			try
			{
				Service = new IgpService(AppSecurity.CurrentUser.AccessToken, AppSecurity.CurrentUser.UserId);
				var deleteAliasRes = Service.DeleteUserAlias(alias);
				if (!deleteAliasRes.ServiceResponse.HasErrors)
				{
					return Json(new { success = true, message = GlobalRes.User_UserController_CreateNewAlias_AddSuccess }, JsonRequestBehavior.DenyGet);
				}
				else
				{
					var res = deleteAliasRes.ServiceResponse.Responses[0].MessageDetails;
					throw new Exception(res);
				}
			}
			catch (Exception e)
			{

				ViewBag.ErrorMessage = e.Message;
				return Json(new { success = false, message = e.Message }, JsonRequestBehavior.DenyGet);

			}
		}

		/// <summary>
		/// Set customer default alias
		/// </summary>
		/// <param name="alias">Alias to be set as default</param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult SetDefaultAlias(string alias)
		{
			try
			{
				Service = new IgpService(AppSecurity.CurrentUser.AccessToken, AppSecurity.CurrentUser.UserId);
				var addNewAliasRes = Service.SetUserDefaultAlias(alias);
				if (!addNewAliasRes.ServiceResponse.HasErrors)
				{
					return Json(new { success = true, message = GlobalRes.User_UserController_CreateNewAlias_AddSuccess }, JsonRequestBehavior.DenyGet);
				}
				else
				{
					_logger.Error(string.Format("SetDefaultAlias error, web service response: {0}", JsonConvert.SerializeObject(addNewAliasRes)));
					var res = addNewAliasRes.ServiceResponse.Responses[0].MessageDetails;
					throw new Exception(res);
				}
			}
			catch (Exception e)
			{
				_logger.Error(string.Format("SetDefaultAlias error, exception: {0}", JsonConvert.SerializeObject(e)));
				ViewBag.ErrorMessage = e.Message;
				return Json(new { success = false, message = e.Message }, JsonRequestBehavior.DenyGet);
				//if (e.Message.Contains("already exists"))
				//{
				//    ViewBag.ErrorMessage = GlobalRes.User_UserController_CreateNewAlias_ErrAdded;
				//    return Json(new { success = false, message = ViewBag.ErrorMessage }, JsonRequestBehavior.DenyGet);
				//}
				//else
				//{
				//    ViewBag.ErrorMessage = e.Message;
				//    return Json(new { success = false, message = e.Message }, JsonRequestBehavior.DenyGet);
				//}
			}
		}

		[HttpPost]
		public JsonResult Ping()
		{
			return Json("Ok");
		}


		[HttpPost]
		public JsonResult AbandonSession()
		{
			AppSecurity.CurrentUser = null;
			Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-30));
			Response.Cache.SetValidUntilExpires(false);
			Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
			Response.Cache.SetCacheability(HttpCacheability.NoCache);
			Response.Cache.SetNoStore();

			FormsAuthentication.SignOut();
			return Json("expired");
		}

		[AllowAnonymous]
		[HttpGet]
		public ActionResult Login(string returnUrl)
		{
			UserLoginModel model = new UserLoginModel();

			if (TempData.ContainsKey("ErrorMessage"))
			{
				ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
			}

			if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
				returnUrl = Server.UrlEncode(Request.UrlReferrer.PathAndQuery);

			if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
			{
				//ViewBag.ReturnURL = returnUrl;
				model.ReturnUrl = returnUrl;
			}
			UserRepository userRepo = new UserRepository();
			var uiVersion = userRepo.GetUiInfo();
			//ViewBag.UiVersion = uiVersion;

			Session["UiVersion"] = uiVersion;
			return View(model);
		}


		[AllowAnonymous]
		[HttpGet]
		public ActionResult GetUiInfo()
		{
			UserRepository userRepo = new UserRepository();
			var uiVersion = userRepo.GetUiInfo();
			Session["UiVersion"] = uiVersion;
			return Json(new StandartResponse(true, uiVersion));
		}

		[AllowAnonymous]
		[HttpPost]
		public ActionResult Login(UserLoginModel model, string returnUrl)
		{
			if (String.IsNullOrEmpty(model.ReturnUrl))
				model.ReturnUrl = returnUrl;
			try
			{
				if (ModelState.IsValid)
				{
					UserRepository userRepo = new UserRepository();
					ViewBag.UiVersion = userRepo.GetUiInfo();

					if (model.ServiceAuthenticate() || model.LocalUserLogin())
					{
						/****** New login logic ******/

						if (AppSecurity.CurrentUser == null)
							return RedirectToAction("Login");

						if (AppSecurity.CurrentUser != null && AppSecurity.CurrentUser.IsBlockLocal)
						{
							ViewBag.ErrorMessage = GlobalRes.User_UserController_Login_LocalUserBlocked;
							_logger.Error("User " + model.Username + " attempted to log in but blocked");
							TempData["ErrorMessage"] = ViewBag.ErrorMessage;
							return RedirectToAction("Login", new { returnUrl = returnUrl });
						}

						if (AppSecurity.CurrentUser.Role != UserRoleType.SuperUser)
						{
							var checkUserRes = _usersServiceMethods.InsertOrUpdateInfo(AppSecurity.CurrentUser);
							if (!checkUserRes.Success || checkUserRes.Obj == null)
							{
								_logger.Error(checkUserRes.Message);
								return RedirectToAction("Login");
							}

							_logger.Info("User " + model.Username + " successfully logged in");
							ViewBag.Message = GlobalRes.User_UserController_Login_UpdateLastLogin;
							AppSecurity.CurrentUser.ShowWelcomeMessage =
								checkUserRes.Obj.User_NeedToSearchWelcomeMessage;
							AppSecurity.CurrentUser.CurrentLoginDate = DateTime.Now;


							if (checkUserRes.Obj.User_IsNewUser)
							{
								// get all currency
								FavoriteCurrencyMethods favoriteCurrencyMethods =
									new FavoriteCurrencyMethods(AppSecurity.CurrentUser);
								FavoriteCurrencyRepository favoriteCurrencyRepository =
									new FavoriteCurrencyRepository();
								string res;

								foreach (var currencyViewModel in favoriteCurrencyMethods.PrepareCurrencies())
								{
									try
									{
										favoriteCurrencyRepository.AddFavoriteCurrency(new FavoriteCurrencyModel()
										{
											CurrencyCode = currencyViewModel.CurrencyCode.ToString(),
											IdUser = AppSecurity.CurrentUser.UserId
										}, out res);
									}
									catch (Exception e)
									{
										_logger.Error(e);
									}
								}
							}


							if (!checkUserRes.Obj.User_IsLocal.HasValue || !checkUserRes.Obj.User_IsLocal.Value)
							{
								var uiMethod = new UserInfoMethods(AppSecurity.CurrentUser);
								var getUserWpayIds = uiMethod?.GetUserAliases();
								if (getUserWpayIds.Count > 0)
								{
									_usersServiceMethods.SaveUserAliases(new UserAliasesSo()
									{
										Wpay_UserId = Guid.Parse(AppSecurity.CurrentUser.UserId),
										Wpay_UserName = AppSecurity.CurrentUser.UserName,
										Wpay_Ids = getUserWpayIds
									});
								}
							}

							/************ Set liquid currency for user **********/
							if (checkUserRes.Obj.User_Role.Role_RoleName == "User")
							{


								var allliquidCurrency = _liquidCcyListServiceMethods.GetAll().Obj
									.Where(w => w.LiquidCcyList_IsLiquidCurrency)
									.OrderBy(ob => ob.LiquidCcyList_LiquidOrder).Select(s => s.LiquidCcyList_Id)
									.ToList();
								var userLiquids =
									_dependencyLiquidForUserService.GetAllSoByUser(
										Guid.Parse(AppSecurity.CurrentUser.UserId));
								if (allliquidCurrency.Count > 0 && userLiquids.Obj?.Count == 0)
								{
									//var liquidsForUser = new List<Guid>();

									if (checkUserRes.Obj.User_UserIdByTSG != null)
									{
										//liquidsForUser = _dependencyLiquidForUserService
										//    .GetAllSoByUser((Guid) checkUserRes.Obj.User_UserIdByTSG).Obj
										//    .Select(s => s.DependencyLiquidForUser_LiquidCcyId).ToList();
										//var expectedList = allliquidCurrency.Except(liquidsForUser).ToList();
										var setLiquidsCcyRes =
											_dependencyLiquidForUserService.BulkInsertLiquidCurrencyForUser(
												string.Join(",", allliquidCurrency),
												(Guid)checkUserRes.Obj.User_UserIdByTSG);
									}
								}

							}

							/****************************************************/
						}

						/************ Getting system setting for user in db *******************/
						var props = userRepo.SysValues();
						SettingClass.SetSysEmailUploadProof(props.FirstOrDefault(f => f.PropertyName == "adminNotificationMailForUploadProof")?.PropertyValue.ToString());
						string decodedUrl = "";
						if (!string.IsNullOrEmpty(returnUrl) && !returnUrl.ToLower().Contains("json"))
							decodedUrl = Server.UrlDecode(returnUrl);
						/**********************************************************************/

						if (!String.IsNullOrEmpty(decodedUrl))
							return Redirect(decodedUrl);
						return RedirectToAction("Index", "Home");
					}
					else
					{
						if (model.ErrorCode == 1000214)
						{
							TempData["ErrorMessage"] = model.ErrorMessageDetails;
						}
						else
						{
							_logger.DebugFormat("Login Error Message: {0} ", model.ErrorMessage);
							ViewBag.ErrorMessage = GlobalRes.User_UserController_Login_PasswordNotMatch;
							_logger.ErrorFormat("User {0} attempted to log in but failed", model.Username);
							TempData["ErrorMessage"] = ViewBag.ErrorMessage;
						}

						return RedirectToAction("Login", new { returnUrl = returnUrl });
					}
				}
				else { _logger.Info("Model state not valid"); }
			}
			catch (Exception e)
			{
				_logger.Error(e);
				_logger.Error(e.InnerException);
			}
			return View(model);
		}


		[AllowAnonymous]
		public ActionResult CheckoutLogin(string src, string token, string message)
		{
			if (!string.IsNullOrEmpty(message))
				ViewBag.ErrorMessage = message;
			return View("CheckoutLogin");
		}

		[AllowAnonymous]
		[HttpPost]
		public ActionResult CheckoutLogin(string src, string token, UserLoginModel model)
		{
			if (ModelState.IsValid)
			{
				UserRepository userRepo = new UserRepository();

				if (model.ServiceLogin() || model.LocalUserLogin())
				{
					if (AppSecurity.CurrentUser == null)
						return RedirectToAction("Login");

					var checkUserRes = _usersServiceMethods.InsertOrUpdateInfo(AppSecurity.CurrentUser);
					if (!checkUserRes.Success || checkUserRes.Obj == null)
					{
						_logger.Error(checkUserRes.Message);
						return RedirectToAction("Login");
					}
					_logger.Info("User " + model.Username + " successfully logged in");
					ViewBag.Message = GlobalRes.User_UserController_Login_UpdateLastLogin;
					AppSecurity.CurrentUser.ShowWelcomeMessage = checkUserRes.Obj.User_NeedToSearchWelcomeMessage;
					AppSecurity.CurrentUser.CurrentLoginDate = DateTime.Now;

					if (checkUserRes.Obj.User_IsNewUser)
					{
						// get all currency
						FavoriteCurrencyMethods favoriteCurrencyMethods = new FavoriteCurrencyMethods(AppSecurity.CurrentUser);
						FavoriteCurrencyRepository favoriteCurrencyRepository = new FavoriteCurrencyRepository();
						string res;

						foreach (var currencyViewModel in favoriteCurrencyMethods.PrepareCurrencies())
						{
							try
							{
								favoriteCurrencyRepository.AddFavoriteCurrency(new FavoriteCurrencyModel()
								{
									CurrencyCode = currencyViewModel.CurrencyCode.ToString(),
									IdUser = AppSecurity.CurrentUser.UserId
								}, out res);
							}
							catch (Exception e)
							{
								_logger.Error(e);
							}
						}
					}

					/************ Getting system setting for user in db *******************/
					var props = userRepo.SysValues();
					SettingClass.SetSysEmailUploadProof(props.FirstOrDefault(f => f.PropertyName == "adminNotificationMailForUploadProof")?.PropertyValue.ToString());
					/**********************************************************************/
					Connection();
					SqlCommand com = _con.CreateCommand();
					try
					{
						_con.Open();
						com.Connection = _con;
						Guid guidtokenKey;
						if (Guid.TryParse(token, out guidtokenKey))
						{
							com = new SqlCommand("CheckOrderIfExist", _con);
							com.CommandType = CommandType.StoredProcedure;
							com.Transaction = null;
							com.Parameters.AddWithValue("@orderToken", guidtokenKey);
							var resMerchantId = (int)com.ExecuteScalar();

							if (resMerchantId != 0)
							{
								com.Parameters.Clear();
								Guid customerIdInTsg;
								if (Guid.TryParse(AppSecurity.CurrentUser.UserId, out customerIdInTsg))
								{
									SqlTransaction transaction = _con.BeginTransaction();
									try
									{
										string login = AppSecurity.CurrentUser.UserName.ToLower();
										com = new SqlCommand("UpdateTsgIdViaTsgService", _con);
										com.CommandType = CommandType.StoredProcedure;
										com.Transaction = transaction;
										com.Parameters.AddWithValue("@userLogin", login);
										com.Parameters.AddWithValue("@userTokenByTSG", customerIdInTsg);
										com.ExecuteNonQuery();
										com.Parameters.Clear();
										com.CommandText =
											String.Format(
												"SELECT [CustomerOrderStatus] FROM [dbo].[TokenKeysForOrders] WHERE [CustomerOrderId]='{0}'",
												guidtokenKey);
										com.CommandType = CommandType.Text;
										com.Transaction = transaction;
										var stateOfOrder = (int)com.ExecuteScalar();
										com.Parameters.Clear();
										if (stateOfOrder == 5)
											throw new Exception("This order has already been paid.");
										var createStoke = Crypto.Encrypt(model.Password, guidtokenKey.ToString());
										com.CommandText =
											String.Format(
												"UPDATE [dbo].[TokenKeysForOrders] SET [CustomerId] = '{0}'," +
												" [CustomerOrderStatus]=2, [Stoke]='{1}' WHERE [CustomerOrderId]='{2}'",
												customerIdInTsg, createStoke, guidtokenKey);
										com.CommandType = CommandType.Text;
										com.Transaction = transaction;
										com.ExecuteNonQuery();
										com.Parameters.Clear();
										com = new SqlCommand("AddOrderHistoryRecs", _con);
										com.CommandType = CommandType.StoredProcedure;
										com.Transaction = transaction;
										com.Parameters.AddWithValue("@OrderToken", guidtokenKey);
										com.Parameters.AddWithValue("@StatusState", 2);
										com.Parameters.AddWithValue("@Comment",
											"Successifully setted unconfirm status");
										com.Parameters.AddWithValue("@MerchantId", resMerchantId);
										com.ExecuteNonQuery();

										transaction.Commit();
										_logger.Info("Commited changes");
										return RedirectToAction("CheckoutIndex", "Home", new { orderToken = guidtokenKey });
									}
									catch (Exception e)
									{
										ViewBag.ErrorMessage = String.Format(GlobalRes.User_UserController_CheckoutLogin_ApplicationError, e.Message);
										_logger.Error("User " + model.Username + " attempted to log in but failed");
										transaction.Rollback();
									}
								}
							}
							else
							{
								ViewBag.ErrorMessage = GlobalRes.User_UserController_CheckoutLogin_OrderError;
								_logger.Error("User " + model.Username + " attempted to log in but failed [Order token don't match]");
							}
						}
					}
					catch (Exception ex)
					{
						_logger.Error(ex.Message);
						ViewBag.ErrorMessage = GlobalRes.User_UserController_CheckoutLogin_ApplicationErrorEmptyParam;
						_logger.Error("User " + model.Username + " attempted to log in but failed [Order token don't match]\n\r" + ex.Message);
					}
					_con.Close();
				}
				else
				{
					ViewBag.ErrorMessage = GlobalRes.User_UserController_Login_PasswordNotMatch;
					_logger.Error("User " + model.Username + " attempted to log in but failed");
				}
			}
			return View();
		}

		public ActionResult LogOut(string returnUrl)
		{
			string username = String.Copy(AppSecurity.CurrentUser.UserName);
			AppSecurity.CurrentUser = null;
			Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-30));
			Response.Cache.SetValidUntilExpires(false);
			Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
			Response.Cache.SetCacheability(HttpCacheability.NoCache);
			Response.Cache.SetNoStore();
			Session.Abandon();
			FormsAuthentication.SignOut();
			_logger.Info("User " + username + " successfully logged out");

			// Clear header logo URL cookie
			ClearHeaderLogoUrlCookie();

			return RedirectToActionPermanent("Login", new { returnUrl });
		}

		[HttpPost]
		public ActionResult HideMessage(bool display)
		{
			UserRepository ur = new UserRepository();
			AppSecurity.CurrentUser.ShowWelcomeMessage = !ur.ChangeWmStatuses(false);

			return Content("DONE");
		}

		// GET: User/GetUsers
		[AuthorizeUser(Roles = Role.SuperUser)]
		[HttpGet]
		public ActionResult GetUsers()
		{
			UserRepository userRepo = new UserRepository();
			ModelState.Clear();
			return View(userRepo.GetUsers().Where(w => w.RoleId == 1).ToList());
		}

		// GET: User/GetUsers
		[AuthorizeUser(Roles = Role.SuperUser)]
		[HttpGet]
		public ActionResult AddAdminUser()
		{
			AdminModel admin = new AdminModel();
			ModelState.Clear();
			return View(admin);
		}

		[AuthorizeUser(Roles = Role.SuperUser)]
		[HttpPost]
		public ActionResult AddAdminUser(AdminModel model)
		{
			UserRepository ur = new UserRepository();
			model.LinkText = Url.Action("RegisterNewAdmin", "User", new { Guid = Guid.NewGuid() }, this.Request.Url.Scheme);

			if (ur.AddAdmin(model))
			{
				var mail = new ContactUsRepository();
				string text = String.Format(@"Dear {0},
<br/>
You were added as an Admin on system.
<br/>
Please follow the link below to set up your password and access the system.
<br/>
{1}
<br/>
<br/>
Thank you,
<br/>
WinstantPay", model.FirstName, model.LinkText);
				string mailSendRes;
				if (
					mail.RegisterMail(0, "noreply@winstantpay.com", model.MailAddress,
						"Please confirm your Admin account", text, new byte[] { },
						"AdminMessage", out mailSendRes, out var mailId))
				{
					Extensions.EmailExtension.EmailService.SendEmail("Please confirm your Admin account", text, model.MailAddress, "noreply@winstantpay.com");
					ViewBag.Message = GlobalRes.User_UserController_AddAdminUser_AddSuccessifully;
				}

			}
			else
			{
				ViewBag.Message = GlobalRes.User_UserController_AddAdminUser_AddUnSuccessifully;
			}

			return View(model);
		}

		// GET: User/GetUsers
		[AuthorizeUser(Roles = Role.SuperUser)]
		[HttpGet]
		public ActionResult EditAdminUser(string username)
		{
			AdminModel admin = new AdminModel();
			UserRepository userRepo = new UserRepository();
			var currUser = userRepo.GetUsers().FirstOrDefault(w => w.RoleId == 1 && w.Username == username);
			if (currUser == null)
				return RedirectToAction("GetUsers");
			admin.Username = currUser.Username;
			admin.MailAddress = currUser.UserMail;
			admin.FirstName = currUser.FirstName;
			admin.LastName = currUser.LastName;
			return View("EditAdminUser", admin);
		}

		[AuthorizeUser(Roles = Role.SuperUser)]
		[HttpPost]
		public ActionResult EditAdminUser(AdminModel model)
		{

			UserRepository userRepo = new UserRepository();
			var currUser = userRepo.GetUsers().FirstOrDefault(w => w.RoleId == 1 && w.Username == model.Username);
			if (currUser == null)
				return RedirectToAction("GetUsers");

			model.LinkText = Url.Action("RegisterNewAdmin", "User", new { Guid = Guid.NewGuid() }, this.Request.Url.Scheme);

			if (userRepo.AddAdmin(model))
			{

				var curr = _usersServiceMethods.GetById(model.Username).Obj;
				if (curr != null)
				{
					curr.User_UserMail = model.MailAddress;
					_usersServiceMethods.UpdateAdmin(model.FirstName, model.LastName, model.MailAddress, curr.User_Username);
				}

				if (model.IsNeedResetPassword)
				{
					// Reset password
					userRepo.ChangePassword(new UserLoginModel() { Username = model.Username }, String.Empty);
					// Create link to restore password
					_saSharedLinkForAdminServiceMethods.Insert(new SharedAdminLinkSo()
					{
						SharedAdminLink_Email = currUser.UserMail,
						SharedAdminLink_FirstName = currUser.FirstName,
						SharedAdminLink_LastName = currUser.LastName,
						SharedAdminLink_LinkAddress = model.LinkText,
						SharedAdminLink_UserName = currUser.Username
					});
					// Create mail
					var mail = new ContactUsRepository();
					string text = String.Format(@"Dear {0},
<br/>
Your password was reseted, enter by this link and create new.
<br/>
Please follow the link below to set up your password and access the system.
<br/>
{1}
<br/>
<br/>
Thank you,
<br/>
WinstantPay", model.FirstName, model.LinkText);
					string mailSendRes;
					if (
						mail.RegisterMail(0, "noreply@winstantpay.com", model.MailAddress,
							"Restore access your Admin account", text, new byte[] { },
							"AdminMessage", out mailSendRes, out var mailId))
					{
						Extensions.EmailExtension.EmailService.SendEmail("Restore access your Admin account", text, model.MailAddress, "noreply@winstantpay.com");
						ViewBag.Message = GlobalRes.User_UserController_AddAdminUser_AddSuccessifully;
					}

				}

				return RedirectToAction("GetUsers");
			}
			else
			{
				ViewBag.Message = GlobalRes.User_UserController_AddAdminUser_AddUnSuccessifully;
			}

			return View(model);
		}

		[AllowAnonymous]
		[HttpGet]
		public ActionResult RegFailureNewAdmin()
		{
			return View();
		}

		[AllowAnonymous]
		[HttpGet]
		public ActionResult RegisterNewAdmin()
		{
			AdminModel am = new AdminModel();
			am.LinkText = Request.Url.AbsoluteUri;
			UserRepository ur = new UserRepository();
			var admLink = ur.GetAdminLink();

			var currRegUserLink = admLink.FirstOrDefault(f => f.LinkText == am.LinkText);
			if (currRegUserLink != null && currRegUserLink.LinkStatus == 1)
			{
				return View(am);
			}
			else
			{
				return RedirectToAction("RegFailureNewAdmin");
			}

			return View(am);
		}
		string Sha1(string input)
		{
			using (SHA1Managed sha1 = new SHA1Managed())
			{
				var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
				var sb = new StringBuilder(hash.Length * 2);

				foreach (byte b in hash)
				{
					// can be "x2" if you want lowercase
					sb.Append(b.ToString("X2"));
				}

				return sb.ToString();
			}
		}
		[AllowAnonymous]
		[HttpPost]
		public ActionResult RegisterNewAdmin(AdminModel model)
		{
			UserRepository ur = new UserRepository();
			var admLink = ur.GetAdminLink();


			var currRegUserLink = admLink.FirstOrDefault(f => f.LinkText == model.LinkText);
			if (currRegUserLink != null && currRegUserLink.LinkStatus == 1)
			{
				string p1 = Sha1(model.UserPassword);
				string p2 = Sha1(model.UserRetPassword);

				if (p1 != p2)
				{
					ViewBag.Message = GlobalRes.User_UserController_RegisterNewAdmin_PasswordsNotMatch;
					return View(currRegUserLink);
				}
				else
				{
					currRegUserLink.UserPassword = p1;
					var curr = _usersServiceMethods.GetById(currRegUserLink.Username).Obj;
					if (curr != null)
					{
						UserRepository userRepo = new UserRepository();
						// Reset password
						userRepo.ChangePassword(new UserLoginModel() { Username = curr.User_Username }, currRegUserLink.UserPassword);

						_saSharedLinkForAdminServiceMethods.Update(new SharedAdminLinkSo()
						{
							SharedAdminLink_ID = currRegUserLink.Id,
							SharedAdminLink_UserName = currRegUserLink.Username,
							SharedAdminLink_FirstName = currRegUserLink.FirstName,
							SharedAdminLink_LastName = currRegUserLink.LastName,
							SharedAdminLink_LinkAddress = model.LinkText,
							SharedAdminLink_Email = currRegUserLink.MailAddress,
							SharedAdminLink_ActivationDate = DateTime.Now,
							SharedAdminLink_CreationDate = currRegUserLink.CreationDate,
							SharedAdminLink_StatusLink = 2,
						});

						_saSharedLinkForAdminServiceMethods.ClearAllOldReferences(model.Username);
						return RedirectToAction("Login", "User");
					}
					else if (ur.AddAdmin(currRegUserLink, currRegUserLink.UserPassword))
						return RedirectToAction("Login", "User");
					else
					{
						ViewBag.Message = GlobalRes.User_UserController_RegisterNewAdmin_UserUnregistredInSystem;
						return View(currRegUserLink);
					}
				}
			}
			else
			{
				ViewBag.Message = "User not created or registred in system";
				return View();
			}
		}

		// GET: User/ChangePassword
		[HttpGet]
		public ActionResult ChangePassword()
		{
			ViewBag.IsError = false;
			return View();
		}

		// POST: User/ChangePassword
		[HttpPost]
		public ActionResult ChangePassword(UserLoginModel obj)
		{
			try
			{
				if (ModelState.IsValid)
				{
					if (obj.OldPassword != AppSecurity.CurrentUser.Password)
						throw new Exception(String.Format(GlobalRes.User_UserController_ChangePassword_PassNotMatched, Url.Action("LogOut", "User")));

					if (obj.Password.Equals(obj.RepeatPassword))
					{
						if (obj.Password.Length < 8)
							throw new Exception(GlobalRes.User_UserController_ChangePassword_NeedEightCharacterLong);
						if (!Regex.Match(obj.Password, @"\d+", RegexOptions.ECMAScript).Success)
							throw new Exception(GlobalRes.User_UserController_ChangePassword_NeedCharacterAndCapitalLetterAndNumber);
						if (!Regex.Match(obj.Password, @"[A-Z]", RegexOptions.ECMAScript).Success)
							throw new Exception(GlobalRes.User_UserController_ChangePassword_NeedCapitalLetterAndSpecCharacter);
						if (!Regex.Match(obj.Password, @"[!,@,#,$,%,^,&,*,?,_,~,-,(,)]", RegexOptions.ECMAScript).Success)
							throw new Exception(GlobalRes.User_UserController_ChangePassword_NeedSpecCharacter);

						string hashedPassword = UserLoginModel.GetSha1(obj.Password);
						UserRepository userRepo = new UserRepository();
						var user = userRepo.GetUsers().FirstOrDefault(f => f.Username.ToLower() == AppSecurity.CurrentUser.UserName.ToLower() /*&& f.UserPassword == UserLoginModel.GetSha1(AppSecurity.CurrentUser.Password)*/);
						ViewBag.IsError = false;
						ViewBag.Message = String.Empty;
						if (user != null && user.IsLocal)
						{
							if (userRepo.ChangePassword(obj, hashedPassword))
							{
								ViewBag.Message = GlobalRes.User_UserController_ChangePassword_Success;
								_logger.Info("User " + AppSecurity.CurrentUser.UserName + " changed his password");
								AppSecurity.CurrentUser.Password = obj.Password;
								ViewBag.IsError = false;
							}

						}
						else if (user != null)
						{
							Service = new IgpService(AppSecurity.CurrentUser.AccessToken, AppSecurity.CurrentUser.UserId);
							var response = Service.ChangePassword(obj.OldPassword, obj.Password);
							if (!response.ServiceResponse.HasErrors)
							{
								if (userRepo.ChangePassword(obj, hashedPassword))
								{
									ViewBag.Message = GlobalRes.User_UserController_ChangePassword_Success;
									_logger.Info("User " + AppSecurity.CurrentUser.UserName + " changed his password");
									AppSecurity.CurrentUser.Password = obj.Password;
									ViewBag.IsError = false;
								}
							}
							else
							{
								throw new Exception(response.ServiceResponse.Responses[0].Message, new Exception(response.ServiceResponse.Responses[0].MessageDetails));
							}
						}
					}
					else
					{
						ViewBag.Message = GlobalRes.User_UserController_ChangePassword_UnSuccess;
						_logger.Error("User " + AppSecurity.CurrentUser.UserName + " could not change password due to pasword mismatch");
						ViewBag.IsError = true;
					}
				}

				return View();
			}
			catch (Exception ex)
			{
				ViewBag.Message = ex.Message;
				ViewBag.IsError = true;
				_logger.Error("User " + AppSecurity.CurrentUser.UserName + " could not change password due to pasword mismatch. " + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException?.Message);
				return View();
			}
		}

		[HttpGet]
		public ActionResult ResetPassword()
		{
			Service = new IgpService(AppSecurity.CurrentUser.UserName, AppSecurity.CurrentUser.Password, AppSecurity.CurrentUser.UserId);
			var allBankUsers = Service.GetBankUsersSearch(100000000);
			var allCustomerUsers = Service.GetCustomerUsersSearch(100000000);
			UserResetModel um = new UserResetModel();
			um.UserList = new List<UserResetModel>();
			var tempUserList = new List<UserResetModel>();

			try
			{
				foreach (var bankUser in allBankUsers.Users)
				{
					var user = new UserResetModel();
					user.FirstName = bankUser.FirstName;
					user.LastName = bankUser.LastName;
					user.Username = bankUser.UserName;
					user.LastLogOnDate = Convert.ToDateTime(bankUser.LastLogin);
					user.UserId = bankUser.UserId;
					user.BranchOrCustomerName = bankUser.BranchName;
					user.Email = bankUser.Email;
					tempUserList.Add(user);
				}

				foreach (var customer in allCustomerUsers.Users)
				{
					var user = new UserResetModel();
					user.FirstName = customer.FirstName;
					user.LastName = customer.LastName;
					user.Username = customer.UserName;
					user.LastLogOnDate = Convert.ToDateTime(customer.LastLogin);
					user.UserId = customer.UserId;
					user.BranchOrCustomerName = customer.CustomerName;
					user.Email = customer.Email;
					tempUserList.Add(user);
				}
				um.UserList = tempUserList;

				if (TempData.ContainsKey("UserResetModel"))
				{
					var tempModel = TempData["UserResetModel"] as UserResetModel;
					if (tempModel != null)
					{
						um.IsSuccessResForAction = tempModel.IsSuccessResForAction;
						um.ActionResultText = tempModel.ActionResultText;
						if (um.IsSuccessResForAction != null && (bool)um.IsSuccessResForAction)
						{
							var user = tempUserList.FirstOrDefault(f => f.UserId == tempModel.UserId);
							if (user != null)
								um.ActionResultText = String.Format(@GlobalRes.User_ResetPassword_SuccessChanged, user.Username);

						}
					}
				}
			}
			catch (Exception e)
			{
				_logger.Error(e);
			}

			return View(um);
		}

		[HttpPost]
		public ActionResult ResetPasswordForUser(string userId)
		{
			UserResetModel rm = new UserResetModel();
			Service = new IgpService(AppSecurity.CurrentUser.UserName, AppSecurity.CurrentUser.Password, AppSecurity.CurrentUser.UserId);
			var result = Service.ResetPassword(userId);
			if (result.ServiceResponse.HasErrors)
			{
				rm.IsSuccessResForAction = false;
				rm.ActionResultText = result.ServiceResponse.Responses[0].Message;
				rm.UserId = userId;
			}
			else
			{
				rm.IsSuccessResForAction = true;
				rm.ActionResultText = result.ServiceResponse.Responses[0].Message;
				rm.UserId = userId;
			}
			TempData["UserResetModel"] = rm;
			return RedirectToAction("ResetPassword");
		}

		[NonAction]
		private IList<DaPayLimitsTypeViewModel> PrepareDaLimitsTypes()
		{
			var models = new List<DaPayLimitsTypeViewModel>();
			var types = _daPayLimitsTypeServiceMethods.GetAll().Obj.Where(w => !w.DaPayLimitsType_IsDeleted).ToList();


			foreach (var item in types)
			{
				models.Add
				(
					new DaPayLimitsTypeViewModel
					{
						ID = item.DaPayLimitsType_ID,
						NameOfPaymentLimit = item.DaPayLimitsType_NameOfPaymentLimit,
						SysNameOfPaymentLimit = item.DaPayLimitsType_SysNameOfPaymentLimit,
						LimitType = item.DaPayLimitsType_LimitType,
						IsDeleted = item.DaPayLimitsType_IsDeleted
					}
				);
			}

			return models;
		}

		[NonAction]
		private IList<DaPayLimitsTabViewModel> PrepareWPayIdDaLimitsTabs(string wPayId)
		{
			var models = new List<DaPayLimitsTabViewModel>();
			// var tabs = _daPayLimitsTabServiceMethods.GetAll().Obj?.Where(w => !w.DaPayLimitsTab_IsDeleted).ToList();
			var tabs = _daPayLimitsTabServiceMethods.GetAll().Obj?.Where(w => !w.DaPayLimitsTab_IsDeleted && w.DaPayLimitsTab_WPayId == wPayId).ToList();

			if (tabs != null && tabs.Count > 0)
			{
				foreach (var item in tabs)
				{
					models.Add
					(
						new DaPayLimitsTabViewModel
						{
							ID = item.DaPayLimitsTab_ID,
							TypeOfLimit = item.DaPayLimitsTab_TypeOfLimit,
							Amount = item.DaPayLimitsTab_Amount,
							//Amount = cryptoCurrency.Contains(selectedBalanceByCcy.CCY)
							//? Decimal.Round(model.Amount, 8, MidpointRounding.AwayFromZero)
							//: Decimal.Round(model.Amount, 2, MidpointRounding.AwayFromZero);
						}
					);
				}
			}
			return models;
		}

		[NonAction]
		private IList<DaPayLimitsTabViewModel> PrepareWPayIdDaLimitsTabs(string wPayId, bool isCrypto)
		{
			var models = new List<DaPayLimitsTabViewModel>();
			// var tabs = _daPayLimitsTabServiceMethods.GetAll().Obj?.Where(w => !w.DaPayLimitsTab_IsDeleted).ToList();
			var tabs = _daPayLimitsTabServiceMethods.GetAll().Obj?.Where(w => !w.DaPayLimitsTab_IsDeleted && w.DaPayLimitsTab_WPayId == wPayId).ToList();

			if (tabs != null && tabs.Count > 0)
			{
				foreach (var item in tabs)
				{
					models.Add
					(
						new DaPayLimitsTabViewModel
						{
							ID = item.DaPayLimitsTab_ID,
							TypeOfLimit = item.DaPayLimitsTab_TypeOfLimit,
							//Amount = item.DaPayLimitsTab_Amount,
							Amount = isCrypto ? Decimal.Round(item.DaPayLimitsTab_Amount, 8, MidpointRounding.AwayFromZero) : Decimal.Round(item.DaPayLimitsTab_Amount, 2, MidpointRounding.AwayFromZero)
						}
					);
				}
			}
			return models;
		}

		[NonAction]
		private IList<SelectListItem> PrepareWPayIds()
		{
			var aliasListItem = new List<SelectListItem>();
			Service = new IgpService(AppSecurity.CurrentUser.UserName, AppSecurity.CurrentUser.Password, AppSecurity.CurrentUser.UserId);

			var aliases = Service.GetAccountAliases();

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

		[NonAction]
		private UserProfileViewModel PrepareDaUserWPayIdSetting(UserProfileViewModel model)
		{
			if (!String.IsNullOrEmpty(model.WPayId))
			{
				//model.Message += "<br />PrepareDaUserWPayIdSetting, daWPayIdSetting is not null";
				var daWPayIdSetting = _daUserWPayIDSettingServiceMethods.GetByWPayId(model.WPayId);
				if (daWPayIdSetting.Success && daWPayIdSetting.Obj != null)
				{
					//model.Message += "<br />PrepareDaUserWPayIdSetting, daWPayIdSetting is not null";
					model.DaUserWPayIDSettingID = daWPayIdSetting.Obj.ID;
					model.CcyCode = daWPayIdSetting.Obj.CcyCode;
					model.PinCode = daWPayIdSetting.Obj.PinCode;
					model.IsPinRequired = daWPayIdSetting.Obj.IsPinRequired;
					// model.ExceedingAmount = daWPayIdSetting.Obj.ExceedingAmount;
					model.ExceedingAmount = model.IsCrypto ? Decimal.Round(daWPayIdSetting.Obj.ExceedingAmount, 8, MidpointRounding.AwayFromZero) : Decimal.Round(daWPayIdSetting.Obj.ExceedingAmount, 2, MidpointRounding.AwayFromZero);
				}
				else
				{
					model.IsPinRequired = false;
				}
			}
			else
			{
				//model.Message += "<br />PrepareDaUserWPayIdSetting, daWPayIdSetting null";
				model.IsPinRequired = true;
			}

			return model;
		}

		[NonAction]
		private void SaveDaUserWPayIdSetting(UserProfileViewModel model, out Guid newUserWPayIDSettingID)
		{
			newUserWPayIDSettingID = model.DaUserWPayIDSettingID;
			var daUserSettingSo = _daUserWPayIDSettingServiceMethods.GetByWPayId(model.WPayId).Obj;

			// if (daUserSettingSo != null model.DaUserWPayIDSettingID == default)
			if (daUserSettingSo == null)
			{
				newUserWPayIDSettingID = Guid.NewGuid();
				model.DaUserWPayIDSettingID = newUserWPayIDSettingID;
				//model.DaUserWPayIDSettingID = Guid.NewGuid();
				//model.Message += "<br />model.DaUserWPayIDSettingID == default";

				var daSettingSo = model.PrepareDaUserWPayIDSettingSo();
				// daSettingSo.ID = Guid.NewGuid();

				daSettingSo.IsDeleted = false;
				var resInsert = _daUserWPayIDSettingServiceMethods.Insert(daSettingSo);
				//model.Message += "<br /> resInsert.Message: " + resInsert.Message;

			}
			else
			{
				//model.Message += "<br />model.DaUserWPayIDSettingID != default";
				// var daUserSettingSo = _daUserWPayIDSettingServiceMethods.GetById(model.DaUserWPayIDSettingID).Obj;
				if (daUserSettingSo != null)
				{
					daUserSettingSo.CcyCode = model.CcyCode;
					daUserSettingSo.IsPinRequired = model.IsPinRequired;
					daUserSettingSo.PinCode = model.PinCode;
					daUserSettingSo.ExceedingAmount = model.ExceedingAmount;
					daUserSettingSo.LastModifiedDate = DateTime.UtcNow;

					var recUpdate = _daUserWPayIDSettingServiceMethods.Update(daUserSettingSo);
					//model.Message += "<br />Updating DaUserWPayIDSetting, recUpdate.Message: " + recUpdate.Message;
				}
			}
		}

		[NonAction]
		private void SaveDaUserWPayIdSetting(UserProfileViewModel model)
		{
			var daUserSettingSo = _daUserWPayIDSettingServiceMethods.GetByWPayId(model.WPayId).Obj;

			// if (daUserSettingSo != null model.DaUserWPayIDSettingID == default)
			if (daUserSettingSo == null)
			{
				// newUserWPayIDSettingID = Guid.NewGuid();
				// model.DaUserWPayIDSettingID = newUserWPayIDSettingID;
				model.DaUserWPayIDSettingID = Guid.NewGuid();
				//model.Message += "<br />model.DaUserWPayIDSettingID == default";

				var daSettingSo = model.PrepareDaUserWPayIDSettingSo();
				// daSettingSo.ID = Guid.NewGuid();

				daSettingSo.IsDeleted = false;
				var resInsert = _daUserWPayIDSettingServiceMethods.Insert(daSettingSo);
				//model.Message += "<br /> resInsert.Message: " + resInsert.Message;

			}
			else
			{
				//model.Message += "<br />model.DaUserWPayIDSettingID != default";
				// var daUserSettingSo = _daUserWPayIDSettingServiceMethods.GetById(model.DaUserWPayIDSettingID).Obj;
				if (daUserSettingSo != null)
				{
					daUserSettingSo.CcyCode = model.CcyCode;
					daUserSettingSo.IsPinRequired = model.IsPinRequired;
					daUserSettingSo.PinCode = model.PinCode;
					daUserSettingSo.ExceedingAmount = model.ExceedingAmount;
					daUserSettingSo.LastModifiedDate = DateTime.UtcNow;

					var recUpdate = _daUserWPayIDSettingServiceMethods.Update(daUserSettingSo);
					//model.Message += "<br />Updating DaUserWPayIDSetting, recUpdate.Message: " + recUpdate.Message;
				}
			}
		}

		private void ClearHeaderLogoUrlCookie()
		{
			if (Request.Cookies["headerLogoUrl"] != null)
			{
				var c = new HttpCookie("headerLogoUrl")
				{
					Expires = DateTime.Now.AddDays(-1)
				};
				Response.Cookies.Add(c);
			}
		}


		/// <summary>
		/// Forgot password
		/// </summary>
		/// <param name="model">user forgot password model</param>
		/// <returns></returns>
		[AllowAnonymous]
		[HttpPost]
		public ActionResult ForgotPassword(UserForgotPasswordModel model)
		{
			_logger.Debug("In forgot password");
			try
			{
				_logger.Debug("Starting forgot password");
				_logger.Debug("model: " + JsonConvert.SerializeObject(model));
				if (!string.IsNullOrEmpty(model.forgotPasswordUserName) && !string.IsNullOrEmpty(model.forgotPasswordEmail))
				{
					_logger.Debug("Resetting password");
					model.ResetPassword();
				}
				else
				{
					model.IsSuccess = false;
					model.ResultText = "Reset Password failed. Null user name or email";
				}
			}
			catch (Exception e)
			{
				_logger.Error(e);
				model.IsSuccess = false;
				model.ResultText = "Reset Password Failed";
			}

			return Json(new { model.IsSuccess, model.ResultText });
		}

		//[NonAction]
		//public DaUserWPayIDSettingSo PrepareDaPayTabSo(UserProfileViewModel model)
		//{
		//    return new DaUserWPayIDSettingSo
		//    {
		//        ID = model.DaUserWPayIDSettingID,
		//        UserID = model.UserID,
		//        WPayId = model.WPayId,
		//        IsPinRequired = model.IsPinRequired,
		//        PinCode = model.PinCode,
		//        ExceedingAmount = model.ExceedingAmount
		//    };
		//}
	}
}
