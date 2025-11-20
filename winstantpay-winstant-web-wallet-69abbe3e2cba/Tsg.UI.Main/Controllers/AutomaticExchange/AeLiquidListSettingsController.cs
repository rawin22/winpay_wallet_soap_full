using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Tsg.UI.Main.ApiMethods.FavoriteCurrency;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Attributes;
using TSG.Models.APIModels;
using TSG.Models.ServiceModels.AutomaticExchange;
using TSG.ServiceLayer.AutomaticExchange.DependencyLiquidForUserServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidCcyListServiceMethods;

namespace Tsg.UI.Main.Controllers.AutomaticExchange
{
    [AuthorizeUser(Roles = Role.Admin)]
    public class AeLiquidListSettingsController : Controller
    {
        private readonly IDependencyLiquidForUserServiceMethods _dependencyLiquidForUserService;
        private readonly ILiquidCcyListServiceMethods _liquidCcyListServiceMethods;

        public AeLiquidListSettingsController(ILiquidCcyListServiceMethods liquidCcyListServiceMethods, IDependencyLiquidForUserServiceMethods dependencyLiquidForUserService)
        {
            _liquidCcyListServiceMethods = liquidCcyListServiceMethods;
            _dependencyLiquidForUserService = dependencyLiquidForUserService;
        }


        //[Route("{lang?}/liquids/settings/")]
        [Route("liquids/settings/")]
        public ActionResult Index()
        {
            var currencies = new FavoriteCurrencyMethods( new TSG.Models.APIModels.UserInfo()
            {
                UserId = ConfigurationManager.AppSettings["cryptoCallerId"],
                UserName = ConfigurationManager.AppSettings["cryptoUser"],
                Password = ConfigurationManager.AppSettings["cryptoPassword"]
            }).PrepareCurrencies();
            var liquids = _liquidCcyListServiceMethods.GetAll();
            int i = 0;

            string liquidationCcys = ConfigurationManager.AppSettings["automaticExchangeCcys"];

            // just run if liquids is empty
            if(currencies != null) {
                if (!String.IsNullOrEmpty(liquidationCcys) && liquids.Obj.Count == 0)
                {
                    // need set default ccy
                    var listCcys = liquidationCcys.Split(',');
                    foreach (var listCcy in listCcys)
                    {
                        var currency = currencies.FirstOrDefault(f => f.CurrencyCode == listCcy);
                        if (currency != null &&
                            liquids.Obj.All(a => a.LiquidCcyList_CurrencyCode != currency.CurrencyCode))
                        {
                            _liquidCcyListServiceMethods.Insert(
                                new LiquidCcyListSo()
                                {
                                    LiquidCcyList_CurrencyCode = currency.CurrencyCode,
                                    //LiquidCcyList_CurrencyId = currency.CurrencyId,
                                    LiquidCcyList_IsLiquidCurrency = true,
                                    LiquidCcyList_LiquidOrder = i++
                                });
                        }
                    }
                    liquids = _liquidCcyListServiceMethods.GetAll();
                }
                foreach (var currency in currencies.OrderBy(ob => ob.CurrencyCode))
                {
                    if (liquids.Obj.All(a => a.LiquidCcyList_CurrencyCode != currency.CurrencyCode))
                    {
                        _liquidCcyListServiceMethods.Insert(
                            new LiquidCcyListSo()
                            {
                                LiquidCcyList_CurrencyCode = currency.CurrencyCode,
                                //LiquidCcyList_CurrencyId = currency.CurrencyId,
                                LiquidCcyList_IsLiquidCurrency = false,
                                LiquidCcyList_LiquidOrder = i++
                            });
                    }
                }
            }
            var liquidsTotal = _liquidCcyListServiceMethods.GetAll();

            return View(liquidsTotal.Obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="liquidId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveOrderForLiquid(List<Guid> liquidId)
        {
            var response = new StandartResponse();
            if (liquidId != null)
            {
                var stringLiquid = string.Join(",", liquidId);
                var res = _liquidCcyListServiceMethods.BulkUpdateOrder(stringLiquid);
                response = new StandartResponse(res.Success, res.Message);
            }
            return Json(response);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="liquidId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SwitchLiquidCcy(Guid liquidId, bool state)
        {
            var response = new StandartResponse(false, "Empty result");
            if (liquidId != default)
            {
                var currLiquid = _liquidCcyListServiceMethods.GetById(liquidId);
                if (currLiquid.Success && currLiquid.Obj != null)
                {
                    currLiquid.Obj.LiquidCcyList_IsLiquidCurrency = state;
                    
                    var resUpdate = _liquidCcyListServiceMethods.Update(currLiquid.Obj);
                    response = new StandartResponse(resUpdate.Success, resUpdate.Message);

                    // Add new liquid currency for all registated user
                    if (state && response.Success)
                    {
                        var depQuery = _dependencyLiquidForUserService.BulkInsertLiquidForUser(currLiquid.Obj.LiquidCcyList_Id);
                    }
                    // Delete new liquid currency for all registated user
                    else if (!state && response.Success)
                    {
                        var depQuery = _dependencyLiquidForUserService.DeleteAllByLiquidCcyCode(currLiquid.Obj.LiquidCcyList_Id);
                    }
                }
            }
            return Json(response);
        }
    }
}