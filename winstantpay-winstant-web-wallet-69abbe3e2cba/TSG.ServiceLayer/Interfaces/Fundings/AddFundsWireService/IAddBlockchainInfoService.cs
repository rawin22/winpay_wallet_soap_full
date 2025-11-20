using System;
using TSG.Models.ServiceModels;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.Interfaces.Fundings.AddFundsWireService
{
    public interface IAddBlockchainInfoService
    {
        IResult<AddFunds_BlockChainInfo> GetBlockChainInfoFundingById(Guid parentId, string userName = "");
        IResult<AddFunds_BlockChainInfo> GetBlockChainInfoFundingByTimeStamp(Guid timestamp);

        IResult<AddFunds_BlockChainInfo> InsertBlockChainInfoTransfer(AddFunds_BlockChainInfo wireTransfer, string userName);
        IResult UpdateBlockChainInfoTransfer(AddFunds_BlockChainInfo wireTransfer);
    }
}