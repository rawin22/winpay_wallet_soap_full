using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Mvc;
using Tsg.Business.Model.Classes;
using Tsg.UI.Main.ApiMethods.Payments;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.APIModels.InstantPayment;
using TSG.Models.ServiceModels.LimitPayment;
using TSG.ServiceLayer.Users;


namespace Tsg.UI.Main.Controllers.Transfers.CommonTransferMethods
{
    [System.Web.Http.Authorize]
    public class CheckAliasController : Controller
    {
        NewInstantPaymentMethods m = new NewInstantPaymentMethods(AppSecurity.CurrentUser);
        ApiNewInstantPaymentViewModel apiNewInstantPayment = new ApiNewInstantPaymentViewModel();
        private UserRepository userRepository = new UserRepository();
        private readonly IUsersServiceMethods _usersServiceMethods;
        public CheckAliasController(IUsersServiceMethods usersServiceMethods) => _usersServiceMethods = usersServiceMethods;

        [System.Web.Http.HttpPost]
        public ActionResult Post(string aliasName)
        {
            var res = _usersServiceMethods.ExistedUserByAliasName(aliasName);
            if (aliasName == ConfigurationManager.AppSettings["redEnvelopeLogin"])
                return Json(new StandartResponse<bool>(false, "You can't use system WPayId"));
            if (!res.Success || res.Obj == null)
            {
                apiNewInstantPayment.FromCustomer = _usersServiceMethods.
                    GetUserAliasesByUserId(new Guid(AppSecurity.CurrentUser.UserId)).Obj.Wpay_Ids.First();
                apiNewInstantPayment.ToCustomer = aliasName;
                apiNewInstantPayment.Amount = Convert.ToDecimal(0.01);
                apiNewInstantPayment.CurrencyCode = "USD";
                apiNewInstantPayment.Memo = "Token transfer test";


                var response = m.Create(apiNewInstantPayment);

                if (response.ServiceResponse.HasErrors)
                {
                    if (response.ServiceResponse.Responses.First().MessageDetails.Contains("cannot be found"))
                    {
                        return Json(new StandartResponse<bool>(false, "WPayId not found in system"));
                    }
                }
                
            
                return Json(new StandartResponse<bool>(false,
                    "please verify the WPayID with the receiving party of the token and ask them to login to their ewallet one time to activate."));

            }
            
            if (res.Obj.Wpay_UserName == AppSecurity.CurrentUser.UserName)
                return Json(new StandartResponse<bool>(false, "You can't use yourself WPayId to transfer token or send red envelope"));
            return Json(new StandartResponse<bool>(true, true, "Ok"));
        }
    }
}