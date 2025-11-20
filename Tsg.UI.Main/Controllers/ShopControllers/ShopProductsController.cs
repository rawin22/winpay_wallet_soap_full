using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Tsg.UI.Main.Methods;
using Tsg.UI.Main.Models.Attributes;
using TSG.Models.DTO.Shop;
using TSG.Models.ServiceModels.Shop;
using TSG.ServiceLayer.WinstantPayShop.Categories;
using TSG.ServiceLayer.WinstantPayShop.ShopInfoService;
using TSG.ServiceLayer.WinstantPayShop.ShopProductImagesService;
using TSG.ServiceLayer.WinstantPayShop.ShopProductService;
using WinstantPayDb;
using Role = TSG.Models.APIModels.Role;

namespace Tsg.UI.Main.Controllers
{
    [Authorize()]
    public class ShopProductsController : Controller
    {
        private readonly IShopProductService _shopProductService;
        private readonly IShopInfoService _shopInfoService;
        private readonly IShopCategoryService _shopCategoryService;
        private readonly IShopProductImagesService _shopProductImagesService;

        public ShopProductsController(IShopProductService shopProductService,
            IShopCategoryService shopCategoryService, IShopInfoService shopInfoService, IShopProductImagesService shopProductImagesService)
        {
            _shopProductService = shopProductService;
            _shopCategoryService = shopCategoryService;
            _shopInfoService = shopInfoService;
            _shopProductImagesService = shopProductImagesService;
        }


        // GET: ShopProducts
        [AuthorizeUser(Roles = Role.Admin)]
        public ActionResult Index()
        {
            return View(_shopProductService.GetAll().Obj);
        }

        // GET: ShopProducts/Create
        [AuthorizeUser(Roles = Role.Admin)]
        public ActionResult Create()
        {
            ViewBag.ListOfCategory = CommonMethods.GetAllCategories(_shopCategoryService.GetAll().Obj);
            ViewBag.ListOfShop = CommonMethods.GetAllShops(_shopInfoService.GetAll().Obj);
            ViewBag.ListOfCurrency = UiHelper.PrepareAvailableCurrenciesByService(ConfigurationManager.AppSettings["shopGetCurrencyUserLogin"], ConfigurationManager.AppSettings["shopGetCurrencyUserPassword"]);

            return View();
        }

        // POST: ShopProducts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(Roles = Role.Admin)]
        public ActionResult Create(ShopProducts shopProducts)
        {
            if (ModelState.IsValid)
            {
                var insertRes = _shopProductService.InsertUpdate(shopProducts);
                if (insertRes.Success && shopProducts.ShopingProducts_Action == "Save")
                    return RedirectToAction("Index");
                if (insertRes.Success && shopProducts.ShopingProducts_Action.Contains("Add Images"))
                    return RedirectToAction("Edit", new {id=insertRes.Obj.ShopProducts_ID});

            }
            ViewBag.ListOfCategory = CommonMethods.GetAllCategories(_shopCategoryService.GetAll().Obj.Where(w=>!w.ShopCategories_IsDeleted).ToList());
            ViewBag.ListOfShop = CommonMethods.GetAllShops(_shopInfoService.GetAll().Obj.Where(w=>!w.ShopInfo_IsDeleted).ToList());
            ViewBag.ListOfCurrency = UiHelper.PrepareAvailableCurrenciesByService(ConfigurationManager.AppSettings["shopGetCurrencyUserLogin"], ConfigurationManager.AppSettings["shopGetCurrencyUserPassword"]);
            
            return View(shopProducts);
        }

        // GET: ShopProducts/Edit/id
        [AuthorizeUser(Roles = Role.Admin)]
        public ActionResult Edit(Guid id)
        {
            var res = _shopProductService.GetById(id);
            if (res.Success && res.Obj != null)
            {
                ViewBag.ListOfCategory = CommonMethods.GetAllCategories(_shopCategoryService.GetAll().Obj);

                ViewBag.ListOfShop = CommonMethods.GetAllShops(_shopInfoService.GetAll().Obj);
                res.Obj.ShopProducts_ShopProductImages = new List<ShopProductImages>();
                res.Obj.ShopProducts_ShopProductImages =
                    _shopProductImagesService.GetImagesByProductId(res.Obj.ShopProducts_ID).Obj;
                ViewBag.ListOfCurrency = UiHelper.PrepareAvailableCurrenciesByService(ConfigurationManager.AppSettings["shopGetCurrencyUserLogin"], ConfigurationManager.AppSettings["shopGetCurrencyUserPassword"]);

                return View(res.Obj);
            }

            if(res.Success && res.Obj == null)
                return HttpNotFound();
            
            return RedirectToAction("Index");
        }

        // POST: ShopProducts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(Roles = Role.Admin)]
        public ActionResult Edit(ShopProducts shopProducts)
        {
            if (ModelState.IsValid)
            {
                var updateRes = _shopProductService.InsertUpdate(shopProducts);
                if (updateRes.Success && shopProducts.ShopingProducts_Action == "Save")
                    return RedirectToAction("Index");
                if (updateRes.Success && shopProducts.ShopingProducts_Action.Contains("Continue"))
                    return RedirectToAction("Edit", new { id = updateRes.Obj.ShopProducts_ID });
            }
            ViewBag.ListOfCategory = CommonMethods.GetAllCategories(_shopCategoryService.GetAll().Obj, shopProducts.ShopProducts_ID);
            ViewBag.ListOfShop = CommonMethods.GetAllShops(_shopInfoService.GetAll().Obj, shopProducts.ShopProducts_ID);
            ViewBag.ListOfCurrency = UiHelper.PrepareAvailableCurrenciesByService(ConfigurationManager.AppSettings["shopGetCurrencyUserLogin"], ConfigurationManager.AppSettings["shopGetCurrencyUserPassword"]);

            shopProducts.ShopProducts_ShopProductImages = _shopProductImagesService.GetImagesByProductId(shopProducts.ShopProducts_ID).Obj;

            return View(shopProducts);
        }

        [HttpPost]
        public ActionResult GetProductDetailsModal(Guid productId)
        {
            var shopProducts = _shopProductService.GetById(productId);
            shopProducts.Obj.ShopProducts_ShopProductImages = _shopProductImagesService.GetImagesByProductId(productId).Obj;            
            return PartialView("~/Views/ShopWindow/_ProductModal.cshtml", shopProducts.Obj);
        }
    }
}
