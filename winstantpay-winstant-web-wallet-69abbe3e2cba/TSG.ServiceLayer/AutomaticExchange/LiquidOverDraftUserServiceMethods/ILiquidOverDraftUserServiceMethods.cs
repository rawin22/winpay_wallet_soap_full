using System;
using System.Collections.Generic;
using TSG.Models.DTO.AutomaticExchanges;
using TSG.Models.ServiceModels.AutomaticExchange;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.AutomaticExchange.LiquidOverDraftUserServiceMethods
{
    public interface ILiquidOverDraftUserServiceMethods : ICrud<LiquidOverDraftUserListSo, Guid>, IInsertByT<LiquidOverDraftUserListSo>, IGetById<LiquidOverDraftUserListSo, Guid>
    {
        IResult<List<LiquidOverDraftUserListSo>> GetAllSo();
        IResult DeleteByCurrencyId(int ccyId);
        IResult DeleteByLiquidCurrencyId(Guid liquidCcyId);
    }
}