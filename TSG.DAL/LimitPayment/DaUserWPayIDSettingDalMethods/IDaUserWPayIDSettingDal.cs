using System;
using System.Collections.Generic;
using TSG.Models.DTO.LimitPayment;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.LimitPayment.DaUserWPayIDSettingDalMethods
{
    public interface IDaUserWPayIDSettingDal : ICrud<DaUserWPayIDSettingDto, Guid>, IInsert<DaUserWPayIDSettingDto>, IGetById<DaUserWPayIDSettingDto, Guid>
    {    
        IResult<List<DaUserWPayIDSettingDto>> GetAll();
        IResult<DaUserWPayIDSettingDto> GetByWPayId(string wpayID);
    }
}