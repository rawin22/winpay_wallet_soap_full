using System;
using System.Linq;
using System.Web.Mvc;
using Tsg.UI.Main.Models.Security;
using TSG.Models.Enums;
using TSG.Models.ServiceModels.Transfers;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsServiceMethods;
using TSG.ServiceLayer.Transfers;

namespace Tsg.UI.Main.Controllers.Transfers.TrustedTokenTransfers
{
    /// <summary>
    /// History by tokens
    /// </summary>
    public class HistoryByTransferedTokensController : Controller
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ITransfersServiceMethods _transfersServiceMethods;
        private readonly IDaPayLimitsServiceMethods _daPayLimitsService;

        public HistoryByTransferedTokensController(ITransfersServiceMethods transfersServiceMethods, IDaPayLimitsServiceMethods daPayLimitsService)
        {
            _transfersServiceMethods = transfersServiceMethods;
            _daPayLimitsService = daPayLimitsService;
        }

        /// <summary>
        /// Shows all transfered or recieved token by User
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //&& (!w.Transfers_IsRejected.HasValue || w.Transfers_IsRejected.Value == false)
            //&& !w.Transfers_AcceptedDate.HasValue && w.Transfers_LinkToSourceRow.HasValue

            var transfers = _transfersServiceMethods.GetAll().Obj.Where(w => w.Transfers_SourceType == TransfersSourceTypeEnum.TokenTransfers &&
                                                                             w.Transfers_TransferParent.ToLower() == AppSecurity.CurrentUser.UserName.ToLower()).ToList();
            var transfersToken = _daPayLimitsService.GetAll().Obj.Join(transfers, so => so.DaPayLimits_ID, t => t.Transfers_LinkToSourceRow, (so, transfersSo) => new HistorycalDataByTokenSo
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
                    Enum.GetName(typeof(DelegatedAuthorirySourceLimitationTypeEnum), so.DaPaymentLimitSourceType.DaPaymentLimitSourceType_EnumNumber)?.ToUpper(),
                StatusByToken = ((!transfersSo.Transfers_IsRejected.HasValue || transfersSo.Transfers_IsRejected.Value == false)
                                &&(!transfersSo.Transfers_AcceptedDate.HasValue && transfersSo.Transfers_LinkToSourceRow.HasValue)) 
                                   ? TransferedTokenStatusesEnum.TokenPending :
                               (transfersSo.Transfers_AcceptedDate.HasValue && transfersSo.Transfers_LinkToSourceRow.HasValue)  &&
                               (!transfersSo.Transfers_IsRejected.HasValue || transfersSo.Transfers_IsRejected.Value == false)
                                   ? TransferedTokenStatusesEnum.TokenTransfered : 
                                     TransferedTokenStatusesEnum.TokenDeclined
            }).ToList();


            return View(transfersToken);
        }

    }
}