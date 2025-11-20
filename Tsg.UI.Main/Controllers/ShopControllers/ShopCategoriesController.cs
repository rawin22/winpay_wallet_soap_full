using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tsg.UI.Main.Methods;
using Tsg.UI.Main.Models.Attributes;
using TSG.Models.APIModels;
using TSG.Models.ServiceModels.Shop;
using TSG.ServiceLayer.WinstantPayShop.Categories;

namespace Tsg.UI.Main.Controllers
{
    public class ShopCategoriesController : Controller
    {

        private readonly IShopCategoryService _shopCategoryService;

        public ShopCategoriesController(IShopCategoryService shopCategoryService)
        {
            _shopCategoryService = shopCategoryService;
        }

        [AuthorizeUser(Roles = Role.Admin)]

        // GET: ShopCategoriesDtoes
        public ActionResult Index()
        {
            return View(_shopCategoryService.GetAll().Obj);
        }

        [AuthorizeUser(Roles = Role.Admin)]
        // GET: ShopCategoriesDtoes/Create
        public ActionResult Create()
        {
            ViewBag.ListOfCategory = CommonMethods.GetAllCategories(_shopCategoryService.GetAll().Obj);
            return View();
        }

        // POST: ShopCategoriesDtoes/Create
        [AuthorizeUser(Roles = Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ShopCategories shopCategories)
        {
            ModelState.Remove("ShopCategories_Parent");
            if (ModelState.IsValid)
            {
                shopCategories.ShopCategories_ID = default;
                if (_shopCategoryService.InsertUpdate(shopCategories).Success)
                    return RedirectToAction("Index");
            }
            ViewBag.ListOfCategory = CommonMethods.GetAllCategories(_shopCategoryService.GetAll().Obj);

            return View(shopCategories);
        }

        [AuthorizeUser(Roles = Role.Admin)]
        // GET: ShopCategoriesDtoes/Edit/5
        public ActionResult Edit(Guid id)
        {
            ShopCategories shopCategories = _shopCategoryService.GetById(id).Obj;
            if (shopCategories == null)
            {
                return HttpNotFound();
            }
            ViewBag.ListOfCategory = CommonMethods.GetAllCategories(_shopCategoryService.GetAll().Obj, id);
            return View(shopCategories);
        }

        // POST: ShopCategoriesDtoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [AuthorizeUser(Roles = Role.Admin)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ShopCategories shopCategories)
        {

            if (ModelState.IsValid)
            {
                if (_shopCategoryService.InsertUpdate(shopCategories).Success)
                    return RedirectToAction("Index");
            }
            ViewBag.ListOfCategory = CommonMethods.GetAllCategories(_shopCategoryService.GetAll().Obj, shopCategories.ShopCategories_ID);
            return View(shopCategories);
        }

        // GET: ShopCategoriesDtoes/Delete/5
        [AuthorizeUser(Roles = Role.Admin)]
        public ActionResult Delete(Guid id)
        {
            ShopCategories shopCategories = _shopCategoryService.GetById(id).Obj;

            if (shopCategories == null)
            {
                return HttpNotFound();
            }
            return View(shopCategories);
        }

        // POST: ShopCategoriesDtoes/Delete/5
        [AuthorizeUser(Roles = Role.Admin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            _shopCategoryService.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
