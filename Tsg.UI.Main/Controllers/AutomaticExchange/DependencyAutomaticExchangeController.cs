using Autofac.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TSGgpwsbeta;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models.Security;
using TSG.Models.APIModels;
using TSG.ServiceLayer.AutomaticExchange.DependencyLiquidForUserServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidCcyListServiceMethods;

namespace Tsg.UI.Main.Controllers.AutomaticExchange
{
	public class DependencyAutomaticExchangeController : Controller
	{
		readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private readonly ILiquidCcyListServiceMethods _liquidCcyListServiceMethods;
		private readonly IDependencyLiquidForUserServiceMethods _dependencyLiquidForUserService;
		protected IgpService Service;

		public DependencyAutomaticExchangeController(IDependencyLiquidForUserServiceMethods dependencyLiquidForUserService, ILiquidCcyListServiceMethods liquidCcyListServiceMethods)
		{
			_dependencyLiquidForUserService = dependencyLiquidForUserService;
			this._liquidCcyListServiceMethods = liquidCcyListServiceMethods;
		}

		//[Route("/liquids/liquidation_preferences/")]
		//[Route("{lang}/liquids/liquidation_preferences/")]
		public ActionResult Index()
		{
			var dCcy = _dependencyLiquidForUserService.GetAllSoByUser(Guid.Parse(AppSecurity.CurrentUser.UserId)).Obj.ToList();
			_logger.Debug(string.Format("dCcy: {0}", JsonConvert.SerializeObject(dCcy)));
			var allowedCurrencies = AppSettingHelper.GetAllowedCurrencies();
			if (allowedCurrencies.Count > 0 && !string.IsNullOrEmpty(allowedCurrencies[0]))
			{
				dCcy = dCcy.Where(c => allowedCurrencies.Any(ac => ac == c.DependencyLiquidForUser_LiquidCcyList.LiquidCcyList_CurrencyCode)).ToList(); ;
				_logger.Debug(string.Format("after filter allowed currencies, dCcy: {0}", JsonConvert.SerializeObject(dCcy)));
			}
			return View(dCcy.OrderBy(ob => ob.DependencyLiquidForUser_LiquidOrder));
		}

		[HttpPost]
		public ActionResult GetAllAvaliableAutomaticExchangeLiquidCcy()
		{
			var allliquidCurrency = _liquidCcyListServiceMethods.GetAll().Obj
				.Where(w => w.LiquidCcyList_IsLiquidCurrency).Select(s => new { liquidId = s.LiquidCcyList_Id, ccyName = s.LiquidCcyList_CurrencyCode }).ToList();
			_logger.Debug(string.Format("allliquidCurrency: {0}", JsonConvert.SerializeObject(allliquidCurrency)));
			var allowedCurrencies = AppSettingHelper.GetAllowedCurrencies();
			_logger.Debug(string.Format("Allowed currencies, count: {0} items: {1}", allowedCurrencies.Count, JsonConvert.SerializeObject(allowedCurrencies)));
			var liquidsForUser = _dependencyLiquidForUserService.GetAllSoByUser(Guid.Parse(AppSecurity.CurrentUser.UserId)).Obj
				.Select(s => new { liquidId = s.DependencyLiquidForUser_LiquidCcyId, ccyName = s.DependencyLiquidForUser_LiquidCcyList.LiquidCcyList_CurrencyCode }).ToList();
			_logger.Debug(string.Format("liquidsForUser: {0}", JsonConvert.SerializeObject(liquidsForUser)));
			liquidsForUser.ForEach(f => allliquidCurrency.RemoveAll(a => a.liquidId == f.liquidId));
			_logger.Debug(string.Format("After removing, liquidsForUser: {0}", JsonConvert.SerializeObject(liquidsForUser)));
			if (allowedCurrencies.Count > 0 && !string.IsNullOrEmpty(allowedCurrencies[0]))
			{
				allliquidCurrency = allliquidCurrency.Where(c => allowedCurrencies.Any(ac => ac == c.ccyName)).ToList();
				_logger.Debug(string.Format("after filter allowed currencies, allliquidCurrency: {0}", JsonConvert.SerializeObject(allliquidCurrency)));
				liquidsForUser = liquidsForUser.Where(c => allowedCurrencies.Any(ac => ac == c.ccyName)).ToList();
				_logger.Debug(string.Format("after filter allowed currencies, liquidsForUser: {0}", JsonConvert.SerializeObject(liquidsForUser)));
			}
			_logger.Debug(string.Format("allliquidCurrency: {0}", JsonConvert.SerializeObject(allliquidCurrency)));
			return Json(allliquidCurrency);
		}


