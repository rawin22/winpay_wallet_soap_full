using System;
using System.Collections.Generic;
using TSG.Dal.LimitPayment.DaPayLimitsTabDalMethods;
using TSG.Models.DTO.LimitPayment;
using TSG.Models.ServiceModels.LimitPayment;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTypeServiceMethods;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.LimitPayment.DaPayLimitsTabServiceMethods
{
    public class DaPayLimitsTabServiceMethods : IDaPayLimitsTabServiceMethods
    {
        private readonly IDaPayLimitsTabDal _daPaymentLimitTabMethodsDal;
        private readonly IDaPayLimitsTypeServiceMethods _daPayLimitsTaDaPayLimitsTypeServiceMethods;

        public DaPayLimitsTabServiceMethods(IDaPayLimitsTabDal qrPaymentLimitTabMethodsDal,
            IDaPayLimitsTypeServiceMethods daPayLimitsTaDaPayLimitsTypeServiceMethods)
        {
            _daPayLimitsTaDaPayLimitsTypeServiceMethods = daPayLimitsTaDaPayLimitsTypeServiceMethods;
            _daPaymentLimitTabMethodsDal = qrPaymentLimitTabMethodsDal;
        }

        public IResult Delete(Guid id) => _daPaymentLimitTabMethodsDal.Delete(id);

        public IResult Update(DaPayLimitsTabSo model) =>
            _daPaymentLimitTabMethodsDal.Update(AutoMapper.Mapper.Map<DaPayLimitsTabDto>(model));

        public IResult<List<DaPayLimitsTabSo>> GetAll()
        {
            var queryRes = _daPaymentLimitTabMethodsDal.GetAll();
            return new Result<List<DaPayLimitsTabSo>>(AutoMapper.Mapper.Map<List<DaPayLimitsTabSo>>(queryRes.Obj), queryRes.Message);
        }

        public IResult<DaPayLimitsTabSo> Insert(DaPayLimitsTabSo model) => _daPaymentLimitTabMethodsDal.Insert(model);
        
        public IResult<List<DaPayLimitsTabSo>> GetAllLimitsByKey(Guid parentId)
        {
            var queryRes = _daPaymentLimitTabMethodsDal.GetAllLimitsByKey(parentId);
            queryRes.Obj.ForEach(f =>
                {
                    f.DaPayLimitsTab_DaPayLimitsType = _daPayLimitsTaDaPayLimitsTypeServiceMethods
                        .GetById(f.DaPayLimitsTab_TypeOfLimit).Obj;
                });

            return new Result<List<DaPayLimitsTabSo>>(AutoMapper.Mapper.Map<List<DaPayLimitsTabSo>>(queryRes.Obj), queryRes.Message);
        }

        public IResult<DaPayLimitsTabSo> GetById(Guid id) => _daPaymentLimitTabMethodsDal.GetById(id);
    }
}