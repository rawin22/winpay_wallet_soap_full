using System;
using TSG.Models.DTO.Transfers;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.Transfers
{
    public interface ITransfersRepository : IGetById<TransfersDto, Guid>, ICrud<TransfersDto, Guid>, IInsert<TransfersDto>
    {
        
    }
}