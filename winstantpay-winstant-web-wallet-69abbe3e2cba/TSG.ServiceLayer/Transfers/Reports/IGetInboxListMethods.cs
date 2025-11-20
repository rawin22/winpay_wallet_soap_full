using System.Collections.Generic;
using TSG.Models.ServiceModels.Transfers.Reports;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.Transfers.Reports
{
    public interface IGetInboxListMethods
    {
        IResult<List<GetInboxListSo>> GetAll(string parent, string recipient);
        IResult<List<GetInboxListSo>> GetByParent(string parent);
        IResult<List<GetInboxListSo>> GetByRecipient(string recipient);
    }
}