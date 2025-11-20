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

namespace TSG.Dal.WinstantPayShopDal.ShopOrdersLogDal
{
    public class ShopOrdersLogDal : IShopOrdersLogDal
    {
        public IResult Delete(Guid id)
        {
            return new Result("Object does not delete");
            
        }

        public IResult<ShopOrderLog> GetById(Guid id)
        {
            var all = GetAll();
            if (!all.Success)
                return new Result<ShopOrderLog>(all.Message);
            var single = all.Obj.FirstOrDefault(f => f.ShopOrderLog_ID == id);
            return single != null ? new Result<ShopOrderLog>(single) : new Result<ShopOrderLog>("Object not found");
        }

        public IResult<ShopOrderLog> InsertUpdate(ShopOrderLogDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                if (model.ID == default)
                {
                    model.ID = Guid.NewGuid();
                    var insertQuery = connection.InsertResult(model);
                    return insertQuery.Success ? GetById(model.ID) : new Result<ShopOrderLog>(insertQuery.Message);
                }
                var updateQuery = connection.UpdateResult(model);
                return updateQuery.Success ? GetById(model.ID) : new Result<ShopOrderLog>(updateQuery.Message);
            }
        }
        
        public IResult<List<ShopOrderLog>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                var resQuery = connection.QueryResult<ShopOrderLogDto>(@"SELECT * FROM dbo.shop_OrderLog");
                return new Result<List<ShopOrderLog>>(AutoMapper.Mapper.Map<List<ShopOrderLog>>(resQuery.Obj));
            }
        }

        public IResult<List<ShopOrderLog>> GetLogByOrder(Guid orderId)
        {
            {
                var all = GetAll();
                if (!all.Success)
                    return new Result<List<ShopOrderLog>>(all.Message);
                var orderLogList = all.Obj.Where(w => w.ShopOrderLog_OrderID == orderId).ToList();
                return new Result<List<ShopOrderLog>>(orderLogList);
            }
        }
    }
}