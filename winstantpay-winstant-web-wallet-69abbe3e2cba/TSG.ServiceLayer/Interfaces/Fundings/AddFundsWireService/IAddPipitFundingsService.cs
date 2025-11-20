using System;
using TSG.Models.ServiceModels;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.Interfaces.Fundings.AddFundsWireService
{
    public interface IAddPipitFundingsService
    {
        IResult<AddFundsPipit> GetPipitFundingById(Guid parentId, string userName = "");
        IResult<AddFundsPipit> GetPipitFundingByBarCode(string barcode, string vendorcode);

        IResult InsertPipitTransfer(AddFundsPipit wireTransfer, string userName);
        IResult UpdatePipitTransfer(AddFundsPipit wireTransfer);
    }
}