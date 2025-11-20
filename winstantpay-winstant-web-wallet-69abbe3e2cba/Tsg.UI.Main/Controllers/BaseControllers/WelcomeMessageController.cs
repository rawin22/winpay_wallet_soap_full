using System;
using System.Web.Mvc;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Attributes;
using Tsg.UI.Main.Repository;

namespace Tsg.UI.Main.Controllers
{
    [AuthorizeUser(Roles = "Admin, SuperUser")]
    public class WelcomeMessageController : BaseController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: WelcomeMessage/GetWelcomeMessages
        [HttpGet]
        public ActionResult GetWelcomeMessages()
        {
            WelcomeMessageRepository wlcMsgRepo = new WelcomeMessageRepository();
            ModelState.Clear();
            return View(wlcMsgRepo.GetWelcomeMessages());
        }

        // GET: WelcomeMessage/AddWelcomeMessage
        [HttpGet]
        public ActionResult AddWelcomeMessage(string username)
        {
            WelcomeMessageModel wlcMsgModel = new WelcomeMessageModel();
            wlcMsgModel.Username = username;
            return View(wlcMsgModel);
        }

        // POST: WelcomeMessage/AddWelcomeMessage
        [HttpPost]
        public ActionResult AddWelcomeMessage(WelcomeMessageModel wlc)
        {
            try
            {
                _logger.Info("Attempting to add welcome message. "+ wlc);
                if (ModelState.IsValid)
                {
                    WelcomeMessageRepository wlcMsgRepo = new WelcomeMessageRepository();

                    if (wlcMsgRepo.AddWelcomeMessage(wlc))
                    {
                        ViewBag.Message = GlobalRes.WelcomeMessage_WelcomeMessageController_AddWelcomeMessage_AddSuccessfully;
                        _logger.Info("Welcome message added successfully. " + wlc);
                    }
                }

                return View();
            }
            catch(Exception ex)
            {
                ViewBag.Message = ex.Message;
                _logger.Error(ex.Message);
                return View();
            }
        }

        // GET: WelcomeMessage/EditWelcomeMessage/5
        [HttpGet]    
        public ActionResult EditWelcomeMessage(int id, string username)
        {
            WelcomeMessageRepository wlcMsgRepo = new WelcomeMessageRepository();
            WelcomeMessageModel model = new WelcomeMessageModel();
            var currRec = wlcMsgRepo.GetWelcomeMessages().Find(wlcMsg => wlcMsg.WelcomeMessageId == id);
            if (currRec != null)
            {
                model = currRec;
                model.Username = username;
            }
            return View(model);
        }

