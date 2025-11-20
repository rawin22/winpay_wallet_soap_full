using System;
using System.Collections.Generic;
using TSG.Dal.LimitPayment.DaPayLimitsLogDalMethods;
using TSG.Models.DTO.LimitPayment;
using TSG.Models.ServiceModels.LimitPayment;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.LimitPayment.DaPayLimitsLogServiceMethods
{
    public class DaPayLimitsLogServiceMethods : IDaPayLimitsLogServiceMethods
    {
        private readonly IDaPayLimitsLogDal _daPayLimitLogMethodsDal;

        public DaPayLimitsLogServiceMethods(IDaPayLimitsLogDal qrPayLimitLogMethodsDal) => _daPayLimitLogMethodsDal = qrPayLimitLogMethodsDal;

        public IResult Insert(DaPayLimitsLogSo model) => _daPayLimitLogMethodsDal.Insert(AutoMapper.Mapper.Map<DaPayLimitsLogDto>(model));

        public IResult<DaPayLimitsLogSo> GetById(Guid id) 
        {
            var res = _daPayLimitLogMethodsDal.GetById(id);
            return  new Result<DaPayLimitsLogSo>(AutoMapper.Mapper.Map<DaPayLimitsLogSo>(res.Obj), res.Message);
        }

        public IResult<List<DaPayLimitsLogSo>> GetAll()
        {
            var res = _daPayLimitLogMethodsDal.GetAll();
            return new Result<List<DaPayLimitsLogSo>>(AutoMapper.Mapper.Map<List<DaPayLimitsLogSo>>(res.Obj), res.Message);
        }

        public IResult<List<DaPayLimitsLogSo>> GetAllByDaParentId(Guid daId)
        {
            var res = _daPayLimitLogMethodsDal.GetAllByDaParentId(daId);
            return new Result<List<DaPayLimitsLogSo>>(AutoMapper.Mapper.Map<List<DaPayLimitsLogSo>>(res.Obj), res.Message);
        }
    }
}