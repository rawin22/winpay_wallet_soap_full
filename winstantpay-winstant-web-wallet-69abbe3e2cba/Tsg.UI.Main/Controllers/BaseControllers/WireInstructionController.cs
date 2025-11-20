using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Attributes;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;

namespace Tsg.UI.Main.Controllers
{
    public class WireInstructionController : BaseController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: WireInstruction/GetWireInstructions
        [AuthorizeUser(Roles = Role.Admin)]
        [HttpGet]
        public ActionResult GetWireInstructions()
        {
            WireInstructionRepository wireInstRepo = new WireInstructionRepository();
            ModelState.Clear();
            return View(wireInstRepo.GetWireInstructions());
        }

        // POST: WireInstruction/GetWireInstruction
        //[AuthorizeUser(Roles = Role.Admin)]
        [HttpPost]
        public ActionResult GetWireInstruction(int? val)
        {
            if(val != null)
            {
                WireInstructionRepository wireInstRepo = new WireInstructionRepository();
                ModelState.Clear();
                string wireInst = wireInstRepo.GetWireInstruction(val.Value);
                if (wireInst.Contains("{client_name}"))
                    wireInst = wireInst.Replace("{client_name}", String.Format("{0} {1}", AppSecurity.CurrentUser.FirstName, AppSecurity.CurrentUser.LastName));
                return Json(new { Success = "true", Data = new { WireInstruction = wireInst } });
            }

            return Json(new { Success = "false" });

        }

        [HttpGet]
        public ContentResult GetPrintFriendlyInstruction(int? val)
        {
            if (val != null)
            {
                WireInstructionRepository wireInstRepo = new WireInstructionRepository();
                ModelState.Clear();
                string wireInst = wireInstRepo.GetWireInstruction(val.Value);
                if (wireInst.Contains("{client_name}"))
                    wireInst = wireInst.Replace("{client_name}", String.Format("{0} {1}", AppSecurity.CurrentUser.FirstName, AppSecurity.CurrentUser.LastName));
                wireInst = "<pre id='wireInst'>" + wireInst + "</pre>";
                wireInst += "<br/><input type='button' value='Print' onclick='window.print();'>&nbsp;<input type='button' value='Close' onclick='window.close();'>";
                return Content(wireInst);
            }

            return Content(GlobalRes.WireInstruction_WireInstructionController_GetPrintFriendlyInstruction_ErrBankCurrencyIdIsNull);
        }


        // GET: WireInstruction/AddWireInstruction
        [AuthorizeUser(Roles = Role.Admin)]
        [HttpGet]
        public ActionResult AddWireInstruction()
        {
            WireInstructionModel wireInstModel = new WireInstructionModel();
            wireInstModel.AvailableBankCurrencies = PrepareAvailableBankCurrencies();

            return View(wireInstModel);
        }

        private IList<SelectListItem> PrepareAvailableBankCurrencies()
        {
            var bankCurrenciesList = new List<SelectListItem>();
            bankCurrenciesList.Add
            (
                new SelectListItem
                {
                    Text = GlobalRes.WireInstruction_WireInstructionController_PrepareAvailableBankCurrencies_SelectCurrencyAndBank,
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

        // POST: WireInstruction/AddWireInstruction
        [AuthorizeUser(Roles = Role.Admin)]
        [HttpPost]
        public ActionResult AddWireInstruction(WireInstructionModel obj)
        {
            try
            {
                _logger.Info("Attempting to add wire instruction. "+ obj);
                if (ModelState.IsValid)
                {
                    WireInstructionRepository wireInstRepo = new WireInstructionRepository();

                    if (wireInstRepo.AddWireInstruction(obj))
                    {
                        ViewBag.Message = GlobalRes.WireInstruction_WireInstructionController_AddWireInstruction_CreatedSuccessfully;
                        _logger.Info("Wire Instruction created successfully. " + obj);
                        return RedirectToAction("GetWireInstructions");
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                _logger.Error(ex.Message);
            }
            obj.AvailableBankCurrencies = PrepareAvailableBankCurrencies();
            return View(obj);
        }

        // GET: WireInstruction/EditWireInstruction/5
        [HttpGet]
        public ActionResult EditWireInstruction(int id)
        {
            WireInstructionRepository wireInstRepo = new WireInstructionRepository();
            return View(wireInstRepo.GetWireInstructions().Find(wireInst => wireInst.WireInstructionId == id));
        }

        // POST: WireInstruction/EditWireInstruction/5    
        [HttpPost]
        public ActionResult EditWireInstruction(int id, WireInstructionModel obj)
        {
            try
            {
                _logger.Info("Attempting to edit wire instruction. " + obj);
                WireInstructionRepository wireInstRepo = new WireInstructionRepository();
                if(wireInstRepo.UpdateWireInstruction(obj))
                {
                    ViewBag.Message = GlobalRes.WireInstruction_WireInstructionController_EditWireInstruction_UpdatedSuccessfully;
                    _logger.Info(GlobalRes.WireInstruction_WireInstructionController_EditWireInstruction_UpdatedSuccessfully+ obj);
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

        // GET: WireInstruction/DeleteWireInstruction/5 
        [HttpGet]
        public ActionResult DeleteWireInstruction(int id)
        {
            try
            {
                _logger.Info("Attempting to delete wire instruction. [Wire Instruction Id=" + id+"]");
                WireInstructionRepository wireInstRepo = new WireInstructionRepository();
                if (wireInstRepo.DeleteWireInstruction(id))
                {
                    ViewBag.AlertMsg = GlobalRes.WireInstruction_WireInstructionController_DeleteWireInstruction_DeletedSuccessfully;
                    _logger.Info("Wire Instruction deleted successfully. [Wire Instruction Id="+id+"]");
                }
                return RedirectToAction("GetWireInstructions");
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message);
                return View();
            }
        }
    }
}
