using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using TSG.Dal.Interfaces.Fundings;
using TSG.Models.DTO;
using TSG.Models.ServiceModels;
using TSG.ServiceLayer.Interfaces.Fundings;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.Funding
{
    public class FundingSourcesServices : IFundingSourcesService
    {
        private IFundingSourcesRepository _repository;

        public FundingSourcesServices(IFundingSourcesRepository repository)
        {
            _repository = repository;

            //Mapper.Initialize(fr=>
            //{
            //    fr.RecognizePrefixes("FundingSources_");
            //    fr.RecognizeDestinationPrefixes("FundingSources_");
            //    fr.CreateMap<FundingSources, FundingSourcesDto>();
            //});
        }
        
        public IResult Delete(Guid id)
        {
            return _repository.Delete(id);
        }

        public IResult<FundingSources> Insert(FundingSources model)
        {
            var modelDto = Mapper.Map<FundingSources, FundingSourcesDto>(model);
            var insertRes = _repository.Insert(modelDto);
            return new Result<FundingSources>(Mapper.Map<FundingSourcesDto, FundingSources>(insertRes.Obj), insertRes.Message);
        }

        public IResult Update(FundingSources model)
        {
            var modelDto = Mapper.Map<FundingSources, FundingSourcesDto>(model);
            return _repository.Update(modelDto);
        }

        public IResult<List<FundingSources>> GetAll()
        {
            var allFs = _repository.GetAll();
            return new Result<List<FundingSources>>(Mapper.Map<List<FundingSourcesDto>, List<FundingSources>>(allFs.Obj), allFs.Message);
        }

        public IResult<FundingSources> GetById(Guid id)
        {
            var singleFs = _repository.GetById(id);
            return new Result<FundingSources>(Mapper.Map<FundingSourcesDto, FundingSources>(singleFs.Obj), singleFs.Message);
        }

        public IResult<List<FundingSources>> GetAllActiveFundingSources()
        {
            var getAllActive = GetAll();
            return new Result<List<FundingSources>>(getAllActive.Obj.Where(w=>!w.FundingSources_IsDeleted).ToList(), getAllActive.Message);
        }
    }
}