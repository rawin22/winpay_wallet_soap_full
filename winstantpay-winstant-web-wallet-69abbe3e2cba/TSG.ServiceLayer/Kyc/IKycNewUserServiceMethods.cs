using System;
using TSG.Models.DTO.Transfers;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.Kyc
{
    public interface IKycNewUserServiceMethods : ICrud<KycNewUserDto, Guid>, IInsertByT<KycNewUserDto>
    {
        
    }
}