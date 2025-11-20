using System;
using System.Collections.Generic;
using TSG.Models.DTO.Shop;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.WinstantPayShop.Categories
{
    public interface IShopCategoryService : IDelete<Guid>, IGetById<ShopCategories, Guid>
    {
        IResult InsertUpdate(ShopCategories model);
        IResult<List<ShopCategories>> GetAll();
    }
}