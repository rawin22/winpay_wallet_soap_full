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

namespace Tsg.UI.Main.Models
{
    public class InstantPaymentSearchViewModel : BaseViewModel
    {        
        public InstantPaymentSearchViewModel()
        {            
            this.Payments = new List<InstantPaymentViewModel>();
            this.AvailableStatus = new List<SelectListItem>();
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

        public IList<InstantPaymentViewModel> Payments { get; set; }
        public IList<SelectListItem> AvailableStatus { get; set; }
        public IList<SelectListItem> AvailableSortby { get; set; }
        public IList<SelectListItem> AvailableSortDirection { get; set; }

        public void PrepareInstantPayments()
        {
            var response = Service.GetLatestInstantPayments(100000000);

            if (!response.ServiceResponse.HasErrors)
            {
                // INFO: eWallet September bug edited --Payment History default is showing oldest transactions first 
                //foreach (InstantPaymentSearchData data in response.Payments.OrderBy(ob => ob.CreatedTime))
                foreach (InstantPaymentSearchData data in response.Payments)
                {
                    InstantPaymentViewModel instantPayment = new InstantPaymentViewModel()
                    {
                        PaymentId = new Guid(data.PaymentId),
                        PaymentReference = data.PaymentReference,
                        Status = data.Status,
                        FromCustomerAlias = data.FromCustomerAlias,
                        ToCustomerAlias = data.ToCustomerAlias,
                        FromCustomerName = data.FromCustomerName,
                        ToCustomerName = data.ToCustomerName,
                        Amount = data.Amount,
                        Currency = data.CCY,
                        ValueDate = Convert.ToDateTime(data.ValueDate),
                        CreatedTime = Convert.ToDateTime(data.CreatedTime),
                        IsIncome = (AppSecurity.CurrentUser != null && AppSecurity.CurrentUser.OrganisationName != null && !AppSecurity.CurrentUser.OrganisationName.ToUpper().Equals(data.FromCustomerName.ToUpper()))
                    };
                    this.Payments.Add(instantPayment);                    
                }
            }
        }       

    }


    public enum Status : int
    {
        All = 0,
        Created = 1,
        Posted = 2,
        Voided = 3,
    }

    public enum SortDirection : int
    {
        Ascending = 0,
        Descending = 1,
    }
}