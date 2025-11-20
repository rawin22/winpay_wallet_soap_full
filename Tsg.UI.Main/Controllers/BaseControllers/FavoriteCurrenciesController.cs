using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;

namespace Tsg.UI.Main.Controllers
{
    public class FavoriteCurrenciesController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            PaymentCurrenciesListViewModel model = new PaymentCurrenciesListViewModel();
            model.PrepareCurrencies();
            return View(model);
            
        }

        [HttpPost]
        public ActionResult AddToFavoriteList(string currencyCode)
        {   
            FavoriteCurrencyModel model = new FavoriteCurrencyModel()
            {
                IdUser = AppSecurity.CurrentUser.UserId,
                CurrencyCode = currencyCode.Replace("currency_", "")
            };
            FavoriteCurrencyRepository cr = new FavoriteCurrencyRepository();
            if(cr.GetFavoriteCurrencyList().Any(a=>a.CurrencyCode == model.CurrencyCode && a.IdUser==model.IdUser))
                return Json(new { success = true, CurrencyCode = currencyCode, message = GlobalRes.FavoriteCurrencies_FavoriteCurrenciesController_AddToFavoriteCurrency_AddSuccess }, JsonRequestBehavior.DenyGet);
            String resAct;
            var addres = cr.AddFavoriteCurrency(model, out resAct);
            if (addres)
            {
                return Json(new { success = true, CurrencyCode = currencyCode, message = GlobalRes.FavoriteCurrencies_FavoriteCurrenciesController_AddToFavoriteCurrency_AddSuccess }, JsonRequestBehavior.DenyGet);
            }
            return Json(new { success = false, CurrencyCode = currencyCode, message = GlobalRes.FavoriteCurrencies_FavoriteCurrenciesController_AddToFavoriteCurrency_AddNoSuccess }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult Delete(string currencyCode)
        {
            FavoriteCurrencyModel model = new FavoriteCurrencyModel()
            {
                IdUser = AppSecurity.CurrentUser.UserId,
                CurrencyCode = currencyCode.Replace("currency_", "")
            };
            FavoriteCurrencyRepository cr = new FavoriteCurrencyRepository();
            string resAct;
            var res = cr.DeleteFavoriteCurrency(model.CurrencyCode, model.IdUser, out resAct);
            if (res)
            {
                return Json(new { success = true, CurrencyCode = currencyCode, message = GlobalRes.FavoriteCurrencies_FavoriteCurrenciesController_DelToFavoriteCurrency_Success }, JsonRequestBehavior.DenyGet);
            }
            return Json(new { success = false, CurrencyCode = currencyCode, message = GlobalRes.FavoriteCurrencies_FavoriteCurrenciesController_DelToFavoriteCurrency_NoSuccess }, JsonRequestBehavior.DenyGet);
        }
    }
}