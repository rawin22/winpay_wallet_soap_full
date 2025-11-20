using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using TSG.Dal.Interfaces.Fundings;
using TSG.Models.DTO;

namespace TSG.Dal.Fundings
{
    public class FundingChangesRepository : IFundingChangesRepository
    {
        public List<FundChangesDto> GetAllChangeById(Guid fundingId)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.Query<FundChangesDto>("GetFundChanges", param: new { fundingId }, commandType: CommandType.StoredProcedure).ToList();
            }
        }
    }
}