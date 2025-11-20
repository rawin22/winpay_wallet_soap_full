using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using TSG.Models.ServiceModels.Shop;

namespace Tsg.UI.Main.Models
{
    public class InstantPaymentDetailsViewModel : BaseViewModel
    {
        public InstantPaymentDetailsViewModel()
        {
            
        }

        public InstantPaymentDetailsViewModel(Guid paymentId)
        {
            PaymentId = paymentId;            
        }

        public Guid PaymentId { get; set; }
        public string PaymentReference { get; set; }
        public string FromCustomerAlias { get; set; }
        public string ToCustomerAlias { get; set; }
        public string FromCustomerName { get; set; }
        public string FromCustomerId { get; set; }
        public string ToCustomerName { get; set; }
        public string ToCustomerId { get; set; }
        public string PaymentStatus { get; set; }

        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public decimal Amount { get; set; }
        public int AmountCurrencyScale { get; set; }
        public string Currency { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime ValueDate { get; set; }
        public string ProcessingBranchName { get; set; }
        public string ProcessingBranchCode { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? PostedTime { get; set; }
        public string PostedByName { get; set; }
        public bool IsDeleted { get; set; }
        public string BankMemo { get; set; }
        public string Invoice { get; set; }
        public string ReasonForPayment { get; set; }

        public UserMailModel MailModel { get; set; }


        public void PrepareDetails()
        {
            InstantPaymentGetSingleResponse response = Service.GetInstantPaymentDetails(this.PaymentId);

            if (!response.ServiceResponse.HasErrors)
            {                
                this.PaymentId = new Guid(response.Payment.PaymentId);
                this.PaymentReference = response.Payment.PaymentReference;
                this.FromCustomerAlias = response.Payment.FromCustomerAlias;
                this.ToCustomerAlias = response.Payment.ToCustomerAlias;
                this.FromCustomerName = response.Payment.FromCustomerName;
                this.ToCustomerName = response.Payment.ToCustomerName;
                this.FromCustomerId = response.Payment.FromCustomerId;
                this.ToCustomerId = response.Payment.ToCustomerId;
                this.PaymentStatus = response.Payment.PaymentStatus == "Posted"? "Paid": response.Payment.PaymentStatus;
                this.Amount = response.Payment.Amount;
                this.AmountCurrencyScale = response.Payment.AmountCurrencyScale;
                this.Currency = response.Payment.CCY;
                this.ValueDate = Convert.ToDateTime(response.Payment.ValueDate);
                this.ProcessingBranchCode = response.Payment.ProcessingBranchCode;
                this.ProcessingBranchName = response.Payment.ProcessingBranchName;
                this.CreatedTime = Convert.ToDateTime(response.Payment.CreatedTime);
                this.CreatedByName = response.Payment.CreatedByName;
                this.PostedTime = String.IsNullOrEmpty(response.Payment.PostedTime)? (DateTime?) null : Convert.ToDateTime(response.Payment.PostedTime);
                this.PostedByName = response.Payment.PostedByName;
                this.IsDeleted = response.Payment.IsDeleted;
                this.BankMemo = response.Payment.BankMemo;
                this.Invoice = response.Payment.ExternalReference;
                this.ReasonForPayment = response.Payment.ReasonForPayment;
            }
        }

        /// <summary>
        /// Link to order if payment contains it
        /// </summary>
        public virtual ShopOrders ShopOrder { get; set; }
    }
}