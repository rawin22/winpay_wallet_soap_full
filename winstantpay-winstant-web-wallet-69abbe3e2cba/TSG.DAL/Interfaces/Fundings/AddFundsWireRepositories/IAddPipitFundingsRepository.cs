using System;
using TSG.Models.DTO;
using TSG.Models.ServiceModels;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.Interfaces.Fundings.AddFundsWireRepositories
{
    public interface IAddPipitFundingsRepository
    {
        IResult<AddFundsPipit> GetPipitFundingById(Guid parentId, string userName = "");
        IResult<AddFundsPipit> GetPipitFundingByBarCode(string barcode, string vendorcode);
        IResult InsertPipitTransfer(AddFunds_PipitDto wireTransfer, string userName);
        IResult UpdatePipitTransfer(AddFunds_PipitDto wireTransfer);
    }
}