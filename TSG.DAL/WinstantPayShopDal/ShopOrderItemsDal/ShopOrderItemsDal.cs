using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using TSG.Dal.WinstantPayShopDal.ShopOrdersDal;
using TSG.Models.DTO.Shop;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.Dal.WinstantPayShopDal.ShopOrderItemsDal
{
    public class ShopOrdersItemsDal : IShopOrderItemsDal
    {
        public IResult Delete(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                string sql = "DELETE FROM dbo.shop_OrderItems WHERE ID = @id";
                return connection.ExecuteResult(sql, new { id });
            }
        }

        public IResult<ShopOrderItems> GetById(Guid id)
        {
            var all = GetAll();
            if (!all.Success)
                return new Result<ShopOrderItems>(all.Message);
            var single = all.Obj.FirstOrDefault(f => f.ShopOrderItems_ID == id);
            if (single == null || single.ShopOrderItems_OrderId == Guid.Empty)
                return new Result<ShopOrderItems>("Object not found");
            return new Result<ShopOrderItems>(single);
        }

        public IResult<ShopOrderItems> InsertUpdate(ShopOrderItemsDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                if (model.ID == default)
                {
                    model.ID = Guid.NewGuid();
                    {
                        return connection.InsertResult(model).Success ? GetById(model.ID) : null;
                    }
                }
                return connection.UpdateResult(model).Success? GetById(model.ID) : null;
            }
        }

        public IResult<List<ShopOrderItems>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                var resQuery = connection.Query<ShopOrderItemsDto, ShopProductsDto, ShopInfoDto, ShopOrderItems>(
                    @"SELECT * FROM dbo.shop_OrderItems AS item
                        INNER JOIN dbo.shop_Products AS prod ON prod.ID = item.ProductId
                        INNER JOIN dbo.shop_Shops AS shop ON shop.ID = prod.ShopID",
                    (shopOrderItemsDto, productDto, shopDto) =>
                    {
                        var res = new ShopOrderItems();
                        res = AutoMapper.Mapper.Map<ShopOrderItems>(shopOrderItemsDto);
                        res.ShopOrderItems_ShopProduct = AutoMapper.Mapper.Map<ShopProducts>(productDto);
                        res.ShopOrderItems_ShopProduct.ShopProducts_ShopInfo = AutoMapper.Mapper.Map<ShopInfo>(shopDto);
                        return res;

                    }, commandType: CommandType.Text, splitOn: "ID,ID");
                return new Result<List<ShopOrderItems>>(AutoMapper.Mapper.Map<List<ShopOrderItems>>(resQuery));
            }
        }

        public IResult<List<ShopOrderItems>> GetItemsByOrderId(Guid orderId)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                var resQuery = connection.Query<ShopOrderItemsDto, ShopProductsDto, ShopInfoDto, ShopOrderItems>(
                    @"SELECT * FROM dbo.shop_OrderItems AS item
                        INNER JOIN dbo.shop_Products AS prod ON prod.ID = item.ProductId
                        INNER JOIN dbo.shop_Shops AS shop ON shop.ID = prod.ShopID
                        WHERE item.OrderId = CASE WHEN @orderId IS NULL THEN item.OrderId ELSE @orderId END",
                    (shopOrderItemsDto, productDto, shopDto) =>
                    {
                        var res = new ShopOrderItems();
                        res = AutoMapper.Mapper.Map<ShopOrderItems>(shopOrderItemsDto);
                        res.ShopOrderItems_ShopProduct = AutoMapper.Mapper.Map<ShopProducts>(productDto);
                        res.ShopOrderItems_ShopProduct.ShopProducts_ShopInfo = AutoMapper.Mapper.Map<ShopInfo>(shopDto);
                        return res;

                    }, commandType: CommandType.Text, param:new {orderId}, splitOn: "ID,ID");
                return new Result<List<ShopOrderItems>>(AutoMapper.Mapper.Map<List<ShopOrderItems>>(resQuery));
            }
        }
    }
}