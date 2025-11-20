using System.Collections.Generic;
using TSG.Dal.SuperAdmin;
using TSG.Models.DTO;
using TSG.Models.ServiceModels.SuperUserModels;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.SuperAdmin
{
    public class SaSharedLinkForAdminServiceMethods : ISaSharedLinkForAdminServiceMethods
    {
        private readonly ISaSharedLinkForAdminRepository _saSharedLinkForAdminRepository;

        public SaSharedLinkForAdminServiceMethods(ISaSharedLinkForAdminRepository saSharedLinkForAdminRepository)
        {
            _saSharedLinkForAdminRepository = saSharedLinkForAdminRepository;
        }

        public IResult ClearAllOldReferences(string userName) => _saSharedLinkForAdminRepository.ClearAllOldReferences(userName);
        
        public IResult Delete(string userName) => _saSharedLinkForAdminRepository.Delete(userName);

        public IResult Update(SharedAdminLinkSo model) => _saSharedLinkForAdminRepository.Update(AutoMapper.Mapper.Map<SharedAdminLinkDto>(model));

        public IResult Insert(SharedAdminLinkSo model)=> _saSharedLinkForAdminRepository.Insert(AutoMapper.Mapper.Map<SharedAdminLinkDto>(model));

        public IResult<int> IfAdminNeedChangePassword(string userName) => _saSharedLinkForAdminRepository.IfAdminNeedChangePassword(userName);
        
        public IResult<List<SharedAdminLinkSo>> GetAll()
        {
            var allRecs = _saSharedLinkForAdminRepository.GetAll();
            return  new Result<List<SharedAdminLinkSo>>(AutoMapper.Mapper.Map<List<SharedAdminLinkSo>>(allRecs.Obj), allRecs.Message);
        }
    }
}