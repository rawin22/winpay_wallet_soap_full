using System;
using System.Collections.Generic;
using System.Data;
using TSG.Models.DTO.AutomaticExchanges;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.AutomaticExchange.LiquidCcyListDalMethods
{
    public class LiquidCcyListDalMethods : ILiquidCcyListDalMethods
    {
        public IResult Delete(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                string sql = "DELETE FROM dbo.ae_LiquidCcyList WHERE Id =  @id";
                return connection.ExecuteResult(sql, new { id });
            }
        }

        public IResult Update(LiquidCcyListDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.UpdateResult(model);
            }
        }
        
        public IResult DeleteByCurrencyId(int ccyId)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                string sql = "DELETE FROM dbo.ae_LiquidCcyList WHERE CurrencyId =  @id";
                return connection.ExecuteResult(sql, new { ccyId });
            }
        }

        public IResult<List<LiquidCcyListDto>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryResult<LiquidCcyListDto>("SELECT Id,CurrencyId,IsLiquidCurrency,LiquidOrder,CurrencyCode FROM dbo.ae_LiquidCcyList ");
            }
        }

        public IResult<LiquidCcyListDto> GetById(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryFirstOrDefaultResult<LiquidCcyListDto>("SELECT TOP 1 Id,CurrencyId,IsLiquidCurrency,LiquidOrder,CurrencyCode FROM dbo.ae_LiquidCcyList WHERE Id = @id", new { id });
            }
        }

        public IResult<LiquidCcyListDto> GetLiquidCcyElementByCurrencyId(int ccyId)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryFirstOrDefaultResult<LiquidCcyListDto>("SELECT TOP 1 Id,CurrencyId,IsLiquidCurrency,LiquidOrder,CurrencyCode FROM dbo.ae_LiquidCcyList WHERE CurrencyId = @ccyId", new { ccyId });
            }
        }

        public IResult<LiquidCcyListDto> GetLiquidCcyElementByCurrencyCode(string ccyCode)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryFirstOrDefaultResult<LiquidCcyListDto>("SELECT TOP 1 Id,CurrencyId,IsLiquidCurrency,LiquidOrder,CurrencyCode FROM dbo.ae_LiquidCcyList WHERE CurrencyCode = @ccyCode", new { ccyCode });
            }
        }

        public IResult BulkUpdateOrder(string ids)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.ExecuteResult("BulkUpdateLuquidorders", new { LiquidIdsString = ids }, CommandType.StoredProcedure);
            }
        }

        public IResult Insert(LiquidCcyListDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.InsertResult(model);
            }
        }
    }
}