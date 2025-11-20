using System;
using System.Collections.Generic;
using TSG.Models.DTO.AutomaticExchanges;
using TSG.Models.ServiceModels.AutomaticExchange;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.AutomaticExchange.LiquidOverDraftUserDalMethods
{
    public interface ILiquidOverDraftUserDalMethods : ICrud<LiquidOverDraftUserListDto, Guid>, IInsert<LiquidOverDraftUserListDto>, IGetById<LiquidOverDraftUserListDto, Guid>
    {
        IResult DeleteByCurrencyId(int ccyId);
        IResult DeleteByLiquidCurrencyId(Guid liquidCcyId);

        IResult<List<LiquidOverDraftUserListSo>> GetAllSo();

    }
}