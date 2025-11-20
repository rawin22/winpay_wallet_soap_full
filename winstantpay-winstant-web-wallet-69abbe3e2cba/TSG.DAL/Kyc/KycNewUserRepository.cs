using System;
using System.Collections.Generic;
using TSG.Models.DTO.Transfers;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.Kyc
{
    public class KycNewUserRepository : IKycNewUserRepository
    {
        public IResult Delete(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.ExecuteResult("DELETE FROM [dbo].[kyc_NewUserByKyc] WHERE Id = @id", new { id });
            }
        }

        public IResult Update(KycNewUserDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.UpdateResult(model);
            }
        }

        public IResult<List<KycNewUserDto>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryResult<KycNewUserDto>(
                    @"SELECT [Id],[KycUserEmail],[KycUserFirstName],[KycUserLastName],[UserInitiator],[CreationDate],[IsCreated] FROM [dbo].[kyc_NewUserByKyc]");
            }
        }

        public IResult Insert(KycNewUserDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.InsertResult(model);
            }
        }
    }
}