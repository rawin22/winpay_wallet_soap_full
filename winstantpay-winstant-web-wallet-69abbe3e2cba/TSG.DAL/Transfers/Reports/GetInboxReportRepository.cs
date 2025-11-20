using System.Collections.Generic;
using System.Data;
using TSG.Models.DTO.Transfers.Report;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.Transfers.Reports
{
    public class GetInboxReportRepository : IGetInboxReportRepository
    {
        public IResult<List<GetInboxListDto>> GetAll(string parent, string recipient)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryResult<GetInboxListDto>("GetInboxList", new { parent = parent, recepient = recipient }, CommandType.StoredProcedure);
            }
        }
    }
}