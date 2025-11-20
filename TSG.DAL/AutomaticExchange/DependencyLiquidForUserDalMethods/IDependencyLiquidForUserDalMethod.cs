using System;
using System.Collections.Generic;
using TSG.Models.DTO.AutomaticExchanges;
using TSG.Models.ServiceModels.AutomaticExchange;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.AutomaticExchange.DependencyLiquidForUserDalMethods
{
    public interface IDependencyLiquidForUserDalMethod : IGetById<DependencyLiquidForUserDto, Guid>, IInsertByT<DependencyLiquidForUserDto>, IDelete<Guid>
    {
        IResult<List<DependencyLiquidForUserDto>> GetAll();
        IResult<List<DependencyLiquidForUserSo>> GetAllSo();
        IResult<List<DependencyLiquidForUserSo>> GetAllSoByUser(Guid userId);
        IResult DeleteAllByLiquidCcyCode(Guid liquidGuid);
        IResult DeleteAllByUserId(Guid userId);
        IResult BulkInsertLiquidForUser(Guid liquidGuid);
        IResult BulkInsertLiquidCurrencyForUser(string liquids, Guid userId);
    }
}