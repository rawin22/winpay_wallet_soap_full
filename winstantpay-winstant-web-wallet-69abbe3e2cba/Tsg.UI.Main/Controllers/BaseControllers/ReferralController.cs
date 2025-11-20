using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mail;
using System.Web.Mvc;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Attributes;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using MailMessage = System.Net.Mail.MailMessage;

namespace Tsg.UI.Main.Controllers
{
    public class ReferralController : BaseController
    {
        // GET: Referral/ReferFriend
        [HttpGet]
        public ActionResult ReferFriend()
        {
            ReferalLinks obj = new ReferalLinks();
            ReferalLinkRepository referalLinkRepository = new ReferalLinkRepository();
            var localObj = referalLinkRepository.GetReferalLinksList().FirstOrDefault(f=>f.IsDefault);
            if (localObj != null)
                obj = localObj;
            return View(obj);
        }

        // POST: Referral/ReferFriend
        [HttpPost]
        public ActionResult ReferFriend(ReferalLinks obj)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                //    string mailFrom = obj.From;
                //    string mailTo = obj.To;
                //    //connect to smtp server and send an email
                //    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient()
                //    {
                //        Host = "smtp.gmail.com",
                //        Port = 587,
                //        EnableSsl = true,
                //        DeliveryMethod = SmtpDeliveryMethod.Network,
                //        UseDefaultCredentials = false,
                //                                                                                   // need to set password
                //        Credentials = new NetworkCredential("yevgeniy.astashev@finnetlimited.com", "****************")
                //    };
                //    var message = new MailMessage(mailFrom, mailTo);
                //    message.Body = obj.Message;
                //    smtp.Send(mailFrom, mailTo, "Ewallet", obj.Message);
                //    ViewBag.ErrorMessage = "Mail sended succesiffully";
                //}
            }
            catch(Exception e)
            {
                ViewBag.ErrorMessage = e.Message;
            }
            ViewBag.OpenPanel = true;
            return View(obj);
        }
            
        [AuthorizeUser(Roles = Role.Admin)]
        [HttpGet]
        public ActionResult ChangeInvintationsLink()
        {
            ReferalLinks model = new ReferalLinks();
            ReferalLinkRepository referalLinkRepository = new ReferalLinkRepository();
            var localModel = referalLinkRepository.GetReferalLinksList().FirstOrDefault();
            if (localModel != null)
                model = localModel;
            return View(model);
        }

        [AuthorizeUser(Roles = Role.Admin)]
        [HttpPost]
        public ActionResult ChangeInvintationsLink(ReferalLinks model)
        {
            ReferalLinkRepository referalLinkRepository = new ReferalLinkRepository();
            var  res = referalLinkRepository.AddOrUpdateReferalLink(model);
            return Redirect("~/");
        }
    }
}