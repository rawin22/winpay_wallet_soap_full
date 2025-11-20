using System;
using TSG.Models.ServiceModels.LimitPayment;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.LimitPayment.DaPayLimitsSourceTypeServiceMethods
{
    public interface IDaPayLimitsSourceTypeServiceMethods : ICrud<DaPaymentLimitSourceTypeSo, Guid>, IInsert<DaPaymentLimitSourceTypeSo>, IGetById<DaPaymentLimitSourceTypeSo, Guid>
    {    
    }
}