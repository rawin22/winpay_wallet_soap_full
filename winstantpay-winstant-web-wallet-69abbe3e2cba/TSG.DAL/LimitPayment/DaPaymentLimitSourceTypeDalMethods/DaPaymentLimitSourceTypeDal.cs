using System;
using System.Collections.Generic;
using TSG.Models.DTO.LimitPayment;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.LimitPayment.DaPaymentLimitSourceTypeDalMethods
{
    public class DaPaymentLimitSourceTypeDal : IDaPaymentLimitSourceTypeDal
    {
        public IResult Delete(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                string sql = "UPDATE dbo.da_PaymentLimitSourceType SET IsDeleted = 1 WHERE ID = @id";
                return connection.ExecuteResult(sql, new { id });
            }
        }

        public IResult Update(DaPaymentLimitSourceTypeDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.UpdateResult(model);
            }
        }

        public IResult<List<DaPaymentLimitSourceTypeDto>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryResult<DaPaymentLimitSourceTypeDto>("SELECT * FROM da_PaymentLimitSourceType");
            }
        }

        public IResult Insert(DaPaymentLimitSourceTypeDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                model.ID = Guid.NewGuid();
                return connection.InsertResult(model);
            }
        }

        public IResult<DaPaymentLimitSourceTypeDto> GetById(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QuerySingleResult<DaPaymentLimitSourceTypeDto>("SELECT * FROM dbo.da_PaymentLimitSourceType WHERE ID=@id", new {id});
            }
        }
    }
}