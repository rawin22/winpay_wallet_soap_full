using System.Collections.Generic;
using TSG.Models.DTO.Transfers.Report;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.Transfers.Reports
{
    public interface IGetInboxReportRepository
    {
        IResult<List<GetInboxListDto>> GetAll(string parent, string recipient);
    }
}