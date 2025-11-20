using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Newtonsoft.Json;
using StaticExtensions;
using Swashbuckle.Swagger;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.ApiMethods.AutomaticExchange;
using Tsg.UI.Main.ApiMethods.Payments;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.DTO.InstantPayment;
using TSG.Models.ServiceModels.AutomaticExchange;
using TSG.Models.ServiceModels.Shop;
using TSG.ServiceLayer.AutomaticExchange.DependencyLiquidForUserServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidCcyListServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidOverDraftUserServiceMethods;
using TSG.ServiceLayer.InstantPayment;
using TSG.ServiceLayer.Users;
using TSG.ServiceLayer.WinstantPayShop.ShopInfoService;
using TSG.ServiceLayer.WinstantPayShop.ShopOrderItemsService;
using TSG.ServiceLayer.WinstantPayShop.ShopOrdersService;
using TSG.ServiceLayer.WinstantPayShop.ShopPaymentService;
using TSG.ServiceLayer.WinstantPayShop.ShopProductImagesService;
using TSG.ServiceLayer.WinstantPayShop.ShopProductService;

namespace Tsg.UI.Main.Controllers
{
	public class PaymentController : BaseController
	{
		private readonly IShopOrdersService _shopOrdersService;
		private readonly IShopOrderItemsService _shopOrderItemsService;
		private readonly IShopProductService _shopProductService;
		private readonly IShopProductImagesService _shopProductImagesService;
		private readonly IShopInfoService _shopInfoService;
		private readonly IShopPaymentService _shopPaymentService;
		private readonly ILiquidCcyListServiceMethods _liquidCcyListServiceMethods;
		private readonly IDependencyLiquidForUserServiceMethods _dependencyLiquidForUserService;
		private readonly ILiquidOverDraftUserServiceMethods _liquidOverDraftUserServiceMethods;
		private readonly IUsersServiceMethods _usersServiceMethods;
		private readonly IInstantPaymentReceiveMethods _instantPaymentReceiveService;
		private readonly IInstantPaymentReceiveMappingMethods _instantPaymentReceiveMappingService;

		readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public PaymentController(IShopOrdersService shopOrdersService, IShopOrderItemsService shopOrderItemsService, IShopProductService shopProductService,
			IShopProductImagesService shopProductImagesService, IShopInfoService shopInfoService, IShopPaymentService shopPaymentService,
			IDependencyLiquidForUserServiceMethods dependencyLiquidForUserService, ILiquidCcyListServiceMethods liquidCcyListServiceMethods,
			ILiquidOverDraftUserServiceMethods liquidOverDraftUserServiceMethods, IUsersServiceMethods usersServiceMethods, IInstantPaymentReceiveMethods instantPaymentReceiveService, IInstantPaymentReceiveMappingMethods instantPaymentReceiveMappingService)
		{
			_shopOrdersService = shopOrdersService;
			_shopOrderItemsService = shopOrderItemsService;
			_shopProductService = shopProductService;
			_shopProductImagesService = shopProductImagesService;
			_shopInfoService = shopInfoService;
			_shopPaymentService = shopPaymentService;
			_dependencyLiquidForUserService = dependencyLiquidForUserService;
			_liquidCcyListServiceMethods = liquidCcyListServiceMethods;
			_liquidOverDraftUserServiceMethods = liquidOverDraftUserServiceMethods;
			_usersServiceMethods = usersServiceMethods;
			_instantPaymentReceiveService = instantPaymentReceiveService;
			_instantPaymentReceiveMappingService = instantPaymentReceiveMappingService;
		}

		private static readonly string cryptoCurrency = ConfigurationManager.AppSettings["cryptoCurrencies"];


		public ActionResult Currencies()
		{
			PaymentCurrenciesListViewModel model = new PaymentCurrenciesListViewModel();
			model.PrepareCurrencies();
			return View(model);
		}

