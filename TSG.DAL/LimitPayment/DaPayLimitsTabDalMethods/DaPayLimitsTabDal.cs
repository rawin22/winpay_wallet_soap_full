using System;
using System.Collections.Generic;
using System.Linq;
using TSG.Models.DTO.LimitPayment;
using TSG.Models.ServiceModels.LimitPayment;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.Dal.LimitPayment.DaPayLimitsTabDalMethods
{
    public class DaPayLimitsTabDal : IDaPayLimitsTabDal
    {
        public IResult Delete(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                string sql = "UPDATE dbo.da_PayLimitsTab SET IsDeleted = 1 WHERE ID = @id";
                return connection.ExecuteResult(sql, new { id });
            }
        }

        public IResult Update(DaPayLimitsTabDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.UpdateResult(model);
            }
        }

        public IResult<List<DaPayLimitsTabDto>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryResult<DaPayLimitsTabDto>(sql: "SELECT * FROM dbo.da_PayLimitsTab");
            }
        }

        public IResult<DaPayLimitsTabSo> Insert(DaPayLimitsTabSo model)
        {
            model.DaPayLimitsTab_ID = Guid.NewGuid();
            var dtoModel = AutoMapper.Mapper.Map<DaPayLimitsTabDto>(model);

            using (var connection = ConnectionFactory.GetConnection())
            {
                var resOfQuery = connection.InsertResult(dtoModel);
                return new Result<DaPayLimitsTabSo>(resOfQuery.Success ? model : null, resOfQuery.Message);
            }
        }

        public IResult<List<DaPayLimitsTabSo>> GetAllLimitsByKey(Guid parentId)
        {
            var resQuery = GetAll();
            var resultDto = resQuery.Obj?.Where(w => w.ParentDaPayId == parentId).ToList() ?? new List<DaPayLimitsTabDto>();
            var resultSo = AutoMapper.Mapper.Map<List<DaPayLimitsTabSo>>(resultDto);
            return new Result<List<DaPayLimitsTabSo>>(resultSo, resQuery.Message);
        }

        public IResult<DaPayLimitsTabSo> GetById(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                var dtoQRes = connection.QueryFirstOrDefaultResult<DaPayLimitsTabDto>("SELECT * FROM dbo.da_PayLimitsTab WHERE ID = @id", param:new {id});
                return new Result<DaPayLimitsTabSo>(AutoMapper.Mapper.Map<DaPayLimitsTabSo>(dtoQRes.Obj), dtoQRes.Success? String.Empty : dtoQRes.Message);
            }
        }
    }
}