using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;
using System.Web.Mvc;
using Tsg.Business.Model.Classes;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Attributes;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using CurrencyViewModel = Tsg.UI.Main.Models.CurrencyViewModel;

namespace Tsg.UI.Main.Controllers
{
    [AuthorizeUser(Roles = Role.Admin)]
    //[Authorize(Roles = Role.Admin)]
    public class BankController : BaseController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: Bank/GetBanks
        [HttpGet]
        public ActionResult GetBanks()
        {
            BankRepository bankRepo = new BankRepository();
            ModelState.Clear();
            if (TempData.ContainsKey("ReturnResult"))
            {
                var tempModel = TempData["ReturnResult"] as MessageInfoModel;
                if (tempModel != null)
                {
                    ViewBag.IsError = !tempModel.IsSuccess;
                    ViewBag.Message = tempModel.MessageText;
                }
            }

            return View(bankRepo.GetBanks());
        }
        // GET: Bank/GetCurrencies
        [HttpGet]
        public ActionResult GetCurrencies()
        {
            BankRepository bankRepo = new BankRepository();
            ModelState.Clear();
            return View(bankRepo.GetCurrencies());
        }

        // GET: Bank/AddBank
        [HttpGet]
        public ActionResult AddBank()
        {
            BankModel bankModel = new BankModel();
            ViewBag.returnUrl = Request.UrlReferrer;

            bankModel.AvailableCurrencies = PrepareAvailableCurrencies();
            return View(bankModel);
        }

        //POST: Bank/AddBank
        [HttpPost]
        public ActionResult AddBank(BankModel bnk)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    BankRepository bankRepo = new BankRepository();

