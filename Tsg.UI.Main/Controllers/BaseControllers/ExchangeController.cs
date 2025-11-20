using StaticExtensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;

namespace Tsg.UI.Main.Controllers
{
	[SessionState(SessionStateBehavior.Required)]
	public class ExchangeController : BaseController
	{
		readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private IgpService Service { get; set; }
		private UserInfo CurrentUser { get; set; }

		public ExchangeController()
		{
			if (AppSecurity.CurrentUser != null)
			{
				Service = new IgpService(AppSecurity.CurrentUser.AccessToken, AppSecurity.CurrentUser.UserId);
				CurrentUser = AppSecurity.CurrentUser;
			}
		}
		public ExchangeController(UserInfo ui)
		{
			Service = new IgpService(ui.AccessToken, ui.UserId);
			CurrentUser = new UserInfo() { UserName = ui.UserName, Password = ui.Password, UserId = ui.UserId };
		}

		public ActionResult CurrencyExchange(ExchangeModel exmod = null)
		{
			return View("CurrencyExchangeParent", exmod);
		}



		[System.Web.Mvc.HttpPost]
		public ActionResult JsonCheckQouteId(ExchangeModel exmod)
		{
			try
			{
				if (exmod.FxDealQuoteResult != null && exmod.FxDealQuoteResult.Quote != null &&
					!String.IsNullOrEmpty(exmod.FxDealQuoteResult.Quote.QuoteId))
				{
					exmod.FxDealQuoteBookResult =
						Service.FxDealQuoteBookAndInstantDeposit(exmod.FxDealQuoteResult.Quote.QuoteId);
					if (exmod.FxDealQuoteBookResult.ServiceResponse.HasErrors)
						throw new Exception(exmod.FxDealQuoteBookResult.ServiceResponse.Responses[0].Message);
					exmod.IsError = false;
					exmod.Message = string.Empty;
					return View("ExchangeResult", exmod);
				}
			}
			catch (Exception exception)
			{
				exmod.IsError = true;
				exmod.Message = exception.Message;
				_logger.Error(exception.Message);
				ViewBag.Message = exception.Message;
			}
			return View("ExchangeResult", exmod);
		}

		[System.Web.Http.HttpPost]
		public JsonResult JsonCreateCurrencyQuote(string from, string to, string paymentCurrncyCode, string amount)
		{
			ExchangeModel exmod = new ExchangeModel();

			if (AppSecurity.CurrentUser == null || !HttpContext.User.Identity.IsAuthenticated)
				return Json(new ExchangeModel() { IsError = true, Message = "User is not authenticated" });
			exmod.FromAccountId = from;
			exmod.ToAccountId = to;
			var listCustomerBalanceData = new List<CustomerBalanceData>();
			var list = new List<SelectListItem>();

			try
			{
				listCustomerBalanceData = Service.GetAccountBalances().Balances.ToList();
			}
			catch
			{
			}
			list.Add(new SelectListItem()
			{
				Text = "---",
				Value = null
			});

			if (!String.IsNullOrEmpty(exmod.SellCurrency))
			{
				list.Clear();
				list.Add(new SelectListItem()
				{
					Text = exmod.SellCurrency,
					Value = listCustomerBalanceData.FirstOrDefault(f => f.CCY == exmod.SellCurrency)?.AccountId,
					Selected = true
				});
				exmod.SelectedCurrency = list;
			}

			decimal decimalAmount = 0;

			try
			{
				if (CurrentUser.UserId == null)
				{
					_logger.Debug("GlobalRes.Exchange_ExchangeController_CurrencyExchange_EmptyUserData");
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_EmptyUserData);
				}
				if (!(exmod.FromAccountId != null && exmod.ToAccountId != null &&
					  exmod.FromAccountId != exmod.ToAccountId))
				{
					_logger.Debug("GlobalRes.Exchange_ExchangeController_CurrencyExchange_ChangeCurrency");
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_ChangeCurrency);
				}
				if (exmod.FromAccountId == exmod.ToAccountId)
				{
					_logger.Debug("GlobalRes.Exchange_ExchangeController_CurrencyExchange_ChangeCurrency");
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_ChangeCurrency);
				}

