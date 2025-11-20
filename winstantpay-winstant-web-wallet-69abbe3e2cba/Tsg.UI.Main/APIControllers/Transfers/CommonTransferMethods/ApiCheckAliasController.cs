using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Http;
using Tsg.Business.Model.Classes;
using Tsg.UI.Main.ApiMethods.Payments;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.APIModels.InstantPayment;
using TSG.Models.ServiceModels.LimitPayment;
using TSG.ServiceLayer.Users;

namespace Tsg.UI.Main.APIControllers.Transfers.CommonTransferMethods
{
    [ApiFilter]
    public class ApiCheckAliasController : ApiController
    {
     
        private readonly IUsersServiceMethods _usersServiceMethods;
        public ApiCheckAliasController(IUsersServiceMethods usersServiceMethods) => _usersServiceMethods = usersServiceMethods;

        [HttpPost]
        public IHttpActionResult Post(string aliasName)
        {
            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();
            IEnumerable<string> outerUserToken;
            var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
            if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
            {
                var res = _usersServiceMethods.ExistedUserByAliasName(aliasName);
                if(aliasName == ConfigurationManager.AppSettings["redEnvelopeLogin"])
                    return Content(HttpStatusCode.BadRequest, new StandartResponse<bool>(false, "You can't use system WPayId"));
                if (!res.Success || res.Obj == null)
                {
                    NewInstantPaymentMethods m = new NewInstantPaymentMethods(ui);

                    ApiNewInstantPaymentViewModel apiNewInstantPayment = new ApiNewInstantPaymentViewModel();
                    apiNewInstantPayment.FromCustomer = _usersServiceMethods.GetUserAliasesByUserId(new Guid(ui.UserId)).Obj.Wpay_Ids.First();
                    apiNewInstantPayment.ToCustomer = aliasName;
                    apiNewInstantPayment.Amount = Convert.ToDecimal(0.01);
                    apiNewInstantPayment.CurrencyCode = "USD";
                    apiNewInstantPayment.Memo = "Token transfer test";


                    var response = m.Create(apiNewInstantPayment);

                    if (response.ServiceResponse.HasErrors)
                    {
                        if (response.ServiceResponse.Responses.First().MessageDetails.Contains("cannot be found"))
                        {
                            return Content(HttpStatusCode.BadRequest, new StandartResponse<bool>(false, "WPayId not found in system"));
                        }
                    }

                    return Content(HttpStatusCode.BadRequest, new StandartResponse<bool>(false, 
                        "please verify the WPayID with the receiving party of the token and ask them to login to their ewallet one time to activate."));
                    
                }
                   
                if(res.Obj.Wpay_UserName == ui.UserName)
                    return Content(HttpStatusCode.BadRequest, new StandartResponse<bool>(false, "You can't use yourself WPayId to transfer"));
                return Ok(new StandartResponse<bool>(true, true, "Ok"));
            }
            return Content(HttpStatusCode.Unauthorized, new StandartResponse("Undefined user"));
        }
    }
}