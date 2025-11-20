using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsg.UI.Main.Models.Security;
using TSG.Models.Enums;
using TSG.Models.ServiceModels.Transfers;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsServiceMethods;
using TSG.ServiceLayer.RedEnvelope;
using TSG.ServiceLayer.Transfers;
using TSG.ServiceLayer.Transfers.Reports;
using WinstantPay.Common.Extension;

namespace Tsg.UI.Main.Controllers.Transfers.TrustedTokenTransfers
{
    [Authorize]
    public class InputOutputObjectsController : Controller
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ITransfersServiceMethods _transfersServiceMethods;
        private readonly IDaPayLimitsServiceMethods _daPayLimitsService;
        private readonly IRedEnvelopeServiceMethods _envelopeServiceMethods;
        private readonly IGetInboxListMethods _getInboxListMethods;

        public InputOutputObjectsController(ITransfersServiceMethods transfersServiceMethods, IDaPayLimitsServiceMethods daPayLimitsService, IRedEnvelopeServiceMethods envelopeServiceMethods,
            IGetInboxListMethods getInboxListMethods)
        {
            _transfersServiceMethods = transfersServiceMethods;
            _daPayLimitsService = daPayLimitsService;
            _envelopeServiceMethods = envelopeServiceMethods;
            _getInboxListMethods = getInboxListMethods;
        }

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult JsonInputHistoricalData()
        {
            var resQuery = _getInboxListMethods.GetByRecipient(AppSecurity.CurrentUser.UserName).Obj;
            resQuery.ForEach(f =>
            {
                f.GetInboxList_Source = EnumHelper<TransfersSourceTypeEnum>.GetDisplayValue(f.GetInboxList_SourceType);
                f.GetInboxList_StatusEnumString = EnumHelper<TransferStatusesEnum>.GetDisplayValue(f.GetInboxList_Status);
                f.GetInboxList_TypeRecBySourceEnumString = EnumHelper<DelegatedAuthorirySourceLimitationTypeEnum>.GetDisplayValue(f.GetInboxList_TypeRecBySource);
            });

            return Json(resQuery.OrderByDescending(ob=>ob.GetInboxList_CreatedDate).ToList());
        }

        public ActionResult JsonOutputHistoricalData()
        {
            var resQuery = _getInboxListMethods.GetByParent(AppSecurity.CurrentUser.UserName).Obj;
            resQuery.ForEach(f =>
            {
                f.GetInboxList_Source = EnumHelper<TransfersSourceTypeEnum>.GetDisplayValue(f.GetInboxList_SourceType);
                f.GetInboxList_StatusEnumString = EnumHelper<TransferStatusesEnum>.GetDisplayValue(f.GetInboxList_Status);
                f.GetInboxList_TypeRecBySourceEnumString = EnumHelper<DelegatedAuthorirySourceLimitationTypeEnum>.GetDisplayValue(f.GetInboxList_TypeRecBySource);
            });

            return Json(resQuery.OrderByDescending(ob => ob.GetInboxList_CreatedDate).ToList());
        }
    }
}