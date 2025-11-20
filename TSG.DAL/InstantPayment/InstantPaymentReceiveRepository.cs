using System;
using System.Collections.Generic;
using TSG.Models.DTO.InstantPayment;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.InstantPayment
{
    public class InstantPaymentReceiveRepository : IInstantPaymentReceiveRepository
    {
        public IResult Delete(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.ExecuteResult("DELETE FROM [dbo].[InstantPaymentReceive] WHERE Id = @id", new { id });
            }
        }

        public IResult Update(InstantPaymentReceiveDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.UpdateResult(model);
            }
        }

        public IResult<List<InstantPaymentReceiveDto>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryResult<InstantPaymentReceiveDto>(
                    @"SELECT [Id], [TsgUserGuId], [Alias]= coalesce([Alias], ''), [Currency]= coalesce([Currency], ''), [Amount], [Invoice]= coalesce([Invoice], ''), [Memo]= coalesce([Memo], ''), [ShortenedUrl] = coalesce([ShortenedUrl], ''), [CreatedDate], [LastModifiedDate], [AttachedFileName], [AttachedFileExtension], [AttachedFileContentType], [AttachedFile] FROM [dbo].[InstantPaymentReceive]");
            }
        }

        public IResult<List<InstantPaymentReceiveDto>> GetByUser(Guid userId)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryResult<InstantPaymentReceiveDto>(
                    @"SELECT [Id], [TsgUserGuId], ​[Alias]= coalesce([Alias], ''), [Currency]= coalesce([Currency], ''), [Amount], [Invoice]= coalesce([Invoice], ''), [Memo]= coalesce([Memo], ''), [ShortenedUrl] = coalesce([ShortenedUrl], ''), [CreatedDate], [LastModifiedDate], [AttachedFileName], [AttachedFileExtension], [AttachedFileContentType], [AttachedFile] FROM [dbo].[InstantPaymentReceive] WHERE TsgUserGuId=@userId", new { userId});
            }
        }


        public IResult Insert(InstantPaymentReceiveDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.InsertResult(model);
            }
        }
    }
}