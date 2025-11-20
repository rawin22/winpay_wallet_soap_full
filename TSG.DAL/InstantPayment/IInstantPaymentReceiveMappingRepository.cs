using System;
using System.Collections.Generic;
using TSG.Models.DTO.InstantPayment;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.InstantPayment
{
    public interface IInstantPaymentReceiveMappingRepository : ICrud<InstantPaymentReceiveMappingDto, Guid>, IInsert<InstantPaymentReceiveMappingDto>
    {
        IResult<List<InstantPaymentReceiveMappingDto>> GetByInstantPaymentReceiveId(Guid instantPaymentReceiveId);
    }
}