using System;
using System.Collections.Generic;
using TSG.Dal.InstantPayment;
using TSG.Models.DTO.InstantPayment;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.InstantPayment
{
    public class InstantPaymentReceiveMethods : IInstantPaymentReceiveMethods
    {
        private readonly IInstantPaymentReceiveRepository _instantPaymentReceiveRepository;

        public InstantPaymentReceiveMethods(IInstantPaymentReceiveRepository instantPaymentReceiveRepository)
        {
            _instantPaymentReceiveRepository = instantPaymentReceiveRepository;
        }

        public IResult Delete(Guid id) => _instantPaymentReceiveRepository.Delete(id);
        public IResult Update(InstantPaymentReceiveDto model)
        {
            model.LastModifiedDate = DateTime.UtcNow;
            var res = _instantPaymentReceiveRepository.Update(model);
            return new Result<InstantPaymentReceiveDto>(model, res.Message);
        }        
        
        // public IResult Update(InstantPaymentReceiveDto model) => _instantPaymentReceiveRepository.Update(model);

        public IResult<List<InstantPaymentReceiveDto>> GetAll() => _instantPaymentReceiveRepository.GetAll();
        public IResult<List<InstantPaymentReceiveDto>> GetByUser(Guid userId) => _instantPaymentReceiveRepository.GetByUser(userId);

        public IResult<InstantPaymentReceiveDto> Insert(InstantPaymentReceiveDto model)
        {
            model.Id = Guid.NewGuid();
            model.CreatedDate = DateTime.UtcNow;
            model.LastModifiedDate = DateTime.UtcNow;
            var res = _instantPaymentReceiveRepository.Insert(model);
            return new Result<InstantPaymentReceiveDto>(model, res.Message);
        }
    }
}