using System;
using TSG.Models.DTO.Transfers;
using TSG.Models.ServiceModels.Transfers.RedEnvelope;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.RedEnvelope
{
    public interface IRedEnvelopeServiceMethods : ICrud<RedEnvelopeSo, Guid>, IInsertByT<RedEnvelopeSo>, IGetById<RedEnvelopeSo, Guid>
    {
        
    }
}