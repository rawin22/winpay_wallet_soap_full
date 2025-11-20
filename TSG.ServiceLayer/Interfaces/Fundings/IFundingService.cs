using System;
using System.Collections.Generic;
using TSG.ServiceLayer.Interfaces.Fundings.AddFundsWireService;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.Interfaces.Fundings
{
    public interface IFundingsService : IAddPipitFundingsService, IAddWireFundingsService, IAddBlockchainInfoService, IDelete<Guid>
    {
        IResult<List<Models.ServiceModels.Fundings>> GetAllFundings();
        IResult<List<Models.ServiceModels.Fundings>> GetAllFundings(int status);
        IResult<List<Models.ServiceModels.Fundings>> GetAllFundings(int status, string userName);
        IResult<List<Models.ServiceModels.Fundings>> GetAllFundings(string userName);
        IResult Update(Models.ServiceModels.Fundings model);
    }
}