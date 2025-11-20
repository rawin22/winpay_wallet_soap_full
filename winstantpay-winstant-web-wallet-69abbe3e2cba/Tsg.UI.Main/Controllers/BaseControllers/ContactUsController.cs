using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Attributes;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;

namespace Tsg.UI.Main.Controllers
{
    public class ContactUsController : BaseController
    {

        readonly log4net.ILog _logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        // GET: ContactUs
        public ActionResult Index()
        {
            UserMailModel userMailModel = new UserMailModel();
            userMailModel.MailText = String.Empty;
            userMailModel.MailFrom = String.Empty;
            if (AppSecurity.CurrentUser != null && !String.IsNullOrEmpty(AppSecurity.CurrentUser.EmailAddress))
                userMailModel.MailFrom = AppSecurity.CurrentUser.EmailAddress;
            userMailModel.ActionResultText = String.Empty;
            ContactUsRepository cr = new ContactUsRepository();
            var departments = cr.GetContactUsList();
            var list = new List<SelectListItem>();
            //list.Add(new SelectListItem()
            //{
            //    Text = GlobalRes.ContactUs_ContactUsController_Index_ChoseCategory,
            //    Value = null
            //});

            list.AddRange(departments.Select(s => new SelectListItem()
            {
                Text = s.DepartmentName,
                Value = s.DepartmentId.ToString()
            }));
            //foreach (var department in departments)
            //{
            //    list.Add(new SelectListItem()
            //    {
            //        Text = department.DepartmentName,
            //        Value = department.DepartmentMail

            //    });
            //}
            userMailModel.ListOfContactList = list;
            return View(userMailModel);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Index(UserMailModel model)
        {
            try
            {
                ContactUsRepository cr = new ContactUsRepository();
                var departments = cr.GetContactUsList();
                var list = new List<SelectListItem>();
                list.Add(new SelectListItem()
                {
                    Text = GlobalRes.ContactUs_ContactUsController_Index_ChoseCategory,
                    Value = null
                });

                list.AddRange(departments.Select(s => new SelectListItem()
                {
                    Text = s.DepartmentName,
                    Value = s.DepartmentId.ToString()
                }));
                model.ListOfContactList = list;

                if (String.IsNullOrEmpty(model.MailTo) || model.MailTo == GlobalRes.ContactUs_ContactUsController_Index_ChoseCategory)
                {
                    model.IsSuccessResForAction = false;
                    model.ActionResultText = GlobalRes.ContactUs_ContactUsController_Index_SelectCategory;
                }
                else if (model.MailText == null || String.IsNullOrEmpty(model.MailText.Trim()))
                {
                    model.IsSuccessResForAction = false;
                    model.ActionResultText = GlobalRes.ContactUs_ContactUsController_Index_MsgIsEmpty;
                }
                else if (String.IsNullOrEmpty(model.Subject.Trim()))
                {
                    model.IsSuccessResForAction = false;
                    model.ActionResultText = GlobalRes.ContactUs_ContactUsController_Index_SubjIsEmpty;
                }
                else
                {
                    string resAct = GlobalRes.ContactUs_ContactUsController_Index_MailIsNotSend;
                    var currDep = departments.FirstOrDefault(f => f.DepartmentId == Convert.ToInt32(model.MailTo));
                    bool res = false;
                    if (currDep != null)
                    {

                        if (cr.RegisterMail(currDep.DepartmentId, model.MailFrom, currDep.DepartmentMail, model.Subject,
                            model.MailText,
                            model.Attachments, GlobalRes.ContactUs_ContactUsController_Index_ContactUs, out resAct,
                            out var mailId))
                        {
                            if (cr.RegistrationRequest(mailId, out var ticketNumber))
                            {

                                var page = new UserTaskModel()
                                {
                                    CompanyName = ConfigurationManager.AppSettings["OrganizationName"],
                                    MailTo = model.MailTo,
                                    DepartmentName = currDep.DepartmentName,
                                    UserTaskNumber = ticketNumber,
                                    Subject = model.Subject,
                                    MailText = model.MailText,
                                    MailFrom = model.MailFrom,
                                    MailCopy = model.MailCopy
                                };
                                var bccMails = new List<string>();
                                bccMails.Add(model.MailFrom);
                                var renderMailPage = RenderRazorViewToString_ContactUs("ContactUsLetterTemplate", page);
                                if (model.MailCopy == null || String.IsNullOrEmpty(model.MailCopy.Trim()))
                                {
                                    EmailExtension.EmailService.SendEmail(
                                        model.Subject, renderMailPage, model.MailFrom, string.Empty, currDep.DepartmentMail, page.DepartmentName, String.Empty, string.Empty, 
                                        null, null, model.Attachments);
                                }
                                else
                                {
                                    var copyMails = new List<string>();
                                    copyMails.Add(model.MailCopy);
                                    EmailExtension.EmailService.SendEmail(model.Subject, renderMailPage, currDep.DepartmentMail,
                                        String.Empty, currDep.DepartmentMail, page.DepartmentName, string.Empty, string.Empty,
                                        null, copyMails, model.Attachments);
                                }
                                model.IsSuccessResForAction = true;
                                model.ActionResultText = GlobalRes.ContactUs_ContactUsRepository_SendMail_SuccessSend;
                            }
                        }
                    }
                    else
                    {
                        model.IsSuccessResForAction = false;
                        model.ActionResultText = resAct;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                model.IsSuccessResForAction = false;
                model.ActionResultText = GlobalRes.ContactUs_ContactUsRepository_SendMail_FailedSend;
            }

            return View("SendMessageInfo", model);
        }

        private string RenderRazorViewToString_ContactUs(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                    viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                    ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult ContactUs(UserMailModel um)
        {
            try
            {
                var userIp = StaticExtensions.IpExtensions.GetUserIp(Request);
                string browserName = Request.Browser.Browser;
                string browserVersion = Request.Browser.Version;
                bool isMobile = Request.Browser.IsMobileDevice;
                string resOperation;
                string resMailOperation;
                string mailTo = ConfigurationManager.AppSettings["anonMailTo"].ToString();
                if (String.IsNullOrEmpty(mailTo))
                    throw new Exception(GlobalRes.ContactUs_ContactUsController_ContactUs_MailToError);

                ContactUsRepository cur = new ContactUsRepository();
                var addrec = cur.AddAnonimUserMail(um, userIp, browserName, browserVersion, isMobile, out resOperation);
                var sendMail = cur.RegisterMail(0, um.MailFrom, mailTo, um.Subject ?? "", um.MailText, um.Attachments, "ContactUs", out resMailOperation, out int mailId, string.Empty);

                if (addrec && sendMail)
                {
                    EmailExtension.EmailService.SendEmail(um.Subject ?? String.Empty, um.MailText, um.MailFrom, string.Empty, mailTo, String.Empty, String.Empty, String.Empty, null, new[] { um.MailCopy }, um.Attachments);
                    um.IsSuccessResForAction = true;
                    um.ActionResultText = GlobalRes.ContactUs_ContactUsController_ContactUs_Ok;
                }
                else
                {
                    if (!addrec && sendMail)
                    {
                        um.IsSuccessResForAction = false;
                        um.ActionResultText = GlobalRes.ContactUs_ContactUsController_ContactUs_DbError;
                    }
                    else if (addrec && !sendMail)
                    {
                        um.IsSuccessResForAction = false;
                        um.ActionResultText = GlobalRes.ContactUs_ContactUsController_ContactUs_MailSendingError;
                    }
                    else
                    {
                        um.IsSuccessResForAction = false;
                        um.ActionResultText = GlobalRes.ContactUs_ContactUsController_ContactUs_MailAndDbError;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                um.IsSuccessResForAction = false;
                um.ActionResultText = e.Message;
            }
            return Json(new { um.IsSuccessResForAction, um.ActionResultText });
        }



        [AuthorizeUser(Roles = Role.Admin)]
        [HttpGet]
        public ActionResult GetAllContactUsList()
        {
            ContactUsModel model = new ContactUsModel();
            ContactUsRepository cr = new ContactUsRepository();

            if (TempData.ContainsKey("ContactUsModel"))
            {
                var tempModel = TempData["ContactUsModel"] as ContactUsModel;
                if (tempModel != null)
                    model = tempModel;
            }
            UserRepository userRepo = new UserRepository();
            var props = userRepo.SysValues();
            model.UploadProofInternalMail = props
                .FirstOrDefault(f => f.PropertyName == "adminNotificationMailForUploadProof")?.PropertyValue.ToString();
            model.ListOfContactList = cr.GetContactUsList();
            return View(model);
        }

        [AuthorizeUser(Roles = Role.Admin)]
        [HttpPost]
        public ActionResult Create(ContactUsModel model)
        {
            ContactUsRepository cr = new ContactUsRepository();
            model.ListOfContactList = cr.GetContactUsList();
            String resAct;
            var addres = cr.AddContactUs(model, out resAct);
            if (addres)
            {
                model.IsSuccessResForAction = true;
                model.ActionResultText = resAct;
            }
            else
            {
                model.IsSuccessResForAction = false;
                model.ActionResultText = resAct;
            }
            TempData["ContactUsModel"] = model;
            return RedirectToAction("GetAllContactUsList", "ContactUs");
        }


        [AuthorizeUser(Roles = Role.Admin)]
        [HttpPost]
        public ActionResult Edit(ContactUsModel model)
        {
            ContactUsRepository cr = new ContactUsRepository();
            string resAct;
            var res = cr.UpdateContactUs(model, out resAct);
            if (res)
            {
                model.IsSuccessResForAction = true;
                model.ActionResultText = resAct;
            }
            else
            {
                model.IsSuccessResForAction = false;
                model.ActionResultText = resAct;
            }
            TempData["ContactUsModel"] = model;
            return RedirectToAction("GetAllContactUsList", "ContactUs");
        }

        [AuthorizeUser(Roles = Role.Admin)]
        [HttpPost]
        public ActionResult Delete(ContactUsModel model)
        {
            ContactUsRepository cr = new ContactUsRepository();
            string resAct;
            var res = cr.DeleteContactUs(model.DepartmentId, out resAct);
            if (res)
            {
                model.IsSuccessResForAction = true;
                model.ActionResultText = resAct;
            }
            else
            {
                model.IsSuccessResForAction = false;
                model.ActionResultText = resAct;
            }
            TempData["ContactUsModel"] = model;
            return RedirectToAction("GetAllContactUsList", "ContactUs");
        }

        //[HttpPost, ValidateInput(false)]
        //public ActionResult Upload(IEnumerable<HttpPostedFileBase> files)
        //{
        //    logger.Debug("Im Entered");
        //    foreach (var file in 
        //        files)
        //    {
        //        if (file.ContentLength > 0)
        //        {
        //            logger.Debug("File Loaded");

        //            var fileName = Path.GetFileName(file.FileName);
        //            var path = String.Format("{0}/{1}", Server.MapPath("~/UserFiles/PaymentDetails"), fileName);
        //            file.SaveAs(path);
        //            Debug.WriteLine(file.FileName);
        //        }
        //    }

        //    return Content("Succesfully");
        //}
        [HttpPost]
        public ActionResult SaveUploadProofSystemMail(string UploadProofInternalMail)
        {
            ContactUsRepository cur = new ContactUsRepository();
            string res;
            if (!String.IsNullOrEmpty(UploadProofInternalMail) && cur.IsValid(UploadProofInternalMail))
            {
                if (cur.UpdateSystemRequirement("adminNotificationMailForUploadProof", UploadProofInternalMail, out res))
                    SettingClass.SetSysEmailUploadProof(UploadProofInternalMail);
            }
            return RedirectToAction("GetAllContactUsList", "ContactUs");
        }
    }
}