using System;
using System.Collections.Generic;
using TSG.Dal.InstantPayment;
using TSG.Models.DTO.InstantPayment;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.InstantPayment
{
    public class InstantPaymentReceiveMappingMethods : IInstantPaymentReceiveMappingMethods
    {
        private readonly IInstantPaymentReceiveMappingRepository _instantPaymentReceiveMappingRepository;

        public InstantPaymentReceiveMappingMethods(IInstantPaymentReceiveMappingRepository instantPaymentReceiveMappingRepository)
        {
            _instantPaymentReceiveMappingRepository = instantPaymentReceiveMappingRepository;
        }

        public IResult Delete(Guid id) => _instantPaymentReceiveMappingRepository.Delete(id);
        
        public IResult Update(InstantPaymentReceiveMappingDto model) => _instantPaymentReceiveMappingRepository.Update(model);

        public IResult<List<InstantPaymentReceiveMappingDto>> GetAll() => _instantPaymentReceiveMappingRepository.GetAll();
        public IResult<List<InstantPaymentReceiveMappingDto>> GetByInstantPaymentReceiveId(Guid instantPaymentReceiveId) => _instantPaymentReceiveMappingRepository.GetByInstantPaymentReceiveId(instantPaymentReceiveId);

        public IResult<InstantPaymentReceiveMappingDto> Insert(InstantPaymentReceiveMappingDto model)
        {
            model.Id = Guid.NewGuid();
            model.CreatedDate = DateTime.UtcNow;
            var res = _instantPaymentReceiveMappingRepository.Insert(model);
            return new Result<InstantPaymentReceiveMappingDto>(model, res.Message);
        }
    }
}