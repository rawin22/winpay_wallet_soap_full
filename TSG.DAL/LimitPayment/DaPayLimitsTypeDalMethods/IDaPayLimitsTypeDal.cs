using System;
using TSG.Models.DTO.LimitPayment;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.LimitPayment.DaPayLimitsTypeDalMethods
{
    public interface IDaPayLimitsTypeDal : ICrud<DaPayLimitsTypeDto, Guid>, IInsert<DaPayLimitsTypeDto>, IGetById<DaPayLimitsTypeDto, Guid>
    {    
    }
}