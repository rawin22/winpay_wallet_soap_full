using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tsg.UI.Main.Models.Security;
using TSG.Models.Enums;
using TSG.Models.ServiceModels.LimitPayment;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsSourceTypeServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTypeServiceMethods;
using WinstantPay.Common.Extension;

namespace Tsg.UI.Main.Controllers
{
    public class DelegatedAuthorityController : Controller
    {
        private readonly IDaPayLimitsSourceTypeServiceMethods _daPayLimitsSourceTypeServiceMethods;
        private readonly IDaPayLimitsServiceMethods _daForPayLimitsMethodsService;
        private readonly IDaPayLimitsTypeServiceMethods _daPayLimitsTypeServiceMethods;

       
        public DelegatedAuthorityController(
            IDaPayLimitsSourceTypeServiceMethods daPayLimitsSourceTypeServiceMethods, 
            IDaPayLimitsServiceMethods payLimitsMethodsService,
            IDaPayLimitsTypeServiceMethods daPayLimitsTypeServiceMethods)
        {
            _daPayLimitsSourceTypeServiceMethods = daPayLimitsSourceTypeServiceMethods;
            _daPayLimitsTypeServiceMethods = daPayLimitsTypeServiceMethods;
            _daForPayLimitsMethodsService = payLimitsMethodsService;
        }


        // GET
        public ActionResult Index()
        {
            var soursec =
                _daPayLimitsSourceTypeServiceMethods.GetAll().Obj?.Where(w =>
                    w.DaPaymentLimitSourceType_IsAcceptableOnWeb && !w.DaPaymentLimitSourceType_IsDeleted).ToList() ??
                new List<DaPaymentLimitSourceTypeSo>();
            var x = $" {EnumHelper<DelegatedAuthorirySourceLimitationTypeEnum>.GetDisplayValue((DelegatedAuthorirySourceLimitationTypeEnum) 1)}";


            var allDaLimitsList = _daForPayLimitsMethodsService.GetAllDaByUserName(AppSecurity.CurrentUser.UserName).Obj
                ?
                .Where(w => !w.DaPayLimits_IsDeleted && !w.DaPayLimits_IsTransfered && (w.DaPayLimits_DateOfExpire == null || (w.DaPayLimits_DateOfExpire !=null && w.DaPayLimits_DateOfExpire > DateTime.Now))).OrderByDescending(ob => ob.DaPayLimits_CreationDate).ToList()
                                  ?? new List<DaPayLimitsSo>();
            
            Tuple <List<DaPaymentLimitSourceTypeSo>, List<DaPayLimitsSo>> tupleModel = new Tuple<List<DaPaymentLimitSourceTypeSo>, List<DaPayLimitsSo>>(soursec, allDaLimitsList); 

            return View(tupleModel);
        }
    }
}