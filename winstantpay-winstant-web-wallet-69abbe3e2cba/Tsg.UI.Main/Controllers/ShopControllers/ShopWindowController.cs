using System.Linq;
using System.Web.Mvc;
using TSG.Models.APIModels;
using TSG.ServiceLayer.WinstantPayShop.ShopProductImagesService;
using TSG.ServiceLayer.WinstantPayShop.ShopProductService;

namespace Tsg.UI.Main.Controllers
{
//    [Authorize(Roles = Role.User)]
    public class ShopWindowController : Controller
    {
        private readonly IShopProductService _shopProductService;
        private readonly IShopProductImagesService _shopProductImagesService;

        public ShopWindowController(IShopProductService shopProductService, IShopProductImagesService shopProductImagesService)
        {
            _shopProductService = shopProductService;
            _shopProductImagesService = shopProductImagesService;
        }

        // GET: ShopWindow
        public ActionResult Index()
        {
            var queryGetAll = _shopProductService.GetAll();
            var queryAllImgs = _shopProductImagesService.GetAll();

            var listProduct = queryGetAll.Obj.Where(w=>!w.ShopProducts_IsDeleted && w.ShopProducts_IsPublished).OrderBy(ob => ob.ShopProducts_ShopCategory.ShopCategories_Order).ToList();
            foreach (var shopProductse in listProduct)
            {
                shopProductse.ShopProducts_ShopProductImages = queryAllImgs.Obj.Where(w=>w.ShopProductImages_ProductID == shopProductse.ShopProducts_ID).ToList();
            }
            return View(listProduct);
        }
    }
}