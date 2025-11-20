using System;
using System.Collections.Generic;
using TSG.Models.DTO.LimitPayment;
using TSG.Models.ServiceModels.LimitPayment;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.LimitPayment.DaPayLimitsTabDalMethods
{
    public interface IDaPayLimitsTabDal : ICrud<DaPayLimitsTabDto, Guid>, IInsertByT<DaPayLimitsTabSo>, IGetById<DaPayLimitsTabSo, Guid>
    {
        IResult<List<DaPayLimitsTabSo>> GetAllLimitsByKey(Guid parentId);
    }
}