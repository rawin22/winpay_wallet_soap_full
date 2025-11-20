using System;
using System.Collections.Generic;
using System.Linq;
using TSG.Dal.WinstantPayShopDal.ShopOrderItemsDal;
using TSG.Dal.WinstantPayShopDal.ShopOrdersDal;
using TSG.Dal.WinstantPayShopDal.ShopPaymentDal;
using TSG.Models.DTO.Shop;
using TSG.Models.Enums;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.WinstantPayShop.ShopOrderItemsService
{
    public class ShopOrderItemsService : IShopOrderItemsService
    {
        private readonly IShopOrderItemsDal _shopOrderItemsDal;
        private readonly IShopOrdersDal _shopOrdersDal;
        private readonly IShopPaymentDal _shopPaymentDal;

        public ShopOrderItemsService(IShopOrderItemsDal shopOrderItemsDal, IShopOrdersDal shopOrdersDal, IShopPaymentDal shopPaymentDal)
        {
            _shopOrderItemsDal = shopOrderItemsDal;
            _shopOrdersDal = shopOrdersDal;
            _shopPaymentDal = shopPaymentDal;
        }

        public IResult Delete(Guid id) => _shopOrderItemsDal.Delete(id);

        public IResult<ShopOrderItems> GetById(Guid id)
        {
            var res = _shopOrderItemsDal.GetById(id);
            if (res.Success)
            {
                res.Obj.ShopOrderItems_ShopOrder = _shopOrdersDal.GetById(res.Obj.ShopOrderItems_OrderId).Obj;
                res.Obj.ShopOrderItems_IsPayed = _shopPaymentDal.GetByOrderId(res.Obj.ShopOrderItems_OrderId).Obj.FirstOrDefault(a =>
                                                     a.ShopPayment_OrderItemId == res.Obj.ShopOrderItems_ID &&
                                                     a.ShopPayment_Status == (int)ShopOrderEnum.SuccessifullyPayed) != null;
            }
            return res;
        }

        public IResult<ShopOrderItems> InsertUpdate(ShopOrderItems model) =>
            _shopOrderItemsDal.InsertUpdate(AutoMapper.Mapper.Map<ShopOrderItemsDto>(model));

        public IResult<List<ShopOrderItems>> GetAll() => _shopOrderItemsDal.GetAll();

        public IResult<List<ShopOrderItems>> GetItemsByOrderId(Guid orderId) =>
            _shopOrderItemsDal.GetItemsByOrderId(orderId);
    }
}