		[HttpPost]
		public ActionResult ReorganizationLiquidList(string ids)
		{
			var res = new StandartResponse();
			try
			{
				_dependencyLiquidForUserService.DeleteAllByUserId(Guid.Parse(AppSecurity.CurrentUser.UserId));

				if (!String.IsNullOrEmpty(ids))
				{
					var resIns = _dependencyLiquidForUserService.BulkInsertLiquidCurrencyForUser(ids,
						Guid.Parse(AppSecurity.CurrentUser.UserId));

					res = new StandartResponse(resIns.Success, resIns.Message);
				}
			}
			catch (Exception e)
			{
				res = new StandartResponse("Error", e.Message);

			}
			return Json(res);
		}

		/// <summary>
		/// Get customer liquidation preferences
		/// </summary>
		/// 
		/// <returns>List of liquidation preferences</returns>
		[HttpPost]
		public ActionResult GetCustomerLiquidationPreferences()
		{
			try
			{
				Service = new IgpService(AppSecurity.CurrentUser.AccessToken, AppSecurity.CurrentUser.UserId);
				var liquidationPreferencesResponse = Service.GetCustomerLiquidationPreferences();
				_logger.ErrorFormat("GetCustomerLiquidationPreferences , web service response: {0}", JsonConvert.SerializeObject(liquidationPreferencesResponse));
				if (!liquidationPreferencesResponse.ServiceResponse.HasErrors)
				{
					return Json(liquidationPreferencesResponse.Preferences);
				}
				else
				{
					_logger.ErrorFormat("GetCustomerLiquidationPreferences error, web service response: {0}", JsonConvert.SerializeObject(liquidationPreferencesResponse.ServiceResponse));
					return Json(null);
				}
			}
			catch (Exception e)
			{
				_logger.ErrorFormat("GetCustomerLiquidationPreferences error, exception: {0}", JsonConvert.SerializeObject(e));
				ViewBag.ErrorMessage = e.Message;
				return Json(null);
			}
		}

		/// <summary>
		/// Add or update customer liquidation preference
		/// </summary>
		/// <param name="currencies">One or more currencies codes separated by comma</param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult AddOrUpdateCustomerLiquidationPreference(string currencies)
		{
			try
			{
				var currenciesList = currencies.Split(',');
				Service = new IgpService(AppSecurity.CurrentUser.AccessToken, AppSecurity.CurrentUser.UserId);
				for (int i = 0; i < currenciesList.Length; i++)
				{
					var AddOrUpdateresponse = Service.AddOrUpdateCustomerLiquidationPreferenceCurrency(currenciesList[i], i);
					_logger.ErrorFormat("AddOrUpdateCustomerLiquidationPreference , AddOrUpdateresponse: {0}", JsonConvert.SerializeObject(AddOrUpdateresponse));
				}
				return Json(null);
			}
			catch (Exception e)
			{
				_logger.ErrorFormat("GetCustomerLiquidationPreferences error, exception: {0}", JsonConvert.SerializeObject(e));
				ViewBag.ErrorMessage = e.Message;
				return Json(null);
			}
		}

		/// <summary>
		/// Update customer liquidation preference
		/// </summary>
		/// <param name="currencyList">List of currencies separated by comma</param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult UpdateCustomerLiquidationPreferences(string currencyList)
		{
			try
			{
				_logger.InfoFormat("UpdateCustomerLiquidationPreferences , currencyList: {0}", currencyList);
				_logger.InfoFormat("UpdateCustomerLiquidationPreferences , UserId: {0}", AppSecurity.CurrentUser.UserId);
				_logger.InfoFormat("UpdateCustomerLiquidationPreferences , OrganisationId: {0}", AppSecurity.CurrentUser.OrganisationId);
				Service = new IgpService(AppSecurity.CurrentUser.AccessToken, AppSecurity.CurrentUser.OrganisationId);

				var UpdateResponse = Service.UpdateCustomerLiquidationPreferences(currencyList);
				_logger.ErrorFormat("UpdateCustomerLiquidationPreferences , UpdateResponse: {0}", JsonConvert.SerializeObject(UpdateResponse));

				return Json(currencyList);
			}
			catch (Exception e)
			{
				_logger.ErrorFormat("UpdateCustomerLiquidationPreferences error, exception: {0}", JsonConvert.SerializeObject(e));
				ViewBag.ErrorMessage = e.Message;
				return Json("Error");
			}
		}

	}
}