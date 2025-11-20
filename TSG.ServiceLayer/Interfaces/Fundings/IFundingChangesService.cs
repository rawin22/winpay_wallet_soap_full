using System.Collections.Generic;
using TSG.Models.ServiceModels;

namespace TSG.ServiceLayer.Interfaces.Fundings
{
    public interface IFundingChangesService
    {
        List<FundChanges> GetAllChangeById(System.Guid fundingId);
    }
}