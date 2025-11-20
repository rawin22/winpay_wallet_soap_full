using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using TSG.Dal.Interfaces.Fundings;
using TSG.Models.DTO;
using TSG.Models.ServiceModels;
using TSG.ServiceLayer.Interfaces.Fundings;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.Funding
{
    public class FundingsServices : IFundingsService
    {
        private readonly IFundingsRepository _fundingRepository;

        public FundingsServices(IFundingsRepository fundingRepository)
        {
            _fundingRepository = fundingRepository;
        }

        #region [Service layer] Get funding methods
        public IResult<List<Fundings>> GetAllFundings()
        {
            return _fundingRepository.GetAllFundings();
        }

        public IResult<List<Fundings>> GetAllFundings(int status)
        {
            var res = _fundingRepository.GetAllFundings();
            return new Result<List<Fundings>>(res.Obj.Where(w => w.Fundings_StatusByFund == status).ToList(), res.Message);
        }

        public IResult<List<Fundings>> GetAllFundings(int status, string userName)
        {
            var res = _fundingRepository.GetAllFundings(userName);
            return new Result<List<Fundings>>(res.Obj.Where(w => w.Fundings_StatusByFund == status).ToList(), res.Message);
        }

        public IResult<List<Fundings>> GetAllFundings(string userName)
        {
            return _fundingRepository.GetAllFundings(userName);
        }
        #endregion

        #region [Service layer] Get data for Wire instruction and Pipit instruction
        public IResult<AddFundsWire> GetWireFundingById(Guid parentId, string userName = "")
        {
            return _fundingRepository.GetWireFundingById(parentId, userName);
        }

        public IResult<AddFundsPipit> GetPipitFundingById(Guid parentId, string userName = "")
        {
            return _fundingRepository.GetPipitFundingById(parentId, userName);
        }

        public IResult<AddFundsPipit> GetPipitFundingByBarCode(string barcode, string vendorcode)
        {
            return _fundingRepository.GetPipitFundingByBarCode(barcode, vendorcode);
        }

        public IResult<AddFunds_BlockChainInfo> GetBlockChainInfoFundingById(Guid parentId, string userName = "")
        {
            return  _fundingRepository.GetBlockChainInfoFundingById(parentId, userName);
        }

        public IResult<AddFunds_BlockChainInfo> GetBlockChainInfoFundingByTimeStamp(Guid timestamp)
        {
            return  _fundingRepository.GetBlockChainInfoFundingByTimeStamp(timestamp);
        }

        #endregion

        #region [Service layer] Update fundings data
        public IResult Update(Fundings model)
        {
            return _fundingRepository.Update(Mapper.Map<FundingsDto>(model));
        }
        #endregion

        #region [Service layer] Insert data for Wire instruction and PipitInstruction
        public IResult<AddFundsWire> InsertWireTransfer(AddFundsWire wireTransfer, string userName)
        {
            return _fundingRepository.InsertWireTransfer(Mapper.Map<AddFunds_WireDto>(wireTransfer), userName);
        }


        public IResult InsertPipitTransfer(AddFundsPipit wireTransfer, string userName)
        {
            return _fundingRepository.InsertPipitTransfer(Mapper.Map<AddFunds_PipitDto>(wireTransfer), userName);
        }
        public IResult<AddFunds_BlockChainInfo> InsertBlockChainInfoTransfer(AddFunds_BlockChainInfo blockChainTransfer, string userName)
        {
            return _fundingRepository.InsertBlockChainInfoTransfer(
                Mapper.Map<AddFunds_BlockChainInfoDto>(blockChainTransfer), userName);
        }
        #endregion

        #region [Service layer] Update data for Wire Instruction and Pipit instruction

        public IResult UpdateWireTransfer(AddFundsWire wireTransfer, string userName, int status = 0)
        {
            return _fundingRepository.UpdateWireTransfer(Mapper.Map<AddFunds_WireDto>(wireTransfer), userName, status);
        }

        public IResult UpdateWireTransferStatusForAdmin(Guid fundId, string userName, int status, string memo)
        {
            return _fundingRepository.UpdateWireTransferStatusForAdmin(fundId, userName, status, memo);
        }

        public IResult UpdatePipitTransfer(AddFundsPipit wireTransfer)
        {
            return new Result("Not realized method");
        }
        
        public IResult UpdateBlockChainInfoTransfer(AddFunds_BlockChainInfo wireTransfer)
        {
            return _fundingRepository.UpdateBlockChainInfoTransfer(Mapper.Map<AddFunds_BlockChainInfoDto>(wireTransfer));
        }

        #endregion

        #region [Service layer] Delete fundings data
        public IResult Delete(Guid id)
        {
            return _fundingRepository.Delete(id);
        } 
        #endregion
    }
}