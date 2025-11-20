using System.Collections.Generic;
using TSG.Models.ServiceModels;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.Interfaces.Fundings
{
    public interface IFundingSourcesService :ICrud<FundingSources, System.Guid>, IGetById<FundingSources, System.Guid>, IInsertByT<FundingSources>
    {
        IResult<List<FundingSources>> GetAllActiveFundingSources();
    }
}