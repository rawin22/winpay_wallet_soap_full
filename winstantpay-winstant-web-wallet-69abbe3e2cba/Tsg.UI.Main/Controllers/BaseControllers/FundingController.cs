using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.BlockchainIntegration;
using TSG.Models.APIModels;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Attributes;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.Enums;
using TSG.Models.ServiceModels;
using TSG.ServiceLayer.Interfaces.Fundings;
using WinstantPay.Common.Extension;
using Role = TSG.Models.APIModels.Role;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using TSG.Models.ServiceModels.BlockchainFunds;

namespace Tsg.UI.Main.Controllers
{
    public class FundingController : BaseController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IFundingsService _fundingsService;
        private readonly IFundingSourcesService _fundingsSourceService;
        private readonly IFundingChangesService _fundingsChangesService;
        private readonly string bitcoinManagerSenderUrl = ConfigurationManager.AppSettings["bitcoinManagerSenderUrl"];
        private readonly string dashManagerSenderUrl = ConfigurationManager.AppSettings["dashManagerSenderUrl"];

        private const string secretKeyForBlockChain = "4EFC770B-4D3B-490A-B5C3-56377995C374";

        public FundingController(IFundingsService fundingsService, IFundingSourcesService fundingSourcesService, IFundingChangesService fundingsChangesService)
        {
            _fundingsService = fundingsService;
            _fundingsSourceService = fundingSourcesService;
            _fundingsChangesService = fundingsChangesService;
        }

        #region New logic for Funding

        [HttpGet]
        public ActionResult GetAllFundings()
        {
            return View(_fundingsService.GetAllFundings(Models.Security.AppSecurity.CurrentUser.UserName).Obj.Where(w => !w.Fundings_IsDeleted && (
                w.Fundings_FundingSource.FundingSources_DesignName.Contains("WinstantPay") || w.Fundings_FundingSource.FundingSources_DesignName.Contains("Blockchain.info")))
                .OrderByDescending(ob => ob.Fundings_CreateDate).ToList());
        }

        [HttpGet]
        public ActionResult InfoByFunds(string fundId, string sourceId)
        {
            try
            {
                Guid.TryParse(sourceId, out var guidSourceId);
                Guid.TryParse(fundId, out var guidFundId);
                if (guidFundId == default || guidSourceId == default)
                    throw new Exception("Source or funds doesn't setted");
                var typeOfPaymentSource = _fundingsSourceService.GetById(guidSourceId);
                if (typeOfPaymentSource.Success && typeOfPaymentSource.Obj.FundingSources_DesignName == "WinstantPay")
                {
                    var res = _fundingsService.GetWireFundingById(guidFundId,
                        Models.Security.AppSecurity.CurrentUser.UserName);
                    ViewBag.InformationsBlock = new StandartResponse()
                    {
                        Success = true,
                        InfoBlock = new InfoBlock() { UserMessage = "Data correct" }
                    };
                    return View("PartialViewFunding/InfoByFundsWire", res.Obj);
                }

                else if (typeOfPaymentSource.Success && typeOfPaymentSource.Obj.FundingSources_DesignName == "PipIt")
                {
                    var res = _fundingsService.GetPipitFundingById(guidFundId,
                        Models.Security.AppSecurity.CurrentUser.UserName);
                    ViewBag.InformationsBlock = new StandartResponse()
                    {
                        Success = true,
                        InfoBlock = new InfoBlock() { UserMessage = "Data correct" }
                    };
                    return View("PartialViewFunding/InfoByPipitWire", res.Obj);
                }

                else if (typeOfPaymentSource.Success && typeOfPaymentSource.Obj.FundingSources_DesignName == "Blockchain.info")
                {
                    var res = _fundingsService.GetBlockChainInfoFundingById(guidFundId,
                        Models.Security.AppSecurity.CurrentUser.UserName);
                    BlockchainInfoApi api = new BlockchainInfoApi();
                    res.Obj.AddFundsBlockChainInfo_UserUrl = api.BlockChainInfo_GetPaymentLink(
                        res.Obj.AddFundsBlockChainInfo_BlockChainAddress,
                        res.Obj.AddFundsWire_Fundings.Fundings_Currency.Currency_CcyCode, res.Obj.AddFundsWire_Fundings.Fundings_Amount,
                        res.Obj.AddFundsBlockChainInfo_Alias);
                    res.Obj.AddFundsBlockChainInfo_CurrencyIndex = res.Obj.AddFundsWire_Fundings.Fundings_CurrencyId;
                    res.Obj.AddFundsBlockChainInfo_CurrencyCode = res.Obj.AddFundsWire_Fundings.Fundings_Currency.Currency_CcyCode;

                    ViewBag.InformationsBlock = new StandartResponse()
                    {
                        Success = true,
                        InfoBlock = new InfoBlock() { UserMessage = "Data correct" }
                    };
                    return View("PartialViewFunding/InfoByBlockchainFunding", res.Obj);
                }

            }
            catch (Exception e)
            {
                ViewBag.InformationsBlock = new StandartResponse()
                {
                    Success = false,
                    InfoBlock = new InfoBlock() { UserMessage = "Incorrect source id" }
                };
            }

            return RedirectToAction("GetAllFundings");
        }

