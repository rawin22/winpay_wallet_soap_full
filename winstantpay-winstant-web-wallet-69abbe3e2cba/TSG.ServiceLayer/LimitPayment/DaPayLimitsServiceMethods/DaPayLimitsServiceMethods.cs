using System;
using System.Collections.Generic;
using System.Linq;
using TSG.Dal.LimitPayment.DaPayLimitsDalMethods;
using TSG.Models.APIModels;
using TSG.Models.DTO.LimitPayment;
using TSG.Models.ServiceModels.LimitPayment;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsLogServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsSourceTypeServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTabServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTypeServiceMethods;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.LimitPayment.DaPayLimitsServiceMethods
{
    public class DaPayLimitsServiceMethods : IDaPayLimitsServiceMethods
    {
        private readonly IDaPayLimitsDal _daForPayLimitsMethodsDal;

        private readonly IDaPayLimitsTabServiceMethods _daPayLimitsTabServiceMethods;
        private readonly IDaPayLimitsLogServiceMethods _daPayLimitsLogServiceMethods;
        private readonly IDaPayLimitsSourceTypeServiceMethods _daPayLimitsSourceTypeServiceMethods;
        private readonly IDaPayLimitsTypeServiceMethods _daPayLimitsTypeServiceMethods;

        public DaPayLimitsServiceMethods(IDaPayLimitsDal qrForPayLimitsMethodsDal,
            IDaPayLimitsTabServiceMethods daPayLimitsTabServiceMethods,
            IDaPayLimitsLogServiceMethods daPayLimitsLogServiceMethods,
            IDaPayLimitsSourceTypeServiceMethods daPayLimitsSourceTypeServiceMethods,
            IDaPayLimitsTypeServiceMethods daPayLimitsTypeServiceMethods)
        {
            _daForPayLimitsMethodsDal = qrForPayLimitsMethodsDal;
            _daPayLimitsTabServiceMethods = daPayLimitsTabServiceMethods;
            _daPayLimitsLogServiceMethods = daPayLimitsLogServiceMethods;
            _daPayLimitsSourceTypeServiceMethods = daPayLimitsSourceTypeServiceMethods;
            _daPayLimitsTypeServiceMethods = daPayLimitsTypeServiceMethods;
        }


        public IResult Delete(Guid id) => _daForPayLimitsMethodsDal.Delete(id);

        public IResult Update(DaPayLimitsSo model) =>
            _daForPayLimitsMethodsDal.Update(AutoMapper.Mapper.Map<DaPayLimitsDto>(model));

        public IResult<DaPayLimitsSo> Insert(DaPayLimitsSo model) => _daForPayLimitsMethodsDal.Insert(model);

        public IResult<List<DaPayLimitsSo>> GetAll()
        {
            var queryRes = _daForPayLimitsMethodsDal.GetAllSo();
            if (queryRes.Obj == null)
                return new Result<List<DaPayLimitsSo>>(new List<DaPayLimitsSo>(), queryRes.Message);
            return new Result<List<DaPayLimitsSo>>(queryRes.Obj,
                queryRes.Message);
        }

        public IResult<DaPayLimitsSo> GetById(Guid id)
        {
            try
            {
                var res = _daForPayLimitsMethodsDal.GetById(id);
                if (!res.Success || res.Obj == null)
                    return new Result<DaPayLimitsSo>(String.IsNullOrEmpty(res.Message)
                        ? "Object not found"
                        : res.Message);
                var resObjSo = AutoMapper.Mapper.Map<DaPayLimitsSo>(res.Obj);
                if (resObjSo == null)
                    return new Result<DaPayLimitsSo>(String.IsNullOrEmpty(res.Message)
                        ? "Object is not mapped"
                        : res.Message);
                //resObjSo.TotalListOfLimits = _daPayLimitsTypeServiceMethods.GetAll().Obj;
                //resObjSo.DaPayLimitsTabs = _daPayLimitsTabServiceMethods.GetAllLimitsByKey(id).Obj;

                //var newTabs = from types in resObjSo.TotalListOfLimits.OrderBy(ob => ob.DaPayLimitsType_LimitType).Where(w => !w.DaPayLimitsType_IsDeleted)
                //    join tabs in resObjSo.DaPayLimitsTabs
                //        on types.DaPayLimitsType_ID equals tabs.DaPayLimitsTab_TypeOfLimit into ps
                //    from tabs in ps.DefaultIfEmpty()
                //    select new DaPayLimitsTabSo()
                //    {
                //        DaPayLimitsTab_ID = tabs?.DaPayLimitsTab_ID ?? Guid.Empty,
                //        DaPayLimitsTab_Amount = tabs?.DaPayLimitsTab_Amount ?? 0,
                //        DaPayLimitsTab_TypeOfLimit = types.DaPayLimitsType_ID,
                //        DaPayLimitsTab_IsDeleted = tabs == null || tabs.DaPayLimitsTab_ID == default || tabs.DaPayLimitsTab_IsDeleted,
                //        DaPayLimitsTab_ParentDaPayId = resObjSo.DaPayLimits_ID,
                //        DaPayLimitsTab_DaPayLimitsType = types
                //    };
                //resObjSo.DaPayLimitsTabs = newTabs.ToList();

                resObjSo.DaPayLimitsLogs =
                    _daPayLimitsLogServiceMethods.GetAllByDaParentId(resObjSo.DaPayLimits_ID).Obj?.Where(w=>w.DaPayLimitsLog_UserName == resObjSo.DaPayLimits_UserName).OrderByDescending(ob=>ob.DaPayLimitsLog_CreateDate).ToList() ??
                    new List<DaPayLimitsLogSo>();
                resObjSo.DaPaymentLimitSourceType =
                    _daPayLimitsSourceTypeServiceMethods.GetById(resObjSo.DaPayLimits_SourceType).Obj;

                resObjSo.Success = true; resObjSo.InfoBlock = new InfoBlock();
                return new Result<DaPayLimitsSo>(resObjSo);
            }
            catch (Exception e)
            {
                return new Result<DaPayLimitsSo>(e.Message);
            }
        }

        public IResult<List<DaPayLimitsSo>> GetAllDaByUserName(string userName) =>
            _daForPayLimitsMethodsDal.GetAllDaByUserName(userName);

        public IResult<List<DaPayLimitsSo>> GetAllDaByDeviceCode(string deviceCode) =>
            _daForPayLimitsMethodsDal.GetAllDaByDeviceCode(deviceCode);

        public IResult<List<string>> GetRandomWordBySecretPhrase() =>
            _daForPayLimitsMethodsDal.GetRandomWordBySecretPhrase();
    }
}