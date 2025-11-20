using System;
using System.Collections.Generic;
using TSG.Models.DTO.Shop;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.WinstantPayShopDal.ShopOrderItemsDal
{
    public interface IShopOrderItemsDal : IDelete<Guid>, IGetById<ShopOrderItems, Guid>
    {
        IResult<ShopOrderItems> InsertUpdate(ShopOrderItemsDto model);
        IResult<List<ShopOrderItems>> GetAll();
        IResult<List<ShopOrderItems>> GetItemsByOrderId(Guid orderId);

    }
}