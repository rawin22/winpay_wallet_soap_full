using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TSG.Dal.Interfaces.Fundings;
using TSG.Models.DTO;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.Fundings
{
    public class FundingSourcesRepository : IFundingSourcesRepository
    {
        public IResult Delete(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                string sql = "UPDATE dbo.FundingSources SET IsDeleted = 1 WHERE ID = @id";
                return connection.ExecuteResult(sql, new { id });
            }
        }

        public IResult<FundingSourcesDto> Insert(FundingSourcesDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                var sql = "INSERT INTO dbo.FundingSources (ID, SourceName) OUTPUT INSERTED.* VALUES (@id, @sourceName)";
                return connection.QueryReturnResult<FundingSourcesDto>(sql, new {id = model.ID, sourceName = model.SourceName });
            }
        }

        public IResult Update(FundingSourcesDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.UpdateResult(model);
            }
        }

        public IResult<List<FundingSourcesDto>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryResult<FundingSourcesDto>("SELECT ID, [SourceName], [DesignName], [IsDeleted] FROM dbo.FundingSources");
            }
        }

        public IResult<FundingSourcesDto> GetById(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QuerySingleResult<FundingSourcesDto>("SELECT ID, [SourceName], [DesignName], [IsDeleted] FROM dbo.FundingSources WHERE ID = @id", new {id});
            }
        }
    }
}