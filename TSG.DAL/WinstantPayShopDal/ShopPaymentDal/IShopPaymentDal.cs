using System;
using System.Collections.Generic;
using TSG.Models.DTO.Shop;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.WinstantPayShopDal.ShopPaymentDal
{
    public interface IShopPaymentDal : IDelete<Guid>, IGetById<ShopPayment, Guid>
    {
        IResult<ShopPayment> InsertUpdate(ShopPaymentDto model);
        IResult<List<ShopPayment>> GetAll();
        IResult<List<ShopPayment>> GetByOrderId(Guid orderId);
        IResult DeleteByOrderItemId(Guid orderItemId);
    }
}