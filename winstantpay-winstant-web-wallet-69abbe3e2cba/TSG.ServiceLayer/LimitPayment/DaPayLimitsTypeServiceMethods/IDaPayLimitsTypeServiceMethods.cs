using System;
using TSG.Models.ServiceModels.LimitPayment;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.LimitPayment.DaPayLimitsTypeServiceMethods
{
    public interface IDaPayLimitsTypeServiceMethods : ICrud<DaPayLimitsTypeSo, Guid>, IInsert<DaPayLimitsTypeSo>, IGetById<DaPayLimitsTypeSo, Guid>
    {    
    }
}