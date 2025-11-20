using System;
using System.Collections.Generic;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.WinstantPayShop.ShopProductService
{
    public interface IShopProductService : IDelete<Guid>, IGetById<ShopProducts, Guid>
    {
        IResult<ShopProducts> InsertUpdate(ShopProducts model);
        IResult<List<ShopProducts>> GetAll();
    }
}