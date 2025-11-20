using System;
using TSG.Models.DTO.Transfers;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.RedEnvelope
{
    public interface IRedEnvelopeRepository : ICrud<RedEnvelopeDto, Guid>, IInsert<RedEnvelopeDto>, IGetById<RedEnvelopeDto, Guid>
    {
        
    }
}