		/// <summary>
		/// Create instant payment
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="ccy"></param>
		/// <param name="amount"></param>
		/// <param name="invoice"></param>
		/// <param name="memo"></param>
		/// <param name="id"></param>
		/// <param name="receiveId"></param>
		/// <returns></returns>
		public ActionResult Create(string alias, string ccy, string amount, string invoice, string memo, Guid? id, Guid? receiveId)
		{
			_logger.Debug("Start payment create");
			ViewBag.OpenPanel = true;
			ViewBag.RepeatPayment = id.HasValue;
			decimal ConvertedAmount = 0;

			Decimal.TryParse(amount, out ConvertedAmount);

			if (id.HasValue)
			{
				return View(new NewInstantPaymentViewModel(id));
			}

			if (receiveId.HasValue)
			{
				_logger.DebugFormat("receiveId: {0}", receiveId.Value);
				var model = PrepareNewIntantPaymentViewModelByReceiveId(receiveId.Value);
				_logger.DebugFormat("After preparing new instant payment model: {0}", JsonConvert.SerializeObject(model));
				return View(model);
			}

			return View(new NewInstantPaymentViewModel(alias, ccy, ConvertedAmount, invoice, memo));
		}

		/// <summary>
		/// Create instant payment
		/// </summary>
		/// <param name="model">new instant payment model</param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult Create(NewInstantPaymentViewModel model)
		{
			try
			{
				if (!String.IsNullOrEmpty(model.CurrencyCode))
				{
					_logger.InfoFormat("Attempting to create payment: {0}", JsonConvert.SerializeObject(model));
					Guid? paymentId;
					if (String.IsNullOrEmpty(model.FromCustomer)) model.FromCustomer = model.AccountAliases.FirstOrDefault()?.Value ?? string.Empty;

					_logger.Info("Can create payment");
					model.Create(out paymentId);

					if (!model.HasError)
					{
						_logger.InfoFormat("Successfully created payment: {0}", JsonConvert.SerializeObject(model));
						if (paymentId == null)
							return RedirectToAction("History");

						var info = new InstantPaymentDetailsViewModel(paymentId.Value);
						info.PrepareDetails();

						var mapping = new InstantPaymentReceiveMappingDto();
						mapping.InstantPaymentId = paymentId.Value;
						mapping.InstantPaymentReceiveId = model.InstantPaymentReceiveId;

						_instantPaymentReceiveMappingService.Insert(mapping);

						if (model.Action == "post")
						{
							_logger.Info(Newtonsoft.Json.JsonConvert.SerializeObject(info));
							Task.Run(() => TSG.UI.Main.Notifications.Notification.SendNotification(
								info.ToCustomerId,
								paymentId.Value, "Instant payment",
								String.Format(GlobalRes.SuccessPayment_InvoiceNotificationText, info.FromCustomerAlias)));
						}

						return RedirectToAction("Details", "Payment", new { id = paymentId });
					}
					else
					{
						model.HasError = true;
						_logger.ErrorFormat("Payment creation error: {0}", JsonConvert.SerializeObject(model.Errors));
					}
				}
				else
				{
					model.HasError = true;
					model.Errors.Add(new ErrorViewModel()
					{
						MessageDetails = "Please select currency"
					});
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
			}
			ViewBag.OpenPanel = true;
			return View(model);
		}

		public ActionResult History(InstantPaymentSearchViewModel model)
		{

			model.PrepareInstantPayments();
			//model.Payments = model.Payments.Where(w => w.FromCustomerName == AppSecurity.CurrentUser.OrganisationName).ToList();
			return View(model);
		}

		public ActionResult Details(Guid id)
		{
			var model = new InstantPaymentDetailsViewModel(id);

			model.MailModel = new UserMailModel();
			model.PrepareDetails();
			if (model.PaymentId != default)
			{
				if (model.Invoice.Length >= 11)
				{
					var num = model.Invoice.Substring(
						model.Invoice.IndexOf(Regex.Match(model.Invoice, @"([-])+(\d{6})").Value,
							StringComparison.Ordinal) + 7);

					if (!string.IsNullOrEmpty(num) && int.TryParse(num, out var intNum))
					{
						var potencialOrder = _shopOrdersService.GetAll().Obj.FirstOrDefault(w =>
							w.ShopOrders_BuyerWpayId == AppSecurity.CurrentUser.UserName &&
							w.ShopOrders_OrderCounter == intNum);
						if (potencialOrder != null)
							model.ShopOrder =
								_shopPaymentService.GetByOrderId(potencialOrder.ShopOrders_ID).Obj
									.Select(s => s.ShopPayment_PaymentId).Contains(id)
									? potencialOrder
									: null;


						var joined = potencialOrder?.ShopOrders_ShopPaymentByOrder.Join(potencialOrder.ShopOrders_ShopOrderItems,
							payment => payment.ShopPayment_OrderItemId, items => items.ShopOrderItems_ID,
							(payment, items) => new { payment, items }).ToList();

						List<Tuple<string, string, decimal, string>> dictCurrensies = new List<Tuple<string, string, decimal, string>>();

						var groupedByMerchAndCurrency = joined.GroupBy(gb => gb.payment.ShopPayment_PaymentId);
						foreach (var merchantAndCcy in groupedByMerchAndCurrency)
						{
							var currRec = joined.Where(f => f.payment.ShopPayment_PaymentId == merchantAndCcy.Key).ToList();
							var items = currRec.Select(s => s.items).ToList();
							dictCurrensies.Add(new Tuple<string, string, decimal, string>(currRec.FirstOrDefault()?.payment.ShopPayment_PaymentNumber, items.FirstOrDefault()?.ShopOrderItems_CurrencyCode,
								items.Where(w => w.ShopOrderItems_CurrencyCode == items.FirstOrDefault()?.ShopOrderItems_CurrencyCode).Select(s =>
									s.ShopOrderItems_Price * Convert.ToDecimal(s.ShopOrderItems_Quantity)).Sum(), currRec.FirstOrDefault()?.payment.ShopPayment_PaymentId?.ToString() ?? ""));
						}

						ViewBag.TotalCost = dictCurrensies;
					}
				}

				return View(model);
			}
			model.Errors.Add(new ErrorViewModel
			{
				Message = "Payment doesn't exist or you doesn't have permission",
				Type = ErrorType.Error,
				Code = 0,
				MessageDetails = "",
				FieldName = "",
				FieldValue = ""
			});
			return View(model);

			//if (model.FromCustomerName == AppSecurity.CurrentUser.OrganisationName)
			//else return RedirectToAction("History");
		}

		[HttpPost]

		public ActionResult SendMail(UserMailModel um)
		{
			ContactUsRepository cus = new ContactUsRepository();
			string sendMailRes = "";
			//if(cus.SendMail(0, AppSecurity.CurrentUser.EmailAddress, String.Format("{0} {1}", AppSecurity.CurrentUser.FirstName, AppSecurity.CurrentUser.LastName),  mailTo, "Payment Details", mailText, null, "Payment Details", out sendMailRes))
			//{

			//    return Json(new {res=true, mess = sendMailRes}, JsonRequestBehavior.DenyGet);
			//}
			//else
			//{
			return Json(new { res = false, mess = sendMailRes }, JsonRequestBehavior.DenyGet);
			//}
		}

		[HttpPost]
		public ActionResult GetImageLink(Guid id, string res)
		{
			var model = new InstantPaymentDetailsViewModel(id);
			model.PrepareDetails();
			string imageId;
			DateTime dt;
			PaymentRepository pr = new PaymentRepository();
			if (!Directory.Exists(Server.MapPath("~/UserFiles/PaymentDetails")))
			{
				Directory.CreateDirectory(Server.MapPath("~/UserFiles/PaymentDetails"));
			}
			var checkPrevPhoto = pr.GetSharedPhoto(id.ToString(), out imageId, out dt);
			if (checkPrevPhoto != null && Convert.ToBoolean(checkPrevPhoto.IsSuccess) && Guid.Parse(imageId) != Guid.Empty && dt != new DateTime())
			{
				if (dt < DateTime.Now)
					System.IO.File.Delete(String.Format("{0}/{1}.png", Server.MapPath("~/UserFiles/PaymentDetails"),
						imageId));

				else if (System.IO.File.Exists(Server.MapPath("~/UserFiles/PaymentDetails/" + imageId + ".png")))
				{
					return Json(imageId, JsonRequestBehavior.DenyGet);
				}
			}
			var createRecRes = pr.AddOrUpdateSharedPhoto(id.ToString(), out imageId);
			if (createRecRes.IsSuccess != null && Convert.ToBoolean(createRecRes.IsSuccess))
			{
				using (Bitmap image = new Bitmap(440, 530))
				{
					using (Graphics g = Graphics.FromImage(image))
					{
						SolidBrush backBrush = new SolidBrush(Color.FromArgb(253, 253, 253));
						SolidBrush headColorBrush = new SolidBrush(Color.FromArgb(0, 131, 193));
						SolidBrush mainColorBrush = new SolidBrush(Color.FromArgb(51, 51, 51));
						SolidBrush smothColorBrush = new SolidBrush(Color.FromArgb(249, 249, 249));
						g.FillRectangle(backBrush, new Rectangle(0, 0, 440, 530));

						Image logo = Image.FromFile(Server.MapPath("~/Content/assets/images/winstantpay-logo-notag.png"));
						logo = logo.ResizeImage(90, 30);
						g.DrawImage(logo, new Point(420 - logo.Width, 510 - logo.Height));

						#region Draw horiz line

						g.FillRectangle(headColorBrush, new Rectangle(20, 60, 250, 2));

						#endregion

						#region Draw Head Text

						string instPayText = GlobalRes.Payment_PaymentController_GetImageLink_PaymentDetails;
						Font drawHeadFont = new Font("Open Sans", 22);
						Font drawBoldFont = new Font("Open Sans", 12, FontStyle.Bold);
						Font drawRegFont = new Font("Open Sans", 10);
						PointF stringPonit = new PointF(20, 20);
						g.DrawString(instPayText, drawHeadFont, mainColorBrush, stringPonit,
							StringFormat.GenericTypographic);

						#endregion

						Pen recPen = new Pen(Color.FromArgb(221, 221, 221));
						//-------------------------------------------------------------------//
						g.DrawRectangle(recPen, new Rectangle(20, 70, 200, 40));
						g.FillRectangle(smothColorBrush, new Rectangle(21, 71, 199, 39));
						g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_PaymentReference, drawBoldFont, mainColorBrush, new PointF(25, 78),
							StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(20, 110, 200, 40));
						g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_Status, drawBoldFont, mainColorBrush, new PointF(25, 118),
							StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(20, 150, 200, 40));
						g.FillRectangle(smothColorBrush, new Rectangle(21, 151, 199, 39));
						g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_From, drawBoldFont, mainColorBrush, new PointF(25, 158),
							StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(20, 190, 200, 40));
						g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_To, drawBoldFont, mainColorBrush, new PointF(25, 198),
							StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(20, 230, 200, 40));
						g.FillRectangle(smothColorBrush, new Rectangle(21, 231, 199, 39));
						g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_Date, drawBoldFont, mainColorBrush, new PointF(25, 238),
							StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(20, 270, 200, 40));
						g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_Currency, drawBoldFont, mainColorBrush, new PointF(25, 278),
							StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(20, 310, 200, 40));
						g.FillRectangle(smothColorBrush, new Rectangle(21, 311, 199, 39));
						g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_Amount, drawBoldFont, mainColorBrush, new PointF(25, 318),
							StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(20, 350, 200, 40));
						g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_Invoice, drawBoldFont, mainColorBrush, new PointF(25, 358),
							StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(20, 390, 200, 40));
						g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_ReasonForPayment, drawBoldFont, mainColorBrush, new PointF(25, 398),
							StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(20, 430, 200, 40));
						g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_Memo, drawBoldFont, mainColorBrush, new PointF(25, 438),
							StringFormat.GenericTypographic);
						//------------------------------------------------------------------------------------------------//

						g.FillRectangle(smothColorBrush, new Rectangle(221, 71, 199, 38));
						g.DrawRectangle(recPen, new Rectangle(220, 70, 199, 40));
						g.DrawString(!String.IsNullOrEmpty(model.PaymentReference) ? model.PaymentReference : "", drawRegFont, mainColorBrush,
							new RectangleF(225, 70, 199, 40), StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(220, 110, 199, 40));
						g.DrawString(!String.IsNullOrEmpty(model.PaymentStatus) ? model.PaymentStatus : "", drawRegFont, mainColorBrush,
							new RectangleF(225, 110, 199, 40), StringFormat.GenericTypographic);

						g.FillRectangle(smothColorBrush, new Rectangle(221, 151, 199, 38));
						g.DrawRectangle(recPen, new Rectangle(220, 150, 199, 40));
						g.DrawString(!String.IsNullOrEmpty(model.FromCustomerAlias) ? model.FromCustomerAlias : "", drawRegFont, mainColorBrush,
							new RectangleF(225, 150, 199, 40), StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(220, 190, 199, 40));
						g.DrawString(!String.IsNullOrEmpty(model.ToCustomerAlias) ? model.ToCustomerAlias : "", drawRegFont, mainColorBrush,
							new RectangleF(225, 190, 199, 40), StringFormat.GenericTypographic);

						g.FillRectangle(smothColorBrush, new Rectangle(221, 231, 199, 38));
						g.DrawRectangle(recPen, new Rectangle(220, 230, 199, 40));
						g.DrawString(model.CreatedTime != default(DateTime) ? model.CreatedTime.ToString(CultureInfo.InvariantCulture) : "", drawRegFont, mainColorBrush,
							new RectangleF(225, 230, 199, 40), StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(220, 270, 199, 40));
						g.DrawString(!String.IsNullOrEmpty(model.Currency) ? model.Currency : "", drawRegFont, mainColorBrush, new RectangleF(225, 270, 199, 40),
							StringFormat.GenericTypographic);

						g.FillRectangle(smothColorBrush, new Rectangle(221, 311, 199, 40));
						g.DrawRectangle(recPen, new Rectangle(220, 310, 199, 40));
						if (model.Currency != "BTC")
						{
							g.DrawString(model.Amount != default(decimal) ? model.Amount.ToString("N2", CultureInfo.GetCultureInfo("en-US")) : "", drawRegFont, mainColorBrush,
								new RectangleF(225, 310, 199, 40), StringFormat.GenericTypographic);
						}
						else
						{
							g.DrawString(model.Amount.ToString("N8", CultureInfo.GetCultureInfo("en-US")), drawRegFont, mainColorBrush,
								new RectangleF(225, 310, 199, 40), StringFormat.GenericTypographic);
						}
						//g.DrawString(model.Amount != default(decimal) ? model.Amount.ToString(CultureInfo.InvariantCulture):"", drawRegFont, mainColorBrush,
						//    new RectangleF(225, 310, 199, 40), StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(220, 350, 199, 40));
						g.DrawString(!String.IsNullOrEmpty(model.Invoice) ? model.Invoice : "", drawRegFont, mainColorBrush, new RectangleF(225, 350, 199, 40), StringFormat.GenericTypographic);

						g.FillRectangle(smothColorBrush, new Rectangle(221, 391, 199, 40));
						g.DrawRectangle(recPen, new Rectangle(220, 390, 199, 90));
						g.DrawString(!String.IsNullOrEmpty(model.ReasonForPayment) ? model.ReasonForPayment : "", drawRegFont, mainColorBrush, new RectangleF(225, 390, 199, 40), StringFormat.GenericTypographic);

						g.DrawRectangle(recPen, new Rectangle(220, 430, 199, 40));
						g.DrawString(!String.IsNullOrEmpty(model.BankMemo) ? model.BankMemo : "", drawRegFont, mainColorBrush, new RectangleF(225, 430, 199, 40), StringFormat.GenericTypographic);
					}
					MemoryStream ms = new MemoryStream();

					image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
					image.Save(String.Format("{0}/{1}.png", Server.MapPath("~/UserFiles/PaymentDetails"), imageId),
						System.Drawing.Imaging.ImageFormat.Png);

				}
				return Json(imageId, JsonRequestBehavior.DenyGet);
			}
			return Json("", JsonRequestBehavior.DenyGet);
		}

		[HttpPost]
		public ActionResult Details(InstantPaymentDetailsViewModel model, Guid id, string res, string ck)
		{
			InstantPaymentDetailsViewModel modelReal = new InstantPaymentDetailsViewModel(id);
			modelReal.PrepareDetails();
			string imageId;
			DateTime dt;
			PaymentRepository pr = new PaymentRepository();

			if (!Directory.Exists(Server.MapPath("~/UserFiles/PaymentDetails")))
			{
				Directory.CreateDirectory(Server.MapPath("~/UserFiles/PaymentDetails"));
			}
			var checkPrevPhoto = pr.GetSharedPhoto(id.ToString(), out imageId, out dt);
			if (checkPrevPhoto.IsSuccess != null && Convert.ToBoolean(checkPrevPhoto.IsSuccess) && Guid.Parse(imageId) != Guid.Empty && dt != new DateTime())
			{
				if (dt < DateTime.Now)
					System.IO.File.Delete(String.Format("{0}/{1}.png", Server.MapPath("~/UserFiles/PaymentDetails"),
						imageId));
				else if (System.IO.File.Exists(Server.MapPath("~/UserFiles/PaymentDetails/" + imageId + ".png")))
				{
					MemoryStream ms = new MemoryStream();
					Image i = Image.FromFile(Server.MapPath("~/UserFiles/PaymentDetails/" + imageId + ".png"));
					i.Save(ms, ImageFormat.Png);
					return File(ms.ToArray(), "application/octetstream",
						String.Format("{0}.png", modelReal.PaymentReference));
				}
			}

			using (Bitmap image = new Bitmap(440, 530))
			{
				using (Graphics g = Graphics.FromImage(image))
				{
					SolidBrush backBrush = new SolidBrush(Color.FromArgb(253, 253, 253));
					SolidBrush headColorBrush = new SolidBrush(Color.FromArgb(0, 131, 193));
					SolidBrush mainColorBrush = new SolidBrush(Color.FromArgb(51, 51, 51));
					SolidBrush smothColorBrush = new SolidBrush(Color.FromArgb(249, 249, 249));
					g.FillRectangle(backBrush, new Rectangle(0, 0, 440, 530));

					Image logo = Image.FromFile(Server.MapPath("~/Content/assets/images/winstantpay-logo-notag.png"));
					logo = logo.ResizeImage(90, 30);
					g.DrawImage(logo, new Point(420 - logo.Width, 510 - logo.Height));

					#region Draw horiz line

					g.FillRectangle(headColorBrush, new Rectangle(20, 60, 250, 2));

					#endregion

					#region Draw Head Text

					string instPayText = GlobalRes.Payment_PaymentController_GetImageLink_PaymentDetails;
					Font drawHeadFont = new Font("Open Sans", 22);
					Font drawBoldFont = new Font("Open Sans", 12, FontStyle.Bold);
					Font drawRegFont = new Font("Open Sans", 10);
					PointF stringPonit = new PointF(20, 20);
					g.DrawString(instPayText, drawHeadFont, mainColorBrush, stringPonit,
						StringFormat.GenericTypographic);

					#endregion

					Pen recPen = new Pen(Color.FromArgb(221, 221, 221));
					//-------------------------------------------------------------------//
					g.DrawRectangle(recPen, new Rectangle(20, 70, 200, 40));
					g.FillRectangle(smothColorBrush, new Rectangle(21, 71, 199, 39));
					g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_PaymentReference, drawBoldFont, mainColorBrush, new PointF(25, 78),
						StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(20, 110, 200, 40));
					g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_Status, drawBoldFont, mainColorBrush, new PointF(25, 118),
						StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(20, 150, 200, 40));
					g.FillRectangle(smothColorBrush, new Rectangle(21, 151, 199, 39));
					g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_From, drawBoldFont, mainColorBrush, new PointF(25, 158),
						StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(20, 190, 200, 40));
					g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_To, drawBoldFont, mainColorBrush, new PointF(25, 198),
						StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(20, 230, 200, 40));
					g.FillRectangle(smothColorBrush, new Rectangle(21, 231, 199, 39));
					g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_Date, drawBoldFont, mainColorBrush, new PointF(25, 238),
						StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(20, 270, 200, 40));
					g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_Currency, drawBoldFont, mainColorBrush, new PointF(25, 278),
						StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(20, 310, 200, 40));
					g.FillRectangle(smothColorBrush, new Rectangle(21, 311, 199, 39));
					g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_Amount, drawBoldFont, mainColorBrush, new PointF(25, 318),
						StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(20, 350, 200, 40));
					g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_Invoice, drawBoldFont, mainColorBrush, new PointF(25, 358),
						StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(20, 390, 200, 40));
					g.FillRectangle(smothColorBrush, new Rectangle(21, 391, 199, 39));
					g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_ReasonForPayment, drawBoldFont, mainColorBrush, new PointF(25, 398),
						StringFormat.GenericTypographic);
					g.DrawRectangle(recPen, new Rectangle(20, 430, 200, 40));
					g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_Memo, drawBoldFont, mainColorBrush, new PointF(25, 438),
						StringFormat.GenericTypographic);

					//------------------------------------------------------------------------------------------------//

					g.FillRectangle(smothColorBrush, new Rectangle(221, 71, 199, 38));
					g.DrawRectangle(recPen, new Rectangle(220, 70, 199, 40));
					g.DrawString(modelReal.PaymentReference, drawRegFont, mainColorBrush,
						new RectangleF(225, 70, 199, 40), StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(220, 110, 199, 40));
					g.DrawString(modelReal.PaymentStatus, drawRegFont, mainColorBrush,
						new RectangleF(225, 110, 199, 40), StringFormat.GenericTypographic);

					g.FillRectangle(smothColorBrush, new Rectangle(221, 151, 199, 38));
					g.DrawRectangle(recPen, new Rectangle(220, 150, 199, 40));
					g.DrawString(modelReal.FromCustomerAlias, drawRegFont, mainColorBrush,
						new RectangleF(225, 150, 199, 40), StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(220, 190, 199, 40));
					g.DrawString(modelReal.ToCustomerAlias, drawRegFont, mainColorBrush,
						new RectangleF(225, 190, 199, 40), StringFormat.GenericTypographic);

					g.FillRectangle(smothColorBrush, new Rectangle(221, 231, 199, 38));
					g.DrawRectangle(recPen, new Rectangle(220, 230, 199, 40));
					g.DrawString(modelReal.CreatedTime.ToString(), drawRegFont, mainColorBrush,
						new RectangleF(225, 230, 199, 40), StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(220, 270, 199, 40));
					g.DrawString(modelReal.Currency, drawRegFont, mainColorBrush, new RectangleF(225, 270, 199, 40),
						StringFormat.GenericTypographic);

					g.FillRectangle(smothColorBrush, new Rectangle(221, 311, 199, 40));
					g.DrawRectangle(recPen, new Rectangle(220, 310, 199, 40));
					if (modelReal.Currency != "BTC")
					{
						g.DrawString(modelReal.Amount.ToString("N2", CultureInfo.GetCultureInfo("en-US")), drawRegFont, mainColorBrush,
						new RectangleF(225, 310, 199, 40), StringFormat.GenericTypographic);
					}
					else
					{
						g.DrawString(modelReal.Amount.ToString("N8", CultureInfo.GetCultureInfo("en-US")), drawRegFont, mainColorBrush,
							new RectangleF(225, 310, 199, 40), StringFormat.GenericTypographic);
					}

					g.DrawRectangle(recPen, new Rectangle(220, 350, 199, 40));
					g.DrawString(modelReal.Invoice, drawRegFont, mainColorBrush, new RectangleF(225, 350, 199, 40),
						StringFormat.GenericTypographic);

					g.FillRectangle(smothColorBrush, new Rectangle(221, 391, 199, 40));
					g.DrawRectangle(recPen, new Rectangle(220, 390, 199, 40));
					g.DrawString(modelReal.ReasonForPayment, drawRegFont, mainColorBrush, new RectangleF(225, 390, 199, 40),
						StringFormat.GenericTypographic);

					g.DrawRectangle(recPen, new Rectangle(220, 430, 199, 40));
					g.DrawString(modelReal.BankMemo, drawRegFont, mainColorBrush, new RectangleF(225, 430, 199, 40),
						StringFormat.GenericTypographic);
				}
				MemoryStream ms = new MemoryStream();

				image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
				if (res == "file")
					return File(ms.ToArray(), "application/octetstream",
						String.Format("{0}.png", modelReal.PaymentReference));
				else return File(ms.ToArray(), "image/png");
			}
		}

		public ActionResult Post(Guid id)
		{
			var model = new InstantPaymentPostResultViewModel(id);
			model.Post();
			if (!model.HasError)
			{
				var info = new InstantPaymentDetailsViewModel(id);
				info.PrepareDetails();
				_logger.Info(Newtonsoft.Json.JsonConvert.SerializeObject(info));

				Task.Run(() => TSG.UI.Main.Notifications.Notification.SendNotification(info.ToCustomerId,
					id, "Instant payment", String.Format(GlobalRes.SuccessPayment_InvoiceNotificationText, info.FromCustomerAlias)));
			}

			return View(model);
		}



		[NonAction]
		private NewInstantPaymentViewModel PrepareNewIntantPaymentViewModelByReceiveId(Guid receiveId)
		{
			var mappings = _instantPaymentReceiveMappingService.GetByInstantPaymentReceiveId(receiveId).Obj;
			var receive = _instantPaymentReceiveService.GetAll().Obj.Where(r => r.Id == receiveId).FirstOrDefault();
			var model = new NewInstantPaymentViewModel();

			if (receive != null)
			{
				model.InstantPaymentReceiveId = receive.Id;
				model.ToCustomer = receive.Alias;
				model.Amount = receive.Amount;
				model.CurrencyCode = receive.Currency;
				model.InstantPay = receive.Invoice;
				model.Memo = receive.Memo;
			}

			return model;
		}
	}
}
