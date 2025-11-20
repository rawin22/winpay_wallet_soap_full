using System;
using System.Collections.Generic;
using TSG.Dal.WinstantPayShopDal.ShopInfoDal;
using TSG.Dal.WinstantPayShopDal.ShopProductImagesDal;
using TSG.Dal.WinstantPayShopDal.ShopProductsDal;
using TSG.Models.DTO.Shop;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.WinstantPayShop.ShopProductImagesService
{
    public class ShopProductImagesService : IShopProductImagesService
    {
        private readonly IShopProductImagesDal _shopIProductsDal;

        public ShopProductImagesService(IShopProductImagesDal shopInfoDal) => _shopIProductsDal = shopInfoDal;

        public IResult Delete(Guid id) => _shopIProductsDal.Delete(id);

        public IResult<ShopProductImages> GetById(Guid id) => _shopIProductsDal.GetById(id);

        public IResult InsertUpdate(ShopProductImages model)
        {
            return _shopIProductsDal.InsertUpdate(AutoMapper.Mapper.Map<ShopProductImagesDto>(model));
        }

        public IResult<List<ShopProductImages>> GetAll() => _shopIProductsDal.GetAll();

        public IResult<List<ShopProductImages>> GetImagesByProductId(Guid productId) => _shopIProductsDal.GetImagesByProductId(productId);
    }
}