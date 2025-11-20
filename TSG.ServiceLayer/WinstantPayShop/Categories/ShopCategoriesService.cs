using System;
using System.Collections.Generic;
using TSG.Dal.WinstantPayShopDal.CategoriesDal;
using TSG.Models.DTO.Shop;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.WinstantPayShop.Categories
{
    public class ShopCategoriesService : IShopCategoryService
    {
        private readonly IShopCategoryDal _shopCategoryDal;
        
        public ShopCategoriesService(IShopCategoryDal shopCategoryDal) => _shopCategoryDal = shopCategoryDal;
        
        public IResult Delete(Guid id) => _shopCategoryDal.Delete(id);
        
        public IResult InsertUpdate(ShopCategories model) => _shopCategoryDal.InsertUpdate(AutoMapper.Mapper.Map<ShopCategoriesDto>(model));
        
        public IResult<List<ShopCategories>> GetAll() => _shopCategoryDal.GetAll();
        
        public IResult<ShopCategories> GetById(Guid id) => _shopCategoryDal.GetById(id);
    }
}