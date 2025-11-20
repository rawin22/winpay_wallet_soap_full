using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using TSG.Models.ServiceModels.Shop;
using TSG.ServiceLayer.InstantPayment;

namespace Tsg.UI.Main.Models
{
    public class InstantPaymentReceiveDetailsViewModel : BaseViewModel
    {
        // private readonly IInstantPaymentReceiveMethods _instantPaymentReceiveService;

        //public InstantPaymentReceiveDetailsViewModel(IInstantPaymentReceiveMethods instantPaymentReceiveService)
        //{
        //    _instantPaymentReceiveService = instantPaymentReceiveService;
        //}

        [Display(Name = "Instant Payment Id")]
        public Guid InstantPaymentReceiveId;
        [Display(Name = "Alias")]
        public string Alias;
        [Display(Name = "Currency")]
        public string Currency;
        [Display(Name = "Amount")]
        public decimal Amount;
        [Display(Name = "Invoice")]
        public string Invoice;
        [Display(Name = "Memo")]
        public string Memo;
        [Display(Name = "Name")]
        public string Name;
        [Display(Name = "Address")]
        public string Address;
        [Display(Name = "Email")]
        public string Email;
        [Display(Name = "Shortened Url")]
        public string ShortenedUrl { get; set; }
        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate;
        [Display(Name = "Attached File Name")]
        public string AttachedFileName { get; set; }

        public IList<InstantPaymentViewModel> InstantPayments { get; set; }
        //public void PrepareDetails(Guid instantPaymentReceiveId)
        //{
        //    var receive = _instantPaymentReceiveService.GetAll().Obj.Where(r => r.Id == instantPaymentReceiveId).FirstOrDefault();

        //    if (receive != null)
        //    {
        //        this.InstantPaymentReceiveId = receive.Id;
        //        this.Alias = receive.Alias;
        //        this.Currency = receive.Currency;
        //        this.Amount = receive.Amount;
        //        this.Invoice = receive.Invoice;
        //        this.Memo = receive.Memo;
        //        this.CreatedDate = receive.CreatedDate;
        //    }
        //}
    }
}