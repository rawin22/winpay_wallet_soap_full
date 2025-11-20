using System;
using System.Linq;
using System.Web.Mvc;
using TSG.Models.APIModels;
using TSG.Models.ServiceModels.Shop;
using TSG.ServiceLayer.WinstantPayShop.ShopProductImagesService;
using WinstantPay.Common.Extension;

namespace Tsg.UI.Main.Controllers
{
    
    public class ShopProductImagesController : Controller
    {
        private readonly IShopProductImagesService _shopProductImages;

        public ShopProductImagesController(IShopProductImagesService shopProductImages)
        {
            _shopProductImages = shopProductImages;
        }

        [HttpGet]
        public ActionResult DeleteImage(Guid productId, Guid id)
        {
            _shopProductImages.Delete(id);
            var imgs = _shopProductImages.GetImagesByProductId(productId);
            return RedirectToAction("Edit", "ShopProducts", new { id = productId });
        }

        [HttpPost]
        public ActionResult InsertImage(ShopProductImages model)
        {
            ModelState.Remove("ShopProductImages_Path");
            if (ModelState.IsValid)
            {
                if (model.ShopProductImage_PostedFile != null && model.ShopProductImage_PostedFile.ContentLength > 0)
                {
                    FilesExtensions.SaveSystemFile(model.ShopProductImage_PostedFile, Server.MapPath("~/ShopFiles/Products/"), out string fileName, out string filePath);
                    model.ShopProductImages_Path = $"~/ShopFiles/Products/{fileName}";
                    var res = _shopProductImages.InsertUpdate(model);
                    if (res.Success)
                        return RedirectToAction("Edit", "ShopProducts", new { id = model.ShopProductImages_ProductID });
                }


            }
            return RedirectToAction("Edit", "ShopProducts", new { id = model.ShopProductImages_ProductID });
        }
    }
}