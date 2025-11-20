using System;
using System.Collections.Generic;
using TSG.Dal.LimitPayment.DaPayLimitsTypeDalMethods;
using TSG.Dal.LimitPayment.DaPaymentLimitSourceTypeDalMethods;
using TSG.Models.DTO.LimitPayment;
using TSG.Models.Enums;
using TSG.Models.ServiceModels.LimitPayment;
using WinstantPay.Common.Extension;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.LimitPayment.DaPayLimitsSourceTypeServiceMethods
{
    public class DaPayLimitsSourceTypeServiceMethods : IDaPayLimitsSourceTypeServiceMethods
    {
        private readonly IDaPaymentLimitSourceTypeDal _paymenListSourceMethodsDal;

        public DaPayLimitsSourceTypeServiceMethods(IDaPaymentLimitSourceTypeDal paymenListSourceMethodsDal) => _paymenListSourceMethodsDal = paymenListSourceMethodsDal;

        public IResult Delete(Guid id) => _paymenListSourceMethodsDal.Delete(id);

        public IResult Update(DaPaymentLimitSourceTypeSo model) => _paymenListSourceMethodsDal.Update(AutoMapper.Mapper.Map<DaPaymentLimitSourceTypeDto>(model));

        public IResult Insert(DaPaymentLimitSourceTypeSo model) => _paymenListSourceMethodsDal.Insert(AutoMapper.Mapper.Map<DaPaymentLimitSourceTypeDto>(model));

        public IResult<List<DaPaymentLimitSourceTypeSo>> GetAll()
        {
            var queryRes = _paymenListSourceMethodsDal.GetAll();
            if(queryRes.Obj == null)
                return new Result<List<DaPaymentLimitSourceTypeSo>>(new List<DaPaymentLimitSourceTypeSo>(), queryRes.Message);
            var mappedObj = AutoMapper.Mapper.Map<List<DaPaymentLimitSourceTypeSo>>(queryRes.Obj);

            mappedObj.ForEach(f =>
            {
                f.DaPaymentLimitSourceType_Label =
                    EnumHelper<DelegatedAuthorirySourceLimitationTypeEnum>.GetDisplayValue(
                        (DelegatedAuthorirySourceLimitationTypeEnum)f.DaPaymentLimitSourceType_EnumNumber);
            });

            return new Result<List<DaPaymentLimitSourceTypeSo>>(mappedObj, queryRes.Message);
        }

        public IResult<DaPaymentLimitSourceTypeSo> GetById(Guid id)
        {
            var queryRes = _paymenListSourceMethodsDal.GetById(id);
            if (queryRes.Obj == null)
                return new Result<DaPaymentLimitSourceTypeSo>(null, queryRes.Message);
            var mappedObj = AutoMapper.Mapper.Map<DaPaymentLimitSourceTypeSo>(queryRes.Obj);
            mappedObj.DaPaymentLimitSourceType_Label =
                EnumHelper<DelegatedAuthorirySourceLimitationTypeEnum>.GetDisplayValue(
                    (DelegatedAuthorirySourceLimitationTypeEnum)mappedObj.DaPaymentLimitSourceType_EnumNumber);
            return new Result<DaPaymentLimitSourceTypeSo>(mappedObj, queryRes.Message);

        }
    }
}