using System;
using System.Collections.Generic;
using TSG.Models.DTO.Shop;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.WinstantPayShopDal.ShopProductsDal
{
    public interface IShopProductsDal : IDelete<Guid>, IGetById<ShopProducts, Guid>
    {
        IResult<ShopProducts> InsertUpdate(ShopProductsDto model);
        IResult<List<ShopProducts>> GetAll();
    }
}