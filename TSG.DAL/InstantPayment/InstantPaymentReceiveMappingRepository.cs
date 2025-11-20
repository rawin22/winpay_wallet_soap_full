using System;
using System.Collections.Generic;
using TSG.Models.DTO.InstantPayment;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.InstantPayment
{
    public class InstantPaymentReceiveMappingRepository : IInstantPaymentReceiveMappingRepository
    {
        public IResult Delete(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.ExecuteResult("DELETE FROM [dbo].[InstantPaymentReceiveMapping] WHERE Id = @id", new { id });
            }
        }

        public IResult Update(InstantPaymentReceiveMappingDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.UpdateResult(model);
            }
        }

        public IResult<List<InstantPaymentReceiveMappingDto>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryResult<InstantPaymentReceiveMappingDto>(
                    @"SELECT [Id], [InstantPaymentReceiveId], [InstantPaymentId], [CreatedDate] FROM [dbo].[InstantPaymentReceiveMapping]");
            }
        }
        public IResult<List<InstantPaymentReceiveMappingDto>> GetByInstantPaymentReceiveId(Guid InstantPaymentReceiveId)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryResult<InstantPaymentReceiveMappingDto>(
                    @"SELECT [Id], [InstantPaymentReceiveId], [InstantPaymentId], [CreatedDate] FROM [dbo].[InstantPaymentReceiveMapping] WHERE [InstantPaymentReceiveId]=@InstantPaymentReceiveId", new { InstantPaymentReceiveId });
            }
        }

        public IResult Insert(InstantPaymentReceiveMappingDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.InsertResult(model);
            }
        }
    }
}