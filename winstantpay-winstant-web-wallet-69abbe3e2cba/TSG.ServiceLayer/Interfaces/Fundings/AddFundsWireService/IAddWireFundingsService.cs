using System;
using TSG.Models.ServiceModels;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.Interfaces.Fundings.AddFundsWireService
{
    public interface IAddWireFundingsService
    {
        IResult<AddFundsWire> GetWireFundingById(Guid parentId, string userName = "");
        IResult<AddFundsWire> InsertWireTransfer(AddFundsWire wireTransfer, string userName);
        IResult UpdateWireTransfer(AddFundsWire wireTransfer, string userName, int status = 0);
        IResult UpdateWireTransferStatusForAdmin(Guid fundId, string userName, int status, string memo);
    }
}