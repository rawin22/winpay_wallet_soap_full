using System.Collections.Generic;
using TSG.Dal.Transfers.Reports;
using TSG.Models.ServiceModels.Transfers.Reports;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.Transfers.Reports
{
    public class GetInboxListMethods : IGetInboxListMethods
    {
        private readonly IGetInboxReportRepository _getInboxReportRepository;

        public GetInboxListMethods(IGetInboxReportRepository getInboxReportRepository)
        {
            _getInboxReportRepository = getInboxReportRepository;
        }

        public IResult<List<GetInboxListSo>> GetAll(string parent, string recipient)
        {
            var res = _getInboxReportRepository.GetAll(parent, recipient);
            return new Result<List<GetInboxListSo>>(AutoMapper.Mapper.Map<List<GetInboxListSo>>(res.Obj), res.Message);
        }

        public IResult<List<GetInboxListSo>> GetByParent(string parent)
        {
            var res = _getInboxReportRepository.GetAll(parent, null);
            return new Result<List<GetInboxListSo>>(AutoMapper.Mapper.Map<List<GetInboxListSo>>(res.Obj), res.Message);
        }

        public IResult<List<GetInboxListSo>> GetByRecipient(string recipient)
        {
            var res = _getInboxReportRepository.GetAll(null, recipient);
            return new Result<List<GetInboxListSo>>(AutoMapper.Mapper.Map<List<GetInboxListSo>>(res.Obj), res.Message);
        }
    }
}