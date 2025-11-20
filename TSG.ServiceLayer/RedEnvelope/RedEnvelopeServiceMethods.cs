using System;
using System.Collections.Generic;
using TSG.Dal.RedEnvelope;
using TSG.Models.DTO.Transfers;
using TSG.Models.ServiceModels.Transfers.RedEnvelope;
using TSG.ServiceLayer.RedEnvelope;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.RedEnvelope
{
    public class RedEnvelopeServiceMethods : IRedEnvelopeServiceMethods
    {
        private readonly IRedEnvelopeRepository _envelopeRepository;

        public RedEnvelopeServiceMethods(IRedEnvelopeRepository envelopeRepository)
        {
            _envelopeRepository = envelopeRepository;
        }

        public IResult Delete(Guid id) => _envelopeRepository.Delete(id);

        public IResult Update(RedEnvelopeSo model) =>
            _envelopeRepository.Update(AutoMapper.Mapper.Map<RedEnvelopeDto>(model));

        public IResult<List<RedEnvelopeSo>> GetAll()
        {
            var resQuery = _envelopeRepository.GetAll();
            return new Result<List<RedEnvelopeSo>>(AutoMapper.Mapper.Map<List<RedEnvelopeSo>>(resQuery.Obj), resQuery.Message);
        }

        public IResult<RedEnvelopeSo> Insert(RedEnvelopeSo model)
        {
            model.RedEnvelope_Id = Guid.NewGuid();
            var res = _envelopeRepository.Insert(AutoMapper.Mapper.Map<RedEnvelopeDto>(model)); 
            return new Result<RedEnvelopeSo>(model, res.Message); 
        }

        public IResult<RedEnvelopeSo> GetById(Guid id)
        {
            var resQuery = _envelopeRepository.GetById(id);
            return new Result<RedEnvelopeSo>(AutoMapper.Mapper.Map<RedEnvelopeSo>(resQuery.Obj), resQuery.Message);
        }
    }
}