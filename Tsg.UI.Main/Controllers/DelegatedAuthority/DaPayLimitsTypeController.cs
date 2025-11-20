using System;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using TSG.Models.DTO.LimitPayment;
using TSG.Models.ServiceModels.LimitPayment;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTypeServiceMethods;

namespace Tsg.UI.Main.Controllers
{
    public class DaPayLimitsTypeController : Controller
    {

        private readonly IDaPayLimitsTypeServiceMethods _paymenListMethodsService;

        public DaPayLimitsTypeController(IDaPayLimitsTypeServiceMethods paymenListMethodsService)
        {
            _paymenListMethodsService = paymenListMethodsService;
        }
        

        // GET: DaPayLimitsTypeDtoes
        public ActionResult Index()
        {
            return View(_paymenListMethodsService.GetAll().Obj);
        }
        
        // GET: DaPayLimitsTypeDtoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DaPayLimitsTypeDtoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DaPayLimitsTypeSo DaPayLimitsTypeSo)
        {
            if (ModelState.IsValid && _paymenListMethodsService.Insert(DaPayLimitsTypeSo).Success)
            {
                return RedirectToAction("Index");
            }
            return View(DaPayLimitsTypeSo);
        }

        // GET: DaPayLimitsTypeDtoes/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var DaPayLimitsTypeSo = _paymenListMethodsService.GetById(id.Value).Obj;
            if (DaPayLimitsTypeSo == null)
            {
                return HttpNotFound();
            }
            return View(DaPayLimitsTypeSo);
        }

        // POST: DaPayLimitsTypeDtoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DaPayLimitsTypeSo DaPayLimitsTypeSo)
        {
            if (ModelState.IsValid && _paymenListMethodsService.Insert(DaPayLimitsTypeSo).Success)
            {
                return RedirectToAction("Index");
            }
            return View(DaPayLimitsTypeSo);
        }

        // GET: DaPayLimitsTypeDtoes/Delete/5
        //public ActionResult Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    var DaPayLimitsTypeSo = _paymenListMethodsService.GetById(id.Value).Obj;
        //    if (DaPayLimitsTypeSo == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(DaPayLimitsTypeSo);
        //}

        //// POST: DaPayLimitsTypeDtoes/Delete/5
        //public ActionResult DeleteConfirmed(Guid id)
        //{
        //    DaPayLimitsTypeDto DaPayLimitsTypeDto = db.DaPayLimitsTypeDtoes.Find(id);
        //    db.DaPayLimitsTypeDtoes.Remove(DaPayLimitsTypeDto);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
    }
}