				if (decimal.TryParse(amount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimalAmount) && (decimalAmount < 0 || decimalAmount == 0))
				{
					_logger.Debug("GlobalRes.Exchange_ExchangeController_CurrencyExchange_EnterAmount");
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_EnterAmount);
				}

			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				return Json(new ExchangeModel() { IsError = true, Message = e.Message });
			}

			exmod.BuyAmount = decimalAmount;
			string fromCcy = String.Empty;
			string toCcy = String.Empty;

			if (listCustomerBalanceData.Any(a => a.AccountId == exmod.FromAccountId) &&
				listCustomerBalanceData.Any(a => a.AccountId == exmod.ToAccountId))
			{
				fromCcy = listCustomerBalanceData.FirstOrDefault(a => a.AccountId == exmod.FromAccountId)?.CCY ?? String.Empty;
				toCcy = listCustomerBalanceData.FirstOrDefault(a => a.AccountId == exmod.ToAccountId)?.CCY ?? String.Empty;
				list.Clear();
				list.Add(new SelectListItem() { Text = fromCcy, Value = fromCcy, Selected = paymentCurrncyCode == fromCcy });
				list.Add(new SelectListItem() { Text = toCcy, Value = toCcy, Selected = paymentCurrncyCode == toCcy });
			}
			exmod.SelectedCurrency = list;
			exmod.SellCurrency = paymentCurrncyCode;
			try
			{
				exmod.FxDealQuoteResult = Service.FxDealQuoteCreate(toCcy, fromCcy, decimalAmount, paymentCurrncyCode, false, "SPOT");
				if (exmod.FxDealQuoteResult?.ServiceResponse.HasErrors ?? true)
					throw new Exception(exmod.FxDealQuoteResult?.ServiceResponse.Responses[0].Message ?? "Inner error");

				exmod.TotalSeconds = Convert.ToInt32((Convert.ToDateTime(exmod.FxDealQuoteResult.Quote.ExpirationTime) - Convert.ToDateTime(exmod.FxDealQuoteResult.Quote.QuoteTime)).TotalSeconds);
				exmod.IsError = false;
				exmod.Message = string.Empty;
			}
			catch (Exception e)
			{
				exmod.IsError = true;
				exmod.Message = e.Message;
				_logger.Error(e);
			}

			return Json(exmod);
		}


		//[System.Web.Mvc.HttpPost]
		//public ActionResult CurrencyExchange(ExchangeModel exmod)
		//{
		//    var listCustomerBalanceData = new List<CustomerBalanceData>();
		//    try
		//    {
		//        var list = new List<SelectListItem>();
		//        exmod.SelectedCurrency = new List<SelectListItem>();

		//        try
		//        {
		//            listCustomerBalanceData = Service.GetAccountBalances().Balances.ToList();
		//        }
		//        catch
		//        {
		//        }
		//        list.Add(new SelectListItem()
		//        {
		//            Text = "---",
		//            Value = null
		//        });

		//        if (!String.IsNullOrEmpty(exmod.SellCurrency))
		//        {
		//            list.Clear();
		//            list.Add(new SelectListItem()
		//            {
		//                Text = exmod.SellCurrency,
		//                Value = listCustomerBalanceData.FirstOrDefault(f=>f.CCY == exmod.SellCurrency)?.AccountId,
		//                Selected = true
		//            });
		//            exmod.SelectedCurrency = list;
		//        }
		//        if (CurrentUser.UserId == null)
		//            throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_EmptyUserData);
		//        if (!(exmod.FromAccountId != null && exmod.ToAccountId != null &&
		//              exmod.FromAccountId != exmod.ToAccountId))
		//            throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_ChangeCurrency);
		//        if (exmod.FromAccountId == exmod.ToAccountId)
		//            throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_ChangeCurrency);

		//        decimal amount = exmod.BuyAmount;
		//        if (amount < 0 || amount == 0)
		//            throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_EnterAmount);

		//        string fromCcy = String.Empty;
		//        string toCcy = String.Empty;

		//        if (listCustomerBalanceData.Any(a => a.AccountId == exmod.FromAccountId) &&
		//            listCustomerBalanceData.Any(a => a.AccountId == exmod.ToAccountId))
		//        {
		//            fromCcy = listCustomerBalanceData.FirstOrDefault(a => a.AccountId == exmod.FromAccountId).CCY;
		//            toCcy = listCustomerBalanceData.FirstOrDefault(a => a.AccountId == exmod.ToAccountId).CCY;
		//            list.Clear();
		//            list.Add(new SelectListItem() { Text = fromCcy, Value = fromCcy });
		//            list.Add(new SelectListItem() { Text = toCcy, Value = toCcy });
		//        }
		//        exmod.SelectedCurrency = list;
		//        if (String.IsNullOrEmpty(fromCcy) || String.IsNullOrEmpty(toCcy) || String.IsNullOrEmpty(exmod.SellCurrency))
		//            throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_NoCorrectCurrency);


		//        if (exmod.FxDealQuoteResult != null && exmod.FxDealQuoteResult.Quote != null &&
		//            !String.IsNullOrEmpty(exmod.FxDealQuoteResult.Quote.QuoteId))
		//        {
		//            exmod.FxDealQuoteBookResult =
		//                Service.FxDealQuoteBookAndInstantDeposit(exmod.FxDealQuoteResult.Quote.QuoteId);
		//            if (exmod.FxDealQuoteBookResult.ServiceResponse.HasErrors)
		//                throw new Exception(exmod.FxDealQuoteBookResult.ServiceResponse.Responses[0].Message);
		//            return View(exmod);
		//        }
		//        else
		//        {
		//            exmod.FxDealQuoteBookResult = null;
		//            exmod.FxDealQuoteResult = Service.FxDealQuoteCreate(toCcy, fromCcy, amount, exmod.SellCurrency, false, "SPOT");
		//            if (exmod.FxDealQuoteResult.ServiceResponse.HasErrors)
		//                throw new Exception(exmod.FxDealQuoteResult.ServiceResponse.Responses[0].Message);
		//        }
		//    }
		//    catch (Exception exception)
		//    {
		//        _logger.Error(exception.Message);
		//        ViewBag.Message = exception.Message;
		//    }
		//    return View(exmod);
		//}


		[System.Web.Mvc.HttpPost]
		public ActionResult CreateQuoteExchange(ExchangeModel exmod)
		{
			if (AppSecurity.CurrentUser == null && !HttpContext.User.Identity.IsAuthenticated)
				return RedirectToAction("LogOut", "User", new { returnUrl = Url.Action("CurrencyExchange") });

			var listCustomerBalanceData = new List<CustomerBalanceData>();
			exmod.FxDealQuoteResult = new FXDealQuoteCreateResponse();
			try
			{
				if (CurrentUser.UserId == null)
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_EmptyUserData);
				if (!(exmod.FromAccountId != null && exmod.ToAccountId != null &&
					  exmod.FromAccountId != exmod.ToAccountId))
				{
					_logger.Error(String.Format("sell {0} ||||  buy {1} ", exmod.FromAccountId, exmod.ToAccountId));
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_ChangeCurrency);
				}
				if (exmod.FromAccountId == exmod.ToAccountId)
				{
					_logger.Error(String.Format("sell {0} ||||  buy {1} ", exmod.FromAccountId, exmod.ToAccountId));
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_EqualCurrencyError);
				}

				decimal amount = exmod.BuyAmount;
				if (amount < 0 || amount == 0)
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_EnterAmount);
				try
				{
					listCustomerBalanceData = Service.GetAccountBalances().Balances.ToList();
				}
				catch
				{
				}
				string fromCcy = String.Empty;
				string toCcy = String.Empty;

				if (listCustomerBalanceData.Any(a => a.AccountId == exmod.FromAccountId) &&
					listCustomerBalanceData.Any(a => a.AccountId == exmod.ToAccountId))
				{
					fromCcy = listCustomerBalanceData.FirstOrDefault(a => a.AccountId == exmod.FromAccountId).CCY;
					toCcy = listCustomerBalanceData.FirstOrDefault(a => a.AccountId == exmod.ToAccountId).CCY;
				}

				if (String.IsNullOrEmpty(fromCcy) || String.IsNullOrEmpty(toCcy))
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_NoCorrectCurrency);
				exmod.FxDealQuoteResult = Service.FxDealQuoteCreate(toCcy, fromCcy, amount, exmod.SellCurrency, false, "SPOT");
				if (exmod.FxDealQuoteResult.ServiceResponse.HasErrors)
					throw new Exception(exmod.FxDealQuoteResult.ServiceResponse.Responses[0].Message);
				exmod.FxDealQuoteResult.ServiceResponse = new ServiceResponse() { HasErrors = false, Responses = new[] { new ServiceResponseData { Message = "Ok" } } };
			}
			catch (Exception exception)
			{
				_logger.Error(exception.Message);
				exmod.FxDealQuoteResult.ServiceResponse = new ServiceResponse() { HasErrors = true, Responses = new[] { new ServiceResponseData { Message = exception.Message } } };
			}

			return Json(exmod.FxDealQuoteResult, JsonRequestBehavior.DenyGet);
		}



		[System.Web.Mvc.HttpPost]
		public ActionResult AcceptBookQuoteExchange()
		{
			var model = new BookingCurrency();
			try
			{
				var exMod = TempData["exMod"] as ExchangeModel;
				if (exMod == null)
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_TempDataIsNull);
				model.FxDealQuoteResult = exMod.FxDealQuoteResult;
			}
			catch (Exception e)
			{
				_logger.Error(e);
			}
			return View(model);
		}

		[System.Web.Mvc.HttpPost]
		public ExchangeModel ApiCurrencyExchange(ExchangeModel exmod)
		{
			var listCustomerBalanceData = new List<CustomerBalanceData>();
			try
			{
				if (CurrentUser.UserId == null)
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_EmptyUserData);
				if (!(exmod.FromAccountId != null && exmod.ToAccountId != null &&
					  exmod.FromAccountId != exmod.ToAccountId))
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_ChangeCurrency);
				if (exmod.FromAccountId == exmod.ToAccountId)
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_ChangeCurrency);

				decimal amount = exmod.BuyAmount;
				if (amount < 0 || amount == 0)
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_EnterAmount);
				try
				{
					listCustomerBalanceData = Service.GetAccountBalances().Balances.ToList();
				}
				catch
				{
				}
				string fromCcy = String.Empty;
				string toCcy = String.Empty;
				string sellCurrency = String.Empty;

				if (listCustomerBalanceData.Any(a => a.AccountId == exmod.FromAccountId) &&
					listCustomerBalanceData.Any(a => a.AccountId == exmod.ToAccountId))
				{
					fromCcy = listCustomerBalanceData.FirstOrDefault(a => a.AccountId == exmod.FromAccountId).CCY;
					toCcy = listCustomerBalanceData.FirstOrDefault(a => a.AccountId == exmod.ToAccountId).CCY;
					sellCurrency = listCustomerBalanceData.FirstOrDefault(a => a.AccountId == exmod.SellCurrency).CCY;
				}
				if (String.IsNullOrEmpty(fromCcy) || String.IsNullOrEmpty(toCcy) || String.IsNullOrEmpty(sellCurrency))
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_NoCorrectCurrency);


				if (exmod.FxDealQuoteResult != null && exmod.FxDealQuoteResult.Quote != null &&
					!String.IsNullOrEmpty(exmod.FxDealQuoteResult.Quote.QuoteId))
				{
					exmod.FxDealQuoteBookResult =
						Service.FxDealQuoteBookAndInstantDeposit(exmod.FxDealQuoteResult.Quote.QuoteId);
					if (exmod.FxDealQuoteBookResult.ServiceResponse.HasErrors)
						throw new Exception(exmod.FxDealQuoteBookResult.ServiceResponse.Responses[0].Message);
					return exmod;
				}
				else
				{
					exmod.FxDealQuoteBookResult = null;
					exmod.FxDealQuoteResult = Service.FxDealQuoteCreate(toCcy, fromCcy, amount, sellCurrency, false, "SPOT");
					if (exmod.FxDealQuoteResult.ServiceResponse.HasErrors)
						throw new Exception(exmod.FxDealQuoteResult.ServiceResponse.Responses[0].Message);
					exmod.FxDealQuoteBookResult = Service.FxDealQuoteBookAndInstantDeposit(exmod.FxDealQuoteResult.Quote.QuoteId);
					if (exmod.FxDealQuoteBookResult.ServiceResponse.HasErrors)
						throw new Exception(exmod.FxDealQuoteBookResult.ServiceResponse.Responses[0].Message);
					return exmod;
				}
			}
			catch (Exception exception)
			{
				_logger.Error(exception.Message);
				ViewBag.Message = exception.Message;
			}
			return exmod;
		}


		[System.Web.Mvc.HttpPost]
		public ExchangeModel ApiCreateQuoteExchange(ExchangeModel exmod)
		{
			var listCustomerBalanceData = new List<CustomerBalanceData>();
			exmod.FxDealQuoteResult = new FXDealQuoteCreateResponse();
			try
			{
				if (CurrentUser.UserId == null)
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_EmptyUserData);
				if (!(exmod.FromAccountId != null && exmod.ToAccountId != null &&
					  exmod.FromAccountId != exmod.ToAccountId))
				{
					_logger.Error(String.Format("sell {0} ||||  buy {1} ", exmod.FromAccountId, exmod.ToAccountId));
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_ChangeCurrency);
				}
				if (exmod.FromAccountId == exmod.ToAccountId)
				{
					_logger.Error(String.Format("sell {0} ||||  buy {1} ", exmod.FromAccountId, exmod.ToAccountId));
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_EqualCurrencyError);
				}

				decimal amount = exmod.BuyAmount;
				if (amount < 0 || amount == 0)
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_EnterAmount);
				try
				{
					listCustomerBalanceData = Service.GetAccountBalances().Balances.ToList();
				}
				catch
				{
				}
				string fromCcy = String.Empty;
				string toCcy = String.Empty;
				string sellCurrency = String.Empty;

				if (listCustomerBalanceData.Any(a => a.AccountId == exmod.FromAccountId) &&
					listCustomerBalanceData.Any(a => a.AccountId == exmod.ToAccountId))
				{
					fromCcy = listCustomerBalanceData.FirstOrDefault(a => a.AccountId == exmod.FromAccountId).CCY;
					toCcy = listCustomerBalanceData.FirstOrDefault(a => a.AccountId == exmod.ToAccountId).CCY;
					sellCurrency = listCustomerBalanceData.FirstOrDefault(a => a.AccountId == exmod.SellCurrency).CCY;
				}

				if (String.IsNullOrEmpty(fromCcy) || String.IsNullOrEmpty(toCcy))
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_NoCorrectCurrency);
				exmod.FxDealQuoteResult = Service.FxDealQuoteCreate(toCcy, fromCcy, amount, sellCurrency, false, "SPOT");
				if (exmod.FxDealQuoteResult.ServiceResponse.HasErrors)
					//throw new Exception(exmod.FxDealQuoteResult.ServiceResponse.Responses[0].Message);
					throw new Exception(exmod.FxDealQuoteResult.ServiceResponse.Responses[0].MessageDetails);
				exmod.FxDealQuoteResult.ServiceResponse = new ServiceResponse() { HasErrors = false, Responses = new[] { new ServiceResponseData { Message = "Ok" } } };
			}
			catch (Exception exception)
			{
				_logger.Error(exception.Message);
				exmod.FxDealQuoteResult.ServiceResponse = new ServiceResponse() { HasErrors = true, Responses = new[] { new ServiceResponseData { Message = exception.Message } } };
			}

			return exmod;
		}

		[System.Web.Mvc.HttpPost]
		public ExchangeModel ApiCreateQuoteExchange2(ExchangeModel exmod)
		{
			var listCustomerBalanceData = new List<CustomerBalanceData>();
			exmod.FxDealQuoteResult = new FXDealQuoteCreateResponse();
			try
			{
				if (CurrentUser.UserId == null)
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_EmptyUserData);
				if (!(exmod.FromAccountId != null && exmod.ToAccountId != null &&
					  exmod.FromAccountId != exmod.ToAccountId))
				{
					_logger.Error(String.Format("sell {0} ||||  buy {1} ", exmod.FromAccountId, exmod.ToAccountId));
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_ChangeCurrency);
				}
				if (exmod.FromAccountId == exmod.ToAccountId)
				{
					_logger.Error(String.Format("sell {0} ||||  buy {1} ", exmod.FromAccountId, exmod.ToAccountId));
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_EqualCurrencyError);
				}

				decimal amount = exmod.BuyAmount;
				if (amount < 0 || amount == 0)
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_EnterAmount);
				try
				{
					listCustomerBalanceData = Service.GetAccountBalances().Balances.ToList();
				}
				catch
				{
				}
				string fromCcy = String.Empty;
				string toCcy = String.Empty;
				string sellCurrency = String.Empty;
				string quoteType = exmod.QuoteType;
				string windowOpenDate = exmod.WindowOpenDate;
				string finalValueDate = exmod.FinalValueDate;


				if (listCustomerBalanceData.Any(a => a.AccountId == exmod.FromAccountId) &&
					listCustomerBalanceData.Any(a => a.AccountId == exmod.ToAccountId))
				{
					fromCcy = listCustomerBalanceData.FirstOrDefault(a => a.AccountId == exmod.FromAccountId).CCY;
					toCcy = listCustomerBalanceData.FirstOrDefault(a => a.AccountId == exmod.ToAccountId).CCY;
					sellCurrency = listCustomerBalanceData.FirstOrDefault(a => a.AccountId == exmod.SellCurrency).CCY;
				}

				if (String.IsNullOrEmpty(fromCcy) || String.IsNullOrEmpty(toCcy))
					throw new Exception(GlobalRes.Exchange_ExchangeController_CurrencyExchange_NoCorrectCurrency);
				exmod.FxDealQuoteResult = Service.FxDealQuoteCreate(toCcy, fromCcy, amount, sellCurrency, false, quoteType, windowOpenDate, finalValueDate);
				if (exmod.FxDealQuoteResult.ServiceResponse.HasErrors)
					throw new Exception(exmod.FxDealQuoteResult.ServiceResponse.Responses[0].Message);
				exmod.FxDealQuoteResult.ServiceResponse = new ServiceResponse() { HasErrors = false, Responses = new[] { new ServiceResponseData { Message = "Ok" } } };
			}
			catch (Exception exception)
			{
				_logger.Error(exception.Message);
				exmod.FxDealQuoteResult.ServiceResponse = new ServiceResponse() { HasErrors = true, Responses = new[] { new ServiceResponseData { Message = exception.Message } } };
			}

			return exmod;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="model">Convert and exchange (FX Deal) search result view model</param>
		/// <returns></returns>
		public ActionResult History(ConvertSearchViewModel model)
		{
			model.PrepareConverts();
			return View(model);
		}


		/// <summary>
		/// Details of a convert (FX)
		/// </summary>
		/// <param name="id">Convert (FX) ID </param>
		/// <param name="reference">Convert (FX) Reference </param>
		/// <returns></returns>
		public ActionResult Details(Guid? id, string reference)
		{
			var model = new ConvertDetailsViewModel();
			if (!string.IsNullOrEmpty(reference))
			{
				model.PrepareDetailsByReference(reference);
				_logger.Debug("reference:" + reference);
			}
			else if (id.HasValue)
			{
				model.FXDealId = id.Value;
				model.MailModel = new UserMailModel();
				model.PrepareDetails();
				if (model.FXDealId != Guid.Empty)
				{
					return View(model);
				}
				model.Errors.Add(new ErrorViewModel
				{
					Message = "Convert doesn't exist or you doesn't have permission",
					Type = ErrorType.Error,
					Code = 0,
					MessageDetails = "",
					FieldName = "",
					FieldValue = ""
				});
			}

			return View(model);
		}

		/// <summary>
		///  Post action of details action
		///  
		/// </summary>
		/// <param name="model"></param>
		/// <param name="id"></param>
		/// <param name="res"></param>
		/// <param name="ck"></param>
		/// <returns></returns>
		[System.Web.Mvc.HttpPost]
		public ActionResult Details(ConvertDetailsViewModel model, Guid id, string res, string ck)
		{
			_logger.Debug("ExchangeController, Details");
			ConvertDetailsViewModel modelReal = new ConvertDetailsViewModel(id);
			modelReal.PrepareDetails();
			string imageId;
			DateTime dt;
			PaymentRepository pr = new PaymentRepository();

			if (!Directory.Exists(Server.MapPath("~/UserFiles/ConvertDetails")))
			{
				_logger.Debug("ExchangeController, Details, ConvertDetails not exists, create it");
				Directory.CreateDirectory(Server.MapPath("~/UserFiles/ConvertDetails"));
			}
			var checkPrevPhoto = pr.GetSharedPhoto(id.ToString(), out imageId, out dt);
			if (checkPrevPhoto.IsSuccess != null && Convert.ToBoolean(checkPrevPhoto.IsSuccess) && Guid.Parse(imageId) != Guid.Empty && dt != new DateTime())
			{
				if (dt < DateTime.Now)
					System.IO.File.Delete(String.Format("{0}/{1}.png", Server.MapPath("~/UserFiles/ConvertDetails"),
						imageId));
				else if (System.IO.File.Exists(Server.MapPath("~/UserFiles/ConvertDetails/" + imageId + ".png")))
				{
					MemoryStream ms = new MemoryStream();
					using (Image i = Image.FromFile(Server.MapPath("~/UserFiles/ConvertDetails/" + imageId + ".png")))
					{
						i.Save(ms, ImageFormat.Png);
					}

					return File(ms.ToArray(), "application/octetstream", String.Format("{0}.png", modelReal.FXDealReference));
				}
			}

			_logger.Debug("ExchangeController, Details, creating Bitmap image");
			using (Bitmap image = new Bitmap(660, 500))
			{
				using (Graphics g = Graphics.FromImage(image))
				{
					SolidBrush backBrush = new SolidBrush(Color.FromArgb(253, 253, 253));
					SolidBrush headColorBrush = new SolidBrush(Color.FromArgb(0, 131, 193));
					SolidBrush mainColorBrush = new SolidBrush(Color.FromArgb(51, 51, 51));
					SolidBrush smothColorBrush = new SolidBrush(Color.FromArgb(249, 249, 249));
					g.FillRectangle(backBrush, new Rectangle(0, 0, 860, 600));

					Image logo = Image.FromFile(Server.MapPath("~/Content/assets/images/winstantpay-logo-notag.png"));
					logo = logo.ResizeImage(90, 30);
					g.DrawImage(logo, new Point(860 - logo.Width - 20, 600 - logo.Height));

					#region Draw horizontal line

					g.FillRectangle(headColorBrush, new Rectangle(20, 60, 250, 2));

					#endregion

					#region Draw Head Text

					string instPayText = GlobalRes.ConvertDetailsPage_Title;
					Font drawHeadFont = new Font("Open Sans", 22);
					Font drawSubHeadFont = new Font("Open Sans", 12, FontStyle.Bold);
					Font drawBoldFont = new Font("Open Sans", 10, FontStyle.Bold);
					Font drawRegFont = new Font("Open Sans", 10);
					PointF stringPoint = new PointF(20, 20);
					g.DrawString(instPayText, drawHeadFont, mainColorBrush, stringPoint,
						StringFormat.GenericTypographic);

					#endregion

					Pen recPen = new Pen(Color.FromArgb(221, 221, 221));
					//------------------------ Convert Information Header -------------------------------------------//
					g.DrawRectangle(recPen, new Rectangle(20, 70, 399, 40));
					g.FillRectangle(smothColorBrush, new Rectangle(21, 71, 398, 39));
					g.DrawString(GlobalRes.ConvertDetailsPage_ConvertInformation, drawSubHeadFont, mainColorBrush, new PointF(25, 78), StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(20, 110, 200, 40));
					g.DrawString(GlobalRes.ConvertDetailsPage_ClientBuys, drawBoldFont, mainColorBrush, new PointF(25, 118), StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(20, 150, 200, 40));
					g.FillRectangle(smothColorBrush, new Rectangle(21, 151, 199, 39));
					g.DrawString(GlobalRes.ConvertDetailsPage_ClientSells, drawBoldFont, mainColorBrush, new PointF(25, 158), StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(20, 190, 200, 40));
					g.DrawString(GlobalRes.ConvertDetailsPage_FinalValueDate, drawBoldFont, mainColorBrush, new PointF(25, 198), StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(20, 230, 200, 40));
					g.FillRectangle(smothColorBrush, new Rectangle(21, 231, 199, 39));
					g.DrawString(GlobalRes.ConvertDetailsPage_DealDate, drawBoldFont, mainColorBrush, new PointF(25, 238), StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(20, 270, 200, 40));
					g.DrawString(GlobalRes.ConvertDetailsPage_BookedRate, drawBoldFont, mainColorBrush, new PointF(25, 278), StringFormat.GenericTypographic);



					//------------------------ Booking Information Header -------------------------------------------//
					g.DrawRectangle(recPen, new Rectangle(440, 70, 399, 40));
					g.FillRectangle(smothColorBrush, new Rectangle(441, 71, 398, 39));
					g.DrawString(GlobalRes.ConvertDetailsPage_BookingInformation, drawSubHeadFont, mainColorBrush, new PointF(445, 78), StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(440, 110, 200, 40));
					g.DrawString(GlobalRes.ConvertDetailsPage_FXDealReference, drawBoldFont, mainColorBrush, new PointF(445, 118),
						StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(440, 150, 200, 40));
					g.FillRectangle(smothColorBrush, new Rectangle(441, 151, 199, 39));
					g.DrawString(GlobalRes.ConvertDetailsPage_DealType, drawBoldFont, mainColorBrush, new PointF(445, 158),
						StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(440, 190, 200, 40));
					g.DrawString(GlobalRes.ConvertDetailsPage_BookedFor, drawBoldFont, mainColorBrush, new PointF(445, 198),
						StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(440, 230, 200, 40));
					g.FillRectangle(smothColorBrush, new Rectangle(441, 231, 199, 39));
					g.DrawString(GlobalRes.ConvertDetailsPage_Branch, drawBoldFont, mainColorBrush, new PointF(445, 238),
						StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(440, 270, 200, 40));
					g.DrawString(GlobalRes.ConvertDetailsPage_BookedBy, drawBoldFont, mainColorBrush, new PointF(445, 278),
						StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(440, 310, 200, 40));
					g.FillRectangle(smothColorBrush, new Rectangle(441, 311, 199, 39));
					g.DrawString(GlobalRes.ConvertDetailsPage_FXDealFee, drawBoldFont, mainColorBrush, new PointF(445, 318),
						StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(440, 350, 200, 40));
					g.DrawString(GlobalRes.ConvertDetailsPage_PricingTemplateFee, drawBoldFont, mainColorBrush, new PointF(445, 358),
						StringFormat.GenericTypographic);

					//--------------------------------- Convert Information ---------------------------------------------------------------//
					g.DrawRectangle(recPen, new Rectangle(220, 110, 199, 40));
					g.DrawString(!String.IsNullOrEmpty(model.BuyAmountTextWithCurrencyCode) ? model.BuyAmountTextWithCurrencyCode : "", drawRegFont, mainColorBrush,
						new RectangleF(225, 120, 199, 40), StringFormat.GenericTypographic);

					g.FillRectangle(smothColorBrush, new Rectangle(221, 151, 199, 38));
					g.DrawRectangle(recPen, new Rectangle(220, 150, 199, 40));
					g.DrawString(!String.IsNullOrEmpty(model.SellAmountTextWithCurrencyCode) ? model.SellAmountTextWithCurrencyCode : "", drawRegFont, mainColorBrush,
						new RectangleF(225, 160, 199, 40), StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(220, 190, 199, 40));
					g.DrawString(!String.IsNullOrEmpty(model.FinalValueDate) ? model.FinalValueDate : "", drawRegFont, mainColorBrush,
						new RectangleF(225, 200, 199, 40), StringFormat.GenericTypographic);

					g.FillRectangle(smothColorBrush, new Rectangle(221, 231, 199, 38));
					g.DrawRectangle(recPen, new Rectangle(220, 230, 199, 40));
					g.DrawString(!String.IsNullOrEmpty(model.DealDate) ? model.DealDate : "", drawRegFont, mainColorBrush,
						new RectangleF(225, 240, 199, 40), StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(220, 270, 199, 40));
					g.DrawString(!String.IsNullOrEmpty(model.BookedRateTextWithCurrencyCodes) ? model.BookedRateTextWithCurrencyCodes : "", drawRegFont, mainColorBrush, new RectangleF(225, 280, 199, 40),
						StringFormat.GenericTypographic);

					//--------------------------------- Booking Information ---------------------------------------------------------------//
					g.DrawRectangle(recPen, new Rectangle(640, 110, 199, 40));
					g.DrawString(!String.IsNullOrEmpty(model.FXDealReference) ? model.FXDealReference : "", drawRegFont, mainColorBrush, new RectangleF(645, 120, 199, 40), StringFormat.GenericTypographic);

					g.FillRectangle(smothColorBrush, new Rectangle(641, 151, 199, 38));
					g.DrawRectangle(recPen, new Rectangle(640, 150, 199, 40));
					g.DrawString(!String.IsNullOrEmpty(model.FXDealTypeName) ? model.FXDealTypeName : "", drawRegFont, mainColorBrush, new RectangleF(645, 160, 199, 40), StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(640, 190, 199, 40));
					g.DrawString(!String.IsNullOrEmpty(model.BookedForCustomerName) ? model.BookedForCustomerName : "", drawRegFont, mainColorBrush, new RectangleF(645, 200, 199, 40), StringFormat.GenericTypographic);

					g.FillRectangle(smothColorBrush, new Rectangle(641, 231, 199, 38));
					g.DrawRectangle(recPen, new Rectangle(640, 230, 199, 40));
					g.DrawString(!String.IsNullOrEmpty(model.BranchName) ? model.BranchName : "", drawRegFont, mainColorBrush, new RectangleF(645, 240, 199, 40), StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(640, 270, 199, 40));
					g.DrawString(!String.IsNullOrEmpty(model.BookedByUserName) ? model.BookedByUserName : "", drawRegFont, mainColorBrush, new RectangleF(645, 280, 199, 40), StringFormat.GenericTypographic);

					g.FillRectangle(smothColorBrush, new Rectangle(641, 311, 199, 38));
					g.DrawRectangle(recPen, new Rectangle(640, 310, 199, 40));
					g.DrawString(!String.IsNullOrEmpty(model.FeeTemplateFeeAmountTextWithCurrencyCode) ? model.FeeTemplateFeeAmountTextWithCurrencyCode : "", drawRegFont, mainColorBrush, new RectangleF(645, 320, 199, 40), StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(640, 350, 199, 40));
					g.DrawString(!String.IsNullOrEmpty(model.PricingTemplateFeeAmountTextWithCurrencyCode) ? model.PricingTemplateFeeAmountTextWithCurrencyCode : "", drawRegFont, mainColorBrush, new RectangleF(645, 360, 199, 40), StringFormat.GenericTypographic);
				}

				MemoryStream ms = new MemoryStream();

				image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
				_logger.Debug("ExchangeController, Details, Bitmap image created");

				if (res == "file")
					return File(ms.ToArray(), "application/octetstream",
						String.Format("{0}.png", modelReal.FXDealReference));
				else return File(ms.ToArray(), "image/png");
			}
		}

		/// <summary>
		/// Get convert (FX) details image link
		/// </summary>
		/// <param name="id"></param>
		/// <param name="res"></param>
		/// <returns></returns>
		[System.Web.Mvc.HttpPost]
		public ActionResult GetImageLink(Guid id, string res)
		{
			_logger.DebugFormat("id: {0}", id);
			var model = new ConvertDetailsViewModel(id);
			model.PrepareDetails();
			string imageId;
			DateTime dt;
			PaymentRepository pr = new PaymentRepository();
			if (!Directory.Exists(Server.MapPath("~/UserFiles/ConvertDetails")))
			{
				_logger.Debug("ConvertDetails folder not exists, creating it");
				Directory.CreateDirectory(Server.MapPath("~/UserFiles/ConvertDetails"));
			}
			var checkPrevPhoto = pr.GetSharedPhoto(id.ToString(), out imageId, out dt);

			_logger.DebugFormat("imageId: {0}", imageId);

			if (checkPrevPhoto != null && Convert.ToBoolean(checkPrevPhoto.IsSuccess) && Guid.Parse(imageId) != Guid.Empty && dt != new DateTime())
			{
				if (dt < DateTime.Now)
					System.IO.File.Delete(String.Format("{0}/{1}.png", Server.MapPath("~/UserFiles/ConvertDetails"),
						imageId));

				else if (System.IO.File.Exists(Server.MapPath("~/UserFiles/ConvertDetails/" + imageId + ".png")))
				{
					_logger.Debug("Image file exists, return it");
					return Json(imageId, JsonRequestBehavior.DenyGet);
				}
			}

			_logger.Debug("Saving image record in database");
			var createRecRes = pr.AddOrUpdateSharedPhoto(id.ToString(), out imageId);
			_logger.Debug("Record saved");

			if (createRecRes.IsSuccess != null && Convert.ToBoolean(createRecRes.IsSuccess))
			{
				_logger.Debug("Creating bitmap image");
				using (Bitmap image = new Bitmap(860, 500))
				{
					using (Graphics g = Graphics.FromImage(image))
					{
						SolidBrush backBrush = new SolidBrush(Color.FromArgb(253, 253, 253));
						SolidBrush headColorBrush = new SolidBrush(Color.FromArgb(0, 131, 193));
						SolidBrush mainColorBrush = new SolidBrush(Color.FromArgb(51, 51, 51));
						SolidBrush smothColorBrush = new SolidBrush(Color.FromArgb(249, 249, 249));
						g.FillRectangle(backBrush, new Rectangle(0, 0, 860, 500));

						Image logo = Image.FromFile(Server.MapPath("~/Content/assets/images/winstantpay-logo-notag.png"));
						logo = logo.ResizeImage(90, 30);
						g.DrawImage(logo, new Point(860 - logo.Width - 20, 500 - logo.Height));

						#region Draw horizontal line

						g.FillRectangle(headColorBrush, new Rectangle(20, 60, 250, 2));

						#endregion

						#region Draw Head Text

						string instPayText = GlobalRes.ConvertDetailsPage_Title;
						Font drawHeadFont = new Font("Open Sans", 22);
						Font drawSubHeadFont = new Font("Open Sans", 12, FontStyle.Bold);
						Font drawBoldFont = new Font("Open Sans", 10, FontStyle.Bold);
						Font drawRegFont = new Font("Open Sans", 10);
						PointF stringPoint = new PointF(20, 20);
						g.DrawString(instPayText, drawHeadFont, mainColorBrush, stringPoint,
							StringFormat.GenericTypographic);

						#endregion

						Pen recPen = new Pen(Color.FromArgb(221, 221, 221));
						//------------------------ Convert Information Header -------------------------------------------//
						g.DrawRectangle(recPen, new Rectangle(20, 70, 399, 40));
						g.FillRectangle(smothColorBrush, new Rectangle(21, 71, 398, 39));
						g.DrawString(GlobalRes.ConvertDetailsPage_ConvertInformation, drawSubHeadFont, mainColorBrush, new PointF(25, 78), StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(20, 110, 200, 40));
						g.DrawString(GlobalRes.ConvertDetailsPage_ClientBuys, drawBoldFont, mainColorBrush, new PointF(25, 118), StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(20, 150, 200, 40));
						g.FillRectangle(smothColorBrush, new Rectangle(21, 151, 199, 39));
						g.DrawString(GlobalRes.ConvertDetailsPage_ClientSells, drawBoldFont, mainColorBrush, new PointF(25, 158), StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(20, 190, 200, 40));
						g.DrawString(GlobalRes.ConvertDetailsPage_FinalValueDate, drawBoldFont, mainColorBrush, new PointF(25, 198), StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(20, 230, 200, 40));
						g.FillRectangle(smothColorBrush, new Rectangle(21, 231, 199, 39));
						g.DrawString(GlobalRes.ConvertDetailsPage_DealDate, drawBoldFont, mainColorBrush, new PointF(25, 238), StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(20, 270, 200, 40));
						g.DrawString(GlobalRes.ConvertDetailsPage_BookedRate, drawBoldFont, mainColorBrush, new PointF(25, 278), StringFormat.GenericTypographic);



						//------------------------ Booking Information Header -------------------------------------------//
						g.DrawRectangle(recPen, new Rectangle(440, 70, 399, 40));
						g.FillRectangle(smothColorBrush, new Rectangle(441, 71, 398, 39));
						g.DrawString(GlobalRes.ConvertDetailsPage_BookingInformation, drawSubHeadFont, mainColorBrush, new PointF(445, 78), StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(440, 110, 200, 40));
						g.DrawString(GlobalRes.ConvertDetailsPage_FXDealReference, drawBoldFont, mainColorBrush, new PointF(445, 118),
							StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(440, 150, 200, 40));
						g.FillRectangle(smothColorBrush, new Rectangle(441, 151, 199, 39));
						g.DrawString(GlobalRes.ConvertDetailsPage_DealType, drawBoldFont, mainColorBrush, new PointF(445, 158),
							StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(440, 190, 200, 40));
						g.DrawString(GlobalRes.ConvertDetailsPage_BookedFor, drawBoldFont, mainColorBrush, new PointF(445, 198),
							StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(440, 230, 200, 40));
						g.FillRectangle(smothColorBrush, new Rectangle(441, 231, 199, 39));
						g.DrawString(GlobalRes.ConvertDetailsPage_Branch, drawBoldFont, mainColorBrush, new PointF(445, 238),
							StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(440, 270, 200, 40));
						g.DrawString(GlobalRes.ConvertDetailsPage_BookedBy, drawBoldFont, mainColorBrush, new PointF(445, 278),
							StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(440, 310, 200, 40));
						g.FillRectangle(smothColorBrush, new Rectangle(441, 311, 199, 39));
						g.DrawString(GlobalRes.ConvertDetailsPage_FXDealFee, drawBoldFont, mainColorBrush, new PointF(445, 318),
							StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(440, 350, 200, 40));
						g.DrawString(GlobalRes.ConvertDetailsPage_PricingTemplateFee, drawBoldFont, mainColorBrush, new PointF(445, 358),
							StringFormat.GenericTypographic);

						//--------------------------------- Convert Information ---------------------------------------------------------------//
						g.DrawRectangle(recPen, new Rectangle(220, 110, 199, 40));
						g.DrawString(!String.IsNullOrEmpty(model.BuyAmountTextWithCurrencyCode) ? model.BuyAmountTextWithCurrencyCode : "", drawRegFont, mainColorBrush,
							new RectangleF(225, 120, 199, 40), StringFormat.GenericTypographic);

						g.FillRectangle(smothColorBrush, new Rectangle(221, 151, 199, 38));
						g.DrawRectangle(recPen, new Rectangle(220, 150, 199, 40));
						g.DrawString(!String.IsNullOrEmpty(model.SellAmountTextWithCurrencyCode) ? model.SellAmountTextWithCurrencyCode : "", drawRegFont, mainColorBrush,
							new RectangleF(225, 160, 199, 40), StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(220, 190, 199, 40));
						g.DrawString(!String.IsNullOrEmpty(model.FinalValueDate) ? model.FinalValueDate : "", drawRegFont, mainColorBrush,
							new RectangleF(225, 200, 199, 40), StringFormat.GenericTypographic);

						g.FillRectangle(smothColorBrush, new Rectangle(221, 231, 199, 38));
						g.DrawRectangle(recPen, new Rectangle(220, 230, 199, 40));
						g.DrawString(!String.IsNullOrEmpty(model.DealDate) ? model.DealDate : "", drawRegFont, mainColorBrush,
							new RectangleF(225, 240, 199, 40), StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(220, 270, 199, 40));
						g.DrawString(!String.IsNullOrEmpty(model.BookedRateTextWithCurrencyCodes) ? model.BookedRateTextWithCurrencyCodes : "", drawRegFont, mainColorBrush, new RectangleF(225, 280, 199, 40),
							StringFormat.GenericTypographic);

						//--------------------------------- Booking Information ---------------------------------------------------------------//
						g.DrawRectangle(recPen, new Rectangle(640, 110, 199, 40));
						g.DrawString(!String.IsNullOrEmpty(model.FXDealReference) ? model.FXDealReference : "", drawRegFont, mainColorBrush, new RectangleF(645, 120, 199, 40), StringFormat.GenericTypographic);

						g.FillRectangle(smothColorBrush, new Rectangle(641, 151, 199, 38));
						g.DrawRectangle(recPen, new Rectangle(640, 150, 199, 40));
						g.DrawString(!String.IsNullOrEmpty(model.FXDealTypeName) ? model.FXDealTypeName : "", drawRegFont, mainColorBrush, new RectangleF(645, 160, 199, 40), StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(640, 190, 199, 40));
						g.DrawString(!String.IsNullOrEmpty(model.BookedForCustomerName) ? model.BookedForCustomerName : "", drawRegFont, mainColorBrush, new RectangleF(645, 200, 199, 40), StringFormat.GenericTypographic);

						g.FillRectangle(smothColorBrush, new Rectangle(641, 231, 199, 38));
						g.DrawRectangle(recPen, new Rectangle(640, 230, 199, 40));
						g.DrawString(!String.IsNullOrEmpty(model.BranchName) ? model.BranchName : "", drawRegFont, mainColorBrush, new RectangleF(645, 240, 199, 40), StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(640, 270, 199, 40));
						g.DrawString(!String.IsNullOrEmpty(model.BookedByUserName) ? model.BookedByUserName : "", drawRegFont, mainColorBrush, new RectangleF(645, 280, 199, 40), StringFormat.GenericTypographic);

						g.FillRectangle(smothColorBrush, new Rectangle(641, 311, 199, 38));
						g.DrawRectangle(recPen, new Rectangle(640, 310, 199, 40));
						g.DrawString(!String.IsNullOrEmpty(model.FeeTemplateFeeAmountTextWithCurrencyCode) ? model.FeeTemplateFeeAmountTextWithCurrencyCode : "", drawRegFont, mainColorBrush, new RectangleF(645, 320, 199, 40), StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(640, 350, 199, 40));
						g.DrawString(!String.IsNullOrEmpty(model.PricingTemplateFeeAmountTextWithCurrencyCode) ? model.PricingTemplateFeeAmountTextWithCurrencyCode : "", drawRegFont, mainColorBrush, new RectangleF(645, 360, 199, 40), StringFormat.GenericTypographic);
					}
					MemoryStream ms = new MemoryStream();

					image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
					image.Save(String.Format("{0}/{1}.png", Server.MapPath("~/UserFiles/ConvertDetails"), imageId),
						System.Drawing.Imaging.ImageFormat.Png);

				}

				_logger.Debug("Bitmap image created");

				return Json(imageId, JsonRequestBehavior.DenyGet);
			}
			return Json("", JsonRequestBehavior.DenyGet);
		}
	}
}