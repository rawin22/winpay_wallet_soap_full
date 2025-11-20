using System;
using TSG.Models.DTO;
using TSG.Models.ServiceModels;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.Interfaces.Fundings.AddFundsWireRepositories
{
    public interface IAddBlockchainInfoRepository
    {
        IResult<AddFunds_BlockChainInfo> GetBlockChainInfoFundingById(Guid parentId, string userName = "");
        IResult<AddFunds_BlockChainInfo> GetBlockChainInfoFundingByTimeStamp(Guid timestamp);

        IResult<AddFunds_BlockChainInfo> InsertBlockChainInfoTransfer(AddFunds_BlockChainInfoDto wireTransfer, string userName);
        IResult UpdateBlockChainInfoTransfer(AddFunds_BlockChainInfoDto wireTransfer);
    }
}