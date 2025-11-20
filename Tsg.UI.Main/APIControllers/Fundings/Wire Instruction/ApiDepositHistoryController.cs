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
    public class ApiDepositHistoryController : ApiController
    {

        private readonly IFundingsService _fundingsService;
        public ApiDepositHistoryController(IFundingsService fundingsService)
        {
            _fundingsService = fundingsService;
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            var result = new DepositHistoryModel
            {
                Success = false,
                InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty },
                Fundings = new List<TSG.Models.ServiceModels.Fundings>()
            };

            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    result.Fundings = _fundingsService.GetAllFundings(ui.UserName).Obj.OrderByDescending(ob=>ob.Fundings_CreateDate).ToList();
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
