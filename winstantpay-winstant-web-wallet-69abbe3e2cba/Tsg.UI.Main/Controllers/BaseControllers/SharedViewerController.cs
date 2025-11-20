using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsg.UI.Main.Models;

namespace Tsg.UI.Main.Controllers
{
    [AllowAnonymous]
    public class SharedViewerController : BaseController
    {
        // GET: SharedViewer
        //public ActionResult Index()
        //{
        //    return View();
        //}
        [AllowAnonymous]
        public ActionResult PaymentDetails(Guid ? photoId)
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
            var model = new SharedViewPhoto();
            model.ImagePath = String.Format("{0}", "~/Content/assets/images/errorimage.png");
            if (photoId != null && photoId != Guid.Empty)
            {
                if (System.IO.File.Exists(String.Format("{0}/{1}.png", Server.MapPath("~/UserFiles/ConvertDetails"),
                    photoId)))
                {
                    model.ImagePath = String.Format("{0}/{1}.png", "~/UserFiles/ConvertDetails", photoId);
                }
            }


            return View(model);
        }

    }
}