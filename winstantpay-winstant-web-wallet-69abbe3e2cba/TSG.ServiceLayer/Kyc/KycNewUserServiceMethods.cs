using System;
using System.Collections.Generic;
using TSG.Dal.Kyc;
using TSG.Models.DTO.Transfers;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.Kyc
{
    public class KycNewUserServiceMethods : IKycNewUserServiceMethods
    {
        private readonly IKycNewUserRepository _kycNewUserRepository;

        public KycNewUserServiceMethods(IKycNewUserRepository kycNewUserRepository)
        {
            _kycNewUserRepository = kycNewUserRepository;
        }

        public IResult Delete(Guid id) => _kycNewUserRepository.Delete(id);
        public IResult Update(KycNewUserDto model) => _kycNewUserRepository.Update(model);

        public IResult<List<KycNewUserDto>> GetAll() => _kycNewUserRepository.GetAll();

        public IResult<KycNewUserDto> Insert(KycNewUserDto model)
        {
            model.Id = Guid.NewGuid();
            var res = _kycNewUserRepository.Insert(model);
            return new Result<KycNewUserDto>(model, res.Message);
        }
    }
}