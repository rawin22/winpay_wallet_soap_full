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

namespace TSG.Dal.WinstantPayShopDal.CategoriesDal
{
    public class ShopCategoriesDal : IShopCategoryDal
    {
        public IResult Delete(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                string sql = "UPDATE dbo.shop_Categories SET IsDeleted = 1 WHERE ID = @id";
                return connection.ExecuteResult(sql, new { id });
            }
        }

        public IResult InsertUpdate(ShopCategoriesDto model)
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

        public IResult<List<ShopCategories>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                var resCategorieses = connection
                    .Query<ShopCategoriesDto, ShopCategoriesDto, ShopCategories>("SELECT * FROM dbo.shop_Categories AS cat LEFT JOIN dbo.shop_Categories AS pcat ON cat.Parent = pcat.ID",
                        (child, parent) =>
                        {
                            ShopCategories res = new ShopCategories();
                            res.ShopCategories_ID = child.ID;
                            res.ShopCategories_IsDeleted = child.IsDeleted;
                            res.ShopCategories_IsPublished = child.IsPublished;
                            res.ShopCategories_Name = child.Name;
                            res.ShopCategories_Parent = child.Parent;
                            if (child.Parent != null)
                            {
                                res.ShopCategories_ParentShopCategory = new ShopCategories()
                                {
                                    ShopCategories_ID = parent.ID,
                                    ShopCategories_IsDeleted = parent.IsDeleted,
                                    ShopCategories_IsPublished = parent.IsPublished,
                                    ShopCategories_Name = parent.Name
                                };
                            }

                            return res;
                        },
                        commandType: CommandType.Text, splitOn: "ID"
                    ).ToList();
                return new Result<List<ShopCategories>>(resCategorieses);
            }
        }


        public IResult<ShopCategories> GetById(Guid id)
        {
            var all = GetAll();
            if (!all.Success)
                return new Result<ShopCategories>(all.Message);
            var single = all.Obj.FirstOrDefault(f => f.ShopCategories_ID == id);
            return  single != null ? new Result<ShopCategories>(single) : new Result<ShopCategories>("Object not found");
        }
    }
}