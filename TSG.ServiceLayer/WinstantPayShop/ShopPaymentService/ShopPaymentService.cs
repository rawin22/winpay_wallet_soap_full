using System;
using System.Collections.Generic;
using System.Linq;
using TSG.Dal.WinstantPayShopDal.ShopPaymentDal;
using TSG.Models.DTO.Shop;
using TSG.Models.Enums;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.WinstantPayShop.ShopPaymentService
{
    public class ShopPaymentService : IShopPaymentService
    {
        private readonly IShopPaymentDal _shopPaymentDal;

        public ShopPaymentService(IShopPaymentDal shopPaymentDal)
        {
            _shopPaymentDal = shopPaymentDal;
        }

        public IResult Delete(Guid id) => _shopPaymentDal.Delete(id);

        public IResult<ShopPayment> GetById(Guid id) => _shopPaymentDal.GetById(id);

        public IResult<ShopPayment> InsertUpdate(ShopPayment model) => _shopPaymentDal.InsertUpdate(AutoMapper.Mapper.Map<ShopPaymentDto>(model));
        public IResult<ShopPayment> UpdateByOrderItem(Guid orderId, List<Guid> orderItemId, int status, string reason, Guid? paymentId = null)
        {
            var items = GetByOrderId(orderId).Obj;
            foreach (var item in items)
            {
                if (orderItemId.Contains(item.ShopPayment_OrderItemId))
                {
                    item.ShopPayment_Status = status;
                    item.ShopPayment_Reason = reason;
                    item.ShopPayment_PaymentId = paymentId;
                    _shopPaymentDal.InsertUpdate(AutoMapper.Mapper.Map<ShopPaymentDto>(item));
                }
            }
            return new Result<ShopPayment>();
        }

        public IResult<List<ShopPayment>> GetAll() => _shopPaymentDal.GetAll();

        public IResult<List<ShopPayment>> GetByOrderId(Guid orderId) => _shopPaymentDal.GetByOrderId(orderId);

        public IResult DeleteByOrderItemId(Guid itemId) => _shopPaymentDal.DeleteByOrderItemId(itemId);
    }
}