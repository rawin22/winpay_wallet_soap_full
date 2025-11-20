using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;
using TSG.Models.DTO.InstantPayment;
using TSG.ServiceLayer.InstantPayment;

namespace Tsg.UI.Main.Controllers
{
	[Authorize]
	public class QrGenController : BaseController
	{
		private readonly IInstantPaymentReceiveMethods _instantPaymentReceiveService;
		private readonly IInstantPaymentReceiveMappingMethods _instantPaymentReceiveMappingService;
		readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public QrGenController(IInstantPaymentReceiveMethods instantPaymentReceiveService, IInstantPaymentReceiveMappingMethods instantPaymentReceiveMappingService)
		{
			_instantPaymentReceiveService = instantPaymentReceiveService;
			_instantPaymentReceiveMappingService = instantPaymentReceiveMappingService;
		}

		// GET: QrGen
		/// <summary>
		/// Create new receive or edit existing one.
		/// </summary>
		/// <param name="id">receive ID (optional)</param>
		/// <returns></returns>
		public ActionResult Index(Guid? id)
		{
			var newReceive = new NewInstantPaymentReceiveViewModel();
			_logger.Debug(string.Format("newReceive.Currencies: {0}", JsonConvert.SerializeObject(newReceive.Currencies)));

			if (id.HasValue && id.Value != Guid.Empty)
			{
				_logger.Debug(string.Format("id: {0}", id.Value));
				var details = PrepareInstantPaymentReceiveDetails(id.Value);
				_logger.Debug(string.Format("details: {0}", JsonConvert.SerializeObject(details)));
				int currencyScale = 4;

				if (!string.IsNullOrEmpty(details.Currency))
				{
					var currency = newReceive.Currencies.First(c => c.CurrencyCode.Equals(details.Currency, StringComparison.OrdinalIgnoreCase));
					currencyScale = currency.CurrencyRateScale;
				}
				newReceive.InstantPaymentReceiveId = details.InstantPaymentReceiveId;
				newReceive.Alias = details.Alias;
				newReceive.Currency = details.Currency;
				newReceive.Amount = Math.Round(details.Amount, currencyScale);
				newReceive.Invoice = details.Invoice;
				newReceive.Memo = details.Memo;
				newReceive.Name = details.Name;
				newReceive.Address = details.Address;
				newReceive.Email = details.Email;
				newReceive.ShortenedUrl = details.ShortenedUrl;
				newReceive.AttachedFileName = details.AttachedFileName; ;
			}

			return View(newReceive);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Index(NewInstantPaymentReceiveViewModel model)
		{
			InstantPaymentReceiveDto receiveDto = new InstantPaymentReceiveDto();
			receiveDto.TsgUserGuId = new Guid(AppSecurity.CurrentUser.UserId);
			receiveDto.Alias = model.Alias;
			receiveDto.Currency = model.Currency;
			receiveDto.Amount = model.Amount;
			receiveDto.Invoice = model.Invoice;
			if (Request.Files?.Count > 0)
			{
				HttpPostedFileBase attachedFile = Request.Files[0];
				if (attachedFile.ContentLength > 0)
				{
					_logger.Debug("File exists");
					// Retrieve file details
					string fileName = attachedFile.FileName;
					string fileExtension = Path.GetExtension(fileName);
					_logger.Debug(string.Format("fileExtension: {0}", fileExtension));
					string contentType = attachedFile.ContentType;
					_logger.Debug(string.Format("contentType: {0}", contentType));
					byte[] fileBytes;
					using (var inputStream = attachedFile.InputStream)
					{
						fileBytes = new byte[inputStream.Length];
						inputStream.Read(fileBytes, 0, fileBytes.Length);
					}

					receiveDto.AttachedFile = fileBytes;
					receiveDto.AttachedFileName = fileName;
					receiveDto.AttachedFileExtension = fileExtension;
					receiveDto.AttachedFileContentType = contentType;
				}
			}

			InstantPaymentReceiveMemoViewModel receiveMemoModel = new InstantPaymentReceiveMemoViewModel();
			receiveMemoModel.Memo = model.Memo;
			receiveMemoModel.Name = model.Name;
			receiveMemoModel.Address = model.Address;
			receiveMemoModel.Email = model.Email;
			//receiveMemoModel.Email = string.IsNullOrEmpty(model.Email) ? null : model.Email;

			var settings = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			};

			var memo = JsonConvert.SerializeObject(receiveMemoModel, settings);
			_logger.Debug(message: string.Format("memo: {0}", memo));
			if (memo.Length > 2)
			{
				receiveDto.Memo = memo;
			}
			//receiveDto.Memo = JsonConvert.SerializeObject(receiveMemoModel, settings);

			_instantPaymentReceiveService.Insert(receiveDto);
			model.InstantPaymentReceiveId = receiveDto.Id;

			return Json(new
			{
				InstantPaymentReceiveId = model.InstantPaymentReceiveId
			});
		}

