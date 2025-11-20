using System;
using System.Collections.Generic;
using TSG.Dal.WinstantPayShopDal.ShopInfoDal;
using TSG.Dal.WinstantPayShopDal.ShopProductsDal;
using TSG.Models.DTO.Shop;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.WinstantPayShop.ShopProductService
{
    public class ShopProductService : IShopProductService
    {
        private readonly IShopProductsDal _shopIProductsDal;

        public ShopProductService(IShopProductsDal shopInfoDal) => _shopIProductsDal = shopInfoDal;

        public IResult Delete(Guid id) => _shopIProductsDal.Delete(id);

        public IResult<ShopProducts> GetById(Guid id) => _shopIProductsDal.GetById(id);

        public IResult<ShopProducts> InsertUpdate(ShopProducts model)
        {
            return _shopIProductsDal.InsertUpdate(AutoMapper.Mapper.Map<ShopProductsDto>(model));
        }

        public IResult<List<ShopProducts>> GetAll() => _shopIProductsDal.GetAll();
    }
}