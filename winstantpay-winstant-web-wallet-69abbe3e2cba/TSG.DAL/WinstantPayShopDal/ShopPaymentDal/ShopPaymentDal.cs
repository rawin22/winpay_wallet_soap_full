using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using TSG.Models.DTO.Shop;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.Dal.WinstantPayShopDal.ShopPaymentDal
{
    public class ShopPaymentDal : IShopPaymentDal
    {
        public IResult Delete(Guid id)
        {
            return new Result("Not implemented method");
        }

        public IResult<ShopPayment> GetById(Guid id)
        {
            var all = GetAll();
            if (!all.Success)
                return new Result<ShopPayment>(all.Message);
            var single = all.Obj.FirstOrDefault(f => f.ShopPayment_ID == id);
            return single != null ? new Result<ShopPayment>(AutoMapper.Mapper.Map<ShopPayment>(single)) : new Result<ShopPayment>("Object not found");
        }

        public IResult<ShopPayment> InsertUpdate(ShopPaymentDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                if (model.ID == default)
                {
                    model.ID = Guid.NewGuid();

                    var insertQuery = connection.InsertResult(model);
                    return insertQuery.Success
                        ? new Result<ShopPayment>(AutoMapper.Mapper.Map<ShopPayment>(model), insertQuery.Message)
                        : new Result<ShopPayment>(insertQuery.Message);
                }

                var updateQuery = connection.UpdateResult(model);
                return updateQuery.Success
                    ? new Result<ShopPayment>(AutoMapper.Mapper.Map<ShopPayment>(model))
                    : new Result<ShopPayment>(AutoMapper.Mapper.Map<ShopPayment>(model),updateQuery.Message);
            }
        }

        public IResult<List<ShopPayment>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                var query = connection.QueryResult<ShopPaymentDto>("SELECT * FROM dbo.shop_PaymentOrder");
                return new Result<List<ShopPayment>>(AutoMapper.Mapper.Map<List<ShopPayment>>(query.Obj), query.Message);
            }
        }

        public IResult<List<ShopPayment>> GetByOrderId(Guid orderId)
        {
            var all = GetAll();
            if (!all.Success)
                return new Result<List<ShopPayment>>(all.Message);
            return new Result<List<ShopPayment>>(all.Obj.Where(w => w.ShopPayment_OrderId == orderId).ToList());
        }

        public IResult DeleteByOrderItemId(Guid orderItemId)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.ExecuteResult("DELETE FROM dbo.shop_PaymentOrder WHERE OrderItemId={orderItemId}", new { orderItemId });
            }
        }
    }
}