using System;
using System.Collections.Generic;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.WinstantPayShop.ShopOrdersLogService
{
    public interface IShopOrdersLogService : IDelete<Guid>, IGetById<ShopOrderLog, Guid>
    {
        IResult<ShopOrderLog> InsertUpdate(ShopOrderLog model);
        IResult<List<ShopOrderLog>> GetAll();

        IResult<List<ShopOrderLog>> GetLogByOrder(Guid orderId);
    }
}