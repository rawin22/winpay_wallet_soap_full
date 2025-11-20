using System;
using System.Collections.Generic;
using TSG.Dal.WinstantPayShopDal.ShopInfoDal;
using TSG.Models.DTO.Shop;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.WinstantPayShop.ShopInfoService
{
    public class ShopInfoService : IShopInfoService
    {
        private readonly IShopInfoDal _shopInfoDal;

        public ShopInfoService(IShopInfoDal shopInfoDal) => _shopInfoDal = shopInfoDal;

        public IResult Delete(Guid id) => _shopInfoDal.Delete(id);

        public IResult<ShopInfo> GetById(Guid id) => _shopInfoDal.GetById(id);

        public IResult InsertUpdate(ShopInfo model) => _shopInfoDal.InsertUpdate(AutoMapper.Mapper.Map<ShopInfoDto>(model));

        public IResult<List<ShopInfo>> GetAll() => _shopInfoDal.GetAll();
    }
}