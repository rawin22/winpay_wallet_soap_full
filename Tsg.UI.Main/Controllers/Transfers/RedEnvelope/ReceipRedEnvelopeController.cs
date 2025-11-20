using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Newtonsoft.Json;
using Tsg.UI.Main.ApiMethods.Payments;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.APIModels.InstantPayment;
using TSG.Models.APIModels.TransfersApiModel;
using TSG.Models.Enums;
using TSG.Models.ServiceModels.Transfers.RedEnvelope;
using TSG.ServiceLayer.AutomaticExchange.DependencyLiquidForUserServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidCcyListServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidOverDraftUserServiceMethods;
using TSG.ServiceLayer.RedEnvelope;
using TSG.ServiceLayer.Transfers;
using TSG.ServiceLayer.Users;
using WinstantPay.Common.Object;

namespace Tsg.UI.Main.Controllers.Transfers.RedEnvelope
{
    [Authorize]
    public class ReceipRedEnvelopeController : Controller
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IUsersServiceMethods _usersServiceMethods;
        private readonly IDependencyLiquidForUserServiceMethods _dependencyLiquidForUserService;
        private readonly ILiquidCcyListServiceMethods _liquidCcyListServiceMethods;
        private readonly ILiquidOverDraftUserServiceMethods _liquidOverDraftUserServiceMethods;
        private readonly IRedEnvelopeServiceMethods _envelopeServiceMethods;
        private readonly ITransfersServiceMethods _transfersServiceMethods;

        private static readonly string cryptoCurrency = ConfigurationManager.AppSettings["cryptoCurrencies"];

