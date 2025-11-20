using System;
using System.Collections.Generic;
using TSG.Models.DTO.LimitPayment;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.LimitPayment.DaPayLimitsLogDalMethods
{
    public interface IDaPayLimitsLogDal : IInsert<DaPayLimitsLogDto>, IGetById<DaPayLimitsLogDto, Guid>
    {    
        IResult<List<DaPayLimitsLogDto>> GetAll();
        IResult<List<DaPayLimitsLogDto>> GetAllByDaParentId(Guid daId);
    }
}