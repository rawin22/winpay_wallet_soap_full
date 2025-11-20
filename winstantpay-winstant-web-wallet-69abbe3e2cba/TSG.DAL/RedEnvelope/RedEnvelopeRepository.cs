using System;
using System.Collections.Generic;
using TSG.Models.DTO.Transfers;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.RedEnvelope
{
    public class RedEnvelopeRepository : IRedEnvelopeRepository
    {
        public IResult Delete(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.ExecuteResult("DELETE FROM [dbo].[re_RedEnvelope] WHERE Id = @id", new { id });
            }
        }

        public IResult Update(RedEnvelopeDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.UpdateResult(model);
            }
        }

        public IResult<List<RedEnvelopeDto>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryResult<RedEnvelopeDto>(
                    @"SELECT [Id],[CurrencyCode],[Amount],[Note],[FilePath],[IsSuccessTransferToRedEnvelopeAcc],[IsNeedToNotifyByEmail],
                        [IsSuccessTransferToRecipient],[DateTransferedToRedEnvelope],[DateTransferedToRecipient],[RedEnvelopePaymentId],[RecipientPaymentId],[RecipientUserName],[RejectionNote],[WPayIdTo],[WPayIdFrom] FROM [dbo].[re_RedEnvelope]");
            }
        }

        public IResult Insert(RedEnvelopeDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.InsertResult(model);
            }
        }

        public IResult<RedEnvelopeDto> GetById(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryFirstOrDefaultResult<RedEnvelopeDto>(
                    "SELECT TOP 1[Id],[CurrencyCode],[Amount],[Note],[FilePath],[IsSuccessTransferToRedEnvelopeAcc],[IsNeedToNotifyByEmail],[IsSuccessTransferToRecipient]," +
                    "[DateTransferedToRedEnvelope],[DateTransferedToRecipient],[RedEnvelopePaymentId],[RecipientPaymentId],[RecipientUserName],[RejectionNote],[WPayIdTo],[WPayIdFrom] FROM[dbo].[re_RedEnvelope] WHERE Id = @id", new {id});
            }
        }
    }
}