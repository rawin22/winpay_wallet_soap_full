using System;
using System.Collections.Generic;
using TSG.Models.DTO.LimitPayment;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.LimitPayment.DaUserWPayIDSettingDalMethods
{
    public class DaUserWPayIDSettingDal : IDaUserWPayIDSettingDal
    {
        public IResult Insert(DaUserWPayIDSettingDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.InsertResult(model);
            }
        }
        public IResult Delete(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                string sql = "UPDATE dbo.da_UserWPayIDSettings SET IsDeleted = 1 WHERE ID = @id";
                return connection.ExecuteResult(sql, new { id });
            }
        }
        public IResult Update(DaUserWPayIDSettingDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.UpdateResult(model);
            }
        }

        public IResult<DaUserWPayIDSettingDto> GetById(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryFirstOrDefaultResult<DaUserWPayIDSettingDto>("SELECT * FROM dbo.da_UserWPayIDSettings WHERE ID=@id", new {id});
            }
        }
        
        public IResult<DaUserWPayIDSettingDto> GetByWPayId(string wpayId)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryFirstOrDefaultResult<DaUserWPayIDSettingDto>("SELECT * FROM dbo.da_UserWPayIDSettings WHERE WPayID=@wpayId", new {wpayId});
            }
        }

        public IResult<List<DaUserWPayIDSettingDto>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryResult<DaUserWPayIDSettingDto>("SELECT * FROM dbo.da_UserWPayIDSettings");
            }
        }
    }
}