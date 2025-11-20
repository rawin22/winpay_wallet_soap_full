using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using Tsg.UI.Main.Models;

namespace Tsg.UI.Main.Controllers
{
	[AllowAnonymous]
	public class SharedViewerController : BaseController
	{
		readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		// GET: SharedViewer
		//public ActionResult Index()
		//{
		//    return View();
		//}
		[AllowAnonymous]
		public ActionResult PaymentDetails(Guid? photoId)
		{
			var model = new SharedViewPhoto();
			model.ImagePath = String.Format("{0}", "~/Content/assets/images/errorimage.png");
			if (photoId != null && photoId != Guid.Empty)
			{
				if (System.IO.File.Exists(String.Format("{0}/{1}.png", Server.MapPath("~/UserFiles/PaymentDetails"),
					photoId)))
				{
					model.ImagePath = String.Format("{0}/{1}.png", "~/UserFiles/PaymentDetails", photoId);
				}
			}


			return View(model);
		}

		[AllowAnonymous]
		public ActionResult ConvertDetails(Guid? photoId)
		{
			_logger.DebugFormat("photoId: {0}", photoId);
			var model = new SharedViewPhoto();
			try
			{
				//var model = new SharedViewPhoto();
				model.ImagePath = String.Format("{0}", "~/Content/assets/images/errorimage.png");
				if (photoId != null && photoId != Guid.Empty)
				{
					var imagePath = String.Format("{0}\\{1}.png", Server.MapPath("~/UserFiles/ConvertDetails"), photoId);
					_logger.DebugFormat("imagePath: {0}", imagePath);

					_logger.DebugFormat("File exists: {0}", System.IO.File.Exists(imagePath));
					//_logger.DebugFormat("File exists: {0}", System.IO.File.Exists(String.Format("{0}/{1}.png", Server.MapPath("~/UserFiles/ConvertDetails"), photoId)));
					//if (System.IO.File.Exists(String.Format("{0}/{1}.png", Server.MapPath("~/UserFiles/ConvertDetails"), photoId)))
					if (System.IO.File.Exists(imagePath))
					{
						_logger.DebugFormat("File exists");
						//model.ImagePath = imagePath;
						model.ImagePath = String.Format("{0}/{1}.png", "/UserFiles/ConvertDetails", photoId);
						//model.ImagePath = String.Format("{0}/{1}.png", "~/UserFiles/PaymentDetails", photoId);
						_logger.DebugFormat("model.ImagePath: {0}", model.ImagePath);
					}
				}

				_logger.DebugFormat("Returning view model: {0}", JsonConvert.SerializeObject(model));
				return View(model);
			}
			catch (Exception exeption)
			{
				_logger.ErrorFormat("Exception: {0}", JsonConvert.SerializeObject(exeption));
			}

			return View(model);
		}

	}
}