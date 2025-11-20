using System;
using TSG.Models.DTO.AutomaticExchanges;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.AutomaticExchange.LiquidCcyListDalMethods
{
    public interface ILiquidCcyListDalMethods : ICrud<LiquidCcyListDto, Guid>, IGetById<LiquidCcyListDto, Guid>, IInsert<LiquidCcyListDto>
    {
        IResult DeleteByCurrencyId(int ccyId);
        IResult<LiquidCcyListDto> GetLiquidCcyElementByCurrencyId(int ccyId);
        IResult<LiquidCcyListDto> GetLiquidCcyElementByCurrencyCode(string ccyCode);
        IResult BulkUpdateOrder(string ids);
    }
}