		/// <summary>
		/// Create new receive
		/// </summary>
		/// <param name="model">NewInstantPaymentReceiveViewModel</param>
		/// <returns></returns>
		public NewInstantPaymentReceiveViewModel CreateReceive(NewInstantPaymentReceiveViewModel model)
		{

			InstantPaymentReceiveDto receiveDto = new InstantPaymentReceiveDto();
			receiveDto.TsgUserGuId = new Guid(AppSecurity.CurrentUser.UserId);
			receiveDto.Alias = model.Alias;
			receiveDto.Currency = model.Currency;
			receiveDto.Amount = model.Amount;
			receiveDto.Invoice = model.Invoice;

			InstantPaymentReceiveMemoViewModel receiveMemoModel = new InstantPaymentReceiveMemoViewModel();
			receiveMemoModel.Memo = model.Memo;
			receiveMemoModel.Name = model.Name;
			receiveMemoModel.Address = model.Address;
			receiveMemoModel.Email = model.Email;

			receiveDto.Memo = JsonConvert.SerializeObject(receiveMemoModel);

			_instantPaymentReceiveService.Insert(receiveDto);
			model.InstantPaymentReceiveId = receiveDto.Id;

			return model;
		}

		/// <summary>
		/// Create new receive
		/// </summary>
		/// <param name="model">NewInstantPaymentReceiveViewModel</param>
		/// <returns></returns>
		public NewInstantPaymentReceiveViewModel UpdateReceive(NewInstantPaymentReceiveViewModel model)
		{
			InstantPaymentReceiveDto receiveDto = new InstantPaymentReceiveDto();
			receiveDto.Id = model.InstantPaymentReceiveId;
			receiveDto.Alias = model.Alias;
			receiveDto.Currency = model.Currency;
			receiveDto.Amount = model.Amount;
			receiveDto.Invoice = model.Invoice;

			InstantPaymentReceiveMemoViewModel receiveMemoModel = new InstantPaymentReceiveMemoViewModel();
			receiveMemoModel.Memo = model.Memo;
			receiveMemoModel.Name = model.Name;
			receiveMemoModel.Address = model.Address;
			receiveMemoModel.Email = model.Email;

			receiveDto.Memo = JsonConvert.SerializeObject(receiveMemoModel);

			_instantPaymentReceiveService.Update(receiveDto);

			return model;
		}

		/// <summary>
		/// Edit receive
		/// </summary>
		/// <param name="id">receive GUID</param>
		/// <returns></returns>
		public ActionResult Edit(Guid id)
		{
			return View(PrepareInstantPaymentReceiveDetails(id));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(NewInstantPaymentReceiveViewModel model)
		{
			try
			{
				_logger.Debug("Edit receive");
				_logger.DebugFormat("model.InstantPaymentReceiveId: {0}, currency code: {1}, short URL: {2}", model.InstantPaymentReceiveId, model.Currency, model.ShortenedUrl);
				var receiveDto = _instantPaymentReceiveService.GetAll().Obj.Where(r => r.Id == model.InstantPaymentReceiveId).FirstOrDefault();

				if (receiveDto == null)
				{
					return Json(new { success = false, responseText = "Updating Receive Failed" });
				}

				receiveDto.Alias = model.Alias;
				receiveDto.Currency = model.Currency;
				receiveDto.Amount = model.Amount;
				receiveDto.Invoice = model.Invoice;

				InstantPaymentReceiveMemoViewModel receiveMemoModel = new InstantPaymentReceiveMemoViewModel();
				receiveMemoModel.Memo = model.Memo;
				receiveMemoModel.Name = model.Name;
				receiveMemoModel.Address = model.Address;
				receiveMemoModel.Email = model.Email;
				receiveMemoModel.KycId = model.KycId;

				var settings = new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore
				};

				var memo = JsonConvert.SerializeObject(receiveMemoModel, settings);
				_logger.DebugFormat("memo: {0}", memo);
				receiveDto.Memo = null;
				if (memo.Length > 2)
				{
					receiveDto.Memo = memo;
				}

				receiveDto.ShortenedUrl = model.ShortenedUrl;

				if (Request.Files?.Count > 0)
				{
					HttpPostedFileBase attachedFile = Request.Files[0];
					if (attachedFile.ContentLength > 0)
					{
						_logger.Debug("File exists, file name:" + attachedFile.FileName);
						string fileName = attachedFile.FileName;
						string fileExtension = Path.GetExtension(fileName);
						string contentType = attachedFile.ContentType;
						byte[] fileBytes;
						using (var inputStream = attachedFile.InputStream)
						{
							fileBytes = new byte[inputStream.Length];
							inputStream.Read(fileBytes, 0, fileBytes.Length);
						}

						receiveDto.AttachedFile = fileBytes;
						receiveDto.AttachedFileName = fileName;
						receiveDto.AttachedFileExtension = fileExtension;
						receiveDto.AttachedFileContentType = contentType;
					}
				}
				_instantPaymentReceiveService.Update(receiveDto);

				return Json(new { success = true, responseText = "Updating Receive Succeeded" });
			}
			catch (Exception ex)
			{
				_logger.Error(" Receive Edit error: " + ex.Message);
				return Json(new { success = false, responseText = "Updating Receive error" });
			}
		}

