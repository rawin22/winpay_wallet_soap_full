using System;
using System.Collections.Generic;
using System.Web.Mvc;


namespace TSG.Models.APIModels.InstantPayment
{
    public class ApiNewInstantPaymentViewModel
    {
        public Guid PaymentId { get; set; }
        public Guid CustomerId { get; set; }
        public string FromCustomer { get; set; }
        public string FromAccount { get; set; }
        public string ToCustomer { get; set; }
        public IList<SelectListItem> PriorUsedAliases { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string Memo { get; set; }
        public Guid ResultPaymentId { get; set; }
        public string ResultPaymentReference { get; set; }
        public string Invoice { get; set; }
        public string ReasonForPayment { get; set; }

        public string Action { get; set; }
        
        public IList<SelectListItem> AvailableCurrencies { get; set; }

        public IList<SelectListItem> AccountAliases { get; set; }
        public IList<ApiInstantPaymentViewModel> LatestPayments { get; set; } = new List<ApiInstantPaymentViewModel>();
        public ApiAccountBalancesListViewModel AccountBalancesModel { get; set; } = new ApiAccountBalancesListViewModel();
    }
}