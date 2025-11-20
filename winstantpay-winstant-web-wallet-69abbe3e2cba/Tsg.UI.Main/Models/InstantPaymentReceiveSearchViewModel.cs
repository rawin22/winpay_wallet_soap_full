using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.Models.Security;
using TSG.ServiceLayer.InstantPayment;

namespace Tsg.UI.Main.Models
{
    public class InstantPaymentReceiveSearchViewModel : BaseViewModel
    {
        
        public InstantPaymentReceiveSearchViewModel()
        {
            this.InstantPaymentReceives = new List<InstantPaymentReceiveViewModel>();
            this.AvailableSortby = new List<SelectListItem>();
            this.AvailableSortDirection = new List<SelectListItem>();
        }

        [Display(Name = "Payment Reference")]
        public string PaymentReference { get; set; }

        [Display(Name = "Status")]
        public Status Status { get; set; }
        [Display(Name = "From Customer")]

        public string FromCustomer { get; set; }
        [Display(Name = "To Customer")]

        public string ToCustomer { get; set; }
        [Display(Name = "Amount Min.")]
        public decimal AmountMin { get; set; }

        [Display(Name = "Amount Max.")]
        public decimal AmountMax { get; set; }

        [Display(Name = "Value Date Min.")]
        [DataType(DataType.DateTime)]
        public DateTime ValueDateMin { get; set; } = new DateTime(2000, 1, 1);

        [Display(Name = "Value Date Max.")]
        [DataType(DataType.DateTime)]
        public DateTime ValueDateMax { get; set; } = new DateTime(2099, 1, 1);

        [Display(Name = "Sort By")]
        public string SortBy { get; set; }

        [Display(Name = "Sort Direction")]
        public SortDirection SortDirection { get; set; }

        [Display(Name = "Page Size")]
        public int PageSize { get; set; }

        [Display(Name = "Page Index")]
        public int PageIndex { get; set; }

        [Display(Name = "Record Count")]
        public int RecordCount { get; set; }

        [Display(Name = "Total Records")]
        public int TotalRecords { get; set; }

        public IList<InstantPaymentReceiveViewModel> InstantPaymentReceives { get; set; }
        public IList<SelectListItem> AvailableSortby { get; set; }
        public IList<SelectListItem> AvailableSortDirection { get; set; }

        //public void PrepareInstantPaymentReceives()
        //{
        //    var userId = new Guid(AppSecurity.CurrentUser.UserId);
        //    var receives= _instantPaymentReceiveService.GetByUser(userId);

        //    foreach (var receive in receives.Obj)
        //    {
        //        InstantPaymentReceiveViewModel instantPaymentReceive = new InstantPaymentReceiveViewModel()
        //        {
        //            InstantPaymentReceiveId = receive.Id,
        //            Alias = receive.Alias,
        //            Amount = receive.Amount,
        //            Currency = receive.Alias,
        //            CreatedDate = receive.CreatedDate,
        //        };
        //        this.InstantPaymentReceives.Add(instantPaymentReceive);                    
        //    }
            
        //}       

    }
}