using System;
using System.Collections.Generic;
using TSG.Dal.AutomaticExchange.LiquidCcyListDalMethods;
using TSG.Models.DTO.AutomaticExchanges;
using TSG.Models.ServiceModels.AutomaticExchange;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.AutomaticExchange.LiquidCcyListServiceMethods
{
    public class LiquidCcyListServiceMethods : ILiquidCcyListServiceMethods
    {
        private readonly ILiquidCcyListDalMethods _liquidCcyListDalMethods;
        public LiquidCcyListServiceMethods(ILiquidCcyListDalMethods liquidCcyListDalMethods) => _liquidCcyListDalMethods = liquidCcyListDalMethods;

        public IResult Delete(Guid id) => _liquidCcyListDalMethods.Delete(id);
        public IResult Update(LiquidCcyListSo model) => _liquidCcyListDalMethods.Update(AutoMapper.Mapper.Map<LiquidCcyListDto>(model));
        public IResult DeleteByCurrencyId(int ccyId) => _liquidCcyListDalMethods.DeleteByCurrencyId(ccyId);

        public IResult<List<LiquidCcyListSo>> GetAll()
        {
            var res = _liquidCcyListDalMethods.GetAll();
            if(!res.Success || res.Obj == null)
                return new Result<List<LiquidCcyListSo>>(res.Message);
            return new Result<List<LiquidCcyListSo>>(AutoMapper.Mapper.Map<List<LiquidCcyListSo>>(res.Obj));
        }

        public IResult<LiquidCcyListSo> GetById(Guid id)
        {
            var res = _liquidCcyListDalMethods.GetById(id);
            if (!res.Success || res.Obj == null)
                return new Result<LiquidCcyListSo>(res.Message);
            return new Result<LiquidCcyListSo>(AutoMapper.Mapper.Map<LiquidCcyListSo>(res.Obj));
        }
        
        public IResult<LiquidCcyListSo> GetLiquidCcyElementByCurrencyId(int ccyId)
        {
            var res = _liquidCcyListDalMethods.GetLiquidCcyElementByCurrencyId(ccyId);
            if (!res.Success || res.Obj == null)
                return new Result<LiquidCcyListSo>(res.Message);
            return new Result<LiquidCcyListSo>(AutoMapper.Mapper.Map<LiquidCcyListSo>(res.Obj));
        }

        public IResult<LiquidCcyListSo> GetLiquidCcyElementByCurrencyCode(string ccyCode)
        {
            var res = _liquidCcyListDalMethods.GetLiquidCcyElementByCurrencyCode(ccyCode);
            if (!res.Success || res.Obj == null)
                return new Result<LiquidCcyListSo>(res.Message);
            return new Result<LiquidCcyListSo>(AutoMapper.Mapper.Map<LiquidCcyListSo>(res.Obj));
        }

        public IResult BulkUpdateOrder(string ids) => _liquidCcyListDalMethods.BulkUpdateOrder(ids);

        public IResult<LiquidCcyListSo> Insert(LiquidCcyListSo model)
        {
            model.LiquidCcyList_Id = Guid.NewGuid();
            var insertRes = _liquidCcyListDalMethods.Insert(AutoMapper.Mapper.Map<LiquidCcyListDto>(model));
            if(!insertRes.Success)
                return new Result<LiquidCcyListSo>(insertRes.Message);
            return new Result<LiquidCcyListSo>(model);
        }
    }
}