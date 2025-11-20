using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Http;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.APIModels.Fundings.Wire_Instructions;
using TSG.Models.Enums;
using TSG.ServiceLayer.Interfaces.Fundings;

namespace Tsg.UI.Main.APIControllers.Fundings.Wire_Instruction
{
    [ApiFilter]
    public class ApiWireTransferDetailsController : ApiController
    {
        private readonly IFundingsService _fundingsService;
        private IFundingSourcesService _fundingsSourceService;

        public ApiWireTransferDetailsController(IFundingsService fundingsService, IFundingSourcesService fundingSourcesService)
        {
            _fundingsService = fundingsService;
            _fundingsSourceService = fundingSourcesService;
        }

        [HttpGet]
        public IHttpActionResult Get(string fundId, string sourceId)
        {
            var result = new WireDetails
            {
                Success = false,
                InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty }
            };

            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {

                    Guid.TryParse(sourceId, out var guidSourceId);
                    Guid.TryParse(fundId, out var guidFundId);
                    if (guidFundId == default || guidSourceId == default)
                        throw new Exception("Source or funds doesn't setted");
                    var typeOfPaymentSource = _fundingsSourceService.GetById(guidSourceId);
                    if (typeOfPaymentSource.Success &&
                        typeOfPaymentSource.Obj.FundingSources_DesignName == "WinstantPay")
                    {
                        var res = _fundingsService.GetWireFundingById(guidFundId, ui.UserName);
                        if(!res.Success)
                            throw new Exception(res.Message);

                        result = AutoMapper.Mapper.Map<WireDetails>(res.Obj);
                        CurrencyRepository curRepo = new CurrencyRepository();
                        var bankCurrencies = curRepo.GetBankCurrencies();

                        result.ListOfLinkedBankAndCcy.AddRange(bankCurrencies.Select(s => new BankCurrencyData()
                        {
                            Id = s.BankCurrencyId,
                            BankCcyName = s.BankCurrencyName
                        }));
                        if (result.Fundings_FundChange != null && result.Fundings_FundChange.FundChanges_FundingToStatus > (int) FundingStatus.Pending)
                        {
                            result.ListOfLinkedBankAndCcy.Clear();
                            result.ListOfLinkedBankAndCcy.Add(bankCurrencies.Where(f=>result.WireDetails_BankCcyId != null && f.BankCurrencyId == result.WireDetails_BankCcyId.Value).Select(s=>new BankCurrencyData()
                            {
                                Id = s.BankCurrencyId,
                                BankCcyName = s.BankCurrencyName
                            }).FirstOrDefault());
                        }

                        if (File.Exists(HostingEnvironment.MapPath("~/Uploads/" + res.Obj.AddFundsWire_FileName)))
                        {
                            result.ImageInBase64 = Convert.ToBase64String(
                                File.ReadAllBytes(
                                    HostingEnvironment.MapPath("~/Uploads/" + res.Obj.AddFundsWire_FileName) ??
                                    String.Empty));
                        }
                        result.WireDetails_FilePath = 
                            res.Obj.AddFundsWire_FilePath?.Replace(HostingEnvironment.MapPath("~/Uploads/") ?? "", "") ?? String.Empty;
                        result.InfoBlock = new InfoBlock(){Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "Ok", UserMessage = ""};
                        result.Success = true;

                        return Ok(result);
                    }
                    else
                    {
                        throw new Exception("Incorrect funding_id and funding_source_id pair");
                    }

                }
            }
            catch (ApiException apiException)
            {
                result.InfoBlock = apiException.CustomInfoBlock;
            }
            catch (Exception e)
            {
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
            }

            return Ok(result);
        }
    }
}