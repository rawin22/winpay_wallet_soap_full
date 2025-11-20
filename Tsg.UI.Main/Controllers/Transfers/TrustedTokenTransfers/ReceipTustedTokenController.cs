using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.APIModels.TransfersApiModel;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsServiceMethods;
using TSG.ServiceLayer.Transfers;
using TSG.ServiceLayer.Users;
using WinstantPay.Common.CryptDecriptInfo;

namespace Tsg.UI.Main.Controllers.Transfers.TrustedTokenTransfers
{
    [Authorize]
    public class ReceipTustedTokenController : Controller
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IUsersServiceMethods _usersServiceMethods;
        private readonly ITransfersServiceMethods _transfersServiceMethods;
        private readonly IDaPayLimitsServiceMethods _daPayLimitsService;

        public ReceipTustedTokenController(IUsersServiceMethods usersServiceMethods, ITransfersServiceMethods transfersServiceMethods, IDaPayLimitsServiceMethods daPayLimitsService)
        {
            _usersServiceMethods = usersServiceMethods;
            _transfersServiceMethods = transfersServiceMethods;
            _daPayLimitsService = daPayLimitsService;
        }

        [HttpPost]
        public ActionResult Post(GettingTustedTokenModel model)
        {
            UserRepository userRepository = new UserRepository();
            var result = new StandartResponse();
            try
            {

                //Check user by transfered record
                var res = _transfersServiceMethods.GetById(model.TransferedRecordId);
                if (!res.Success || res.Obj == null)
                    return Json(new StandartResponse("Not found transfered token by this Id."));
                var userRecipient = _usersServiceMethods.GetUserAliasesByUserName(AppSecurity.CurrentUser.UserName);
                // Check recipient
                if (!userRecipient.Success || userRecipient.Obj == null)
                    return Json(new StandartResponse("User not found for this transfered token by this Id."));
                // Check all WPayId by recipient
                if (userRecipient.Obj.Wpay_UserName != res.Obj.Transfers_TransferRecipient) /*|| userRecipient.Obj.Wpay_Ids.All(a => a != res.Obj.Transfers_TransferRecipient))*/
                    return Json(new StandartResponse("User not found for this transfered token by this Id."));

                res.Obj.Transfers_AcceptedDate = DateTime.Now;
                // update rec into transfers table

                if (res.Obj.Transfers_LinkToSourceRow.HasValue)
                {
                    var token = _daPayLimitsService.GetById(res.Obj.Transfers_LinkToSourceRow.Value);
                    if (!token.Success || token.Obj == null)
                        return Json(new StandartResponse("Error by token transfer", token.Message));
                    token.Obj.DaPayLimits_LastModifiedDate = DateTime.Now;
                    token.Obj.DaPayLimits_IsTransfered = false;

                    // Set recepient as new owner
                    if (model.Action)
                    {
                        token.Obj.DaPayLimits_UserData = Crypto.Encrypt(Newtonsoft.Json.JsonConvert.SerializeObject(
                            new UserData()
                            {
                                UserId = AppSecurity.CurrentUser.UserId,
                                UserName = AppSecurity.CurrentUser.UserName,
                                Password = AppSecurity.CurrentUser.Password
                            }
                        ), token.Obj.DaPayLimits_SecretCode.ToString());
                        token.Obj.DaPayLimits_UserName = AppSecurity.CurrentUser.UserName;

                        res.Obj.Transfers_IsRejected = false;
                        var tokenUpdateRes = _daPayLimitsService.Update(token.Obj);
                        if (!tokenUpdateRes.Success)
                            return Json(new StandartResponse("Error by token transfer", tokenUpdateRes.Message));
                    }
                    // Return token
                    else
                    {
                        res.Obj.Transfers_IsRejected = true;
                        var tokenUpdateRes = _daPayLimitsService.Update(token.Obj);
                        if (!tokenUpdateRes.Success)
                            return Json(new StandartResponse("Error by token transfer", tokenUpdateRes.Message));
                    }
                }
                var trasferRes = _transfersServiceMethods.Update(res.Obj);
                if (!trasferRes.Success)
                    return Json(new StandartResponse("Error by token transfer", trasferRes.Message));
                return Json(new StandartResponse(true, "Transfer for token finished successfully"));
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
            return Json(result);
        }
    }
}