using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using Tsg.UI.Main.Models.Attributes;
using TSG.Models.APIModels;
using TSG.Models.ServiceModels.AutomaticExchange;
using TSG.ServiceLayer.AutomaticExchange.LiquidCcyListServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidOverDraftUserServiceMethods;
using TSG.ServiceLayer.Users;

namespace Tsg.UI.Main.Controllers.AutomaticExchange
{
    [AuthorizeUser(Roles = Role.Admin)]
    public class AeOverdraftUsersController : BaseController
    {

        private readonly ILiquidCcyListServiceMethods _liquidCcyListServiceMethods;
        private readonly ILiquidOverDraftUserServiceMethods _liquidOverDraftUserServiceMethods;
        private readonly IUsersServiceMethods _usersServiceMethods;


        public AeOverdraftUsersController(ILiquidOverDraftUserServiceMethods liquidOverDraftUserServiceMethods, IUsersServiceMethods usersServiceMethods, ILiquidCcyListServiceMethods liquidCcyListServiceMethods)
        {
            _liquidOverDraftUserServiceMethods = liquidOverDraftUserServiceMethods;
            _liquidCcyListServiceMethods = liquidCcyListServiceMethods;
            _usersServiceMethods = usersServiceMethods;
        }

        //[System.Web.Mvc.Route("{lang?}/liquids/overdraftuser/")]
        [System.Web.Mvc.Route("liquids/overdraftuser/")]
        public ActionResult Index()
        {
            var users = _usersServiceMethods.GetAllUsers().Obj.Where(w=>w.User_Role.Role_RoleName == "User" && w.User_UserIdByTSG != null).
                Select(s => new SelectListItem() { Value = s.User_UserIdByTSG.ToString(), Text = $@"{s.User_FirstName} {s.User_LastName} ({s.User_Username})" }).ToList();
            var liquidUsers = _liquidOverDraftUserServiceMethods.GetAllSo().Obj.Select(s => new SelectListItem()
                { Value = s.LiquidOverDraftUserList_UserId.ToString(), Text = $@"{s.LiquidOverDraftUserList_FullName} ({s.LiquidOverDraftUserList_UserName})" }).ToList();

            liquidUsers.ForEach(f => users.RemoveAll(a=>a.Value == f.Value));// users.Except(liquidUsers);
            
            ViewBag.Users = users;
            ViewBag.Currency = UiHelper.PrepareAvailableCurrenciesByService(ConfigurationManager.AppSettings["cryptoUser"], ConfigurationManager.AppSettings["cryptoPassword"]);
            return View();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult GetOverdraftUser()
        {
            return Json(_liquidOverDraftUserServiceMethods.GetAllSo().Obj);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult GetUserWihtoutOverdraft()
        {
            // get all user
            var users = _usersServiceMethods.GetAllUsers().Obj.Where(w => w.User_Role.Role_RoleName == "User" && w.User_UserIdByTSG != null).Select(s=> new SelectListItem() {Value =  s.User_UserIdByTSG.ToString(), Text = $@"{s.User_FirstName} {s.User_LastName} ({s.User_Username})"});
            var liquidUsers = _liquidOverDraftUserServiceMethods.GetAllSo().Obj.Select(s => new SelectListItem()
                {Value = s.LiquidOverDraftUserList_UserId.ToString(), Text = $@"{s.LiquidOverDraftUserList_FullName} ({s.LiquidOverDraftUserList_UserName})"});

            var expectedList = users.Except(liquidUsers);

            return Json(_liquidOverDraftUserServiceMethods.GetAll().Obj);
        }
        
        [System.Web.Mvc.HttpPost]
        public ActionResult SaveOverdraftUser(string userId/*, string currencyId, decimal overdraftAmount*/)
        {
            var  tryUserId = Guid.TryParse(userId, out var gUserId);

            if (!tryUserId)
                return Json(new StandartResponse("Undefined user"));
            var user = _usersServiceMethods.GetAllUsers().Obj.FirstOrDefault(w => w.User_UserIdByTSG == gUserId);
            if(user == null)
                return Json(new StandartResponse("User not found"));

            //if (overdraftAmount == default && overdraftAmount <= 0)
            //    return Json(new StandartResponse("Invalid amount"));
            //var ccy = _liquidCcyListServiceMethods.GetLiquidCcyElementByCurrencyCode(currencyId);
            //if(!ccy.Success || ccy.Obj == null)
            //    return Json(new StandartResponse("Invalid currency"));
            var overdraftUsers = _liquidOverDraftUserServiceMethods.GetAllSo();
            if(overdraftUsers.Obj.Any(a=>a.LiquidOverDraftUserList_UserId == user.User_UserIdByTSG))
                return Json(new StandartResponse("User stay in overdraft list"));

            var insertOverdraftUser = _liquidOverDraftUserServiceMethods.Insert(new LiquidOverDraftUserListSo()
            {
                LiquidOverDraftUserList_AccountRep = User.Identity.Name,
                LiquidOverDraftUserList_CreationDate = DateTime.Now,
                LiquidOverDraftUserList_UserId = user.User_UserIdByTSG ?? Guid.Empty,
                LiquidOverDraftUserList_FullName = $@"{user.User_FirstName} {user.User_LastName}",
                LiquidOverDraftUserList_UserName = user.User_Username,
            });

            if(!insertOverdraftUser.Success)
                return Json(insertOverdraftUser.Message);
            return Json(new StandartResponse<Guid>(user.User_UserIdByTSG ?? Guid.Empty, true, "Data saved"));
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Delete(Guid id)
        {
            var delQuery = _liquidOverDraftUserServiceMethods.Delete(id);

            return Json(new StandartResponse(delQuery.Success, delQuery.Message));
        }

    }
}