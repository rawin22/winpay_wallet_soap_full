using System;
using System.Collections.Generic;
using TSG.Dal.Interfaces.Fundings.AddFundsWireRepositories;
using TSG.Models.DTO;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.Interfaces.Fundings
{
    public interface IFundingsRepository : IDelete<Guid>, IGetById<Models.ServiceModels.Fundings, Guid>, IAddWireFundingsRepository, IAddPipitFundingsRepository, IAddBlockchainInfoRepository
    {
        IResult<List<Models.ServiceModels.Fundings>> GetAllFundings();
        IResult<List<Models.ServiceModels.Fundings>> GetAllFundings(string userName);
        IResult Update(FundingsDto model);

        //IResult InsertWireTransfer(AddFundsWire wireTransfer);
        //IResult InsertWireTransfer(AddFundsPipit wireTransfer);
        //IResult<AddFundsWire> GetWireFundingById(Guid parentId, string userName = "");
        //IResult<AddFundsPipit> GetPipitFundingById(Guid parentId, string userName = "");
    }
}