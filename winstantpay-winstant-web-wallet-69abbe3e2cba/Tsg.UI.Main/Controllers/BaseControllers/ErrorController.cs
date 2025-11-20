using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tsg.UI.Main.Controllers
{
    public class ErrorController : BaseController
    {
        public ViewResult Index()
        {
            return View("Error");
        }

        public ViewResult NotFound()
        {
            Response.StatusCode = 200;
            return View("NotFound");
        }

        public ActionResult InternalServer()
        {
            Response.StatusCode = 200;
            return View("InternalServer");
        }
    }
}