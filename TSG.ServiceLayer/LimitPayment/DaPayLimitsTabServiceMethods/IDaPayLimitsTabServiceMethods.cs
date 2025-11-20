using System;
using System.Collections.Generic;
using TSG.Models.ServiceModels.LimitPayment;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.LimitPayment.DaPayLimitsTabServiceMethods
{
    public interface IDaPayLimitsTabServiceMethods : ICrud<DaPayLimitsTabSo, Guid>, IInsertByT<DaPayLimitsTabSo>, IGetById<DaPayLimitsTabSo, Guid>
    {
        IResult<List<DaPayLimitsTabSo>> GetAllLimitsByKey(Guid parentId);
    }
}