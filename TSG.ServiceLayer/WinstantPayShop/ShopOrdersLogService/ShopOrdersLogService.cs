using System;
using System.Collections.Generic;
using System.Linq;
using TSG.Dal.WinstantPayShopDal.ShopOrdersLogDal;
using TSG.Models.DTO.Shop;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.WinstantPayShop.ShopOrdersLogService
{
    public class ShopOrdersLogService : IShopOrdersLogService
    {
        private readonly IShopOrdersLogDal _shopOrdersLogDal;

        public ShopOrdersLogService(IShopOrdersLogDal shopOrdersLogDal) => _shopOrdersLogDal = shopOrdersLogDal;

        public IResult Delete(Guid id) => _shopOrdersLogDal.Delete(id);

        public IResult<ShopOrderLog> GetById(Guid id)
        {
            var all = GetAll();
            if (!all.Success)
                return new Result<ShopOrderLog>(all.Message);
            var single = all.Obj.FirstOrDefault(f => f.ShopOrderLog_ID == id);
            return single != null ? new Result<ShopOrderLog>(single) : new Result<ShopOrderLog>("Object not found");
        }
        public IResult<ShopOrderLog> InsertUpdate(ShopOrderLog model) => _shopOrdersLogDal.InsertUpdate(AutoMapper.Mapper.Map<ShopOrderLogDto>(model));

        public IResult<List<ShopOrderLog>> GetAll() => _shopOrdersLogDal.GetAll();

        public IResult<List<ShopOrderLog>> GetLogByOrder(Guid orderId) => _shopOrdersLogDal.GetLogByOrder(orderId);
    }
}