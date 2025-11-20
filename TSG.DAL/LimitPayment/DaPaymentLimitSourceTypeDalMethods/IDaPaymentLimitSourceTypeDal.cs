using System;
using TSG.Models.DTO.LimitPayment;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.LimitPayment.DaPaymentLimitSourceTypeDalMethods
{
    public interface IDaPaymentLimitSourceTypeDal : ICrud<DaPaymentLimitSourceTypeDto, Guid>, IInsert<DaPaymentLimitSourceTypeDto>, IGetById<DaPaymentLimitSourceTypeDto, Guid>
    {    
    }
}