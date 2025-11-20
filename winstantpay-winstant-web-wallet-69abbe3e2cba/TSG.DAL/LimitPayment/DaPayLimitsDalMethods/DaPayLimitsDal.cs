using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using TSG.Models.DTO.LimitPayment;
using TSG.Models.ServiceModels.LimitPayment;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.Dal.LimitPayment.DaPayLimitsDalMethods
{
    public class DaPayLimitsDal : IDaPayLimitsDal
    {
        public IResult Delete(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                string sql = "UPDATE dbo.da_PayLimits SET IsDeleted = 1 WHERE ID = @id";
                return connection.ExecuteResult(sql, new { id });
            }
        }

        public IResult Update(DaPayLimitsDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.UpdateResult(model);
            }
        }

        public IResult<List<DaPayLimitsDto>> GetAll()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryResult<DaPayLimitsDto>(sql: "SELECT da_limit.* FROM dbo.da_PayLimits AS da_limit INNER JOIN " +
                                                                   "dbo.da_PaymentLimitSourceType AS da_lim_type ON da_limit.SourceType = da_lim_type.ID");
            }
        }

        public IResult<List<DaPayLimitsSo>> GetAllSo()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                var res =  connection.Query<DaPayLimitsDto, DaPaymentLimitSourceTypeDto, DaPayLimitsSo>(sql: "SELECT da_limit.*, da_lim_type.* FROM dbo.da_PayLimits AS da_limit " +
                                                                                                         "INNER JOIN dbo.da_PaymentLimitSourceType AS da_lim_type ON da_limit.SourceType = da_lim_type.ID",
                    map: (limitsDto, sourceDto) => new DaPayLimitsSo()
                    {
                        DaPaymentLimitSourceType = AutoMapper.Mapper.Map<DaPaymentLimitSourceTypeSo>(sourceDto),
                        DaPayLimits_ID = limitsDto.ID,
                        //DaPayLimits_CcyCode = limitsDto.CcyCode,
                        DaPayLimits_CreationDate = limitsDto.CreationDate,
                        DaPayLimits_DateOfExpire = limitsDto.DateOfExpire,
                        DaPayLimits_LastModifiedDate = limitsDto.LastModifiedDate,
                        DaPayLimits_SourceType = limitsDto.SourceType,
                        DaPayLimits_LimitCodeInitialization = limitsDto.LimitCodeInitialization,
                        //DaPayLimits_PinCode = limitsDto.PinCode,
                        DaPayLimits_IsPinProtected = limitsDto.IsPinProtected,
                        DaPayLimits_StatusByLimit = limitsDto.StatusByLimit,
                        DaPayLimits_UserData = limitsDto.UserData,
                        DaPayLimits_UserName = limitsDto.UserName,
                        DaPayLimits_IsDeleted = limitsDto.IsDeleted,
                        DaPayLimits_MediaName = limitsDto.MediaName,
                        DaPayLimits_IsTransfered = limitsDto.IsTransfered,
                        DaPayLimits_WPayId = limitsDto.WPayId
                    }, splitOn: "ID").ToList();
                return new Result<List<DaPayLimitsSo>>(res, res.Count == 0 ? "Empty enumerable" : String.Empty);
            }
        }

        public IResult<DaPayLimitsSo> Insert(DaPayLimitsSo model)
        {
            model.DaPayLimits_ID = Guid.NewGuid();
            var dtoModel = AutoMapper.Mapper.Map<DaPayLimitsDto>(model);

            using (var connection = ConnectionFactory.GetConnection())
            {
                var resOfQuery = connection.ExecuteResult("InsertToDaPaymentLimit", new
                {
                    LimitCodeInitialization = dtoModel.LimitCodeInitialization,
                    UserName = dtoModel.UserName,
                    CreationDate = dtoModel.CreationDate,
                    UserData = dtoModel.UserData,
                    DateOfExpire = dtoModel.DateOfExpire,
                    //CcyCode = dtoModel.CcyCode,
                    SourceType = dtoModel.SourceType,
                    ID = dtoModel.ID,
                    //PinCode = dtoModel.PinCode,
                    //IsPinProtected = dtoModel.IsPinProtected,
                    SecretCode = dtoModel.SecretCode,
                    MediaName = dtoModel.MediaName,
                    WPayId = dtoModel.WPayId,
                    //RequiredPinAmount = dtoModel.RequiredPinAmount,
                    //IsRestrictedToSelectedCurrency = dtoModel.IsRestrictedToSelectedCurrency
                }, CommandType.StoredProcedure);
                return new Result<DaPayLimitsSo>(resOfQuery.Success ? model : null, resOfQuery.Success ? String.Empty : resOfQuery.Message);
            }
        }

        public IResult<DaPayLimitsDto> GetById(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.QueryFirstOrDefaultResult<DaPayLimitsDto>("SELECT da_limit.* FROM dbo.da_PayLimits AS da_limit WHERE da_limit.ID = @id", new { id });
            }
        }

        public IResult<List<DaPayLimitsSo>> GetAllDaByUserName(string userName)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                string sql = "SELECT da_limit.*, da_lim_type.* FROM dbo.da_PayLimits AS da_limit " +
                             "INNER JOIN dbo.da_PaymentLimitSourceType AS da_lim_type ON da_limit.SourceType = da_lim_type.ID WHERE da_limit.UserName = @userName";
                var resList = connection.Query<DaPayLimitsDto, DaPaymentLimitSourceTypeDto, DaPayLimitsSo>(sql, (limitsDto, sourceDto) => new DaPayLimitsSo()
                {
                    DaPaymentLimitSourceType = AutoMapper.Mapper.Map<DaPaymentLimitSourceTypeSo>(sourceDto),
                    DaPayLimits_ID = limitsDto.ID,
                    //DaPayLimits_CcyCode = limitsDto.CcyCode,
                    DaPayLimits_CreationDate = limitsDto.CreationDate,
                    DaPayLimits_DateOfExpire = limitsDto.DateOfExpire,
                    DaPayLimits_LastModifiedDate = limitsDto.LastModifiedDate,
                    DaPayLimits_SourceType = limitsDto.SourceType,
                    DaPayLimits_LimitCodeInitialization = limitsDto.LimitCodeInitialization,
                    //DaPayLimits_PinCode = limitsDto.PinCode,
                    DaPayLimits_IsPinProtected = limitsDto.IsPinProtected,
                    DaPayLimits_StatusByLimit = limitsDto.StatusByLimit,
                    DaPayLimits_UserData = limitsDto.UserData,
                    DaPayLimits_UserName = limitsDto.UserName,
                    DaPayLimits_IsDeleted = limitsDto.IsDeleted,
                    DaPayLimits_MediaName = limitsDto.MediaName,
                    DaPayLimits_IsTransfered = limitsDto.IsTransfered,
                    DaPayLimits_WPayId = limitsDto.WPayId,
                    //DaPayLimits_RequiredPinAmount = limitsDto.RequiredPinAmount,
                    //DaPayLimits_IsRestrictedToSelectedCurrency = limitsDto.IsRestrictedToSelectedCurrency                  
                }, param: new { userName }, splitOn: "ID").ToList();

                return new Result<List<DaPayLimitsSo>>(resList);

            }
        }

        public IResult<List<DaPayLimitsSo>> GetAllDaByDeviceCode(string deviceCode)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                string sql = "SELECT da_limit.*, da_lim_type.* FROM dbo.da_PayLimits AS da_limit " +
                             "INNER JOIN dbo.da_PaymentLimitSourceType AS da_lim_type ON da_limit.SourceType = da_lim_type.ID WHERE da_limit.LimitCodeInitialization LIKE N'%'+@deviceCode+'%'";
                var resList = connection.Query<DaPayLimitsDto, DaPaymentLimitSourceTypeDto, DaPayLimitsSo>(sql, (limitsDto, sourceDto) => new DaPayLimitsSo()
                {
                    DaPaymentLimitSourceType = AutoMapper.Mapper.Map<DaPaymentLimitSourceTypeSo>(sourceDto),
                    DaPayLimits_ID = limitsDto.ID,
                    //DaPayLimits_CcyCode = limitsDto.CcyCode,
                    DaPayLimits_CreationDate = limitsDto.CreationDate,
                    DaPayLimits_DateOfExpire = limitsDto.DateOfExpire,
                    DaPayLimits_LastModifiedDate = limitsDto.LastModifiedDate,
                    DaPayLimits_SourceType = limitsDto.SourceType,
                    DaPayLimits_LimitCodeInitialization = limitsDto.LimitCodeInitialization,
                    //DaPayLimits_PinCode = limitsDto.PinCode,
                    DaPayLimits_IsPinProtected = limitsDto.IsPinProtected,
                    DaPayLimits_StatusByLimit = limitsDto.StatusByLimit,
                    DaPayLimits_UserData = limitsDto.UserData,
                    DaPayLimits_UserName = limitsDto.UserName,
                    DaPayLimits_IsDeleted = limitsDto.IsDeleted,
                    DaPayLimits_MediaName = limitsDto.MediaName,
                    DaPayLimits_IsTransfered = limitsDto.IsTransfered,
                    DaPayLimits_WPayId = limitsDto.WPayId,
                    //DaPayLimits_RequiredPinAmount = limitsDto.RequiredPinAmount,
                    //DaPayLimits_IsRestrictedToSelectedCurrency = limitsDto.IsRestrictedToSelectedCurrency
                }, param: new { deviceCode }, splitOn: "ID").ToList();
                return new Result<List<DaPayLimitsSo>>(resList);
            }
        }

        public IResult<List<string>> GetRandomWordBySecretPhrase()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                var r = connection.Query<string>("GetRandomWords", commandType: CommandType.StoredProcedure);
                return new Result<List<string>>(r?.ToList() ?? new List<string>());
            }
        }
    }
}