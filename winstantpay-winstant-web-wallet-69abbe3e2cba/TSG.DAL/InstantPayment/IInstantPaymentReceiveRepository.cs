using System;
using TSG.Models.DTO.InstantPayment;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.InstantPayment
{
    public interface IInstantPaymentReceiveRepository : ICrud<InstantPaymentReceiveDto, Guid>, IInsert<InstantPaymentReceiveDto>, ICrudByUser<InstantPaymentReceiveDto>
    {
        
    }
}