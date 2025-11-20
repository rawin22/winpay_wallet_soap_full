using System;
using System.Collections.Generic;
using TSG.Models.DTO.InstantPayment;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.InstantPayment
{
    public interface IInstantPaymentReceiveMappingMethods : ICrud<InstantPaymentReceiveMappingDto, Guid>, IInsertByT<InstantPaymentReceiveMappingDto>
    {
        IResult<List<InstantPaymentReceiveMappingDto>> GetByInstantPaymentReceiveId(Guid instantPaymentReceiveId);
    }
}