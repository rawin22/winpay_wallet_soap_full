using System;
using System.Collections.Generic;
using TSG.Dal.AutomaticExchange.LiquidOverDraftUserDalMethods;
using TSG.Models.DTO.AutomaticExchanges;
using TSG.Models.ServiceModels.AutomaticExchange;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.AutomaticExchange.LiquidOverDraftUserServiceMethods
{
    public class LiquidOverDraftUserServiceMethods : ILiquidOverDraftUserServiceMethods
    {
        private readonly ILiquidOverDraftUserDalMethods _dalMethods;

        public LiquidOverDraftUserServiceMethods(ILiquidOverDraftUserDalMethods dalMethods)
        {
            _dalMethods = dalMethods;
        }

        public IResult Delete(Guid id) => _dalMethods.Delete(id);

        public IResult Update(LiquidOverDraftUserListSo model)
        {
            var res = _dalMethods.Update(AutoMapper.Mapper.Map<LiquidOverDraftUserListDto>(model));
            if (!res.Success)
                return new Result<LiquidOverDraftUserListSo>(res.Message);
            return new Result<LiquidOverDraftUserListSo>(model);
        }

        public IResult<List<LiquidOverDraftUserListSo>> GetAll()
        {
            var res = _dalMethods.GetAll();
            if (!res.Success)
                return new Result<List<LiquidOverDraftUserListSo>>(res.Message);
            return new Result<List<LiquidOverDraftUserListSo>>(AutoMapper.Mapper.Map<List<LiquidOverDraftUserListSo>>(res.Obj));
        }

        public IResult<LiquidOverDraftUserListSo> Insert(LiquidOverDraftUserListSo model)
        {
            if(model.LiquidOverDraftUserList_Id == default)
                model.LiquidOverDraftUserList_Id = Guid.NewGuid();

            var res = _dalMethods.Insert(AutoMapper.Mapper.Map<LiquidOverDraftUserListDto>(model));
            if(!res.Success)
                return new Result<LiquidOverDraftUserListSo>(res.Message);
            return new Result<LiquidOverDraftUserListSo>(model);
        }

        public IResult<LiquidOverDraftUserListSo> GetById(Guid id)
        {
            var res = _dalMethods.GetById(id);
            if (!res.Success || res.Obj == null)
                return new Result<LiquidOverDraftUserListSo>(res.Message);
            return new Result<LiquidOverDraftUserListSo>(AutoMapper.Mapper.Map<LiquidOverDraftUserListSo>(res.Obj));
        }

        public IResult<List<LiquidOverDraftUserListSo>> GetAllSo() => _dalMethods.GetAllSo();

        public IResult DeleteByCurrencyId(int ccyId) => _dalMethods.DeleteByCurrencyId(ccyId);

        public IResult DeleteByLiquidCurrencyId(Guid liquidCcyId) => _dalMethods.DeleteByLiquidCurrencyId(liquidCcyId);
    }
}