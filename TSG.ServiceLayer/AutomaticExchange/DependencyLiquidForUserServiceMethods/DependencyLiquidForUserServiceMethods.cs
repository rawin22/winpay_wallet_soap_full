using System;
using System.Collections.Generic;
using TSG.Dal.AutomaticExchange.DependencyLiquidForUserDalMethods;
using TSG.Models.DTO.AutomaticExchanges;
using TSG.Models.ServiceModels.AutomaticExchange;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.AutomaticExchange.DependencyLiquidForUserServiceMethods
{
    public class DependencyLiquidForUserServiceMethods : IDependencyLiquidForUserServiceMethods
    {
        private readonly IDependencyLiquidForUserDalMethod _dependencyLiquidForUserDalMethod;
        public DependencyLiquidForUserServiceMethods(IDependencyLiquidForUserDalMethod dependencyLiquidForUserDalMethod)
        {
            _dependencyLiquidForUserDalMethod = dependencyLiquidForUserDalMethod;
        }
        public IResult<DependencyLiquidForUserSo> GetById(Guid id)
        {
            var getObj = _dependencyLiquidForUserDalMethod.GetById(id);

            if(!getObj.Success || getObj.Obj==null)
                return new Result<DependencyLiquidForUserSo>(getObj.Message);
            return new Result<DependencyLiquidForUserSo>(AutoMapper.Mapper.Map<DependencyLiquidForUserSo>(getObj.Obj));
        }

        public IResult<DependencyLiquidForUserSo> Insert(DependencyLiquidForUserSo model)
        {
            if(model.DependencyLiquidForUser_Id == default) model.DependencyLiquidForUser_Id = Guid.NewGuid();

            var query = _dependencyLiquidForUserDalMethod.Insert(
                AutoMapper.Mapper.Map<DependencyLiquidForUserDto>(model));
            if(!query.Success || query.Obj==null)
                return new Result<DependencyLiquidForUserSo>(query.Message);
            return new Result<DependencyLiquidForUserSo>(AutoMapper.Mapper.Map<DependencyLiquidForUserSo>(query.Obj));
        }

        public IResult<List<DependencyLiquidForUserSo>> GetAll()
        {
            var res = _dependencyLiquidForUserDalMethod.GetAll();
            if (!res.Success || res.Obj == null)
                return new Result<List<DependencyLiquidForUserSo>>(res.Message);
            return new Result<List<DependencyLiquidForUserSo>>(AutoMapper.Mapper.Map<List<DependencyLiquidForUserSo>>(res.Obj));
        }
        public IResult Delete(Guid id) => _dependencyLiquidForUserDalMethod.Delete(id);

        public IResult<List<DependencyLiquidForUserSo>> GetAllSo() => _dependencyLiquidForUserDalMethod.GetAllSo();

        public IResult<List<DependencyLiquidForUserSo>> GetAllSoByUser(Guid userId) =>
            _dependencyLiquidForUserDalMethod.GetAllSoByUser(userId);

        public IResult DeleteAllByLiquidCcyCode(Guid liquidGuid) =>
            _dependencyLiquidForUserDalMethod.DeleteAllByLiquidCcyCode(liquidGuid);

        public IResult DeleteAllByUserId(Guid userId) => _dependencyLiquidForUserDalMethod.DeleteAllByUserId(userId);

        public IResult BulkInsertLiquidForUser(Guid liquidGuid) =>
            _dependencyLiquidForUserDalMethod.BulkInsertLiquidForUser(liquidGuid);

        public IResult BulkInsertLiquidCurrencyForUser(string liquids, Guid userId) =>
            _dependencyLiquidForUserDalMethod.BulkInsertLiquidCurrencyForUser(liquids, userId);
    }
}