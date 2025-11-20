using System;
using System.Collections.Generic;
using TSG.Models.DTO.Shop;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.WinstantPayShopDal.ShopOrdersDal
{
    public interface IShopOrdersDal : IDelete<Guid>, IGetById<ShopOrders, Guid>
    {
        IResult<ShopOrders> InsertUpdate(ShopOrdersDto model);
        IResult<List<ShopOrders>> GetAll();

        IResult<List<ShopOrders>> GetOrdersByWpayId(string wpayId);
    }
}