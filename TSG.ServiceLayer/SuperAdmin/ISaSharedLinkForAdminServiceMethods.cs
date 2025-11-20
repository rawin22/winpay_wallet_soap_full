using TSG.Models.ServiceModels.SuperUserModels;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.SuperAdmin
{
    public interface ISaSharedLinkForAdminServiceMethods : ICrud<SharedAdminLinkSo, string>, IInsert<SharedAdminLinkSo>
    {
        IResult ClearAllOldReferences(string userName);
        IResult<int> IfAdminNeedChangePassword(string userName);
    }
}