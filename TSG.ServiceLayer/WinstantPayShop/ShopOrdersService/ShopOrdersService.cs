using System;
using System.Collections.Generic;
using System.Linq;
using TSG.Dal.WinstantPayShopDal.ShopOrdersDal;
using TSG.Models.DTO.Shop;
using TSG.Models.Enums;
using TSG.Models.ServiceModels.Shop;
using TSG.ServiceLayer.WinstantPayShop.ShopOrdersLogService;
using TSG.ServiceLayer.WinstantPayShop.ShopPaymentService;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.WinstantPayShop.ShopOrdersService
{
    public class ShopOrdersService : IShopOrdersService
    {
        private readonly IShopOrdersDal _shopOrdersDal;
        private readonly IShopOrdersLogService _shopOrdersLogService;
        private readonly IShopPaymentService _shopPaymentService;

        public ShopOrdersService(IShopOrdersDal shopOrdersDal, IShopOrdersLogService shopOrdersLogService, IShopPaymentService shopPaymentService)
        {
            _shopOrdersDal = shopOrdersDal;
            _shopOrdersLogService = shopOrdersLogService;
            _shopPaymentService = shopPaymentService;
        }

        public IResult Delete(Guid id) => _shopOrdersDal.Delete(id);

        public IResult<ShopOrders> GetById(Guid id)
        {
            var query = _shopOrdersDal.GetById(id);
            if (!query.Success)
                return new Result<ShopOrders>(query.Message);
            query.Obj.ShopOrders_ShopPaymentByOrder = _shopPaymentService.GetByOrderId(id).Obj;
            return query;
        }

        public IResult<ShopOrders> InsertUpdate(ShopOrders model) => _shopOrdersDal.InsertUpdate(AutoMapper.Mapper.Map<ShopOrdersDto>(model));

        public IResult<List<ShopOrders>> GetAll() => _shopOrdersDal.GetAll();
        public IResult<ShopOrders> GetOpenOrder(string wpayId)
        {
            var res = _shopOrdersDal.GetOrdersByWpayId(wpayId);
            var lastOpenOrder = res.Obj.LastOrDefault(f =>
                f.ShopOrders_LastShopOrderLog.ShopOrderLog_Status == (int)ShopOrderEnum.Opened ||
                f.ShopOrders_LastShopOrderLog.ShopOrderLog_Status == (int)ShopOrderEnum.Created);

            if (res.Success && lastOpenOrder != null)
            {
                lastOpenOrder.ShopOrders_ShopPaymentByOrder = _shopPaymentService.GetByOrderId(lastOpenOrder.ShopOrders_ID).Obj;
                lastOpenOrder.ShopOrders_ShopOrderItems.ForEach(f =>
                    {
                        f.ShopOrderItems_IsPayed = lastOpenOrder.ShopOrders_ShopPaymentByOrder.FirstOrDefault(a =>
                                                       a.ShopPayment_OrderItemId == f.ShopOrderItems_ID &&
                                                       a.ShopPayment_Status == (int)ShopOrderEnum.SuccessifullyPayed) != null;
                    });
                return new Result<ShopOrders>(lastOpenOrder);
            }
            return _shopOrdersDal.InsertUpdate(new ShopOrdersDto() { BuyerWpayId = wpayId });
        }

        public IResult<List<ShopOrders>> GetOrdersByWpayId(string wpayId)
        {
            var query = _shopOrdersDal.GetOrdersByWpayId(wpayId);
            if(!query.Success)
                return new Result<List<ShopOrders>>(query.Message);
            return query;
        }

        public IResult CloseOrder(Guid orderId, int status)
        {
            var order = _shopOrdersDal.GetById(orderId);

            var log = _shopOrdersLogService.InsertUpdate(new ShopOrderLog()
            {
                ShopOrderLog_OrderID = orderId,
                ShopOrderLog_Reason = "Close order. Payment success",
                ShopOrderLog_Status = status,
                ShopOrderLog_Timestamp = DateTime.Now.Ticks

            });
            if(!log.Success)
                return new Result(log.Message);
            order.Obj.ShopOrders_LastLogItem = log.Obj.ShopOrderLog_ID;
            return _shopOrdersDal.InsertUpdate(AutoMapper.Mapper.Map<ShopOrdersDto>(order.Obj));
        }
    }
}