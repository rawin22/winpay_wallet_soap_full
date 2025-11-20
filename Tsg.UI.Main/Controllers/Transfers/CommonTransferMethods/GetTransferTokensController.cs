using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Models.Security;
using TSG.Models.APIModels;
using TSG.Models.Enums;
using TSG.Models.ServiceModels.Transfers;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsServiceMethods;
using TSG.ServiceLayer.Transfers;

namespace Tsg.UI.Main.Controllers.Transfers.CommonTransferMethods
{
    [Authorize]
    public class GetTransferTokensController : Controller
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ITransfersServiceMethods _transfersServiceMethods;
        private readonly IDaPayLimitsServiceMethods _daPayLimitsService;

        public GetTransferTokensController(ITransfersServiceMethods transfersServiceMethods, IDaPayLimitsServiceMethods daPayLimitsService)
        {
            _transfersServiceMethods = transfersServiceMethods;
            _daPayLimitsService = daPayLimitsService;
        }
        public ActionResult Index()
        {
            var transfers = _transfersServiceMethods.GetAll().Obj.Where(w => w.Transfers_SourceType == TransfersSourceTypeEnum.TokenTransfers &&
                                                                             w.Transfers_TransferRecipient.ToLower() == AppSecurity.CurrentUser.UserName.ToLower() && (!w.Transfers_IsRejected.HasValue || w.Transfers_IsRejected.Value == false)
                                                                             && !w.Transfers_AcceptedDate.HasValue && w.Transfers_LinkToSourceRow.HasValue).ToList();
            var transfersToken = _daPayLimitsService.GetAll().Obj.Join(transfers, so => so.DaPayLimits_ID, t => t.Transfers_LinkToSourceRow, (so, transfersSo) => new TransfersTokenSo
            {
                Transfers_TransferRecipient = transfersSo.Transfers_TransferRecipient,
                Transfers_LinkToSourceRow = transfersSo.Transfers_LinkToSourceRow,
                Transfers_IsRejected = transfersSo.Transfers_IsRejected,
                Transfers_AcceptedDate = transfersSo.Transfers_AcceptedDate,
                Transfers_CreatedDate = transfersSo.Transfers_CreatedDate,
                Transfers_Id = transfersSo.Transfers_Id,
                Transfers_IsKycCreated = transfersSo.Transfers_IsKycCreated,
                Transfers_Source = transfersSo.Transfers_Source,
                Transfers_SourceType = transfersSo.Transfers_SourceType,
                Transfers_TransferParent = transfersSo.Transfers_TransferParent,
                DaPayLimits_MediaName = so.DaPayLimits_MediaName,
                DaPaymentLimitSourceType_EnumKey = so.DaPaymentLimitSourceType.DaPaymentLimitSourceType_EnumNumber,
                DaPaymentLimitSourceType_EnumName =
                    Enum.GetName(typeof(DelegatedAuthorirySourceLimitationTypeEnum), so.DaPaymentLimitSourceType.DaPaymentLimitSourceType_EnumNumber)?.ToUpper()
            }).ToList();


            return View(transfersToken);
        }

        [System.Web.Http.HttpGet]
        public ActionResult Get()
        {
            var result = new StandartResponse();
            try
            {
                var transfers = _transfersServiceMethods.GetAll().Obj.Where(w =>
                    w.Transfers_TransferRecipient.ToLower() == AppSecurity.CurrentUser.UserName.ToLower() && (!w.Transfers_IsRejected.HasValue || w.Transfers_IsRejected.Value == false)
                    && !w.Transfers_AcceptedDate.HasValue && !w.Transfers_AcceptedDate.HasValue).ToList();

                return Json(new StandartResponse<List<TransfersSo>>(transfers, true, transfers.Count > 0 ? "Ok" : "Empty enumerable"));
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
