using System;
using System.Collections.Generic;
using TSG.Models.DTO.Shop;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.WinstantPayShop.ShopPaymentService
{
    public interface IShopPaymentService : IDelete<Guid>, IGetById<ShopPayment, Guid>
    {
        IResult<ShopPayment> InsertUpdate(ShopPayment model);
        IResult<ShopPayment> UpdateByOrderItem(Guid orderId, List<Guid> orderItemId, int status, string reason, Guid? paymentId);
        IResult<List<ShopPayment>> GetAll();
        IResult<List<ShopPayment>> GetByOrderId(Guid orderId);
        IResult DeleteByOrderItemId(Guid orderItemId);
    }
}