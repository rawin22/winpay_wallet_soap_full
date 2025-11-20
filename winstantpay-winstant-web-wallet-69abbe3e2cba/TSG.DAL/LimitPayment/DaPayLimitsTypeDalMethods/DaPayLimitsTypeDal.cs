using System;
using System.Collections.Generic;
using TSG.Models.DTO.LimitPayment;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.LimitPayment.DaPayLimitsTypeDalMethods
{
    public class DaPayLimitsTypeDal : IDaPayLimitsTypeDal
    {
        public IResult Delete(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                string sql = "UPDATE dbo.da_PayLimitsType SET IsDeleted = 1 WHERE ID = @id";
                return connection.ExecuteResult(sql, new { id });
            }
        }

        public IResult Update(DaPayLimitsTypeDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.UpdateResult(model);
            }
        }

        public IResult<List<DaPayLimitsTypeDto>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryResult<DaPayLimitsTypeDto>("SELECT * FROM dbo.da_PayLimitsType");
            }
        }

        public IResult Insert(DaPayLimitsTypeDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                model.ID = Guid.NewGuid();
                return connection.InsertResult(model);
            }
        }

        public IResult<DaPayLimitsTypeDto> GetById(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QuerySingleResult<DaPayLimitsTypeDto>("SELECT * FROM dbo.da_PayLimitsType WHERE ID=@id", new {id});
            }
        }
    }
}