		public ActionResult GenerateQrByName(string userAlias)
		{
			return Json(string.Empty);
		}

		public ActionResult History(InstantPaymentReceiveSearchViewModel model)
		{
			var model1 = new InstantPaymentReceiveSearchViewModel();
			//if (model == null)
			//    model = new InstantPaymentReceiveSearchViewModel();
			//model1 = PrepareInstantPaymentReceives(model1);
			// return View(model1);
			return View(PrepareInstantPaymentReceives(model));
		}

		public ActionResult Details(Guid id)
		{
			return View(PrepareInstantPaymentReceiveDetails(id));
		}

		/// <summary>
		/// Delete a receive
		/// </summary>
		/// <param name="id">Receive GUID</param>
		/// <returns></returns>
		public ActionResult Delete(Guid id)
		{
			_instantPaymentReceiveService.Delete(id);
			return RedirectToAction("History");
		}

		/// <summary>
		/// Get attached file data
		/// </summary>
		/// <param name="id">Receive GUID</param>
		/// <returns></returns>
		public ActionResult AttachedFileTest(Guid id)
		{

			//var receive = _instantPaymentReceiveService.GetAll().Obj.Where(r => r.Id == id).FirstOrDefault();
			//if (receive != null)
			//{
			//    return File(receive.AttachedFile, receive.AttachedFileContentType, receive.AttachedFileName);
			//}

			//// Retrieve the file bytes from the database
			byte[] fileBytes = GetAttachedFileBytes(id, out string fileName, out string contentType);
			_logger.Debug("fileName:" + fileName);
			// Set the file name
			//string fileName = "example.txt";

			// Get the file's content type
			//string contentType = MimeMapping.GetMimeMapping(fileName);
			_logger.Debug("contentType:" + contentType);
			// Return the file for download
			// return File(fileBytes, contentType, fileName);
			//return Json(new {Convert.ToBase64String(fileBytes)});
			//return Json(new { success = true, responseText = "Updating Receive Succeeded" });
			//return File(fileBytes, "application/octet-stream", fileName);

			return HttpNotFound("File not found.");
		}

		/// <summary>
		/// Download attached file data
		/// </summary>
		/// <param name="id">Receive GUID</param>
		/// <returns></returns>
		public ActionResult DownloadAttachedFile(Guid id)
		{

			var receive = _instantPaymentReceiveService.GetAll().Obj.FirstOrDefault(r => r.Id == id);
			if (receive != null)
			{
				return File(receive.AttachedFile, receive.AttachedFileContentType, receive.AttachedFileName);
			}

			return HttpNotFound("File not found.");
		}

		/// <summary>
		/// Return attached file data
		/// </summary>
		/// <param name="id">Receive GUID</param>
		/// <returns></returns>
		public ActionResult AttachedFile(Guid id)
		{

			var receive = _instantPaymentReceiveService.GetAll().Obj.FirstOrDefault(r => r.Id == id);
			if (receive != null)
			{
				//return File(receive.AttachedFile, receive.AttachedFileContentType, receive.AttachedFileName);
				//return Json(new { Convert.ToBase64String(fileBytes) });
				return Json(new { success = true, attachedFileName = receive.AttachedFileName, attachedFileExtension = receive.AttachedFileExtension, attachedFileData = Convert.ToBase64String(receive.AttachedFile) });
			}

			return HttpNotFound("File not found.");
		}

