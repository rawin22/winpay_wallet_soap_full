using System;
using TSG.Models.ServiceModels.Transfers;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.Transfers
{
    public interface ITransfersServiceMethods: IGetById<TransfersSo, Guid>, ICrud<TransfersSo, Guid>, IInsert<TransfersSo>
    {
    }
}