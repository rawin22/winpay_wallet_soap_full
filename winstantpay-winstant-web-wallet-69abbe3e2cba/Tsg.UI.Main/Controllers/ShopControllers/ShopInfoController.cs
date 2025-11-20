using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Tsg.UI.Main.Models.Attributes;
using TSG.Models.APIModels;
using TSG.Models.ServiceModels.Shop;
using TSG.ServiceLayer.WinstantPayShop.ShopInfoService;
using WinstantPay.Common.Extension;


namespace Tsg.UI.Main.Controllers
{
    public class ShopInfoController : Controller
    {
        private readonly IShopInfoService _shopInfoService;

        public ShopInfoController(IShopInfoService shopInfoService) => _shopInfoService = shopInfoService;

        // GET: ShopInfoDtoes
        [AuthorizeUser(Roles = Role.Admin)]

        public ActionResult Index()
        {
            return View(_shopInfoService.GetAll().Obj.ToList());
        }


        // GET: ShopInfoDtoes/Create
        [AuthorizeUser(Roles = Role.Admin)]

        public ActionResult Create()
        {
            return View();
        }

        // POST: ShopInfoDtoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AuthorizeUser(Roles = Role.Admin)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ShopInfo shopInfo)
        {
            ModelState.Remove("ShopInfo_LogoPath");
            if (ModelState.IsValid)
            {
                if (shopInfo.ShopInfo_PostedFile != null && shopInfo.ShopInfo_PostedFile.ContentLength > 0)
                {
                    FilesExtensions.SaveSystemFile(shopInfo.ShopInfo_PostedFile, shopInfo.ShopInfo_Name, Server.MapPath("~/ShopFiles/ShopInfo/"), out string fileName, out string filePath);
                    shopInfo.ShopInfo_LogoPath = filePath;
                }

                if (_shopInfoService.InsertUpdate(shopInfo).Success)
                    return RedirectToAction("Index");
            }

            return View(shopInfo);
        }

        // GET: ShopInfoDtoes/Edit/5
        [AuthorizeUser(Roles = Role.Admin)]

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var shopInfo = _shopInfoService.GetById((Guid) id).Obj;
            if (shopInfo == null)
            {
                return HttpNotFound();
            }
            return View(shopInfo);
        }

        // POST: ShopInfoDtoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(Roles = Role.Admin)]

        public ActionResult Edit(ShopInfo shopInfo)
        {
            ModelState.Remove("ShopInfo_LogoPath");

            if (ModelState.IsValid)
            {
                if (shopInfo.ShopInfo_PostedFile != null && shopInfo.ShopInfo_PostedFile.ContentLength > 0)
                {
                    FilesExtensions.SaveSystemFile(shopInfo.ShopInfo_PostedFile, shopInfo.ShopInfo_Name, Server.MapPath("~/ShopFiles/ShopInfo/"), out string fileName, out string filePath);
                    shopInfo.ShopInfo_LogoPath = filePath;
                }

                if (_shopInfoService.InsertUpdate(shopInfo).Success)
                    return RedirectToAction("Index");
            }
            return View(shopInfo);
        }
    }
}
