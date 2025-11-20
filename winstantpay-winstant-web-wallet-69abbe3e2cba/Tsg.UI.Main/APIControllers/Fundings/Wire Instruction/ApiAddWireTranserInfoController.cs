using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Tsg.UI.Main.ApiMethods.FavoriteCurrency;
using TSG.Models.APIModels;
using TSG.Models.APIModels.Fundings.Wire_Instructions;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Repository;
using TSG.ServiceLayer.Interfaces.Fundings;

namespace Tsg.UI.Main.APIControllers.Fundings.Wire_Instruction
{
    [ApiFilter]
    public class ApiAddWireTranserInfoController : ApiController
    {

        [HttpGet]
        public IHttpActionResult Get()
        {
            var result = new AddWireInstructionModel
            {
                Success = false,
                InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty },
                ListOfLinkedBankAndCcy = new List<BankCurrencyData>()
            };

            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    CurrencyRepository curRepo = new CurrencyRepository();
                    var bankCurrencies = curRepo.GetBankCurrencies();

                    result.ListOfLinkedBankAndCcy.AddRange(bankCurrencies.Select(s => new BankCurrencyData()
                    {
                        Id = s.BankCurrencyId,
                        BankCcyName = s.BankCurrencyName
                    }));
                    result.Success = true;
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK", UserMessage = "Data for instant payment selected correct" };
                }
                else
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.CheckUserError, DeveloperMessage = "Data for user not correct", UserMessage = "Data for user not correct" };
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

        [HttpPost]
        public IHttpActionResult Post(int bankCcyId)
        {
            var result = new AddWireInstructionModel
            {
                Success = false,
                InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty },
                ListOfLinkedBankAndCcy = new List<BankCurrencyData>()
            };

            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    CurrencyRepository curRepo = new CurrencyRepository();
                    var bankCurrencies = curRepo.GetBankCurrencies();

                    result.ListOfLinkedBankAndCcy.AddRange(bankCurrencies.Select(s => new BankCurrencyData()
                    {
                        Id = s.BankCurrencyId,
                        BankCcyName = s.BankCurrencyName
                    }));

                    WireInstructionRepository wireInstRepo = new WireInstructionRepository();
                    string wireInst = wireInstRepo.GetWireInstruction(bankCcyId);
                    if (wireInst.Contains("{client_name}"))
                        wireInst = wireInst.Replace("{client_name}", String.Format("{0} {1}", ui.FirstName, ui.LastName));
                    result.WireInstruction = wireInst;
                    result.Success = true;
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK", UserMessage = "Data for instant payment selected correct" };
                }
                else
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.CheckUserError, DeveloperMessage = "Data for user not correct", UserMessage = "Data for user not correct" };
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
