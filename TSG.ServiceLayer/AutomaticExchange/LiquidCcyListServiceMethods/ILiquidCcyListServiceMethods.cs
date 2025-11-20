using System;
using TSG.Models.ServiceModels.AutomaticExchange;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.AutomaticExchange.LiquidCcyListServiceMethods
{
    public interface ILiquidCcyListServiceMethods : ICrud<LiquidCcyListSo, Guid>, IGetById<LiquidCcyListSo, Guid>, IInsertByT<LiquidCcyListSo>
    {
        IResult DeleteByCurrencyId(int ccyId);
        IResult<LiquidCcyListSo> GetLiquidCcyElementByCurrencyId(int ccyId);
        IResult<LiquidCcyListSo> GetLiquidCcyElementByCurrencyCode(string ccyCode);
        IResult BulkUpdateOrder(string ids);

    }
}