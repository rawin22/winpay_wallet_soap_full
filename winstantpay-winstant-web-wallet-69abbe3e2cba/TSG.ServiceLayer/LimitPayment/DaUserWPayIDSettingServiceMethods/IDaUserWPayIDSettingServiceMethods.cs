using System;
using System.Collections.Generic;
using TSG.Models.ServiceModels.LimitPayment;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.LimitPayment.DaUserWPayIDSettingServiceMethods
{
    public interface IDaUserWPayIDSettingServiceMethods : ICrud<DaUserWPayIDSettingSo, Guid>, IInsert<DaUserWPayIDSettingSo>, IGetById<DaUserWPayIDSettingSo, Guid>
    {
        IResult<List<DaUserWPayIDSettingSo>> GetAll();
        IResult<DaUserWPayIDSettingSo> GetByWPayId(string wpayId);
    }
}