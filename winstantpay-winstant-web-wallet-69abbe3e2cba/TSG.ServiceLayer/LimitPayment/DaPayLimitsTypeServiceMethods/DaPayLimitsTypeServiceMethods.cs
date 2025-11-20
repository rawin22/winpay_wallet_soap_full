using System;
using System.Collections.Generic;
using TSG.Dal.LimitPayment.DaPayLimitsTypeDalMethods;
using TSG.Models.DTO.LimitPayment;
using TSG.Models.Enums;
using TSG.Models.ServiceModels.LimitPayment;
using WinstantPay.Common.Extension;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.LimitPayment.DaPayLimitsTypeServiceMethods
{
    public class DaPayLimitsTypeServiceMethods : IDaPayLimitsTypeServiceMethods
    {
        private readonly IDaPayLimitsTypeDal _paymenListMethodsDal;

        public DaPayLimitsTypeServiceMethods(IDaPayLimitsTypeDal paymenListMethodsDal) => _paymenListMethodsDal = paymenListMethodsDal;

        public IResult Delete(Guid id) => _paymenListMethodsDal.Delete(id);

        public IResult Update(DaPayLimitsTypeSo model) => _paymenListMethodsDal.Update(AutoMapper.Mapper.Map<DaPayLimitsTypeDto>(model));

        public IResult Insert(DaPayLimitsTypeSo model) => _paymenListMethodsDal.Insert(AutoMapper.Mapper.Map<DaPayLimitsTypeDto>(model));

        public IResult<List<DaPayLimitsTypeSo>> GetAll()
        {
            var queryRes = _paymenListMethodsDal.GetAll();
            if(queryRes.Obj == null)
                return new Result<List<DaPayLimitsTypeSo>>(new List<DaPayLimitsTypeSo>(), queryRes.Message);           
            var mappedObj = AutoMapper.Mapper.Map<List<DaPayLimitsTypeSo>>(queryRes.Obj);
            mappedObj.ForEach(f =>
            {
                f.DaPayLimitsType_NameOfPaymentLimit =
                    EnumHelper<DelegatedAuthoriryLimitationType>.GetDisplayValue(
                        (DelegatedAuthoriryLimitationType)f.DaPayLimitsType_LimitType);
            });
            return new Result<List<DaPayLimitsTypeSo>>(AutoMapper.Mapper.Map<List<DaPayLimitsTypeSo>>(queryRes.Obj), queryRes.Message);
        }

        public IResult<DaPayLimitsTypeSo> GetById(Guid id)
        {
            var queryRes = _paymenListMethodsDal.GetById(id);
            if (queryRes.Obj == null)
                return new Result<DaPayLimitsTypeSo>(null, queryRes.Message);
            var mappedObj = AutoMapper.Mapper.Map<DaPayLimitsTypeSo>(queryRes.Obj);
            mappedObj.DaPayLimitsType_NameOfPaymentLimit = EnumHelper<DelegatedAuthoriryLimitationType>.GetDisplayValue(
                (DelegatedAuthoriryLimitationType)mappedObj.DaPayLimitsType_LimitType);
            return new Result<DaPayLimitsTypeSo>(mappedObj, queryRes.Message);

        }
    }
}