using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tsg.Business.Model.Classes;
using TSG.Models.APIModels;
using Tsg.UI.Main.Models.Attributes;
using Tsg.UI.Main.Models.Pipit;
using Tsg.UI.Main.Models.Security;
using TSG.Models.APIModels.Fundings.Pipit;
using TSG.ServiceLayer.Interfaces.Fundings;

namespace Tsg.UI.Main.Controllers
{
    [Authorize]
    public class PipitController : BaseController
    {
        private IgpService _service;
        private List<HttpCookie> cookies;
        private List<SelectListItem> _currencyList = new List<SelectListItem>();
        private readonly IFundingsService _fundingsService;

        public PipitController()
        {
            try
            {
                _service = new IgpService(AppSecurity.CurrentUser.UserName, AppSecurity.CurrentUser.Password,
                    AppSecurity.CurrentUser.UserId);
                cookies = _service.LoginInPipIt();
                _currencyList.Add(new SelectListItem() { Value = "EUR", Text = "EUR" });
                _currencyList.Add(new SelectListItem() { Value = "GBP", Text = "GBP" });
                _currencyList.Add(new SelectListItem() { Value = "CAD", Text = "CAD" });
            }
            catch (Exception e)
            { }
        }

        public PipitController(IFundingsService fundingsService):this()
        {
            _fundingsService = fundingsService;
        }

        // GET: Pipit
        [HttpGet]
        [AuthorizeUser(Roles = Role.Admin)]
        public ActionResult Index()
        {
            List<PipitContent> listOfOrders = new List<PipitContent>();

            if (_service != null)
            {
                foreach (var cookie in cookies)
                {
                    Session[cookie.Name] = cookie.Value;
                    HttpContext.Response.Cookies.Add(cookie);
                }
                var cook = cookies.FirstOrDefault();
                if (cook != null)
                {
                    var expiredOrdersString = _service.GetOrdersByType(cook, "expired");
                    var expiredOrders = JsonConvert.DeserializeObject<PipitRootObject>(expiredOrdersString);
                    var paidOrdersString = _service.GetOrdersByType(cook, "paid");
                    var paidOrders = JsonConvert.DeserializeObject<PipitRootObject>(paidOrdersString);
                    var pendingOrdersString = _service.GetOrdersByType(cook, "pending");
                    var pendingOrders = JsonConvert.DeserializeObject<PipitRootObject>(pendingOrdersString);
                    //expiredOrders.content;
                    if(expiredOrders.Content != null)
                        listOfOrders.AddRange(expiredOrders.Content);
                    if (paidOrders.Content != null)
                        listOfOrders.AddRange(paidOrders.Content);
                    if (pendingOrders.Content != null)
                        listOfOrders.AddRange(pendingOrders.Content);
                }
            }

            return View(listOfOrders);
        }

        [HttpGet]
        public ActionResult CreateOrder()
        {
            ViewBag.ListOfCurrency = _currencyList;
            foreach (var cookie in cookies)
            {
                Session[cookie.Name] = cookie.Value;
                HttpContext.Response.Cookies.Add(cookie);
            }
            PipitNewOrder model = new PipitNewOrder()
            {
                CustomerEmail = AppSecurity.CurrentUser.EmailAddress,
                VendorName = ConfigurationManager.AppSettings["pipitLogin"],
                CustomerPhone = AppSecurity.CurrentUser.Phone
            };
            ViewBag.Order = null;
            ViewBag.Error = null;

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateOrder(PipitNewOrder model)
        {
            var cook = cookies.FirstOrDefault();
            ViewBag.ListOfCurrency = _currencyList;
            foreach (var cookie in cookies)
            {
                Session[cookie.Name] = cookie.Value;
                HttpContext.Response.Cookies.Add(cookie);
            }
#if DEBUG
            model.VendorOrderReference = $"TSGroup-{Guid.NewGuid()}[DevMode]";
#else
            model.VendorOrderReference = $"TSGroup-{Guid.NewGuid()}";
#endif
            var createdOrderString = _service.PostCreateOrders(cook, JsonConvert.SerializeObject(model));
            if (createdOrderString.Contains("INVALID_ORDER"))
            {
                ViewBag.Error = JsonConvert.DeserializeObject<ApiPipItCreateFundingModel.PipItCreateOrderException>(createdOrderString);
                ViewBag.Order = null;
            }
            else
            {
                var expiredOrders = JsonConvert.DeserializeObject<TSG.Models.ServiceModels.AddFundsPipit>(createdOrderString);
                expiredOrders.AddFundsPipit_Alias = model.Alias;
                if (_fundingsService.InsertPipitTransfer(expiredOrders, AppSecurity.CurrentUser.UserName).Success)
                {
                    ViewBag.Order = expiredOrders;
                    ViewBag.Error = null;
                }
            }
            return View(model);
        }
    }
}