using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using Dapper;
using TSG.Dal.Interfaces.Fundings;
using TSG.Models.DTO;
using TSG.Models.ServiceModels;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.Dal.Fundings
{
    public class FundingsRepository : IFundingsRepository
    {
        public IResult Delete(Guid id)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                string sql = "UPDATE dbo.Fundings SET IsDeleted = 1 WHERE ID = @id";
                return connection.ExecuteResult(sql, new { id });
            }
        }

        public IResult<List<Models.ServiceModels.Fundings>> GetAllFundings(string userName)
        {
            try
            {
                using (var connection = ConnectionFactory.GetConnection())
                {
                    var resFundings = connection
                        .Query<FundingsDto, UserDto, CurrencyDto, FundingSourcesDto, FundChangesDto,
                            Models.ServiceModels.Fundings>("FundingDataSelect",
                            (funding, user, currency, source, history) =>
                            {
                                Models.ServiceModels.Fundings res = Mapper.Map<Models.ServiceModels.Fundings>(funding);
                                res.Fundings_Currency = Mapper.Map<Currency>(currency);
                                res.Fundings_FundChange = Mapper.Map<FundChanges>(history);
                                res.Fundings_FundingSource = Mapper.Map<FundingSources>(source);
                                res.Fundings_User = Mapper.Map<User>(user);
                                return res;
                            },
                            commandType: CommandType.StoredProcedure, splitOn: "username,ccyId, ID, fndStatChangeId", param: new { userName }
                        ).ToList();
                    return new Result<List<Models.ServiceModels.Fundings>>(resFundings);
                }
            }
            catch (Exception e)
            {
                return new Result<List<Models.ServiceModels.Fundings>>(e.Message);
            }
        }

        public IResult Update(FundingsDto model)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.UpdateResult(model);
            }
        }

        
        public IResult<List<Models.ServiceModels.Fundings>> GetAllFundings()
        {
            try
            {
                using (var connection = ConnectionFactory.GetConnection())
                {
                    var resFundings = connection
                        .Query<FundingsDto, UserDto, CurrencyDto, FundingSourcesDto, FundChangesDto,
                            Models.ServiceModels.Fundings>("FundingDataSelect",
                            (funding, user, currency, source, history) =>
                            {
                                Models.ServiceModels.Fundings res = Mapper.Map<Models.ServiceModels.Fundings>(funding);
                                res.Fundings_Currency = Mapper.Map<Currency>(currency);
                                res.Fundings_FundChange = Mapper.Map<FundChanges>(history);
                                res.Fundings_FundingSource = Mapper.Map<FundingSources>(source);
                                res.Fundings_User = Mapper.Map<User>(user);
                                return res;
                            },
                            commandType: CommandType.StoredProcedure, splitOn: "username,ccyId, ID, fndStatChangeId"
                        ).ToList();
                    return new Result<List<Models.ServiceModels.Fundings>>(resFundings);
                }
            }
            catch (Exception e)
            {
                return new Result<List<Models.ServiceModels.Fundings>>(e.Message);
            }
        }
        
        #region GetWireInstruction By parentId
        public IResult<AddFundsWire> GetWireFundingById(Guid parentId, string userName)
        {
            try
            {
                using (var connection = ConnectionFactory.GetConnection())
                {
                    var resWireFundById = connection
                        .Query<AddFunds_WireDto, FundingsDto, UserDto, CurrencyDto, FundingSourcesDto, FundChangesDto,
                            AddFundsWire>("FundingDetailsData",
                            (fundDetail, funding, user, currency, source, history) =>
                            {
                                Models.ServiceModels.Fundings fundings =
                                    Mapper.Map<Models.ServiceModels.Fundings>(funding);
                                fundings.Fundings_Currency = Mapper.Map<Currency>(currency);
                                fundings.Fundings_FundChange = Mapper.Map<FundChanges>(history);
                                fundings.Fundings_FundingSource = Mapper.Map<FundingSources>(source);
                                fundings.Fundings_User = Mapper.Map<User>(user);

                                AddFundsWire wireFund = Mapper.Map<AddFundsWire>(fundDetail);
                                wireFund.AddFundsWire_Fundings = fundings;
                                return wireFund;
                            },
                            commandType: CommandType.StoredProcedure,
                            splitOn: "Id, username,ccyId, ID, fndStatChangeId", param: new { parentId }
                        ).SingleOrDefault();
                    return new Result<AddFundsWire>(resWireFundById);
                }
            }
            catch (Exception e)
            {
                return new Result<AddFundsWire>(e.Message);
            }
        }

        public IResult<AddFundsPipit> GetPipitFundingById(Guid parentId, string userName = "")
        {
            try
            {
                using (var connection = ConnectionFactory.GetConnection())
                {
                    var resPipitById = connection
                        .Query<AddFunds_PipitDto, FundingsDto, UserDto, CurrencyDto, FundingSourcesDto, FundChangesDto,
                            AddFundsPipit>("FundingDetailsData",
                            (fundDetail, funding, user, currency, source, history) =>
                            {
                                Models.ServiceModels.Fundings fundings =
                                    Mapper.Map<Models.ServiceModels.Fundings>(funding);
                                fundings.Fundings_Currency = Mapper.Map<Currency>(currency);
                                fundings.Fundings_FundChange = Mapper.Map<FundChanges>(history);
                                fundings.Fundings_FundingSource = Mapper.Map<FundingSources>(source);
                                fundings.Fundings_User = Mapper.Map<User>(user);

                                AddFundsPipit wireFund = Mapper.Map<AddFundsPipit>(fundDetail);
                                wireFund.AddFundsWire_Fundings = fundings;
                                return wireFund;
                            },
                            commandType: CommandType.StoredProcedure,
                            splitOn: "Id, username,ccyId, ID, fndStatChangeId", param: new { parentId }
                        ).SingleOrDefault();
                    return new Result<AddFundsPipit>(resPipitById);
                }
            }
            catch (Exception e)
            {
                return new Result<AddFundsPipit>(e.Message);
            }
        }

        public IResult<AddFundsPipit> GetPipitFundingByBarCode(string barcode, string vendorcode)
        {
            try
            {
                using (var connection = ConnectionFactory.GetConnection())
                {
                    var resPipitById = connection
                        .Query<AddFunds_PipitDto, FundingsDto, UserDto, CurrencyDto, FundingSourcesDto, FundChangesDto,
                            AddFundsPipit>("Pipit_FundingDetailsData",
                            (fundDetail, funding, user, currency, source, history) =>
                            {
                                Models.ServiceModels.Fundings fundings =
                                    Mapper.Map<Models.ServiceModels.Fundings>(funding);
                                fundings.Fundings_Currency = Mapper.Map<Currency>(currency);
                                fundings.Fundings_FundChange = Mapper.Map<FundChanges>(history);
                                fundings.Fundings_FundingSource = Mapper.Map<FundingSources>(source);
                                fundings.Fundings_User = Mapper.Map<User>(user);

                                AddFundsPipit wireFund = Mapper.Map<AddFundsPipit>(fundDetail);
                                wireFund.AddFundsWire_Fundings = fundings;
                                return wireFund;
                            },
                            commandType: CommandType.StoredProcedure,
                            splitOn: "Id, username,ccyId, ID, fndStatChangeId", param: new { barcode, vendorcode }
                        ).SingleOrDefault();
                    return new Result<AddFundsPipit>(resPipitById);
                }
            }
            catch (Exception e)
            {
                return new Result<AddFundsPipit>(e.Message);
            }
        }

        public IResult<AddFunds_BlockChainInfo> GetBlockChainInfoFundingById(Guid parentId, string userName = "")
        {
            try
            {
                using (var connection = ConnectionFactory.GetConnection())
                {
                    var resBlockChainInfoFundById = connection
                        .Query<AddFunds_BlockChainInfoDto, FundingsDto, UserDto, CurrencyDto, FundingSourcesDto, FundChangesDto,
                            AddFunds_BlockChainInfo>("FundingDetailsData",
                            (fundDetail, funding, user, currency, source, history) =>
                            {
                                Models.ServiceModels.Fundings fundings =
                                    Mapper.Map<Models.ServiceModels.Fundings>(funding);
                                fundings.Fundings_Currency = Mapper.Map<Currency>(currency);
                                fundings.Fundings_FundChange = Mapper.Map<FundChanges>(history);
                                fundings.Fundings_FundingSource = Mapper.Map<FundingSources>(source);
                                fundings.Fundings_User = Mapper.Map<User>(user);

                                AddFunds_BlockChainInfo blockchainFund = Mapper.Map<AddFunds_BlockChainInfo>(fundDetail);
                                blockchainFund.AddFundsWire_Fundings = fundings;
                                return blockchainFund;
                            },
                            commandType: CommandType.StoredProcedure,
                            splitOn: "Id, username,ccyId, ID, fndStatChangeId", param: new { parentId }
                        ).SingleOrDefault();
                    return new Result<AddFunds_BlockChainInfo>(resBlockChainInfoFundById);
                }
            }
            catch (Exception e)
            {
                return new Result<AddFunds_BlockChainInfo>(e.Message);
            }
        }

        public IResult<AddFunds_BlockChainInfo> GetBlockChainInfoFundingByTimeStamp(Guid timestamp)
        {
            try
            {
                using (var connection = ConnectionFactory.GetConnection())
                {
                    var resBlockChainInfoFundById = connection
                        .Query<AddFunds_BlockChainInfoDto, FundingsDto, UserDto, CurrencyDto, FundingSourcesDto, FundChangesDto,
                            AddFunds_BlockChainInfo>("GetBlockChainInfoDataByTimeStamp",
                            (fundDetail, funding, user, currency, source, history) =>
                            {
                                Models.ServiceModels.Fundings fundings =
                                    Mapper.Map<Models.ServiceModels.Fundings>(funding);
                                fundings.Fundings_Currency = Mapper.Map<Currency>(currency);
                                fundings.Fundings_FundChange = Mapper.Map<FundChanges>(history);
                                fundings.Fundings_FundingSource = Mapper.Map<FundingSources>(source);
                                fundings.Fundings_User = Mapper.Map<User>(user);

                                AddFunds_BlockChainInfo blockchainFund = Mapper.Map<AddFunds_BlockChainInfo>(fundDetail);
                                blockchainFund.AddFundsWire_Fundings = fundings;
                                return blockchainFund;
                            },
                            commandType: CommandType.StoredProcedure,
                            splitOn: "Id, username,ccyId, ID, fndStatChangeId", param: new { timeStamp = timestamp }
                        ).SingleOrDefault();
                    return new Result<AddFunds_BlockChainInfo>(resBlockChainInfoFundById);
                }
            }
            catch (Exception e)
            {
                return new Result<AddFunds_BlockChainInfo>(e.Message);
            }
        }

        #endregion

        #region Inserted methods
        public IResult<AddFundsWire> InsertWireTransfer(AddFunds_WireDto wireTransfer, string userName)
        {

            using (var connection = ConnectionFactory.GetConnection())
            {
                var resWireFundById = connection
                    .Query<AddFunds_WireDto, FundingsDto, UserDto, CurrencyDto, FundingSourcesDto, FundChangesDto,
                        AddFundsWire>("AddFundsWireTransfer",
                        (fundDetail, funding, user, currency, source, history) =>
                        {
                            Models.ServiceModels.Fundings fundings =
                                Mapper.Map<Models.ServiceModels.Fundings>(funding);
                            fundings.Fundings_Currency = Mapper.Map<Currency>(currency);
                            fundings.Fundings_FundChange = Mapper.Map<FundChanges>(history);
                            fundings.Fundings_FundingSource = Mapper.Map<FundingSources>(source);
                            fundings.Fundings_User = Mapper.Map<User>(user);

                            AddFundsWire wireFund = Mapper.Map<AddFundsWire>(fundDetail);
                            wireFund.AddFundsWire_Fundings = fundings;
                            return wireFund;
                        },
                        commandType: CommandType.StoredProcedure,
                        splitOn: "Id, username,ccyId, ID, fndStatChangeId", param: new
                        {
                            userName = userName,
                            amount = wireTransfer.Amount,
                            custName = wireTransfer.custName,
                            bankName = wireTransfer.bankName,
                            lastFourDigits = wireTransfer.lastFourDigits,
                            other = wireTransfer.other,
                            fileName = wireTransfer.fileName,
                            filePath = wireTransfer.filePath,
                            bankCcyId = wireTransfer.bankCcyId,
                            paymentDate = Convert.ToDateTime(wireTransfer.paymentDate.ToString("F"))
                        }
                    ).SingleOrDefault();
                return new Result<AddFundsWire>(resWireFundById);
            }

            //using (var connection = ConnectionFactory.GetConnection())
            //{
            //    return connection.ExecuteResult("AddFundsWireTransfer", new
            //    {
            //        userName = userName,
            //        amount = wireTransfer.Amount,
            //        custName = wireTransfer.custName,
            //        bankName = wireTransfer.bankName,
            //        lastFourDigits = wireTransfer.lastFourDigits,
            //        other = wireTransfer.other,
            //        fileName = wireTransfer.fileName,
            //        filePath = wireTransfer.filePath,
            //        bankCcyId = wireTransfer.bankCcyId,
            //        paymentDate = wireTransfer.paymentDate
            //    }, CommandType.StoredProcedure);
            //}
        }

        public IResult InsertPipitTransfer(AddFunds_PipitDto wireTransfer, string userName)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.ExecuteResult("AddPipitWireInstruction", new
                {
                    BarCode = wireTransfer.BarCode,
                    Reference = wireTransfer.Reference,
                    VendorReference = wireTransfer.VendorReference,
                    OrderValue = wireTransfer.OrderValue,
                    TotalValue = wireTransfer.TotalValue,
                    CreatedDate = wireTransfer.CreatedDate,
                    ExpiredDate = wireTransfer.ExpiredDate,
                    Status = wireTransfer.Status,
                    userName = userName,
                    ccyName = wireTransfer.CurrencyCode,
                    userAlias = wireTransfer.Alias
                }, CommandType.StoredProcedure);
            }
        }

        public IResult<AddFunds_BlockChainInfo> InsertBlockChainInfoTransfer(AddFunds_BlockChainInfoDto wireTransfer, string userName)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                var resWireFundById = connection
                    .Query<AddFunds_BlockChainInfoDto, FundingsDto, UserDto, CurrencyDto, FundingSourcesDto, FundChangesDto,
                        AddFunds_BlockChainInfo>("AddBlockChainInfoWireTransfer",
                        (fundDetail, funding, user, currency, source, history) =>
                        {
                            Models.ServiceModels.Fundings fundings =
                                Mapper.Map<Models.ServiceModels.Fundings>(funding);
                            fundings.Fundings_Currency = Mapper.Map<Currency>(currency);
                            fundings.Fundings_FundChange = Mapper.Map<FundChanges>(history);
                            fundings.Fundings_FundingSource = Mapper.Map<FundingSources>(source);
                            fundings.Fundings_User = Mapper.Map<User>(user);

                            AddFunds_BlockChainInfo wireFund = Mapper.Map<AddFunds_BlockChainInfo>(fundDetail);
                            wireFund.AddFundsWire_Fundings = fundings;
                            return wireFund;
                        },
                        commandType: CommandType.StoredProcedure,
                        splitOn: "Id, username,ccyId, ID, fndStatChangeId", param: new
                        {
                            Alias = wireTransfer.Alias,
                            Message = wireTransfer.Message,
                            BlockChainAddress = wireTransfer.BlockChainAddress,
                            CallBackAddress = wireTransfer.CallBackAddress,
                            Index = wireTransfer.Index,
                            TimeStampPayment = wireTransfer.TimeStampPayment,
                            TotalValue = wireTransfer.TotalValue,
                            userName = userName,
                            ccyId = wireTransfer.CurrencyIndex,
                            transactionId =wireTransfer.TransactionId,
                            destinationBitcoinAddress = wireTransfer.DestinatedBitcoinAddress,
                            operation = wireTransfer.Operation,
                            numberOfConfirmation = wireTransfer.NumberOfConfirmation
                        }
                    ).SingleOrDefault();
                return new Result<AddFunds_BlockChainInfo>(resWireFundById);
            } 
        }

        #endregion

        #region Update tab parts methods
        public IResult UpdatePipitTransfer(AddFunds_PipitDto wireTransfer)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.ExecuteResult("UpdatePipitInfo", new
                {
                    parentId = wireTransfer.ParentId,
                    paidDate = wireTransfer.PaymentDate
                }, CommandType.StoredProcedure);
            }
        }

        public IResult UpdateWireTransfer(AddFunds_WireDto wireTransfer, string userName, int status)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.ExecuteResult("UpdateWireTransfer", new
                {
                    proofDocId = wireTransfer.proofDocId,
                    custName = wireTransfer.custName,
                    bankName = wireTransfer.bankName,
                    lastFourDigits = wireTransfer.lastFourDigits,
                    other = wireTransfer.other,
                    fileName = wireTransfer.fileName,
                    filePath = wireTransfer.filePath,
                    bankCcyId = wireTransfer.bankCcyId,
                    paymentDate = wireTransfer.paymentDate,
                    ParentId = wireTransfer.ParentId,
                    amount = wireTransfer.Amount,
                    userName = userName,
                    statusByFunds=status
                }, CommandType.StoredProcedure);
            }
        }

        public IResult UpdateWireTransferStatusForAdmin(Guid fundId, string userName, int status, string memo)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.ExecuteResult("UpdateWireTransferStatusForAdmin", new
                {
                    ParentId = fundId,
                    userName = userName,
                    statusByFunds = status,
                    memo = memo
                }, CommandType.StoredProcedure);
            }
        }
        #endregion

        public IResult UpdateBlockChainInfoTransfer(AddFunds_BlockChainInfoDto wireTransfer)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.ExecuteResult("UpdateBlockChainInfoData", new
                {
                    parentId = wireTransfer.ParentId,
                    currencyCode = wireTransfer.CurrencyIndex,
                    amount = wireTransfer.TotalValue,
                    transactionHash = wireTransfer.TransactionHash,
                    confirmatedTransactions = wireTransfer.ConfirmatedTransaction,
                    destinatedBitcoinAddress = wireTransfer.DestinatedBitcoinAddress,
                    valueInSatoshi = wireTransfer.ValueInSatoshi,
                    customParameter = wireTransfer.CustomParameter,
                    requestUrl = wireTransfer.RequestUrl
                }, CommandType.StoredProcedure);
            }
        }

        public IResult<Models.ServiceModels.Fundings> GetById(Guid id)
        {
            return null;
        }
    }
}