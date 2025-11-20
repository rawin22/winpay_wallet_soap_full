using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSG.Models.APIModels.Payment
{
    public class ApiCreatePaymentRequest
    {                
        /// <summary>
        /// 
        /// </summary>
        public string PaymentReference { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BankOperationCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryABN { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryACN { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryARBN { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryAccountNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryAccountNumberPrefix { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryAccountNumberSuffix { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryAccountTypeCode { get; set; }
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
        public string BeneficiaryAddress3 { get; set; }
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
        public string BeneficiaryBranchBIC { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBranchCity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBranchCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBranchCountryCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBranchId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBranchName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBranchNationalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBranchNationalCodeType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBranchPostalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBranchStateOrProvince { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBranchStreetAddress1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBranchStreetAddress2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryBusinessStructureTypeCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "Beneficiary Cell Phone")]
        public string BeneficiaryCellPhone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "Beneficiary City")]
        public string BeneficiaryCity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryCompanyRegistrationCountryCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryCompanyRegistrationNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "Beneficiary Country Code")]
        public string BeneficiaryCountryCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryCountryOfBirthCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Beneficiary Date of Birth"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public string BeneficiaryDateOfBirth { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required, EmailAddress]
        [Display(Name = "Beneficiary Email")]
        public string BeneficiaryEmail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "Beneficiary First Name")]
        public string BeneficiaryFirstName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryIdentificationCountryCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryIdentificationNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int BeneficiaryIdentificationTypeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryInfoLine1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryInfoLine2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryInfoLine3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryInfoLine4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "Beneficiary Last Name")]
        public string BeneficiaryLastName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryMiddleName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryOccupationCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryOccupationDescription { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int BeneficiaryOccupationTypeId { get; set; }
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
        [Required]
        [Display(Name = "Beneficiary Street Address (Line 1)")]
        public string BeneficiaryStreetAddress1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryStreetAddress2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryTaxId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int BeneficiaryTypeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ChargeDetail { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InitiatingInstitutionABN { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InitiatingInstitutionACN { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InitiatingInstitutionARBN { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InitiatingInstitutionBIC { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InitiatingInstitutionCity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InitiatingInstitutionCountryCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InitiatingInstitutionName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InitiatingInstitutionNationalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InitiatingInstitutionNationalCodeType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InitiatingInstitutionPostalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool InitiatingInstitutionSameAsOrderingInstitution { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InitiatingInstitutionStateOrProvince { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InitiatingInstitutionStreetAddress1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InitiatingInstitutionStreetAddress2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IntermediaryBankAddress1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IntermediaryBankAddress2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IntermediaryBankAddress3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IntermediaryBankBIC { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IntermediaryBankCity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IntermediaryBankCountryCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IntermediaryBankName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IntermediaryBankNationalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IntermediaryBankNationalCodeType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IntermediaryBankPostalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IntermediaryBankStateOrProvince { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IntermediaryBankStreetAddress1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IntermediaryBankStreetAddress2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OrderingCustomerBankBIC { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OrderingCustomerBankCity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OrderingCustomerBankCountryCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OrderingCustomerBankName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OrderingCustomerBankNationalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OrderingCustomerBankNationalCodeType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OrderingCustomerBankPostalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OrderingCustomerBankStateOrProvince { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OrderingCustomerBankStreetAddress1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OrderingCustomerBankStreetAddress2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReasonForPayment { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReasonForPaymentCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReceiverBIC { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReceivingInstitutionBIC { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReceivingInstitutionCity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReceivingInstitutionCountryCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReceivingInstitutionName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReceivingInstitutionNationalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReceivingInstitutionNationalCodeType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReceivingInstitutionPostalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReceivingInstitutionStateOrProvince { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReceivingInstitutionStreetAddress1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReceivingInstitutionStreetAddress2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SenderToReceiverLine1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SenderToReceiverLine2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SenderToReceiverLine3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SenderToReceiverLine4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SenderToReceiverLine5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SenderToReceiverLine6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SendingInstitutionABN { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SendingInstitutionACN { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SendingInstitutionARBN { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SendingInstitutionBIC { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SendingInstitutionBusinessStructureTypeCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SendingInstitutionCity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SendingInstitutionCountryCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SendingInstitutionEmail { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SendingInstitutionName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SendingInstitutionNationalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SendingInstitutionNationalCodeType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SendingInstitutionOccupationCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SendingInstitutionOccupationDescription { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SendingInstitutionOccupationTypeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SendingInstitutionPhone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SendingInstitutionPostalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool SendingInstitutionSameAsOrderingInstitution { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SendingInstitutionStateOrProvince { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SendingInstitutionStreetAddress1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SendingInstitutionStreetAddress2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CustomerId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FXDealId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Range(0.01, 100000000000), DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string AmountCurrencyCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CountryCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "Value Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public string ValueDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal FeeAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FeeAmountCurrencyCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PaymentValueType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryExternalReference { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryWKYCId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SwiftUETR { get; set; }
        /// <summary>
        /// Beneficiary Street Name
        /// </summary>
        public string BeneficiaryStreetName { get; set; }
    }
}
