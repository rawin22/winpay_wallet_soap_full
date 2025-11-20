using System;
using System.Collections.Generic;
using TSG.Dal.Transfers;
using TSG.Models.DTO.Transfers;
using TSG.Models.ServiceModels.Transfers;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.Transfers
{
    public class TransfersServiceMethods : ITransfersServiceMethods
    {
        private readonly ITransfersRepository _transfersRepository;

        public TransfersServiceMethods(ITransfersRepository transfersRepository)
        {
            _transfersRepository = transfersRepository;
        }
        
        public IResult<TransfersSo> GetById(Guid id)
        {
            var query = _transfersRepository.GetById(id);
            return new Result<TransfersSo>(AutoMapper.Mapper.Map<TransfersSo>(query.Obj), query.Message);
        }

        public IResult Delete(Guid id) => _transfersRepository.Delete(id);

        public IResult Update(TransfersSo model) => _transfersRepository.Update(AutoMapper.Mapper.Map<TransfersDto>(model));

        public IResult<List<TransfersSo>> GetAll()
        {
            var query = _transfersRepository.GetAll();
            return new Result<List<TransfersSo>>(AutoMapper.Mapper.Map<List<TransfersSo>>(query.Obj), query.Message);
        }

        public IResult Insert(TransfersSo model) =>
            _transfersRepository.Insert(AutoMapper.Mapper.Map<TransfersDto>(model));
    }
}