        [HttpGet]
        public ActionResult AddFundsWire()
        {
            return View("PartialViewFunding/InfoByFundsWire", new AddFundsWire());
        }

        [HttpPost]
        public ActionResult UpdateWireInstruction(AddFundsWire model)
        {
            if (!ModelState.IsValid)
                return View("PartialViewFunding/InfoByFundsWire", model);
            if (model.Status > (int)FundingStatus.Pending)
                return RedirectToAction("GetAllFundings");

            IFormatProvider cultureThai = CultureInfo.CreateSpecificCulture("th-TH");
            var c = CultureInfo.CurrentCulture;
            model.AddFundsWire_PaymentDate = DateTime.ParseExact(model.AddFundsWire_PaymentDateString.Replace(".", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);

            string path = Server.MapPath("~/Uploads/");

            if (model.AddFundsWire_ParentId == default)
            {
                if (model.AddFundsWire_PostedFile != null && model.AddFundsWire_PostedFile.ContentLength > 0)
                {
                    FilesExtensions.SaveSystemFile(model.AddFundsWire_PostedFile, path, out string fileName, out string filePath);
                    model.AddFundsWire_FileName = fileName;
                    model.AddFundsWire_FilePath = filePath;
                }

                var insertRes = _fundingsService.InsertWireTransfer(model, AppSecurity.CurrentUser.UserName);
                var mail = new ContactUsRepository();
                if (insertRes.Success)
                {
                    string text = String.Format(@GlobalRes.SysProperty_EmailForUploadProof_AdminText, $"{insertRes.Obj.AddFundsWire_ProofDocId}", $"{UiHelper.PrepareAvailableBankCurrencies().FirstOrDefault(f => f.Value == model.AddFundsWire_BankCcyId.ToString())?.Text}",
                        insertRes.Obj.AddFundsWire_Fundings.Fundings_Amount, insertRes.Obj.AddFundsWire_PaymentDate.ToString("MMM dd, yyyy"), insertRes.Obj.AddFundsWire_CustName, insertRes.Obj.AddFundsWire_BankName, insertRes.Obj.AddFundsWire_LastFourDigits, insertRes.Obj.AddFundsWire_Other);
                    string mailSendRes;

                    if (mail.RegisterMail(0, ConfigurationManager.AppSettings["NoReplyMail"], SettingClass.SysNotificationEmailInUploadProof, "System Message", text, new string[] { model.AddFundsWire_FilePath }, "AdminMessage", out mailSendRes, out var mailId))
                        EmailExtension.EmailService.SendEmail(ConfigurationManager.AppSettings["OrganizationName"], text, ConfigurationManager.AppSettings["NoReplyMail"], ConfigurationManager.AppSettings["OrganizationName"], SettingClass.SysNotificationEmailInUploadProof, "", null, new[] { model.AddFundsWire_FilePath });
                    return RedirectToAction("GetAllFundings");
                }

            }
            else
            {
                if (model.AddFundsWire_PostedFile != null && model.AddFundsWire_PostedFile.ContentLength > 0)
                {

                    FilesExtensions.SaveSystemFile(model.AddFundsWire_PostedFile, path, out string fileName, out string filePath);
                    model.AddFundsWire_FileName = fileName;
                    model.AddFundsWire_FilePath = filePath;
                }
                var updateRes = _fundingsService.UpdateWireTransfer(model, AppSecurity.CurrentUser.UserName, (int)FundingStatus.Pending);
                if (updateRes.Success)
                    return RedirectToAction("GetAllFundings");
            }

            return View("PartialViewFunding/InfoByFundsWire", model);
        }


        [HttpGet]
        public ActionResult AddBlockChainInfoFunds()
        {
            return View("PartialViewFunding/InfoByBlockchainFunding", new AddFunds_BlockChainInfo());
        }

        [HttpPost]
        public ActionResult AddBlockChainInfoFunds(AddFunds_BlockChainInfo model)
        {
            string data = "{\"status\": \"UNMATCHED\", \"wpyId\": \"" + model.AddFundsBlockChainInfo_Alias + "\"}";
            _logger.Debug(data);
            HttpClient client = new HttpClient();
            if (string.Equals(model.AddFundsBlockChainInfo_CCY, "BTC"))
            {
                try
                {
                    var uri = new Uri(bitcoinManagerSenderUrl);

                    client.BaseAddress = uri;


                    var content = new StringContent(data, Encoding.UTF8, "application/json");

                    var httpRes = client.PostAsync(uri, content).Result;
                    var strings = httpRes.Content.ReadAsStringAsync().Result;
                    _logger.Info(strings);
                    BitcoinManagerSenderApiResponse response = JsonConvert.DeserializeObject<BitcoinManagerSenderApiResponse>(strings);
                    model.AddFundsBlockChainInfo_DestinatedBitcoinAddress = response.receiverBTCAddress;
                    _logger.Info(response.receiverBTCAddress);
                    _logger.Info(model.AddFundsBlockChainInfo_DestinatedBitcoinAddress);
                    if (httpRes.StatusCode.ToString().Equals("Accepted", StringComparison.OrdinalIgnoreCase))
                    {
                        AddFunds_BlockChainInfo m = new AddFunds_BlockChainInfo();
                        _logger.Info(httpRes.StatusCode.ToString());
                        m.AddFundsBlockChainInfo_Message = "No Available Wallet";
                        //need to inform the user that no available address in wallet
                        return View("PartialViewFunding/InfoByBlockchainFunding", m);
                    }
                    else if (!httpRes.StatusCode.ToString().Equals("Created", StringComparison.OrdinalIgnoreCase))
                    {
                        AddFunds_BlockChainInfo m = new AddFunds_BlockChainInfo();
                        _logger.Info(httpRes.StatusCode.ToString());
                        m.AddFundsBlockChainInfo_Message = "BTC Manager Error";
                        //need to inform the user to try again there is a problem in crypto manager server
                        return View("PartialViewFunding/InfoByBlockchainFunding", m);
                    }

                    model.AddFundsBlockChainInfo_CurrencyCode = "BTC";

                }
                catch (Exception ex)
                {
                    _logger.Debug(ex.Message);

                    AddFunds_BlockChainInfo m = new AddFunds_BlockChainInfo();
                    m.AddFundsBlockChainInfo_Message = "BTC Manager Error";
                    //need to inform the user to try again there is a problem in crypto manager server
                    return View("PartialViewFunding/InfoByBlockchainFunding", m);
                }
            }
            else if(string.Equals(model.AddFundsBlockChainInfo_CCY, "DASH"))
            {
                try
                {
                    var uri = new Uri(dashManagerSenderUrl);

                    client.BaseAddress = uri;


                    var content = new StringContent(data, Encoding.UTF8, "application/json");

                    var httpRes = client.PostAsync(uri, content).Result;
                    var strings = httpRes.Content.ReadAsStringAsync().Result;
                    _logger.Info(strings);
                    DashManagerSenderApiResponse response = JsonConvert.DeserializeObject<DashManagerSenderApiResponse>(strings);
                    model.AddFundsBlockChainInfo_DestinatedBitcoinAddress = response.receiverAddress;
                    _logger.Info(response.receiverAddress);
                    _logger.Info(model.AddFundsBlockChainInfo_DestinatedBitcoinAddress);
                    if (!httpRes.StatusCode.ToString().Equals("Created", StringComparison.OrdinalIgnoreCase))
                    {
                        AddFunds_BlockChainInfo m = new AddFunds_BlockChainInfo();
                        _logger.Info(httpRes.StatusCode.ToString());
                        m.AddFundsBlockChainInfo_Message = "Switch Error";
                        //need to inform the user to try again there is a problem in crypto manager server
                        return View("PartialViewFunding/InfoByBlockchainFunding", m);
                    }

                    model.AddFundsBlockChainInfo_CurrencyCode = "DASH";

                }
                catch (Exception ex)
                {
                    _logger.Debug(ex.Message);

                    AddFunds_BlockChainInfo m = new AddFunds_BlockChainInfo();
                    m.AddFundsBlockChainInfo_Message = "Switch Error";
                    //need to inform the user to try again there is a problem in crypto manager server
                    return View("PartialViewFunding/InfoByBlockchainFunding", m);
                }
            }

            


            
            

           

            return View("PartialViewFunding/InfoByBlockchainFunding", model);
        }

        // GET: Funding/GetFundRequests
        #region Admin Part
        [HttpGet]
        [AuthorizeUser(Roles = Role.Admin)]
        public ActionResult GetFundRequests()
        {
            return View(_fundingsService.GetAllFundings().Obj.Where(w => !w.Fundings_IsDeleted).OrderByDescending(ob => ob.Fundings_CreateDate).ToList());
        }

        // GET: Funding/UpdateFundRequestStatus/5
        [AuthorizeUser(Roles = Role.Admin)]
        [HttpGet]
        public ActionResult UpdateFundRequestStatus(string fundId, string sourceId)
        {
            try
            {
                Guid.TryParse(sourceId, out var guidSourceId);
                Guid.TryParse(fundId, out var guidFundId);
                if (guidFundId == default || guidSourceId == default)
                    throw new Exception("Source or funds doesn't setted");
                var typeOfPaymentSource = _fundingsSourceService.GetById(guidSourceId);
                if (typeOfPaymentSource.Success && typeOfPaymentSource.Obj.FundingSources_DesignName == "WinstantPay")
                {
                    var res = _fundingsService.GetWireFundingById(guidFundId,
                        Models.Security.AppSecurity.CurrentUser.UserName);
                    ViewBag.InformationsBlock = new StandartResponse()
                    {
                        Success = true,
                        InfoBlock = new InfoBlock() { UserMessage = "Data correct" }
                    };
                    res.Obj.Status = res.Obj.AddFundsWire_Fundings.Fundings_StatusByFund;
                    return View("PartialViewFunding/InfoByFundsWireReadOnly", res.Obj);
                }

                else if (typeOfPaymentSource.Success && typeOfPaymentSource.Obj.FundingSources_DesignName == "PipIt")
                {
                    var res = _fundingsService.GetPipitFundingById(guidFundId,
                        Models.Security.AppSecurity.CurrentUser.UserName);
                    ViewBag.InformationsBlock = new StandartResponse()
                    {
                        Success = true,
                        InfoBlock = new InfoBlock() { UserMessage = "Data correct" }
                    };
                    return View("PartialViewFunding/InfoByPipitWire", res.Obj);
                }

            }
            catch (Exception e)
            {
                _logger.Error(e);
                ViewBag.InformationsBlock = new StandartResponse()
                {
                    Success = false,
                    InfoBlock = new InfoBlock() { UserMessage = "Incorrect source or funds id's" }
                };
            }

            return RedirectToAction("GetFundRequests");

        }

        // POST: Funding/UpdateFundRequestStatus/5    
        [AuthorizeUser(Roles = Role.Admin)]
        [HttpPost]
        public ActionResult UpdateFundRequestStatus(string fundId, string sourceId, int status, string memo)
        {
            try
            {
                var result = _fundingsService.UpdateWireTransferStatusForAdmin(Guid.Parse(fundId), AppSecurity.CurrentUser.UserName, status, memo);

                if (result.Success)
                    return RedirectToAction("GetFundRequests");

                else
                {
                    return RedirectToAction("UpdateFundRequestStatus", new { fundId, sourceId });
                }
                //FundingRepository fundingRepo = new FundingRepository();
                //BankDepositModel model = fundingRepo.GetFundRequests().Find(fndRqst => fndRqst.DepositId == id);
                //if (obj.Status == model.Status && obj.Notes == model.Notes)
                //    return RedirectToAction("GetFundRequests");

                //string responsibleUser = Tsg.UI.Main.Models.Security.AppSecurity.CurrentUser.UserName;

                //if (String.IsNullOrEmpty(obj.Notes))
                //    obj.Notes = "";
                //if (fundingRepo.UpdateFundRequestStatus(responsibleUser, obj))
                //{
                //    ViewBag.Message = GlobalRes.Funding_FundingController_UpdateFundsRequest_Success;
                //    _logger.Info("Fund request status updated successfully. " + obj);
                //    return RedirectToAction("GetFundRequests");
                //}
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                _logger.Error(ex.Message);
            }

            return RedirectToAction("GetFundRequests");
        }

        [AuthorizeUser(Roles = Role.Admin)]
        [HttpGet]
        public ActionResult GetHystoryByFunding(Guid fundId)
        {
            return View(_fundingsChangesService.GetAllChangeById(fundId).OrderByDescending(ob => ob.FundChanges_ChangedDate).ToList());
        }
        #endregion

        // POST: Funding/GetFundRequests
        //        [AuthorizeUser(Roles = Role.Admin)]
        //        [HttpPost]
        //        public ActionResult GetFundRequests(BankFilterModel filterModel)
        //        {
        //            FundingRepository fundingRepo = new FundingRepository();
        //            ModelState.Clear();
        //            var model = new ListOfBankModel();
        //            List<BankDepositModel> listOfFundings = fundingRepo.GetFundRequests();
        //            model.FilterModel = filterModel;
        //            var filteredList = listOfFundings;
        //            if (filterModel.BankId != null)
        //                filteredList = filteredList.Where(w => w.BankId == filterModel.BankId).ToList();
        //            if (filterModel.BeginDate != null)
        //                filteredList = filteredList.Where(w => w.CreatedDate >= filterModel.BeginDate).ToList();
        //            if (filterModel.EndDate != null)
        //                filteredList = filteredList.Where(w => w.CreatedDate <= filterModel.EndDate).ToList();
        //            if (filterModel.CurrencyId != null)
        //                filteredList = filteredList.Where(w => w.BankCurrencyId == filterModel.CurrencyId).ToList();
        //            if (filterModel.AmountMin != null)
        //                filteredList = filteredList.Where(w => w.Amount >= filterModel.AmountMin).ToList();
        //            if (filterModel.AmountMax != null)
        //                filteredList = filteredList.Where(w => w.Amount <= filterModel.AmountMax).ToList();
        //            //------------------------------------------------------------------------------------//
        //            if (filterModel.NameOfSender != null)
        //                filteredList = filteredList.Where(w => w.CustomerName.Contains(filterModel.NameOfSender)).ToList();
        //            if (filterModel.NameOfSendingBank != null)
        //                filteredList = filteredList.Where(w => w.CustomerName.Contains(filterModel.NameOfSendingBank)).ToList();
        //            if (filterModel.AccountNumber != null)
        //                filteredList = filteredList.Where(w => w.ClientAccountNumber.Contains(filterModel.AccountNumber)).ToList();
        //            if (filterModel.OtherInformation != null)
        //                filteredList = filteredList.Where(w => w.ClientOtherInfo.Contains(filterModel.OtherInformation)).ToList();
        //            //------------------------------------------------------------------------------------//
        //
        //            model.EnumBankDepositModel = filteredList;
        //            return View(model);
        //        }

        #endregion

        #region Old logic for Fundings
        // GET: Funding/AddBankDeposit
        [HttpGet]
        public ActionResult AddBankDeposit()
        {
            BankDepositModel bankDepositModel = new BankDepositModel();
            bankDepositModel.AvailableBankCurrencies = PrepareAvailableBankCurrencies();

            ModelState.Clear();
            return View(bankDepositModel);
        }

        private IList<SelectListItem> PrepareAvailableBankCurrencies()
        {
            var bankCurrenciesList = new List<SelectListItem>();
            bankCurrenciesList.Add
            (
                new SelectListItem
                {
                    Text = GlobalRes.Funding_FundingController_PrepareAvalBanks_SelectBank,
                    Value = ""
                }
            );

            CurrencyRepository curRepo = new CurrencyRepository();
            var bankCurrencies = curRepo.GetBankCurrencies();
            foreach (var item in bankCurrencies)
            {
                bankCurrenciesList.Add
                (
                    new SelectListItem
                    {
                        Text = item.BankCurrencyName,
                        Value = item.BankCurrencyId.ToString()
                    }
                );
            }
            return bankCurrenciesList;
        }
        private IList<SelectListItem> PrepareAvailableFundingStatuses()
        {
            var fundingStatusList = new List<SelectListItem>();
            foreach (var item in Enum.GetValues(typeof(FundingStatus)))
            {

                var fieldInfo = item.GetType().GetField(item.ToString());

                var descriptionAttributes = fieldInfo.GetCustomAttributes(
                    typeof(DisplayAttribute), false) as DisplayAttribute[];

                //if (descriptionAttributes == null) string.Empty;
                //return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : item.ToString();



                fundingStatusList.Add
                (
                    new SelectListItem
                    {
                        Text = (descriptionAttributes != null && descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : item.ToString(),
                        Value = (descriptionAttributes != null && descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : item.ToString()
                    }
                );

            }
            return fundingStatusList;
        }


        // POST: Funding/AddBankDeposit
        [HttpPost]
        public ActionResult AddBankDeposit(BankDepositModel obj)
        {
            return RedirectToAction("AddFundsWire");
        }

        // GET: Funding/GetBankDeposits
        [HttpGet]
        public ActionResult GetBankDeposits()
        {
            FundingRepository fundingRepo = new FundingRepository();
            ModelState.Clear();
            string currentUser = Tsg.UI.Main.Models.Security.AppSecurity.CurrentUser.UserName;
            return View(fundingRepo.GetBankDeposits(currentUser));
        }
        // POST: Funding/DeleteBankDeposit/5 
        [HttpPost]
        public ActionResult DeleteBankDeposit(int depositId)
        {
            try
            {
                FundingRepository fundingRepo = new FundingRepository();
                if (fundingRepo.DeleteBankDeposit(depositId))
                {
                    ViewBag.Message = GlobalRes.Funding_FundingController_DeleteBankDeposit_Success;
                    _logger.Info("Bank deposit deleted from database. [Deposit Id=" + depositId + "]");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                _logger.Error(ex.Message);
                // ReSharper disable once Mvc.ViewNotResolved
                return Json("Error");
            }
            return RedirectToAction("GetBankDeposits", "Funding");
        }


        #endregion
    }
}