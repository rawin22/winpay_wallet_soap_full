using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.Enums;
using WinstantPayDb;

namespace Tsg.UI.Main.Controllers
{
    public class ProofDocController : BaseController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: ProofDoc/UploadProofDoc
        [HttpGet]
        public ActionResult UploadProofDoc(int? id)
        {
            ProofDocModel model = new ProofDocModel();

            if (id == null)
            {
                model.PaymentDate = DateTime.Now;
                model.PaymentDateString = model.PaymentDate.ToString("dd/MM/yyyy");
                model.AvailableBankCurrencies = UiHelper.PrepareAvailableBankCurrencies();
                return View(model);
            }
            else
            {
                ProofDocRepository pr = new ProofDocRepository();
                model = pr.GetProofDocById((int)id);
                model.AvailableBankCurrencies = UiHelper.PrepareAvailableBankCurrencies();
                return View(model);
            }
        }

        // POST: ProofDoc/UploadProofDoc/4
        [HttpPost]
        public ActionResult UploadProofDoc(ProofDocModel obj)
        {
            obj.AvailableBankCurrencies = UiHelper.PrepareAvailableBankCurrencies();
            try
            {
                ProofDocRepository pr = new ProofDocRepository();
                bool isNew = ReferenceEquals(obj.ProofDocId, null);
                string prefix = isNew ? "[Added]" : "[Changed]";
                obj.PaymentDate = DateTime.ParseExact(obj.PaymentDateString.Replace(".","/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                var serverData = pr.GetProofDocById(obj.ProofDocId ?? 0);
                //if (serverData != null && serverData.FundingEnumStatus!=null &&serverData.FundingEnumStatus == FundingStatus.Credited)
                if (serverData != null && serverData.FundingEnumStatus == FundingStatus.Credited)
                    return RedirectToAction("GetBankDeposits", "Funding");

                FundingRepository fundingRepo = new FundingRepository();
                if (obj.ProofDocId == null)
                {   
                    BankDepositModel bankModel = new BankDepositModel();
                    bankModel.Amount = obj.Amount;
                    bankModel.Username = AppSecurity.CurrentUser.UserName;
                    bankModel.BankCurrencyId = obj.BankCurrencyId;
                    int bankInsId;
                    var bankrepAddRes = fundingRepo.AddBankDeposit(bankModel, out bankInsId);
                    if (!bankrepAddRes && bankInsId == 0)
                        throw new Exception(GlobalRes.ProofDoc_ProofDocController_UploadProofDoc_ErrCanntCreateBankDeposit);
                    obj.DepositId = bankInsId;

                    _logger.Info("Attempting to upload proof document. " + obj);
                }
                if (obj.PostedFile != null && obj.PostedFile.ContentLength > 0)
                {
                    var fileName = String.Format("{0}{1}{2}",Path.GetFileNameWithoutExtension(obj.PostedFile.FileName),Guid.NewGuid(),Path.GetExtension(obj.PostedFile.FileName));
                    string path = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    var destinationPath = Path.Combine(path, fileName);
                    obj.PostedFile.SaveAs(destinationPath);
                    _logger.Info(GlobalRes.ProofDoc_ProofDocController_UploadProofDoc_ProofDocUploadAt + destinationPath);
                    obj.ProofDocName = fileName;
                    obj.ProofDocPath = destinationPath;
                }

                ProofDocRepository proofDocRepo = new ProofDocRepository();
                if (proofDocRepo.UploadProofDoc(obj))
                {
                    ViewBag.Message = GlobalRes.ProofDoc_ProofDocController_UploadProofDoc_UploadSuccessifully;
                    _logger.Info("Proof of document uploaded successfully");
                }
                var mail = new ContactUsRepository();
                //{0} = Proof ID [obj.ProofDocId]
                //{1} = Receiving Bank & CCY $"{obj.AvailableBankCurrencies.FirstOrDefault(f=>f.Value==obj.BankCurrencyId.ToString())?.Text}"
                //{2} = Amount [obj.Amount]
                //{3} = Payment Date obj.PaymentDate.ToString("MMM dd, yyyy")
                //{4} = Name of Sender [obj.CustomerName]
                //{5} = Name of Sending Bank 
                //{6} = Account Number
                //{7} = Other Info
                string text = String.Format(@GlobalRes.SysProperty_EmailForUploadProof_AdminText, $"{obj.ProofDocId}", $"{obj.AvailableBankCurrencies.FirstOrDefault(f=>f.Value==obj.BankCurrencyId.ToString())?.Text}",
                    obj.Amount, obj.PaymentDate.ToString("MMM dd, yyyy"), obj.CustomerName, obj.BankName, obj.LastFourDigits, obj.OtherInfo);
                string mailSendRes;

                if (isNew)
                {
                    if(mail.RegisterMail(0, ConfigurationManager.AppSettings["NoReplyMail"], SettingClass.SysNotificationEmailInUploadProof, "System Message", text, new string[]{obj.ProofDocPath}, "AdminMessage", out mailSendRes, out var mailId))
                        EmailExtension.EmailService.SendEmail("System Message", text, ConfigurationManager.AppSettings["NoReplyMail"], ConfigurationManager.AppSettings["OrganizationName"], SettingClass.SysNotificationEmailInUploadProof, "", null, new []{ obj.ProofDocPath });
                }
                ViewBag.Message = GlobalRes.User_UserController_AddAdminUser_AddSuccessifully;
                return RedirectToAction("GetBankDeposits", "Funding");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                _logger.Error(ex.Message);
                return View();
            }
        }

        // GET: ProofDoc/RemoveProofDoc
        [HttpGet]
        public ActionResult RemoveProofDoc(int id)
        {
            ProofDocRepository docRepo = new ProofDocRepository();
            string fileName = docRepo.GetDocumentName(id);

            try
            {
                _logger.Info("Attempting to remove proof of document. [Proof Doc Id=" + id + "]");
                if (!String.IsNullOrEmpty(fileName))
                {
                    FileInfo file = new FileInfo(Server.MapPath("~/Uploads/" + fileName));
                    if (file.Exists)
                    {
                        file.Delete();
                    }

                    if (docRepo.DeleteProofDoc(id))
                    {
                        ViewBag.AlertMsg = GlobalRes.ProofDoc_ProofDocController_RemoveProofDoc_DelSuccess;
                        _logger.Info(GlobalRes.ProofDoc_ProofDocController_RemoveProofDoc_DelSuccess);
                    }
                    return RedirectToAction("GetBankDeposits", "Funding");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                _logger.Error(ex.Message);
            }

            return Json("Ok");
        }

        // GET: ProofDoc/OpenProofDoc
        [HttpGet]
        public FileResult OpenProofDoc(int id)
        {
            ProofDocRepository docRepo = new ProofDocRepository();
            string fileName = docRepo.GetDocumentName(id);

            try
            {
                _logger.Info("Attempting to open proof document. [Proof Doc Id=" + id + "]");
                if (!String.IsNullOrEmpty(fileName) && System.IO.File.Exists(Server.MapPath("~/Uploads/" + fileName)))
                    return File(new FileStream(Server.MapPath("~/Uploads/" + fileName), FileMode.Open),
                        "application/octetstream", fileName);
                throw new Exception("File missed or doesn't exist");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                _logger.Error(ex.Message);
            }
            return null;
        }
    }
}