using System;
using System.Collections.Generic;
using TSG.Models.ServiceModels.LimitPayment;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.LimitPayment.DaPayLimitsServiceMethods
{
    public interface IDaPayLimitsServiceMethods : ICrud<DaPayLimitsSo, Guid>, IInsertByT<DaPayLimitsSo>, IGetById<DaPayLimitsSo, Guid>
    {
        IResult<List<DaPayLimitsSo>> GetAllDaByUserName(string userName);
        IResult<List<DaPayLimitsSo>> GetAllDaByDeviceCode(string deviceCode);
        IResult<List<string>> GetRandomWordBySecretPhrase();
    }
}