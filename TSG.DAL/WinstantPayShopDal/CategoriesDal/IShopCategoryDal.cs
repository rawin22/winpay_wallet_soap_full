using System;
using System.Collections.Generic;
using TSG.Models.DTO.Shop;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.WinstantPayShopDal.CategoriesDal
{
    public interface IShopCategoryDal : IDelete<Guid>, IGetById<ShopCategories, Guid>
    {
        IResult InsertUpdate(ShopCategoriesDto model);
        IResult<List<ShopCategories>> GetAll();
    }
}