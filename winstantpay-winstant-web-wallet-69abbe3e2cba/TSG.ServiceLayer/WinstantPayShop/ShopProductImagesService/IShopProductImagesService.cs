using System;
using System.Collections.Generic;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.WinstantPayShop.ShopProductImagesService
{
    public interface IShopProductImagesService : IDelete<Guid>, IGetById<ShopProductImages, Guid>
    {
        IResult InsertUpdate(ShopProductImages model);
        IResult<List<ShopProductImages>> GetAll();
        IResult<List<ShopProductImages>> GetImagesByProductId(Guid productId);

    }
}