		[NonAction]
		private byte[] GetAttachedFileBytes(Guid id, out string fileName, out string contentType)
		{
			var receive = _instantPaymentReceiveService.GetAll().Obj.FirstOrDefault(r => r.Id == id);
			fileName = string.Empty;
			contentType = string.Empty;

			if (receive != null)
			{
				fileName = receive.AttachedFileName;
				contentType = receive.AttachedFileContentType;
				_logger.Debug("fileName: " + fileName + ", contentType: " + contentType + ", length: " + receive.AttachedFile.Length);
				return receive.AttachedFile;
			}

			return null;

			//model.InstantPayments = PrepareIntantPaymentsByReceiveId(instantPaymentReceiveId);

			//return model;

			//using (var dbContext = new YourDbContext())
			//{
			//    // Retrieve the file record from the database
			//    var file = dbContext.Files.FirstOrDefault();

			//    if (file != null)
			//    {
			//        // Return the file bytes
			//        return file.FileBytes;
			//    }
			//}

			//return null;
		}

		[NonAction]
		private InstantPaymentReceiveDetailsViewModel PrepareInstantPaymentReceiveDetails(Guid instantPaymentReceiveId)
		{
			var model = new InstantPaymentReceiveDetailsViewModel();

			var receive = _instantPaymentReceiveService.GetAll().Obj.Where(r => r.Id == instantPaymentReceiveId).FirstOrDefault();

			if (receive != null)
			{
				model.InstantPaymentReceiveId = receive.Id;
				model.Alias = receive.Alias;
				model.Currency = receive.Currency;
				model.Amount = receive.Amount;
				model.Invoice = receive.Invoice;
				// model.Memo = receive.Memo;
				model.CreatedDate = receive.CreatedDate;
				model.AttachedFileName = receive.AttachedFileName;
				if (!string.IsNullOrEmpty(receive.Memo))
				{
					InstantPaymentReceiveMemoViewModel memo = JsonConvert.DeserializeObject<InstantPaymentReceiveMemoViewModel>(receive.Memo);

					model.Memo = memo.Memo;
					model.Name = memo.Name;
					model.Address = memo.Address;
					model.Email = memo.Email;
				}
				model.ShortenedUrl = receive.ShortenedUrl;
			}

			model.InstantPayments = PrepareIntantPaymentsByReceiveId(instantPaymentReceiveId);

			return model;
		}

		[NonAction]
		private InstantPaymentReceiveSearchViewModel PrepareInstantPaymentReceives(InstantPaymentReceiveSearchViewModel model)
		{
			var userId = new Guid(AppSecurity.CurrentUser.UserId);
			var receives = _instantPaymentReceiveService.GetByUser(userId);
			// var model = new InstantPaymentReceiveSearchViewModel();
			//if (model == null)
			//    model = new InstantPaymentReceiveSearchViewModel();
			if (receives != null && receives.Obj != null)
			{
				foreach (var receive in receives.Obj)
				{
					InstantPaymentReceiveViewModel instantPaymentReceive = new InstantPaymentReceiveViewModel()
					{
						InstantPaymentReceiveId = receive.Id,
						Alias = receive.Alias,
						Amount = receive.Amount,
						Currency = receive.Currency,
						CreatedDate = receive.CreatedDate,
					};
					model.InstantPaymentReceives.Add(instantPaymentReceive);
				}
			}
			return model;

		}
		[NonAction]
		private IList<InstantPaymentViewModel> PrepareIntantPaymentsByReceiveId(Guid receiveId)
		{
			var mappings = _instantPaymentReceiveMappingService.GetByInstantPaymentReceiveId(receiveId).Obj;

			var instantPayments = new InstantPaymentSearchViewModel();

			if (mappings != null && mappings.Count > 0)
			{
				instantPayments.PrepareInstantPayments();
				instantPayments.Payments = instantPayments.Payments.Where(p => mappings.Any(z => p.PaymentId == z.InstantPaymentId)).ToList();
			}

			return instantPayments.Payments;

			//        var CategoriesNotOnLst = Categories
			//.Where(x => !lst.Any(z => x.Description == z))
			//.ToList();

		}

	}
}