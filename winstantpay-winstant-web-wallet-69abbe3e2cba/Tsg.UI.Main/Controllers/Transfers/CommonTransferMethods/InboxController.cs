using System;
using System.Linq;
using System.Web.Mvc;
using Tsg.UI.Main.Models.Security;
using TSG.Models.Enums;
using TSG.Models.ServiceModels.Transfers;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsServiceMethods;
using TSG.ServiceLayer.Transfers;
using TSG.ServiceLayer.Transfers.Reports;

namespace Tsg.UI.Main.Controllers.Transfers.CommonTransferMethods
{
    [Authorize]
    public class InboxController : BaseController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IGetInboxListMethods _getInboxListMethods;

        public InboxController(IGetInboxListMethods getInboxListMethods)
        {
            _getInboxListMethods = getInboxListMethods;
        }

        /// <summary>
        /// Shows all transfered or recieved token by User
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(_getInboxListMethods.GetByRecipient(AppSecurity.CurrentUser.UserName).Obj);
        }
    }
}