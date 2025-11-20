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
using TSG.ServiceLayer.LimitPayment.DaPayLimitsServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTabServiceMethods;
using TSG.ServiceLayer.Transfers;
using TSG.ServiceLayer.Users;
using WinstantPay.Common.CryptDecriptInfo;

namespace Tsg.UI.Main.APIControllers.Transfers.TrustedTokenTransfers
{
    [ApiFilter]
    public class ApiReceipTustedTokenController : ApiController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IUsersServiceMethods _usersServiceMethods;
        private readonly ITransfersServiceMethods _transfersServiceMethods;
        private readonly IDaPayLimitsServiceMethods _daPayLimitsService;
        private readonly IDaPayLimitsTabServiceMethods _daPayLimitsTabServiceMethods;

        public ApiReceipTustedTokenController(IUsersServiceMethods usersServiceMethods, ITransfersServiceMethods transfersServiceMethods, IDaPayLimitsServiceMethods daPayLimitsService,
            IDaPayLimitsTabServiceMethods daPayLimitsTabServiceMethods)
        {
            _usersServiceMethods = usersServiceMethods;
            _transfersServiceMethods = transfersServiceMethods;
            _daPayLimitsService = daPayLimitsService;
            _daPayLimitsTabServiceMethods = daPayLimitsTabServiceMethods;
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] GettingTustedTokenModel model)
        {
            UserRepository userRepository = new UserRepository();
            var result = new StandartResponse();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out var ui))
                {

                    //Check user by transfered record
                    var res = _transfersServiceMethods.GetById(model.TransferedRecordId);
                    if (!res.Success || res.Obj == null || res.Obj.Transfers_SourceType != TransfersSourceTypeEnum.TokenTransfers)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("Not found transfered token by this Id."));


                    if (res.Obj.Transfers_AcceptedDate.HasValue)
                    {
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("Transfered token was activated early"));
                    }

                    if (res.Obj.Transfers_IsRejected.HasValue && (bool)res.Obj.Transfers_IsRejected)
                    {
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("Transfered token was rejected early"));
                    }

                    var userRecipient = _usersServiceMethods.GetUserAliasesByUserName(ui.UserName);
                    // Check recipient
                    if (!userRecipient.Success || userRecipient.Obj == null)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("User not found for this transfered token by this Id."));
                    // Check all WPayId by recipient
                    if (userRecipient.Obj.Wpay_UserName != res.Obj.Transfers_TransferRecipient) /*|| userRecipient.Obj.Wpay_Ids.All(a => a != res.Obj.Transfers_TransferRecipient))*/
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("User not found for this transfered token by this Id."));

                    res.Obj.Transfers_AcceptedDate = DateTime.Now;
                    // update rec into transfers table
                    
                    if (res.Obj.Transfers_LinkToSourceRow.HasValue)
                    {
                        var token = _daPayLimitsService.GetById(res.Obj.Transfers_LinkToSourceRow.Value);
                        if (!token.Success || token.Obj == null)
                            return Content(HttpStatusCode.BadRequest, new StandartResponse("Error by token transfer", token.Message));
                        token.Obj.DaPayLimits_LastModifiedDate = DateTime.Now;
                        token.Obj.DaPayLimits_IsTransfered = false;

                        // Set recepient as new owner
                        if (model.Action)
                        {
                            token.Obj.DaPayLimits_UserData = Crypto.Encrypt(Newtonsoft.Json.JsonConvert.SerializeObject(
                                new UserData()
                                {
                                    UserId = ui.UserId,
                                    UserName = ui.UserName,
                                    Password = ui.Password
                                }
                            ), token.Obj.DaPayLimits_SecretCode.ToString());
                            token.Obj.DaPayLimits_UserName = ui.UserName;

                            res.Obj.Transfers_IsRejected = false;
                            var tokenUpdateRes = _daPayLimitsService.Update(token.Obj);
                            if (!tokenUpdateRes.Success)
                                return Content(HttpStatusCode.BadRequest, new StandartResponse("Error by token transfer", tokenUpdateRes.Message));
                        }
                        // Return token
                        else
                        {
                            res.Obj.Transfers_IsRejected = true;
                            var tokenUpdateRes = _daPayLimitsService.Update(token.Obj);
                            if (!tokenUpdateRes.Success)
                                return Content(HttpStatusCode.BadRequest, new StandartResponse("Error by token transfer", tokenUpdateRes.Message));
                        }
                    }
                    var trasferRes = _transfersServiceMethods.Update(res.Obj);
                    if (!trasferRes.Success)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("Error by token transfer", trasferRes.Message));
                    return Ok(new StandartResponse(true,  $"Transfer for token {(model.Action  ? "finished successfully": "is rejected")}"));
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