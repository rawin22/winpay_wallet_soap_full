using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using TSG.Dal.WinstantPayShopDal.ShopOrderItemsDal;
using TSG.Dal.WinstantPayShopDal.ShopOrdersLogDal;
using TSG.Models.DTO;
using TSG.Models.DTO.Shop;
using TSG.Models.Enums;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.Dal.WinstantPayShopDal.ShopOrdersDal
{
    public class ShopOrdersDal : IShopOrdersDal
    {
        private readonly IShopOrderItemsDal _shopOrderItemsDal;
        private readonly IShopOrdersLogDal _shopOrdersLogDal;
        
        public ShopOrdersDal(IShopOrderItemsDal shopOrderItemsDal, IShopOrdersLogDal shopOrdersLogDal)
        {
            _shopOrderItemsDal = shopOrderItemsDal;
            _shopOrdersLogDal = shopOrdersLogDal;

        }

        public IResult Delete(Guid id)
        {
            return new Result("Object does not delete");
            //using (var connection = ConnectionFactory.GetConnection())
            //{
            //    string sql = "UPDATE dbo.shop_Shops SET IsDeleted = 1 WHERE ID = @id";
            //    return connection.ExecuteResult(sql, new { id });
            //}
        }

        public IResult<ShopOrders> GetById(Guid id)
        {
            var all = GetAll();
            if (!all.Success)
                return new Result<ShopOrders>(all.Message);
            var single = all.Obj.FirstOrDefault(f => f.ShopOrders_ID == id);
            return single != null ? new Result<ShopOrders>(single) : new Result<ShopOrders>("Object not found");
        }

        public IResult<ShopOrders> InsertUpdate(ShopOrdersDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                if (model.ID == default)
                {
                    model.ID = Guid.NewGuid();
                    
                    var insertQuery = connection.InsertResult(model);
                    if(insertQuery.Success)
                    {
                        var addlog = _shopOrdersLogDal.InsertUpdate(new ShopOrderLogDto()
                        {
                            OrderID = model.ID,
                            Status = (int) ShopOrderEnum.Created,
                            Timestamp = DateTime.Now.Ticks,
                            Reason = "Created new order"
                        });
                        if (addlog.Success)
                        {
                            model.LastOrderHistoryRec = addlog.Obj.ShopOrderLog_ID;
                            var updateOrderQueryRes = connection.UpdateResult(model);
                            return updateOrderQueryRes.Success ? GetById(model.ID) : new Result<ShopOrders>(updateOrderQueryRes.Message);
                        }
                        return new Result<ShopOrders>("Doesn't insert row order");
                    }
                }
                var updateQuery = connection.UpdateResult(model);
                return updateQuery.Success? GetById(model.ID) : new Result<ShopOrders>(updateQuery.Message);
            }
        }

        public IResult<List<ShopOrders>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                var resQuery = connection.Query<ShopOrdersDto, ShopOrderLogDto, UserDto, ShopOrders>(
                    @"SELECT * FROM dbo.shop_Orders AS oreders
                        LEFT JOIN dbo.shop_OrderLog AS logs ON oreders.LastOrderHistoryRec = logs.ID
                        INNER JOIN dbo.[User] AS users ON users.username = oreders.BuyerWpayId",
                    (shopOrdersDto, shopOrderLogDto, userDto) =>
                    {
                        var res = new ShopOrders();
                        res = AutoMapper.Mapper.Map<ShopOrders>(shopOrdersDto);
                        var logs = _shopOrdersLogDal.GetLogByOrder(shopOrdersDto.ID).Obj;
                        res.ShopOrders_ShopOrderLogs = logs;
                        res.ShopOrders_ShopOrderItems = _shopOrderItemsDal.GetItemsByOrderId(shopOrdersDto.ID).Obj;
                        res.ShopOrders_ShopOrderItems.ForEach(f => { f.ShopOrderItems_ShopOrder = res; });
                        res.ShopOrders_LastShopOrderLog = logs.OrderBy(ob=>ob.ShopOrderLog_Timestamp).LastOrDefault(f => f.ShopOrderLog_ID == res.ShopOrders_LastLogItem);
                        return res;

                    }, commandType: CommandType.Text, splitOn: "ID,username");
                return new Result<List<ShopOrders>>(AutoMapper.Mapper.Map<List<ShopOrders>>(resQuery));
            }
        }

        public IResult<List<ShopOrders>> GetOrdersByWpayId(string wpayId)
        {
            var res = GetAll();
            if(!res.Success)
                return new Result<List<ShopOrders>>(res.Message);
            return new Result<List<ShopOrders>>(res.Obj.Where(w=>w.ShopOrders_BuyerWpayId == wpayId).ToList());
        }
    }
}