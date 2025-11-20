using System;
using System.Collections.Generic;
using TSG.Models.DTO.Shop;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.WinstantPayShopDal.ShopProductImagesDal
{
    public interface IShopProductImagesDal : IDelete<Guid>, IGetById<ShopProductImages, Guid>
    {
        IResult InsertUpdate(ShopProductImagesDto model);
        IResult<List<ShopProductImages>> GetAll();
        IResult<List<ShopProductImages>> GetImagesByProductId(Guid productId);
    }
}