using System;
using System.Collections.Generic;
using TSG.Models.DTO.LimitPayment;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.LimitPayment.DaPayLimitsLogDalMethods
{
    public class DaPayLimitsLogDal : IDaPayLimitsLogDal
    {
        public IResult Insert(DaPayLimitsLogDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.InsertResult(model);
            }
        }

        public IResult<DaPayLimitsLogDto> GetById(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryFirstOrDefaultResult<DaPayLimitsLogDto>("SELECT * FROM dbo.da_PayLimitsLog WHERE ID=@id", new {id});
            }
        }

        public IResult<List<DaPayLimitsLogDto>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryResult<DaPayLimitsLogDto>("SELECT * FROM dbo.da_PayLimitsLog");
            }
        }

        public IResult<List<DaPayLimitsLogDto>> GetAllByDaParentId(Guid daId)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryResult<DaPayLimitsLogDto>("SELECT * FROM dbo.da_PayLimitsLog WHERE DaPayParentId = @daId", new {daId});
            }
        }
    }
}