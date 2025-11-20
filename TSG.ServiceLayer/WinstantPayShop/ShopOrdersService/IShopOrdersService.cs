using System;
using System.Collections.Generic;
using TSG.Models.DTO.Shop;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.WinstantPayShop.ShopOrdersService
{
    public interface IShopOrdersService : IDelete<Guid>, IGetById<ShopOrders, Guid>
    {
        IResult<ShopOrders> InsertUpdate(ShopOrders model);
        IResult<List<ShopOrders>> GetAll();
        IResult<ShopOrders> GetOpenOrder(string wpayId);
        IResult<List<ShopOrders>> GetOrdersByWpayId(string wpayId);
        IResult CloseOrder(Guid orderId, int status);
    }
}