        // POST: WelcomeMessage/EditWelcomeMessage/5    
        [HttpPost]
        public ActionResult EditWelcomeMessage(WelcomeMessageModel obj)
        {
            try
            {
                _logger.Info("Attempting to edit welcome message. " + obj);
                WelcomeMessageRepository wlcMsgRepo = new WelcomeMessageRepository();

                obj.WelcomeMessage =   System.Net.WebUtility.HtmlDecode(obj.WelcomeMessage.Trim());
                obj.WelcomeMessagePh = System.Net.WebUtility.HtmlDecode(obj.WelcomeMessagePh.Trim());
                obj.WelcomeMessageRu = System.Net.WebUtility.HtmlDecode(obj.WelcomeMessageRu.Trim());
                obj.WelcomeMessageFr = System.Net.WebUtility.HtmlDecode(obj.WelcomeMessageFr.Trim());
                obj.WelcomeMessageAe = System.Net.WebUtility.HtmlDecode(obj.WelcomeMessageAe.Trim());
                obj.WelcomeMessageTh = System.Net.WebUtility.HtmlDecode(obj.WelcomeMessageTh.Trim());

                var currRec = wlcMsgRepo.GetWelcomeMessages().Find(wlcMsg => wlcMsg.WelcomeMessageId == obj.WelcomeMessageId);

                currRec.WelcomeMessage =   System.Net.WebUtility.HtmlDecode(currRec.WelcomeMessage.Trim());
                currRec.WelcomeMessagePh = System.Net.WebUtility.HtmlDecode(currRec.WelcomeMessagePh.Trim());
                currRec.WelcomeMessageRu = System.Net.WebUtility.HtmlDecode(currRec.WelcomeMessageRu.Trim());
                currRec.WelcomeMessageFr = System.Net.WebUtility.HtmlDecode(currRec.WelcomeMessageFr.Trim());
                currRec.WelcomeMessageAe = System.Net.WebUtility.HtmlDecode(currRec.WelcomeMessageAe.Trim());
                currRec.WelcomeMessageTh = System.Net.WebUtility.HtmlDecode(currRec.WelcomeMessageTh.Trim());


                var copyObj = (WelcomeMessageModel)obj.Clone();

                copyObj.WelcomeMessage = copyObj.WelcomeMessage.StripHTML().Trim();
                copyObj.WelcomeMessagePh = copyObj.WelcomeMessagePh.StripHTML().Trim();
                copyObj.WelcomeMessageRu = copyObj.WelcomeMessageRu.StripHTML().Trim();
                copyObj.WelcomeMessageFr = copyObj.WelcomeMessageFr.StripHTML().Trim();
                copyObj.WelcomeMessageAe = copyObj.WelcomeMessageAe.StripHTML().Trim();
                copyObj.WelcomeMessageTh = copyObj.WelcomeMessageTh.StripHTML().Trim();

                var copyCurr = (WelcomeMessageModel)currRec.Clone();

                copyCurr.WelcomeMessage = copyCurr.WelcomeMessage.StripHTML().Trim();
                copyCurr.WelcomeMessagePh = copyCurr.WelcomeMessagePh.StripHTML().Trim();
                copyCurr.WelcomeMessageRu = copyCurr.WelcomeMessageRu.StripHTML().Trim();
                copyCurr.WelcomeMessageFr = copyCurr.WelcomeMessageFr.StripHTML().Trim();
                copyCurr.WelcomeMessageAe = copyCurr.WelcomeMessageAe.StripHTML().Trim();
                copyCurr.WelcomeMessageTh = copyCurr.WelcomeMessageTh.StripHTML().Trim();


                bool isTextChanged = true;
                if (copyObj.WelcomeMessage == copyCurr.WelcomeMessage && copyObj.WelcomeMessageFr == copyCurr.WelcomeMessageFr &&
                    copyObj.WelcomeMessagePh == copyCurr.WelcomeMessagePh && copyObj.WelcomeMessageRu == copyCurr.WelcomeMessageRu 
                    && copyObj.WelcomeMessageAe == copyCurr.WelcomeMessageAe && copyObj.WelcomeMessageTh == copyCurr.WelcomeMessageTh)
                {
                    isTextChanged = false;
                }
                //if (obj.WelcomeMessage == currRec.WelcomeMessage && obj.WelcomeMessageFr == currRec.WelcomeMessageFr &&
                //         obj.WelcomeMessagePh == currRec.WelcomeMessagePh && obj.WelcomeMessageRu == currRec.WelcomeMessageRu)
                //{
                //    isTextChanged = false;
                //}
                if (!isTextChanged)
                {
                    return RedirectToAction("GetWelcomeMessages");
                }
                else
                {
                    wlcMsgRepo.ChangeWmStatusesForAllUsers(true);

                    if (wlcMsgRepo.UpdateWelcomeMessage(obj))
                    {
                        ViewBag.Message = GlobalRes
                            .WelcomeMessage_WelcomeMessageController_EditWelcomeMessage_AddSuccessfully;
                        _logger.Info("Welcome message updated successfully. " + obj);
                        return RedirectToAction("GetWelcomeMessages");
                    }
                }

                return View(obj);
            }
            catch(Exception ex)
            {
                ViewBag.Message = ex.Message;
                _logger.Error(ex.Message);
                return View(obj);
            }
        }

        // GET: WelcomeMessage/DeleteWelcomeMessage/5 
        [HttpGet]   
        public ActionResult DeleteWelcomeMessage(int id)
        {
            try
            {
                _logger.Info("Attempting to delete welcome message. [Welcome Message Id=" + id + "]");
                WelcomeMessageRepository wlcMsgRepo = new WelcomeMessageRepository();
                if (wlcMsgRepo.DeleteWelcomeMessage(id))
                {
                    ViewBag.AlertMsg = GlobalRes.WelcomeMessage_WelcomeMessageController_DeleteWelcomeMessage_DeletedSuccessfully;
                    _logger.Info("Welcome Message deleted successfully. [Welcome Message Id=" + id + "]");
                }
                return RedirectToAction("GetWelcomeMessages");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return RedirectToAction("GetWelcomeMessages");
            }
        }
    }
}
