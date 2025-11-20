using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TSG.Models.DTO.Shop;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.Dal.WinstantPayShopDal.ShopProductImagesDal
{
    public class ShopProductImagesDal : IShopProductImagesDal
    {
        public IResult Delete(Guid id)
        {
            //return new Result("Not realized");
            var obj = GetById(id);
            if (obj.Success && obj.Obj != null)
            {
                if (File.Exists(obj.Obj.ShopProductImages_Path))
                {
                    try { File.Delete(obj.Obj.ShopProductImages_Path); }
                    catch (Exception e) { /* ignored */ }
                }
                using (var connection = ConnectionFactory.GetConnection())
                {
                    return connection.DeleteResult(AutoMapper.Mapper.Map<ShopProductImagesDto>(obj.Obj));
                }
            }
            return new Result();
        }

        public IResult<ShopProductImages> GetById(Guid id)
        {
            var all = GetAll();
            if (!all.Success)
                return new Result<ShopProductImages>(all.Message);
            var single = all.Obj.FirstOrDefault(f => f.ShopProductImages_ID == id);
            return single != null ? new Result<ShopProductImages>(single) : new Result<ShopProductImages>("Object not found");
        }

        public IResult InsertUpdate(ShopProductImagesDto model)
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

        public IResult<List<ShopProductImages>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                var resQuery = connection.QueryResult<ShopProductImagesDto>(@"SELECT * FROM dbo.shop_ProductImages");
                return new Result<List<ShopProductImages>>(AutoMapper.Mapper.Map<List<ShopProductImages>>(resQuery.Obj));
            }
        }

        public IResult<List<ShopProductImages>> GetImagesByProductId(Guid productId)
        {
            var getByProd = GetAll();
            return new Result<List<ShopProductImages>>(
                getByProd.Obj.Where(w => w.ShopProductImages_ProductID == productId).ToList(), getByProd.Message);
        }
    }
}