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
    /// <summary>
    /// Payout details view model
    /// </summary>
    public class PayoutDetailsViewModel : BaseViewModel
    {
        ///// <summary>
        ///// Payout details default constructor
        ///// </summary>
        ////public PayoutDetailsViewModel()
        ////{
            
        ////}

        /// <summary>
        /// Payout details constructor
        /// </summary>
        /// <param name="paymentId"></param>
        public PayoutDetailsViewModel(Guid paymentId)
        {
            PaymentId = paymentId;            
        }

        /// <summary>
        /// Payment GUID
        /// </summary>
        public Guid PaymentId { get; set; }
        /// <summary>
        /// Payment amount
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Payment amount currency code
        /// </summary>
        public string AmountCurrencyCode { get; set; }
        /// <summary>
        /// Payment amount currency scale
        /// </summary>
        public int AmountCurrencyScale { get; set; }
        /// <summary>
        /// payment amount text with currency code
        /// </summary>
        public string AmountTextWithCurrencyCode { get; set; }
        /// <summary>
        /// Beneficiary account number
        /// </summary>
        public string BeneficiaryAccountNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryAccountTypeCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryAccountTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryAddress1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryAddress2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryCellPhone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryCity { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryCountryCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryCountryName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryEmail { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryPostalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryStateOrProvince { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryStreetAddress1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryStreetAddress2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PaymentReference { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PaymentStatusTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReasonForPayment { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime ValueDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBankAddress1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBankAddress2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBankAddress3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBankBIC { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBankCity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBankCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBankCountryCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBankCountryName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBankName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBankNationalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBankNationalCodeType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBankNationalCodeTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBankPostalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBankStateOrProvince { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBankStreetAddress1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBankStreetAddress2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBranchName { get; set; }
        /// <summary>
        /// Beneficiary Type Id
        /// </summary>
        public int BeneficiaryTypeId { get; set; }
        /// <summary>
        /// Beneficiary Type Name
        /// </summary>
        public string BeneficiaryTypeName { get; set; }
        /// <summary>
        /// Timestamp
        /// </summary>
        public string Timestamp { get; set; }
        /// <summary>
        /// SubmittedBy
        /// </summary>
        public string SubmittedBy { get; set; }
        /// <summary>
        /// Submitted By Full Name
        /// </summary>
        public string SubmittedByFullName { get; set; }
        /// <summary>
        /// Submitted Time
        /// </summary>
        public DateTime? SubmittedTime { get; set; }
        /// <summary>
        /// Verified By
        /// </summary>
        public string VerifiedBy { get; set; }
        /// <summary>
        /// Verified by Full Name
        /// </summary>
        public string VerifiedByFullName { get; set; }
        /// <summary>
        /// Verified Time
        /// </summary>
        public DateTime? VerifiedTime { get; set; }
        /// <summary>
        /// User mail
        /// </summary>
        public UserMailModel MailModel { get; set; }
        
        /// <summary>
        /// Get payment details by using web service
        /// </summary>
        public void PrepareDetails()
        {
            PaymentGetSingleResponse response = Service.GetPaymentDetails(this.PaymentId);

            if (!response.ServiceResponse.HasErrors)
            {                
                this.PaymentId = new Guid(response.Payment.PaymentId);
                this.PaymentReference = response.Payment.PaymentReference;
                this.Amount = Math.Round(response.Payment.Amount, response.Payment.AmountCurrencyScale);
                this.AmountCurrencyScale = response.Payment.AmountCurrencyScale;
                this.AmountCurrencyCode = response.Payment.AmountCurrencyCode;
                this.AmountCurrencyScale = response.Payment.AmountCurrencyScale;
                this.AmountTextWithCurrencyCode = response.Payment.AmountTextWithCurrencyCode;
                this.BeneficiaryTypeId = response.Payment.BeneficiaryTypeId;
                this.BeneficiaryTypeName = response.Payment.BeneficiaryTypeName;                
                this.BeneficiaryBankCountryName = response.Payment.BeneficiaryBankCountryName;
                this.BeneficiaryName = response.Payment.BeneficiaryName;
                this.BeneficiaryEmail = response.Payment.BeneficiaryEmail;
                
                this.BeneficiaryAccountTypeCode = response.Payment.BeneficiaryAccountTypeCode;
                this.BeneficiaryAccountTypeName = response.Payment.BeneficiaryAccountTypeName;
                this.BeneficiaryAccountNumber = response.Payment.BeneficiaryAccountNumber;
                this.BeneficiaryAddress1 = response.Payment.BeneficiaryAddress1;
                this.BeneficiaryAddress2 = response.Payment.BeneficiaryAddress2;
                this.BeneficiaryCellPhone = response.Payment.BeneficiaryCellPhone;
                this.BeneficiaryCity = response.Payment.BeneficiaryCity;
                this.BeneficiaryCountryCode = response.Payment.BeneficiaryCountryCode;
                this.BeneficiaryCountryName = response.Payment.BeneficiaryCountryName;
               
                this.BeneficiaryBankName = response.Payment.BeneficiaryBankName;
                this.BeneficiaryBankBIC = response.Payment.BeneficiaryBankBIC;
                this.BeneficiaryBankNationalCode = response.Payment.BeneficiaryBankNationalCode;
                this.BeneficiaryBankNationalCodeType = response.Payment.BeneficiaryBankNationalCodeType;
                this.BeneficiaryBankNationalCodeTypeName = response.Payment.BeneficiaryBankNationalCodeTypeName;
                this.BeneficiaryBranchName = response.Payment.BeneficiaryBranchName;
                this.BeneficiaryBankStreetAddress1 = response.Payment.BeneficiaryBankStreetAddress1;
                this.BeneficiaryBankStreetAddress2 = response.Payment.BeneficiaryBankStreetAddress2;
                this.BeneficiaryBankCity = response.Payment.BeneficiaryBankCity;
                this.BeneficiaryBankStateOrProvince = response.Payment.BeneficiaryBankStateOrProvince;
                this.BeneficiaryBankPostalCode = response.Payment.BeneficiaryBankPostalCode;
                this.CreatedTime = Convert.ToDateTime(response.Payment.CreatedTime);
                this.PaymentStatusTypeName = response.Payment.PaymentStatusTypeName;
                this.ReasonForPayment = response.Payment.ReasonForPayment;
                this.ValueDate = Convert.ToDateTime(response.Payment.ValueDate);
                this.Timestamp = response.Payment.Timestamp;
                this.SubmittedBy = response.Payment.SubmittedBy;
                this.SubmittedByFullName = response.Payment.SubmittedByFullName;
                this.SubmittedTime = String.IsNullOrEmpty(response.Payment.SubmittedTime)? (DateTime?) null : Convert.ToDateTime(response.Payment.SubmittedTime);
                this.VerifiedBy = response.Payment.VerifiedBy;
                this.VerifiedByFullName = response.Payment.VerifiedByFullName;
                this.VerifiedTime = String.IsNullOrEmpty(response.Payment.VerifiedTime) ? (DateTime?)null : Convert.ToDateTime(response.Payment.VerifiedTime);
            }
        }

        /// <summary>
        /// Submit pay out
        /// </summary>
        public void Submit()
        {
            var response = Service.SubmitPayment(this.PaymentId, Timestamp);

            if (!response.ServiceResponse.HasErrors)
            {
                PrepareDetails();
            }
        }

        /// <summary>
        /// Delete pay out
        /// </summary>
        public void Delete()
        {
            var response = Service.DeletePayment(this.PaymentId, Timestamp);            
        }
    }
}