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

namespace TSG.Dal.WinstantPayShopDal.ShopInfoDal
{
    public class ShopInfoDal : IShopInfoDal
    {
        public IResult Delete(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                string sql = "UPDATE dbo.shop_Shops SET IsDeleted = 1 WHERE ID = @id";
                return connection.ExecuteResult(sql, new { id });
            }
        }

        public IResult<ShopInfo> GetById(Guid id)
        {
            var all = GetAll();
            if (!all.Success)
                return new Result<ShopInfo>(all.Message);
            var single = all.Obj.FirstOrDefault(f => f.ShopInfo_ID == id);
            return single != null ? new Result<ShopInfo>(single) : new Result<ShopInfo>("Object not found");
        }

        public IResult InsertUpdate(ShopInfoDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                if (model.ID == default)
                {
                    model.ID = Guid.NewGuid();
                    return connection.InsertResult(model);
                }
                return connection.UpdateResult(model);
            }
        }

        public IResult<List<ShopInfo>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                var res = connection.QueryResult<ShopInfoDto>("SELECT * FROM dbo.shop_Shops");
                return new Result<List<ShopInfo>>(AutoMapper.Mapper.Map<List<ShopInfo>>(res.Obj), res.Message);
            }
        }
    }
}