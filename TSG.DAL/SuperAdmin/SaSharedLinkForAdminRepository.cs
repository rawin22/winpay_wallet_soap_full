using System;
using System.Collections.Generic;
using System.Linq;
using TSG.Models.DTO;
using TSG.Models.DTO.Transfers;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.Dal.SuperAdmin
{
    public class SaSharedLinkForAdminRepository : ISaSharedLinkForAdminRepository
    {
        public IResult Delete(string userName)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.ExecuteResult("DELETE FROM dbo.SharedAdminLink WHERE UserName = @userName", new { userName });
            }
        }

        public IResult Update(SharedAdminLinkDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.UpdateResult(model);
            }
        }

        public IResult<List<SharedAdminLinkDto>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryResult<SharedAdminLinkDto>(@"SELECT ID,UserName,FirstName,LastName,Email,CreationDate,LinkAddress,StatusLink,ActivationDate FROM dbo.SharedAdminLink");
            }
        }

        public IResult Insert(SharedAdminLinkDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.InsertResult(model);
            }
        }

        public IResult ClearAllOldReferences(string userName)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.ExecuteResult("DELETE FROM dbo.SharedAdminLink WHERE UserName = @userName AND StatusLink = 1", new {userName});
            }
        }

        public IResult<int> IfAdminNeedChangePassword(string userName)
        {
            var allRecs = GetAll().Obj.Where(w=>w.UserName == userName && w.StatusLink.HasValue && w.StatusLink.Value == 1).ToList();
            return new Result<int>(allRecs.Count, true, $"Count links {allRecs.Count} for {userName}");
        }
    }
}