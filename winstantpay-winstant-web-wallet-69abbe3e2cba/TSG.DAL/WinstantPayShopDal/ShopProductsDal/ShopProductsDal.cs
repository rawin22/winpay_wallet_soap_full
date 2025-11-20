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

namespace TSG.Dal.WinstantPayShopDal.ShopProductsDal
{
    public class ShopProductsDal : IShopProductsDal
    {
        public IResult Delete(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                string sql = "UPDATE dbo.shop_Shops SET IsDeleted = 1 WHERE ID = @id";
                return connection.ExecuteResult(sql, new { id });
            }
        }

        public IResult<ShopProducts> GetById(Guid id)
        {
            var all = GetAll();
            if (!all.Success)
                return new Result<ShopProducts>(all.Message);
            var single = all.Obj.FirstOrDefault(f => f.ShopProducts_ID == id);
            return single != null ? new Result<ShopProducts>(single) : new Result<ShopProducts>("Object not found");
        }

        public IResult<ShopProducts> InsertUpdate(ShopProductsDto model)
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

        public IResult<List<ShopProducts>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                var resQuery = connection.Query<ShopProductsDto, ShopCategoriesDto, ShopInfoDto, ShopProducts>(
                    @"SELECT * FROM dbo.shop_Products AS prod
                      INNER JOIN dbo.shop_Categories AS cat ON cat.ID = prod.CategoryID
                      INNER JOIN dbo.shop_Shops AS shop ON shop.ID = prod.ShopID",
                    (shopProductDto, categoriesDto, shopDto) =>
                    {
                        var res = new ShopProducts();
                        res = AutoMapper.Mapper.Map<ShopProducts>(shopProductDto);
                        res.ShopProducts_ShopCategory = AutoMapper.Mapper.Map<ShopCategories>(categoriesDto);
                        res.ShopProducts_ShopInfo = AutoMapper.Mapper.Map<ShopInfo>(shopDto);
                        return res;

                    }, commandType: CommandType.Text, splitOn:"ID,ID");
                return new Result<List<ShopProducts>>(AutoMapper.Mapper.Map<List<ShopProducts>>(resQuery));
            }
        }
    }
}