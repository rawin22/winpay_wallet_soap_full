using System;
using System.Collections.Generic;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.WinstantPayShop.ShopOrderItemsService
{
    public interface IShopOrderItemsService : IDelete<Guid>, IGetById<ShopOrderItems, Guid>
    {
        IResult<ShopOrderItems> InsertUpdate(ShopOrderItems model);
        IResult<List<ShopOrderItems>> GetAll();
        IResult<List<ShopOrderItems>> GetItemsByOrderId(Guid orderId);

    }
}