                    if (bankRepo.AddBank(bnk))
                    {
                        ViewBag.Message = GlobalRes.Bank_BankController_AddBank_ViewBagMessage;
                        _logger.Info(bnk.BankName + " is added to database");
                        return RedirectToAction("GetBanks");
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                _logger.Error(ex.Message);
            }
            bnk.AvailableCurrencies = PrepareAvailableCurrencies();
            return View(bnk);
        }

        [HttpPost]
        public ActionResult AddBankBankAndInstruction(BankAndInstructionModel biwmodel)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                BankRepository bankRepo = new BankRepository();
                int addedBankId = 0;
                if (bankRepo.AddBank(new BankModel() { BankCountry = biwmodel.BankCountry, BankName = biwmodel.BankName, BankCountryId = biwmodel.BankCountryId }, out addedBankId).IsSuccesifully)
                {
                    if (addedBankId != 0)
                        biwmodel.BankId = addedBankId;
                    ViewBag.Message = GlobalRes.Bank_BankController_AddBankBankAndInstruction_ViewBagMessage;
                    biwmodel.Message = new MessageInfoModel() { IsModal = true, IsSuccess = true, MessageText = GlobalRes.Bank_BankController_AddBankBankAndInstruction_ViewBagMessage, MessageTitle = GlobalRes.Bank_BankController_AddBankBankAndInstruction_Title };
                    biwmodel.AvaliableBanks = UiHelper.PrepareAvailableBank();
                    biwmodel.AvailableCurrencies = UiHelper.PrepareAvailableCurrencies();
                    biwmodel.AvaliableCountries = UiHelper.PrepareAvailableCountries();
                    return Json(biwmodel, JsonRequestBehavior.DenyGet);
                }
                //}
            }
            catch (Exception ex)
            {
                biwmodel.Message = new MessageInfoModel() { IsModal = true, IsSuccess = false, MessageText = ex.Message, MessageTitle = GlobalRes.Bank_BankController_AddBankBankAndInstruction_Title };
                ViewBag.Message = ex.Message;
                _logger.Error(ex.Message);
            }
            return Json(biwmodel);
        }

        // GET: Bank/AddCurrency
        [HttpGet]
        public ActionResult AddCurrency()
        {
            CurrencyViewModel ccyModel = new CurrencyViewModel();
            return View(ccyModel);
        }

        // POST: Bank/AddCurrency
        [HttpPost]
        public ActionResult AddCurrency(CurrencyViewModel ccyModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CurrencyRepository ccyRepo = new CurrencyRepository();
                    BankRepository bankRepo = new BankRepository();
                    int id;
                    if (ccyModel.CurrencyId != 0)
                    {
                        if (bankRepo.UpdateCurrency(ccyModel))
                        {
                            ViewBag.Message = GlobalRes.Bank_BankController_UpdateCurrency_ViewBagMessage;
                            _logger.Info(ccyModel.CurrencyName + " information is updated in database");
                            return RedirectToAction("GetCurrencies", "Bank");
                        }
                    }
                    else
                    {
                        if (ccyRepo.AddCurrency(ccyModel, out id))
                        {
                            ViewBag.Message = GlobalRes.Bank_BankController_AddCurrency_ViewBagMessage;
                            _logger.Info(ccyModel.CurrencyCode + " is added to database");
                            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
                                return Redirect(TempData["ReturnUrl"].ToString());

                            return RedirectToAction("GetCurrencies", "Bank");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                _logger.Error(ex.Message);
            }
            return View(ccyModel);
        }

        // POST: Bank/AddCurrency
        [HttpPost]
        public ActionResult AddCurrencyBankAndInstruction(string currencyId, string currencyName, string currencyCode, string currencySymbol)
        {
            BankAndInstructionModel model = new BankAndInstructionModel();

            try
            {
                CurrencyRepository ccyRepo = new CurrencyRepository();
                if (!String.IsNullOrEmpty(currencyId))
                    model.CurrencyId = Convert.ToInt32(currencyId);
                model.CurrencyCode = currencyCode;
                model.CurrencyName = currencyName;
                model.CurrencySymbol = currencySymbol;
                model.Message = new MessageInfoModel() { IsModal = true, IsSuccess = false, MessageText = "", MessageTitle = GlobalRes.Bank_BankController_AddCurrencyBankAndInstruction_Title };
                int ccyId = 0;
                if (ccyRepo.AddCurrency(new CurrencyViewModel() { CurrencyId = model.CurrencyId, CurrencyCode = model.CurrencyCode, CurrencyName = model.CurrencyName, Symbol = model.CurrencySymbol }, out ccyId))
                {
                    ViewBag.Message = GlobalRes.Bank_BankController_AddCurrencyBankAndInstruction_ViewBagMessage;
                    model.CurrencyId = ccyId;
                    model.Message = new MessageInfoModel() { IsModal = true, IsSuccess = true, MessageText = GlobalRes.Bank_BankController_AddCurrencyBankAndInstruction_ViewBagMessage, MessageTitle = GlobalRes.Bank_BankController_AddCurrencyBankAndInstruction_Title };
                    _logger.Info(model.CurrencyCode + " is added to database");
                    return Json(model);
                }

            }
            catch (Exception ex)
            {
                model.Message = new MessageInfoModel() { IsModal = true, IsSuccess = false, MessageText = ex.Message, MessageTitle = "Add Currency" };

                ViewBag.Message = ex.Message;
                _logger.Error(ex.Message);
            }
            return Json(model);
        }

        private IList<SelectListItem> PrepareAvailableCurrencies()
        {
            var currenciesList = new List<SelectListItem>();
            currenciesList.Add
            (
                new SelectListItem
                {
                    Text = GlobalRes.Shared_NewInstantPaymentFormPage_SelectCurrency,
                    Value = ""
                }
            );

            CurrencyRepository curRepo = new CurrencyRepository();
            var currencies = curRepo.GetCurrencies();
            foreach (var item in currencies)
            {
                currenciesList.Add
                (
                    new SelectListItem
                    {
                        Text = item.CurrencyCode,
                        Value = item.CurrencyId.ToString()
                    }
                );
            }
            return currenciesList;
        }

        // GET: Bank/EditBank/5
        [HttpGet]
        public ActionResult EditBank(int id)
        {
            BankRepository bankRepo = new BankRepository();
            BankModel bankModel = bankRepo.GetBanks().Find(bnk => bnk.BankId == id);
            bankModel.AvailableCurrencies = PrepareAvailableCurrencies();
            return View(bankModel);
        }

        // POST: Bank/EditBank/5    
        [HttpPost]
        public ActionResult EditBank(int id, BankModel obj)
        {
            try
            {
                BankRepository bankRepo = new BankRepository();
                if (bankRepo.UpdateBank(obj))
                {
                    ViewBag.Message = GlobalRes.Bank_BankController_EditBank_ViewMessage;
                    _logger.Info(obj.BankName + " information is updated in database");
                    return RedirectToAction("GetBanks");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                _logger.Error(ex.Message);
            }
            obj.AvailableCurrencies = PrepareAvailableCurrencies();
            return View(obj);
        }

        // GET: Bank/DeleteBank/5 
        [HttpPost]
        public ActionResult DeleteBank(string BankId, string BankName)
        {
            try
            {
                int id = Convert.ToInt32(BankId);
                MessageInfoModel message = new MessageInfoModel(){IsModal = true};
                BankRepository bankRepo = new BankRepository();
                if (bankRepo.DeleteBank(id))
                {
                    message.IsSuccess = true;
                    message.MessageText = String.Format(GlobalRes.Bank_BankController_DeleteBank_ViewMessageSuccess, BankName);
                    _logger.Info("Bank with id=" + id + " deleted successfully");
                }
                else
                {
                    message.IsSuccess = false;
                    message.MessageText = String.Format(GlobalRes.Bank_BankController_DeleteBank_ViewMessageUnSuccess, BankName);
                    _logger.Info("Bank with id=" + id + " deleted unsuccessfully");
                }
                TempData["ReturnResult"] = message;
                return View("GetBanks");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                _logger.Error(ex.Message);
                return View("GetBanks");
                //TODO: Узнать нужна ли страница для удаления
                //return View();
            }
        }

        // GET: Bank/EditCurrency/5
        [HttpGet]
        public ActionResult EditCurrency(int id)
        {
            BankRepository bankRepo = new BankRepository();
            TempData["ReturnUrl"] = System.Web.HttpContext.Current.Request.UrlReferrer!=null?System.Web.HttpContext.Current.Request.UrlReferrer.ToString(): "";
            CurrencyViewModel currModel = bankRepo.GetCurrencies().Find(cur => cur.CurrencyId == id);
            return View("AddCurrency",currModel);
        }

        // POST: Bank/EditCurrency/5    
        [HttpPost]
        public ActionResult EditCurrency(int id, CurrencyViewModel obj)
        {
            try
            {
                BankRepository bankRepo = new BankRepository();
                if (bankRepo.UpdateCurrency(obj))
                {
                    ViewBag.Message = GlobalRes.Bank_BankController_EditCurrency_ViewMessage;
                    _logger.Info(obj.CurrencyName + " information is updated in database");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                _logger.Error(ex.Message);
            }
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
                return Redirect(TempData["ReturnUrl"].ToString());
            return RedirectToAction("GetCurrencies");
            //return View(obj);
        }

        // GET: Bank/DeleteCurrency/5 
        [HttpGet]
        public ActionResult DeleteCurrency(int id)
        {
            try
            {
                BankRepository bankRepo = new BankRepository();
                if (bankRepo.DeleteCurrency(id))
                {
                    ViewBag.Message = GlobalRes.Bank_BankController_DeleteCurrency_ViewMessageSuccess;
                    _logger.Info("Currency with id=" + id + " deleted successfully");
                }
                else
                {
                    ViewBag.Message = GlobalRes.Bank_BankController_DeleteCurrency_ViewMessageUnSuccess;
                    _logger.Info("Currency with id=" + id + " deleted unsuccessfully");
                }
                return RedirectToAction("GetCurrencies");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                _logger.Error(ex.Message);
                return RedirectToAction("GetCurrencies");
                //TODO: Узнать нужна ли страница для удаления
                //return View();
            }
        }

        [HttpGet]
        public ActionResult GetBankAndWireInstructions()
        {
            try
            {
                BankRepository bankRepo = new BankRepository();
                ModelState.Clear();
                return View(bankRepo.GetBanksAndWiredInstruction());
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult AddBankAndWireInst(int? bankCcyId)
        {
            try
            {
                BankAndInstructionModel model = new BankAndInstructionModel();
                model.AvaliableBanks = UiHelper.PrepareAvailableBank();
                model.AvailableCurrencies = UiHelper.PrepareAvailableCurrencies();
                model.AvaliableCountries = UiHelper.PrepareAvailableCountries();

                if (TempData.ContainsKey("BankAndInstructionModel"))
                {
                    var tempModel = TempData["BankAndInstructionModel"] as BankAndInstructionModel;
                    if (tempModel != null)
                    {
                        model = tempModel;
                        model.AvaliableBanks = UiHelper.PrepareAvailableBank();
                        model.AvailableCurrencies = UiHelper.PrepareAvailableCurrencies();
                        model.AvaliableCountries = UiHelper.PrepareAvailableCountries();
                    }
                    return View(model);
                }
                model.AvaliableBanks = UiHelper.PrepareAvailableBank();
                model.AvailableCurrencies = UiHelper.PrepareAvailableCurrencies();
                model.AvaliableCountries = UiHelper.PrepareAvailableCountries();
                if (bankCcyId != null)
                {
                    model.BankCcyId = (int)bankCcyId;
                    
                    BankRepository br = new BankRepository();
                    var bankAndWrInstDet = br.GetBanksAndWiredInstructionById((int)bankCcyId);
                    if (bankAndWrInstDet != null && bankAndWrInstDet.BankCcyId != 0)
                    {
                        model.BankName = bankAndWrInstDet.BankName;
                        model.BankCountryId = bankAndWrInstDet.BankCountryId;
                        model.BankCountry = bankAndWrInstDet.BankCountry;
                        model.BankId = (int)bankAndWrInstDet.BankId;
                        model.CurrencyId = (int)bankAndWrInstDet.CurrencyId;
                        //model.BankName = bankAndWrInstDet.BankName;
                        //model.BankCountry = bankAndWrInstDet.BankCountry;
                        if (bankAndWrInstDet.WireInstructionId != null && bankAndWrInstDet.WireInstructionId != 0)
                        {
                            model.WireInstructionId = bankAndWrInstDet.WireInstructionId;
                            model.WireInstructionText = bankAndWrInstDet.WireInstructionText;
                        }
                    }
                    return View(model);
                }

                return View(model);
            }
            catch (Exception e)
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult AddBankAndWireInst(BankAndInstructionModel model)
        {
            try
            {
                BankRepository bankRepository = new BankRepository();
                WireInstructionRepository wr = new WireInstructionRepository();
                var bank = bankRepository.GetBanks().Find(f=>f.BankId == model.BankCcyId);

                if (model.BankCcyId == 0)
                {
                    var addnewbank = bankRepository.AddBankCurrencies(new BankModel()
                    {
                        CurrencyId = model.CurrencyId,
                        BankId = model.BankId,
                        BankCountry = model.BankCountry,
                        BankCountryId = model.BankCountryId,
                        BankName = model.BankName
                    }, true);
                    if (addnewbank.IsSuccesifully && !String.IsNullOrEmpty(model.WireInstructionText))
                    {
                        var addwireinst = wr.AddWireInstruction(new WireInstructionModel()
                        {
                            BankCurrencyId = addnewbank.BankCcyId,
                            WireInstruction = model.WireInstructionText
                        });
                        if (addwireinst)
                            return RedirectToAction("GetBankAndWireInstructions");
                    }
                    return RedirectToAction("GetBankAndWireInstructions");
                }
                else if (model.WireInstructionId == null || model.WireInstructionId == 0)
                {
                    var addwireinst = wr.AddWireInstruction(new WireInstructionModel()
                    {
                        BankCurrencyId = model.BankCcyId,
                        WireInstruction = model.WireInstructionText
                    });

                    if (addwireinst)
                        return RedirectToAction("GetBankAndWireInstructions");
                }
                else
                {
                    var updatewireinst = wr.UpdateWireInstruction(new WireInstructionModel()
                    {
                        WireInstructionId = (int)model.WireInstructionId,
                        BankCurrencyId = model.BankCcyId,
                        WireInstruction = String.IsNullOrEmpty(model.WireInstructionText) ? "" : model.WireInstructionText
                    });
                    var updateBankCurrencyId = bankRepository.UpdateBankCurrency(model.BankCcyId, model.BankId,
                        model.CurrencyId, (int) model.WireInstructionId);

                    if (updatewireinst && updateBankCurrencyId)
                        return RedirectToAction("GetBankAndWireInstructions");
                }

                return View(model);
            }
            catch (Exception e)
            {
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult DeletePayInInstructions(int id)
        {
            try
            {
                BankRepository bankRepository = new BankRepository();
                if (bankRepository.DeleteBankAndPayInstruction(id))
                {
                    ViewBag.Message = GlobalRes.Bank_BankController_GetBankAndWireInstructions_Success;
                    _logger.Info("PayIn instructions deleted from database. [Deposit Id=" + id + "]");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                _logger.Error(ex.Message);
                // ReSharper disable once Mvc.ViewNotResolved
                return Json("Error");
            }
            return RedirectToAction("GetBankAndWireInstructions");

        }
    }
}
