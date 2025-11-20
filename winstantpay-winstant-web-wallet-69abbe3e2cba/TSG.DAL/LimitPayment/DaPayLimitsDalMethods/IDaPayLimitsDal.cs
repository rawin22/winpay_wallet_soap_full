using System;
using System.Collections.Generic;
using TSG.Models.DTO.LimitPayment;
using TSG.Models.ServiceModels.LimitPayment;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.LimitPayment.DaPayLimitsDalMethods
{
    public interface IDaPayLimitsDal : ICrud<DaPayLimitsDto, Guid>, IInsertByT<DaPayLimitsSo>, IGetById<DaPayLimitsDto, Guid>
    {
        IResult<List<DaPayLimitsSo>> GetAllDaByUserName(string userName);
        IResult<List<DaPayLimitsSo>> GetAllSo();
        IResult<List<DaPayLimitsSo>> GetAllDaByDeviceCode(string deviceCode);
        IResult<List<string>> GetRandomWordBySecretPhrase();
    }
}