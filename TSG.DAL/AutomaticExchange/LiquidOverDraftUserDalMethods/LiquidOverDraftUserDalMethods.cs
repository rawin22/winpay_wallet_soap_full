using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using TSG.Models.DTO.AutomaticExchanges;
using TSG.Models.ServiceModels.AutomaticExchange;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.Dal.AutomaticExchange.LiquidOverDraftUserDalMethods
{
    public class LiquidOverDraftUserDalMethods : ILiquidOverDraftUserDalMethods
    {
        public IResult Delete(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                string sql = "DELETE FROM dbo.ae_LiquidOverDraftUserList WHERE Id =  @id";
                return connection.ExecuteResult(sql, new { id });
            }
        }

        public IResult Update(LiquidOverDraftUserListDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.UpdateResult(model);
            }
        }

        public IResult<List<LiquidOverDraftUserListDto>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryResult<LiquidOverDraftUserListDto>("SELECT lou.[Id],lou.[UserId],lou.[UserName],lou.[CreationDate],lou.[AccountRep]," +
                                                                          "(u.firstName + N' ' + u.lastName) AS FullName FROM [dbo].[ae_LiquidOverDraftUserList] AS lou INNER JOIN  dbo.[User] AS u ON u.userIdByTSG = lou.UserId");
            }
        }

        public IResult Insert(LiquidOverDraftUserListDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.InsertResult(model);
            }
        }

        public IResult<LiquidOverDraftUserListDto> GetById(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryFirstOrDefaultResult<LiquidOverDraftUserListDto>("SELECT lou.[Id],lou.[UserId],lou.[UserName],lou.[CreationDate],lou.[AccountRep]," +
                                                                          "(u.firstName + N' ' + u.lastName) AS FullName FROM [dbo].[ae_LiquidOverDraftUserList] AS lou INNER JOIN  dbo.[User] AS u ON u.userIdByTSG = lou.UserId WHERE lou.[Id]=@id", new { id });
            }
        }

        public IResult DeleteByCurrencyId(int ccyId)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                string sql = "DELETE FROM dbo.ae_LiquidOverDraftUserList WHERE CurrencyId =  @ccyId";
                return connection.ExecuteResult(sql, new { ccyId });
            }
        }

        public IResult DeleteByLiquidCurrencyId(Guid liquidCcyId)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                string sql = "DELETE FROM dbo.ae_LiquidOverDraftUserList WHERE LiquidCurrencyId = @liquidCcyId";
                return connection.ExecuteResult(sql, new { liquidCcyId });
            }
        }

        public IResult<List<LiquidOverDraftUserListSo>> GetAllSo()
        {
            var resSoList = new List<LiquidOverDraftUserListSo>();
            using (var connection = ConnectionFactory.GetConnection())
            {
                var res = connection.QueryResult<LiquidOverDraftUserListDto>("SELECT lou.[Id],lou.[UserId],lou.[UserName],lou.[CreationDate],lou.[AccountRep]," +
                                                                   "(u.firstName + N' ' + u.lastName) AS FullName FROM [dbo].[ae_LiquidOverDraftUserList] AS lou INNER JOIN  dbo.[User] AS u ON u.userIdByTSG = lou.UserId");
                resSoList = AutoMapper.Mapper.Map<List<LiquidOverDraftUserListSo>>(res.Obj);
            }
            return new Result<List<LiquidOverDraftUserListSo>>(resSoList);
        }
    }
}