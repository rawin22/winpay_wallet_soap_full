using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using TSG.Models.DTO.AutomaticExchanges;
using TSG.Models.ServiceModels.AutomaticExchange;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.Dal.AutomaticExchange.DependencyLiquidForUserDalMethods
{
    public class DependencyLiquidForUserDalMethod : IDependencyLiquidForUserDalMethod
    {
        public IResult<DependencyLiquidForUserDto> GetById(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryFirstOrDefaultResult<DependencyLiquidForUserDto>("SELECT Id, UserId,  LiquidCcyId, LiquidOrder FROM dbo.ae_DependencyLiquidForUser WHERE Id=@id", new {id});
            }
        }

        public IResult<DependencyLiquidForUserDto> Insert(DependencyLiquidForUserDto model)
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
                string sql = "DELETE FROM dbo.ae_DependencyLiquidForUser WHERE Id = @id";
                return connection.ExecuteResult(sql, new { id });
            }
        }

        public IResult<List<DependencyLiquidForUserDto>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryResult<DependencyLiquidForUserDto>("SELECT Id, UserId, LiquidCcyId, LiquidOrder FROM dbo.ae_DependencyLiquidForUser");
            }
        }

        public IResult<List<DependencyLiquidForUserSo>> GetAllSo()
        {
            var resSoList = new List<DependencyLiquidForUserSo>();
            using (var connection = ConnectionFactory.GetConnection())
            {
                var sql = @"SELECT dllu.Id,
                               dllu.UserId,
                               dllu.LiquidCcyId,
                               dllu.LiquidOrder,
	                           llc.Id,
                               llc.CurrencyId,
                               llc.IsLiquidCurrency,
                               llc.LiquidOrder,
                               llc.CurrencyCode
                            FROM dbo.ae_DependencyLiquidForUser AS dllu INNER JOIN dbo.ae_LiquidCcyList AS llc
                            ON llc.Id = dllu.LiquidCcyId";

                resSoList = connection.Query<DependencyLiquidForUserDto, LiquidCcyListDto, DependencyLiquidForUserSo>(sql,
                    (dto, listDto) =>
                    {
                        var soObj = AutoMapper.Mapper.Map<DependencyLiquidForUserSo>(dto);
                        soObj.DependencyLiquidForUser_LiquidCcyList = AutoMapper.Mapper.Map<LiquidCcyListSo>(listDto);
                        return soObj;
                    }, splitOn: "Id").ToList();
            }
            return new Result<List<DependencyLiquidForUserSo>>(resSoList);
        }

        public IResult<List<DependencyLiquidForUserSo>> GetAllSoByUser(Guid userId)
        {
            var resSoList = new List<DependencyLiquidForUserSo>();
            using (var connection = ConnectionFactory.GetConnection())
            {
                var sql = @"SELECT dllu.Id,
                               dllu.UserId,
                               dllu.LiquidCcyId,
                               dllu.LiquidOrder,
	                           llc.Id,
                               llc.CurrencyId,
                               llc.IsLiquidCurrency,
                               llc.LiquidOrder,
                               llc.CurrencyCode
                            FROM dbo.ae_DependencyLiquidForUser AS dllu INNER JOIN dbo.ae_LiquidCcyList AS llc
                            ON llc.Id = dllu.LiquidCcyId WHERE dllu.UserId = @userId";

                resSoList = connection.Query<DependencyLiquidForUserDto, LiquidCcyListDto, DependencyLiquidForUserSo>(sql,
                    (dto, listDto) =>
                    {
                        var soObj = AutoMapper.Mapper.Map<DependencyLiquidForUserSo>(dto);
                        soObj.DependencyLiquidForUser_LiquidCcyList = AutoMapper.Mapper.Map<LiquidCcyListSo>(listDto);
                        return soObj;
                    }, new{ userId }, splitOn: "Id").ToList();
            }
            return new Result<List<DependencyLiquidForUserSo>>(resSoList);
        }
    
        public IResult DeleteAllByLiquidCcyCode(Guid liquidGuid)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                string sql = "DELETE FROM dbo.ae_DependencyLiquidForUser WHERE LiquidCcyId =  @liquidGuid";
                return connection.ExecuteResult(sql, new { liquidGuid });
            }
        }

        public IResult DeleteAllByUserId(Guid userId)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                string sql = "DELETE FROM dbo.ae_DependencyLiquidForUser WHERE UserId =  @userId";
                return connection.ExecuteResult(sql, new { userId });
            }
        }

        public IResult BulkInsertLiquidForUser(Guid liquidGuid)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.ExecuteResult("BulkInsertLiquidsForAllUsers", new { LiquidId = liquidGuid }, CommandType.StoredProcedure);
            }
        }

        public IResult BulkInsertLiquidCurrencyForUser(string liquids, Guid userId)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.ExecuteResult("BulkInsertLiquidsForUser", new { LiquidIdsString = liquids, UserId = userId }, CommandType.StoredProcedure);
            }
        }
    }
}