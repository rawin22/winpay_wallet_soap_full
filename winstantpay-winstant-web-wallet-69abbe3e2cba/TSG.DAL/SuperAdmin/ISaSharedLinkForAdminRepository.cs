using System;
using TSG.Models.DTO;
using TSG.Models.DTO.Transfers;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.SuperAdmin
{
    public interface ISaSharedLinkForAdminRepository : ICrud<SharedAdminLinkDto, string>, IInsert<SharedAdminLinkDto>
    {
        IResult ClearAllOldReferences(string userName);
        IResult<int> IfAdminNeedChangePassword(string userName);
    }
}