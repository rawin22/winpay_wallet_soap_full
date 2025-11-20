using System;
using System.Collections.Generic;
using TSG.Models.DTO.Shop;
using TSG.Models.ServiceModels.Shop;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.WinstantPayShopDal.ShopInfoDal
{
    public interface IShopInfoDal : IDelete<Guid>, IGetById<ShopInfo, Guid>
    {
        IResult InsertUpdate(ShopInfoDto model);
        IResult<List<ShopInfo>> GetAll();
    }
}