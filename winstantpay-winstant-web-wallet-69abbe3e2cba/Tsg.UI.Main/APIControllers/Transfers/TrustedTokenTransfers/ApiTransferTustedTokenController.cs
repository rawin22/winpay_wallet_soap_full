using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.APIModels.TransfersApiModel;
using TSG.Models.Enums;
using TSG.Models.ServiceModels.Transfers;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsServiceMethods;
using TSG.ServiceLayer.Transfers;
using TSG.ServiceLayer.Users;

namespace Tsg.UI.Main.APIControllers.Transfers.TrustedTokenTransfers
{
    [ApiFilter]
    public class ApiTransferTustedTokenController : ApiController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IUsersServiceMethods _usersServiceMethods;
        private readonly ITransfersServiceMethods _transfersServiceMethods;
        private readonly IDaPayLimitsServiceMethods _daPayLimitsService;

        public ApiTransferTustedTokenController(IUsersServiceMethods usersServiceMethods, ITransfersServiceMethods transfersServiceMethods, IDaPayLimitsServiceMethods daPayLimitsService)
        {
            _usersServiceMethods = usersServiceMethods;
            _transfersServiceMethods = transfersServiceMethods;
            _daPayLimitsService = daPayLimitsService;
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] TransferTustedTokenModel model)
        {
            UserRepository userRepository = new UserRepository();
            var result = new StandartResponse();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out var ui))
                {
                    //Check user by WPayId
                    var res = _usersServiceMethods.ExistedUserByAliasName(model.AliasToUser);
                    if (!res.Success || res.Obj == null)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("Not found user by this WPayId."));

                    // Check token validation
                    var tokenQueryRes = _daPayLimitsService.GetById(model.TokenId);
                    if (!tokenQueryRes.Success || tokenQueryRes.Obj == null || tokenQueryRes.Obj.DaPayLimits_UserName != ui.UserName)
                    {
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("Not found Transfered token."));
                    }
                    if (tokenQueryRes.Obj.DaPayLimits_IsTransfered)
                    {
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("Token was transferred earlier."));
                    }

                    var transfers = new TransfersSo()
                    {
                        Transfers_LinkToSourceRow = tokenQueryRes.Obj.DaPayLimits_ID,
                        Transfers_IsKycCreated = false,
                        Transfers_Source = "Transfer token",
                        Transfers_SourceType = TransfersSourceTypeEnum.TokenTransfers,
                        Transfers_TransferParent = ui.UserName,
                        Transfers_TransferRecipient = res.Obj.Wpay_UserName
                    };
                    var querySaveTransfer = _transfersServiceMethods.Insert(transfers);
                    if (querySaveTransfer.Success)
                        return Ok(new StandartResponse(true, "Ok"));
                    return Content(HttpStatusCode.BadRequest, new StandartResponse(querySaveTransfer.Message));
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (ApiException apiException)
            {
                result.InfoBlock = apiException.CustomInfoBlock;
                _logger.Error(apiException);
            }
            catch (Exception e)
            {
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
                _logger.Error(e);
            }
            return Ok(result);
        }
    }
}