        public ReceipRedEnvelopeController(IDependencyLiquidForUserServiceMethods dependencyLiquidForUserService,
            ILiquidCcyListServiceMethods liquidCcyListServiceMethods,
            ILiquidOverDraftUserServiceMethods liquidOverDraftUserServiceMethods, IUsersServiceMethods usersServiceMethods,
            IRedEnvelopeServiceMethods envelopeServiceMethods, ITransfersServiceMethods transfersServiceMethods)
        {
            _usersServiceMethods = usersServiceMethods;
            _dependencyLiquidForUserService = dependencyLiquidForUserService;
            _liquidCcyListServiceMethods = liquidCcyListServiceMethods;
            _liquidOverDraftUserServiceMethods = liquidOverDraftUserServiceMethods;
            _envelopeServiceMethods = envelopeServiceMethods;
            _transfersServiceMethods = transfersServiceMethods;
        }
        [System.Web.Http.HttpPost]
        public ActionResult Post(GettingTustedTokenModel model)
        {
            UserRepository userRepository = new UserRepository();
            var result = new StandartResponse();
            try
            {
                IEnumerable<string> outerUserToken;
                //Check user by transfered record
                var resTransferedObj = _transfersServiceMethods.GetById(model.TransferedRecordId);
                if (!resTransferedObj.Success || resTransferedObj.Obj == null || resTransferedObj.Obj.Transfers_SourceType != TransfersSourceTypeEnum.RedEnvelope)
                    return Json(new StandartResponse("Not found transfered  by this Id."));

                if (resTransferedObj.Obj.Transfers_AcceptedDate.HasValue && (!resTransferedObj.Obj.Transfers_IsRejected.HasValue || !(bool)resTransferedObj.Obj.Transfers_IsRejected))
                    return Json(new StandartResponse("Transfered red envelope was activated early"));

                if (resTransferedObj.Obj.Transfers_IsRejected.HasValue && (bool)resTransferedObj.Obj.Transfers_IsRejected)
                    return Json(new StandartResponse("Transfered red envelope was rejected early"));

                var userRecipient = _usersServiceMethods.GetUserAliasesByUserName(AppSecurity.CurrentUser.UserName);
                // Check recipient
                if (!userRecipient.Success || userRecipient.Obj == null)
                    return Json(new StandartResponse("User not found for this transfered red envelope by this Id."));
                // Check all WPayId by recipient
                if (userRecipient.Obj.Wpay_UserName != resTransferedObj.Obj.Transfers_TransferRecipient)
                    return Json(new StandartResponse("User not found for this transfered red envelope by this Id."));


                // update rec into transfers table
                Result<RedEnvelopeSo> redenvelopeGlobal = null;
                if (resTransferedObj.Obj.Transfers_LinkToSourceRow.HasValue)
                {
                    var redEnvelope = _envelopeServiceMethods.GetById(resTransferedObj.Obj.Transfers_LinkToSourceRow.Value);
                    if (!redEnvelope.Success || redEnvelope.Obj == null)
                        return Json(new StandartResponse("Error by red envelope transfer", redEnvelope.Message));

                    ApiNewInstantPaymentViewModel apiNewInstantPayment = new ApiNewInstantPaymentViewModel();
                    apiNewInstantPayment.FromCustomer = ConfigurationManager.AppSettings["redEnvelopeWPayIdInTheMiddle"];
                    apiNewInstantPayment.ToCustomer = model.Action ? redEnvelope.Obj.RedEnvelope_WPayIdTo : redEnvelope.Obj.RedEnvelope_WPayIdFrom;
                    apiNewInstantPayment.Amount = Convert.ToDecimal(redEnvelope.Obj.RedEnvelope_Amount);
                    apiNewInstantPayment.CurrencyCode = redEnvelope.Obj.RedEnvelope_CurrencyCode;
                    apiNewInstantPayment.Memo =
                        model.Action ? $"Red Envelope from {redEnvelope.Obj.RedEnvelope_WPayIdFrom}"
                        : $"Red Envelope reverted to {resTransferedObj.Obj.Transfers_TransferParent} with WPayID {redEnvelope.Obj.RedEnvelope_WPayIdFrom} " +
                          $"[{redEnvelope.Obj.RedEnvelope_CurrencyCode} {redEnvelope.Obj.RedEnvelope_Amount}]";

                    var implementedRedEnvelopePaymentAcc = new TSG.Models.APIModels.UserInfo()
                    {
                        UserName = ConfigurationManager.AppSettings["redEnvelopeLogin"],
                        Password = ConfigurationManager.AppSettings["redEnvelopePassword"],
                        UserId = ConfigurationManager.AppSettings["redEnvelopeCallerId"]
                    };

                    #region Payment to Red envelope service process 
                    NewInstantPaymentMethods m = new NewInstantPaymentMethods(implementedRedEnvelopePaymentAcc);
                    var res = m.Create(apiNewInstantPayment);
                    if (res.ServiceResponse.HasErrors)
                        return Json(new StandartResponse(StringExtensions.ConvertServiceResponseToSingleString(res.ServiceResponse.Responses)));
                    _logger.Info(JsonConvert.SerializeObject(res));
                    var paymentTorecipientResult = m.Post(Guid.Parse(res.PaymentInformation.PaymentId));
                    if (paymentTorecipientResult.ServiceResponse.HasErrors)
                        return Json(new StandartResponse(StringExtensions.ConvertServiceResponseToSingleString(paymentTorecipientResult.ServiceResponse.Responses)));
                    _logger.Info(JsonConvert.SerializeObject(paymentTorecipientResult));

                    #endregion

                    redEnvelope.Obj.RedEnvelope_DateTransferedToRecipient = model.Action ? DateTime.Now : (DateTime?)null;
                    redEnvelope.Obj.RedEnvelope_RecipientPaymentId = model.Action ? Guid.Parse(AppSecurity.CurrentUser.UserId) : (Guid?)null;
                    redEnvelope.Obj.RedEnvelope_RecipientUserName = AppSecurity.CurrentUser.UserName;
                    redEnvelope.Obj.RedEnvelope_RejectionNote = !String.IsNullOrEmpty(model.RejectionNote) ? model.RejectionNote : null;
                    redEnvelope.Obj.RedEnvelope_RedEnvelopePaymentId = Guid.Parse(res.PaymentInformation.PaymentId);

                    var resUpdateRedEnvelope = _envelopeServiceMethods.Update(redEnvelope.Obj);
                    if (!resUpdateRedEnvelope.Success)
                    {
                        _logger.Error(resUpdateRedEnvelope.Message);
                    }

                    redenvelopeGlobal = redEnvelope as Result<RedEnvelopeSo>;
                }
                resTransferedObj.Obj.Transfers_AcceptedDate = DateTime.Now;
                resTransferedObj.Obj.Transfers_IsRejected = !model.Action;

                var trasferRes = _transfersServiceMethods.Update(resTransferedObj.Obj);
                if (!trasferRes.Success)
                    return Json(new StandartResponse("Error by red envelope transfer", trasferRes.Message));
                return Json(new StandartResponse(true, model.Action ? "Transfer for red envelope finished successfully" : "Transfer for red envelope is rejected"));

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

        [HttpPost]
        public ActionResult Details(Guid rowId)
        {
            var transfer = _transfersServiceMethods.GetById(rowId);
            if (!transfer.Success || transfer.Obj == null || transfer.Obj.Transfers_SourceType != TransfersSourceTypeEnum.RedEnvelope)
                return View("~/Views/InputOutputObjects/_RedEnvelopDetails.cshtml", new StandartResponse<RedEnvelopeSo>(null,"Error by red envelope get details", transfer.Message));

            if (!transfer.Obj.Transfers_LinkToSourceRow.HasValue)
                return View("~/Views/InputOutputObjects/_RedEnvelopDetails.cshtml", new StandartResponse<RedEnvelopeSo>(null,"Error by red envelope get details", transfer.Message));

            var redEnvelope = _envelopeServiceMethods.GetById(transfer.Obj.Transfers_LinkToSourceRow.Value);
            if (!redEnvelope.Success || redEnvelope.Obj == null)
                return View("~/Views/InputOutputObjects/_RedEnvelopDetails.cshtml", new StandartResponse<RedEnvelopeSo>(redEnvelope.Obj, "Error by red envelope get details", redEnvelope.Message));
            return View("~/Views/InputOutputObjects/_RedEnvelopDetails.cshtml", new StandartResponse<RedEnvelopeSo>(redEnvelope.Obj, redEnvelope.Success, redEnvelope.Message));

        }
    }
}