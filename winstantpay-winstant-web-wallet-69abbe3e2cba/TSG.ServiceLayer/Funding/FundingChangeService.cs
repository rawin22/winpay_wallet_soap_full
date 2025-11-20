using System;
using System.Collections.Generic;
using AutoMapper;
using TSG.Dal.Interfaces.Fundings;
using TSG.Models.ServiceModels;
using TSG.ServiceLayer.Interfaces.Fundings;

namespace TSG.ServiceLayer.Funding
{
    public class FundingChangeService : IFundingChangesService
    {
        private readonly IFundingChangesRepository _fundingsChangesRepository;

        public FundingChangeService(IFundingChangesRepository fundingsChangesRepository)
        {
            _fundingsChangesRepository = fundingsChangesRepository;
        }

        public List<FundChanges> GetAllChangeById(Guid fundingId)
        {
            return Mapper.Map<List<FundChanges>>(_fundingsChangesRepository.GetAllChangeById(fundingId));
        }
    }
}