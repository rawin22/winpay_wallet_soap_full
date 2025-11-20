using System;
using TSG.Models.DTO.Transfers;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.Kyc
{
    public interface IKycNewUserRepository : ICrud<KycNewUserDto, Guid>, IInsert<KycNewUserDto>
    {
        
    }
}