using System;
using System.Collections.Generic;
using TSG.Models.DTO.Transfers;
using TSG.Models.Enums;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.Dal.Transfers
{
    public class TransfersRepository : ITransfersRepository
    {
        public IResult<TransfersDto> GetById(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryFirstOrDefaultResult<TransfersDto>(
                    "SELECT TOP (1) [Id],[TransferParent],[TransferRecipient],[CreatedDate],[AcceptedDate],[IsKycCreated],[SourceType],[Source],[LinkToSourceRow],[IsRejected]" +
                    "FROM [dbo].[tr_Transfers] WHERE Id = @id", new {id});
            }
        }

        public IResult Delete(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.ExecuteResult("DELETE FROM [dbo].[tr_Transfers] WHERE Id = @id", new {id});
            }
        }

        public IResult Update(TransfersDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.UpdateResult(model);
            }
        }

        public IResult<List<TransfersDto>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryResult<TransfersDto>(
                    @"SELECT [Id],[TransferParent],[TransferRecipient],[CreatedDate],[AcceptedDate],[IsKycCreated],[SourceType],[Source],[LinkToSourceRow],[IsRejected] FROM [dbo].[tr_Transfers]");
            }
        }

        public IResult Insert(TransfersDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                var mainRes = connection.InsertResult(model);
                var additional = new Result();

                if (model.LinkToSourceRow.HasValue && model.SourceType == (int) TransfersSourceTypeEnum.TokenTransfers)
                {
                    additional = connection.ExecuteResult("UPDATE dbo.da_PayLimits SET IsTransfered = @result WHERE ID = @id", new {result = mainRes.Success? 1: 0, id = model.LinkToSourceRow.Value}) as Result;
                }
                return new Result();
            }
        }
    }
}