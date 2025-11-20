using System;
using System.Linq;
using System.Web.Mvc;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models.Merchants;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;

namespace Tsg.UI.Main.Controllers
{
    public class MerchantsController : BaseController
    {
        // GET: Merchants
        //[AuthorizeUser(Roles = Role.Admin)]
        public ActionResult Index()
        {
            MercantsList ml = new MercantsList();
            MerchantsRepository mrRepository = new MerchantsRepository();
            if (TempData.ContainsKey("MerchantModel"))
            {
                var tempModel = TempData["MerchantModel"] as MerchantModel;
                if (tempModel != null)
                {
                    foreach (var message in tempModel.Messages)
                    {
                        if(message.IsError)
                            ml.AddError(message.Message);
                        if(message.IsSuccess)
                            ml.AddSuccess(message.Message);
                    }
                }
            }
            ml.AddCloseBtn = true;
            ml.ListOfMerchants = mrRepository.GetMerchantsForUser(AppSecurity.CurrentUser.UserName, AppSecurity.CurrentUser.UserId);
            return View(ml.ListOfMerchants);
        }

        [HttpGet]
        public ActionResult GetDataForMerchant(int merchantId, Guid merchantUniqueId)
        {
            MerchantModel mm = new MerchantModel();
            MerchantsRepository mr = new MerchantsRepository();


            mm = mr.GetMerchantsForUser(AppSecurity.CurrentUser.UserName, AppSecurity.CurrentUser.UserId).Find(f => f.Id == merchantId);

            mm.IntegrateJsIntoSite = false;
            mm.MerchantParentAddress = HttpContext.Request.Url?.GetLeftPart(UriPartial.Authority) + Url.Content("~/");
            return PartialView("~/Views/Merchants/PartialModalView/GenerateThirdPartMerchantButton.cshtml", mm);
        }



        //[AuthorizeUser(Roles = Role.Admin)]
        //[HttpPost]
        //public ActionResult (MerchantModel model)
        //{
        //    
        //    String resAct;
        //    model.MerchantUserId = AppSecurity.CurrentUser.UserName;
        //    model.MerchantUserGuid = AppSecurity.CurrentUser.UserId;

        //    //var add
        //    //if (addres)
        //    //{
        //    //    model.IsSuccessResForAction = true;
        //    //    model.ActionResultText = resAct;
        //    //}
        //    //else
        //    //{
        //    //    model.IsSuccessResForAction = false;
        //    //    model.ActionResultText = resAct;
        //    //}
        //    TempData["MerchantModel"] = model;
        //    return RedirectToAction("Index", "Merchants");
        //}

        [HttpGet]
        public ActionResult CreateOrEdit(int Id)
        {
            MerchantModel mm = new MerchantModel();
            mm.MerchantPrivateTokenKey = $"{Guid.NewGuid()}{Guid.NewGuid()}".Replace("-", "").TruncateLongString(50);
            mm.MerchantPublicTokenKey = $"{Guid.NewGuid()}{Guid.NewGuid()}".Replace("-", "").TruncateLongString(50);
            MerchantsRepository mr = new MerchantsRepository();

            if (Id != 0)
            {
                mm = mr.GetMerchantsForUser(AppSecurity.CurrentUser.UserName, AppSecurity.CurrentUser.UserId).Find(f=>f.Id == Id);
            }
            
            return PartialView("AddEditMerchant", mm);
        }

        [HttpPost]
        public ActionResult CreateOrEdit(MerchantModel model)
        {
            model.MerchantUserId = AppSecurity.CurrentUser.UserName;
            model.MerchantUserGuid = AppSecurity.CurrentUser.UserId;
            MerchantsRepository mr = new MerchantsRepository();
            string resAct;
            bool res = false;
            res = model.Id == 0 ? mr.AddMerchants(model, out resAct) : mr.UpdateMerchants(model, out resAct);
            if (res) model.AddSuccess(resAct);
            else model.AddError(resAct);
            TempData["MerchantModel"] = model;
            return RedirectToAction("Index", "Merchants");
        }

        //[AuthorizeUser(Roles = Role.Admin)]
        [HttpPost]
        public ActionResult SaveNewCallBackAddress(string merchantGuid, string callBackAddress)
        {
            MerchantsRepository mr = new MerchantsRepository();
            var allMercByUser = mr.GetMerchantsForUser(AppSecurity.CurrentUser.UserName, AppSecurity.CurrentUser.UserId);
            string resAct = "";

            if (allMercByUser.Any(a => a.MerchantGuid == merchantGuid))
            {
                var res = mr.UpdateMerchantCallBackAddress(merchantGuid, callBackAddress, out resAct);
                //if (res)
                //{
                //    model.IsSuccessResForAction = true;
                //    model.ActionResultText = resAct;
                //}
                //else
                //{
                //    model.IsSuccessResForAction = false;
                //    model.ActionResultText = resAct;
                //}
            }
            //TempData["MerchantModel"] = model;
            return Json(new { isSuccess = Resolver, resMessage = resAct });
        }

        //[AuthorizeUser(Roles = Role.Admin)]
        //[HttpPost]
        //public ActionResult Delete(MerchantModel model)
        //{
        //    ContactUsRepository cr = new ContactUsRepository();
        //    string resAct;
        //    var res = cr.DeleteContactUs(model.DepartmentId, out resAct);
        //    if (res)
        //    {
        //        model.IsSuccessResForAction = true;
        //        model.ActionResultText = resAct;
        //    }
        //    else
        //    {
        //        model.IsSuccessResForAction = false;
        //        model.ActionResultText = resAct;
        //    }
        //    TempData["MerchantModel"] = model;
        //    return RedirectToAction("Index", "Merchants");
        //}
    }
}