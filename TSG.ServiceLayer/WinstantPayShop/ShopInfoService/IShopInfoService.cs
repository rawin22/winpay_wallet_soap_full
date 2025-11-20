using System;
using System.Collections.Generic;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.WinstantPayShop.ShopInfoService
{
    public interface IShopInfoService : IDelete<Guid>, IGetById<ShopInfo, Guid>
    {
        IResult InsertUpdate(ShopInfo model);
        IResult<List<ShopInfo>> GetAll();
    }
}