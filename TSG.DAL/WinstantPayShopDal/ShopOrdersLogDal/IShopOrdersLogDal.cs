using System;
using System.Collections.Generic;
using TSG.Models.DTO.Shop;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.WinstantPayShopDal.ShopOrdersLogDal
{
    public interface IShopOrdersLogDal : IDelete<Guid>, IGetById<ShopOrderLog, Guid>
    {
        IResult<ShopOrderLog> InsertUpdate(ShopOrderLogDto model);
        IResult<List<ShopOrderLog>> GetAll();

        IResult<List<ShopOrderLog>> GetLogByOrder(Guid orderId);
    }
}