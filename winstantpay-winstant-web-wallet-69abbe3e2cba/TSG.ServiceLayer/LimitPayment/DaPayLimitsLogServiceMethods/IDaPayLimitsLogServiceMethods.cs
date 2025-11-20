using System;
using System.Collections.Generic;
using TSG.Models.ServiceModels.LimitPayment;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.LimitPayment.DaPayLimitsLogServiceMethods
{
    public interface IDaPayLimitsLogServiceMethods : IInsert<DaPayLimitsLogSo>, IGetById<DaPayLimitsLogSo, Guid>
    {
        IResult<List<DaPayLimitsLogSo>> GetAll();
        IResult<List<DaPayLimitsLogSo>> GetAllByDaParentId(Guid daId);   
    }
}