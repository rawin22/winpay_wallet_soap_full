using System;
using TSG.Models.DTO.InstantPayment;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.InstantPayment
{
    public interface IInstantPaymentReceiveMethods : ICrud<InstantPaymentReceiveDto, Guid>, IInsertByT<InstantPaymentReceiveDto>, ICrudByUser<InstantPaymentReceiveDto>
    {
        
    }
}