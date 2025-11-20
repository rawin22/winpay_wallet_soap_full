using System;
using TSG.Models.DTO;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.Interfaces.Fundings.AddFundsWireRepositories
{
    public interface IAddWireFundingsRepository
    {
        IResult<Models.ServiceModels.AddFundsWire> GetWireFundingById(Guid parentId, string userName = "");
        IResult<Models.ServiceModels.AddFundsWire> InsertWireTransfer(AddFunds_WireDto wireTransfer, string userName);
        IResult UpdateWireTransfer(AddFunds_WireDto wireTransfer, string userName, int status);
        IResult UpdateWireTransferStatusForAdmin(Guid fundId, string userName, int status, string memo);
    }
}