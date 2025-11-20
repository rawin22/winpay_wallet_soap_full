using System.Collections.Generic;
using TSG.Models.DTO;

namespace TSG.Dal.Interfaces.Fundings
{
    public interface IFundingChangesRepository
    {
        List<FundChangesDto> GetAllChangeById(System.Guid fundingId);

    }
}