using TSG.Models.DTO;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.Interfaces.Fundings
{
    public interface IFundingSourcesRepository : ICrud<FundingSourcesDto, System.Guid>, IGetById<FundingSourcesDto, System.Guid>, IInsertByT<FundingSourcesDto>
    {
        
    }
}