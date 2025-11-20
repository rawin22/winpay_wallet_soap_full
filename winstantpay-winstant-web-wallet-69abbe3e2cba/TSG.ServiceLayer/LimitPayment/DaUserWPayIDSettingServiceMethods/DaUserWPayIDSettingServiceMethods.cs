using System;
using System.Collections.Generic;
using TSG.Dal.LimitPayment.DaUserWPayIDSettingDalMethods;
using TSG.Models.DTO.LimitPayment;
using TSG.Models.ServiceModels.LimitPayment;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.LimitPayment.DaUserWPayIDSettingServiceMethods
{
    public class DaUserWPayIDSettingServiceMethods : IDaUserWPayIDSettingServiceMethods
    {
        private readonly IDaUserWPayIDSettingDal _daUserWPayIDSettingsDal;
        //        private readonly IDaPayLimitsLogDal _daPayLimitLogMethodsDal;

        public DaUserWPayIDSettingServiceMethods(IDaUserWPayIDSettingDal daUserWPayIDSettingsDal) => _daUserWPayIDSettingsDal = daUserWPayIDSettingsDal;

        public IResult Delete(Guid id) => _daUserWPayIDSettingsDal.Delete(id);

        public IResult Update(DaUserWPayIDSettingSo model) =>
            _daUserWPayIDSettingsDal.Update(AutoMapper.Mapper.Map<DaUserWPayIDSettingDto>(model));

        public IResult Insert(DaUserWPayIDSettingSo model) => _daUserWPayIDSettingsDal.Insert(AutoMapper.Mapper.Map<DaUserWPayIDSettingDto>(model));

        public IResult<DaUserWPayIDSettingSo> GetById(Guid id) 
        {
            var res = _daUserWPayIDSettingsDal.GetById(id);
            return  new Result<DaUserWPayIDSettingSo>(AutoMapper.Mapper.Map<DaUserWPayIDSettingSo>(res.Obj), res.Message);
        }
        
        public IResult<DaUserWPayIDSettingSo> GetByWPayId(string wpayId) 
        {
            var res = _daUserWPayIDSettingsDal.GetByWPayId(wpayId);
            return  new Result<DaUserWPayIDSettingSo>(AutoMapper.Mapper.Map<DaUserWPayIDSettingSo>(res.Obj), res.Message);
        }

        public IResult<List<DaUserWPayIDSettingSo>> GetAll()
        {
            var res = _daUserWPayIDSettingsDal.GetAll();
            return new Result<List<DaUserWPayIDSettingSo>>(AutoMapper.Mapper.Map<List<DaUserWPayIDSettingSo>>(res.Obj), res.Message);
        }

    }
}