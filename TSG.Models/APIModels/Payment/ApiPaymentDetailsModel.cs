using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.Payment
{
    public class ApiPaymentDetailsResponse : StandartResponse
    {
        public ApiPaymentDetailsModel PaymentDetails { get; set; }
    }

    public class ApiPaymentDetailsModel
    {
        /// <summary>
        /// Payment GUID
        /// </summary>
        public string PaymentId { get; set; }
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
        /// Payment amount text
        /// </summary>
        public string AmountTextBare { get; set; }
        /// <summary>
        /// Payment amount text SWIFT
        /// </summary>
        public string AmountTextBareSWIFT { get; set; }
        /// <summary>
        /// payment amount text with currency code
        /// </summary>
        public string AmountTextWithCurrencyCode { get; set; }
        /// <summary>
        /// Application Id
        /// </summary>
        public int ApplicationId { get; set; }
        /// <summary>
        /// Approved By
        /// </summary>
        public string ApprovedBy { get; set; }
        /// <summary>
        /// Approved By Full Name
        /// </summary>
        public string ApprovedByFullName { get; set; }
        /// <summary>
        /// Approved Time
        /// </summary>
        public string ApprovedTime { get; set; }
        /// <summary>
        /// Bank Id
        /// </summary>
        public string BankId { get; set; }
        /// <summary>
        /// Bank Operation Code
        /// </summary>
        public string BankOperationCode { get; set; }
        /// <summary>
        /// Beneficiary ABN
        /// </summary>
        public string BeneficiaryABN { get; set; }
        /// <summary>
        /// Beneficiary ACN
        /// </summary>
        public string BeneficiaryACN { get; set; }
        /// <summary>
        /// Beneficiary ARBN
        /// </summary>
        public string BeneficiaryARBN { get; set; }
        /// <summary>
        /// Beneficiary account number
        /// </summary>
        public string BeneficiaryAccountNumber { get; set; }
        /// <summary>
        /// Beneficiary Account Number Prefix
        /// </summary>
        public string BeneficiaryAccountNumberPrefix { get; set; }
        /// <summary>
        /// Beneficiary Account Number Suffix
        /// </summary>
        public string BeneficiaryAccountNumberSuffix { get; set; }
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
        /// Beneficiary Address 3
        /// </summary>
        public string BeneficiaryAddress3 { get; set; }
        /// <summary>
        /// Beneficiary Bank Address 1
        /// </summary>
        public string BeneficiaryBankAddress1 { get; set; }
        /// <summary>
        /// Beneficiary Bank Address 2
        /// </summary>
        public string BeneficiaryBankAddress2 { get; set; }
        /// <summary>
        /// Beneficiary Bank Address 3
        /// </summary>
        public string BeneficiaryBankAddress3 { get; set; }
        /// <summary>
        /// Beneficiary Bank BIC
        /// </summary>
        public string BeneficiaryBankBIC { get; set; }
        /// <summary>
        /// Beneficiary Bank City
        /// </summary>
        public string BeneficiaryBankCity { get; set; }
        /// <summary>
        /// Beneficiary Bank Code
        /// </summary>
        public string BeneficiaryBankCode { get; set; }
        /// <summary>
        /// Beneficiary Bank Country Code
        /// </summary>
        public string BeneficiaryBankCountryCode { get; set; }
        /// <summary>
        /// Beneficiary Bank Country Name
        /// </summary>
        public string BeneficiaryBankCountryName { get; set; }
        /// <summary>
        /// Beneficiary Bank Name
        /// </summary>
        public string BeneficiaryBankName { get; set; }
        /// <summary>
        /// Beneficiary Bank National Code
        /// </summary>
        public string BeneficiaryBankNationalCode { get; set; }
        /// <summary>
        /// Beneficiary Bank National Code Type
        /// </summary>
        public string BeneficiaryBankNationalCodeType { get; set; }
        /// <summary>
        /// Beneficiary Bank National Code Type Name
        /// </summary>
        public string BeneficiaryBankNationalCodeTypeName { get; set; }
        /// <summary>
        /// Beneficiary Bank Postal Code
        /// </summary>
        public string BeneficiaryBankPostalCode { get; set; }
        /// <summary>
        /// Beneficiary Bank State Or Province
        /// </summary>
        public string BeneficiaryBankStateOrProvince { get; set; }
        /// <summary>
        /// Beneficiary Bank State Or Province Text
        /// </summary>
        public string BeneficiaryBankStateOrProvinceText { get; set; }
        /// <summary>
        /// Beneficiary Bank Street Address 1
        /// </summary>
        public string BeneficiaryBankStreetAddress1 { get; set; }
        /// <summary>
        /// Beneficiary Bank Street Address 2
        /// </summary>
        public string BeneficiaryBankStreetAddress2 { get; set; }
        /// <summary>
        /// Beneficiary Branch BIC
        /// </summary>
        public string BeneficiaryBranchBIC { get; set; }
        /// <summary>
        /// Beneficiary Branch City
        /// </summary>
        public string BeneficiaryBranchCity { get; set; }
        /// <summary>
        /// Beneficiary Branch Code
        /// </summary>
        public string BeneficiaryBranchCode { get; set; }
        /// <summary>
        /// Beneficiary Branch Country Code
        /// </summary>
        public string BeneficiaryBranchCountryCode { get; set; }
        /// <summary>
        /// Beneficiary Branch Country Name
        /// </summary>
        public string BeneficiaryBranchCountryName { get; set; }
        /// <summary>
        /// Beneficiary Branch Id
        /// </summary>
        public string BeneficiaryBranchId { get; set; }
        /// <summary>
        /// Beneficiary Branch Name
        /// </summary>
        public string BeneficiaryBranchName { get; set; }
        /// <summary>
        /// Beneficiary Branch National Code
        /// </summary>
        public string BeneficiaryBranchNationalCode { get; set; }
        /// <summary>
        /// Beneficiary Branch National Code Type
        /// </summary>
        public string BeneficiaryBranchNationalCodeType { get; set; }
        /// <summary>
        /// Beneficiary Branch National Code Type Name
        /// </summary>
        public string BeneficiaryBranchNationalCodeTypeName { get; set; }
        /// <summary>
        /// Beneficiary Branch Postal Code
        /// </summary>
        public string BeneficiaryBranchPostalCode { get; set; }
        /// <summary>
        /// Beneficiary Branch State Or Province
        /// </summary>
        public string BeneficiaryBranchStateOrProvince { get; set; }
        /// <summary>
        /// Beneficiary Branch State Or Province Text
        /// </summary>
        public string BeneficiaryBranchStateOrProvinceText { get; set; }
        /// <summary>
        /// Beneficiary Branch Street Address 1
        /// </summary>
        public string BeneficiaryBranchStreetAddress1 { get; set; }
        /// <summary>
        /// Beneficiary Branch Street Address 2
        /// </summary>
        public string BeneficiaryBranchStreetAddress2 { get; set; }
        /// <summary>
        /// Beneficiary Business Structure Type Code
        /// </summary>
        public string BeneficiaryBusinessStructureTypeCode { get; set; }
        /// <summary>
        /// Beneficiary Business Structure Type Name
        /// </summary>
        public string BeneficiaryBusinessStructureTypeName { get; set; }
        /// <summary>
        /// Beneficiary Cell Phone
        /// </summary>
        public string BeneficiaryCellPhone { get; set; }
        /// <summary>
        /// Beneficiary city
        /// </summary>
        public string BeneficiaryCity { get; set; }
        /// <summary>
        /// Beneficiary Company Registration Country Code
        /// </summary>
        public string BeneficiaryCompanyRegistrationCountryCode { get; set; }
        /// <summary>
        /// Beneficiary Company Registration Country Name
        /// </summary>
        public string BeneficiaryCompanyRegistrationCountryName { get; set; }        
        /// <summary>
        /// Beneficiary Company Registration Number
        /// </summary>
        public string BeneficiaryCompanyRegistrationNumber { get; set; }
        /// <summary>
        /// Beneficiary Country Code
        /// </summary>
        public string BeneficiaryCountryCode { get; set; }
        /// <summary>
        /// Beneficiary Country Name
        /// </summary>
        public string BeneficiaryCountryName { get; set; }
        /// <summary>
        /// Beneficiary Country Of Birth Code
        /// </summary>
        public string BeneficiaryCountryOfBirthCode { get; set; }
        /// <summary>
        /// Beneficiary Country OfBirth Name
        /// </summary>
        public string BeneficiaryCountryOfBirthName { get; set; }
        /// <summary>
        /// Beneficiary DateOf Birth
        /// </summary>
        public string BeneficiaryDateOfBirth { get; set; }
        /// <summary>
        /// Beneficiary Email
        /// </summary>
        public string BeneficiaryEmail { get; set; }
        /// <summary>
        /// Beneficiary First Name
        /// </summary>
        public string BeneficiaryFirstName { get; set; }
        /// <summary>
        /// Beneficiary Identification Country Code
        /// </summary>
        public string BeneficiaryIdentificationCountryCode { get; set; }
        /// <summary>
        /// Beneficiary Identification Country Name
        /// </summary>
        public string BeneficiaryIdentificationCountryName { get; set; }
        /// <summary>
        /// Beneficiary Identification Number
        /// </summary>
        public string BeneficiaryIdentificationNumber { get; set; }
        /// <summary>
        /// Beneficiary Identification Type Id
        /// </summary>
        public int BeneficiaryIdentificationTypeId { get; set; }
        /// <summary>
        /// Beneficiary Identification Type Name
        /// </summary>
        public string BeneficiaryIdentificationTypeName { get; set; }
        /// <summary>
        /// Beneficiary Info Line1
        /// </summary>
        public string BeneficiaryInfoLine1 { get; set; }
        /// <summary>
        /// Beneficiary Info Line 2
        /// </summary>
        public string BeneficiaryInfoLine2 { get; set; }
        /// <summary>
        /// Beneficiary Info Line 3
        /// </summary>
        public string BeneficiaryInfoLine3 { get; set; }
        /// <summary>
        /// Beneficiary Info Line 3
        /// </summary>
        public string BeneficiaryInfoLine4 { get; set; }
        /// <summary>
        /// Beneficiary Last Name
        /// </summary>
        public string BeneficiaryLastName { get; set; }
        /// <summary>
        /// Beneficiary Middle Name
        /// </summary>
        public string BeneficiaryMiddleName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryName { get; set; }
        /// <summary>
        /// Beneficiary Occupation Code
        /// </summary>
        public string BeneficiaryOccupationCode { get; set; }
        /// <summary>
        /// Beneficiary Occupation Description
        /// </summary>
        public string BeneficiaryOccupationDescription { get; set; }
        /// <summary>
        /// Beneficiary Occupation Type Id
        /// </summary>
        public int BeneficiaryOccupationTypeId { get; set; }
        /// <summary>
        /// Beneficiary Occupation Type Name
        /// </summary>
        public string BeneficiaryOccupationTypeName { get; set; }
        /// <summary>
        /// Beneficiary Postal Code
        /// </summary>
        public string BeneficiaryPostalCode { get; set; }
        /// <summary>
        /// Beneficiary State Or Province
        /// </summary>
        public string BeneficiaryStateOrProvince { get; set; }
        /// <summary>
        /// Beneficiary State Or Province Text
        /// </summary>
        public string BeneficiaryStateOrProvinceText { get; set; }
        /// <summary>
        /// Beneficiary Street Address1
        /// </summary>
        public string BeneficiaryStreetAddress1 { get; set; }
        /// <summary>
        /// Beneficiary Street Address2
        /// </summary>
        public string BeneficiaryStreetAddress2 { get; set; }
        /// <summary>
        /// Beneficiary Tax Id
        /// </summary>
        public string BeneficiaryTaxId { get; set; }
        /// <summary>
        /// Beneficiary Type Id
        /// </summary>
        public int BeneficiaryTypeId { get; set; }
        /// <summary>
        /// Beneficiary Type Name
        /// </summary>
        public string BeneficiaryTypeName { get; set; }
        /// <summary>
        /// Charge Detail
        /// </summary>
        public string ChargeDetail { get; set; }
        /// <summary>
        /// Created By User Id
        /// </summary>
        public string CreatedBy { get; set; }
        /// <summary>
        /// Created By Full Name
        /// </summary>
        public string CreatedByFullName { get; set; }
        /// <summary>
        /// Created Time
        /// </summary>
        public string CreatedTime { get; set; }       
        /// <summary>
        /// Deleted By
        /// </summary>
        public string DeletedBy { get; set; }
        /// <summary>
        /// Deleted By Full Name
        /// </summary>
        public string DeletedByFullName { get; set; }
        /// <summary>
        /// Deleted Time
        /// </summary>
        public string DeletedTime { get; set; }
        /// <summary>
        /// Destination Country Code
        /// </summary>
        public string DestinationCountryCode { get; set; }
        /// <summary>
        /// Destination Country Name
        /// </summary>
        public string DestinationCountryName { get; set; }
        /// <summary>
        /// FX Cover Deal Execution Id
        /// </summary>
        public string FXCoverDealExecutionId { get; set; }
        /// <summary>
        /// FX Cover Deal Trade Id
        /// </summary>
        public string FXCoverDealTradeId { get; set; }
        /// <summary>
        /// FX Deal Id
        /// </summary>
        public string FXDealId { get; set; }
        /// <summary>
        /// FX Deal Reference
        /// </summary>
        public string FXDealReference { get; set; }
        /// <summary>
        /// FX Sell Amount Equivalent
        /// </summary>
        public decimal FXSellAmountEquivalent { get; set; }
        /// <summary>
        /// FX Sell Amount Equivalent CurrencyCode
        /// </summary>
        public string FXSellAmountEquivalentCurrencyCode { get; set; }
        /// <summary>
        /// FX Sell Amount Equivalent Currency Scale
        /// </summary>
        public int FXSellAmountEquivalentCurrencyScale { get; set; }
        /// <summary>
        /// FX Sell Amount Equivalent Text Bare
        /// </summary>
        public string FXSellAmountEquivalentTextBare { get; set; }
        /// <summary>
        /// FX Sell Amount Equivalent Text Bare SWIFT
        /// </summary>
        public string FXSellAmountEquivalentTextBareSWIFT { get; set; }
        /// <summary>
        /// FX Sell Amount Equivalent Text With Currency Code
        /// </summary>
        public string FXSellAmountEquivalentTextWithCurrencyCode { get; set; }
        /// <summary>
        /// Fee Amount
        /// </summary>
        public decimal FeeAmount { get; set; }
        /// <summary>
        /// Fee Amount Currency Code
        /// </summary>
        public string FeeAmountCurrencyCode { get; set; }
        /// <summary>
        /// Fee Amount Currency Scale
        /// </summary>
        public int FeeAmountCurrencyScale { get; set; }
        /// <summary>
        /// Fee Amount Text Bare
        /// </summary>
        public string FeeAmountTextBare { get; set; }
        /// <summary>
        /// Fee Amount Text With Currency Code
        /// </summary>
        public string FeeAmountTextWithCurrencyCode { get; set; }
        /// <summary>
        /// Initiating Institution ABN
        /// </summary>
        public string InitiatingInstitutionABN { get; set; }
        /// <summary>
        /// Initiating Institution ACN
        /// </summary>
        public string InitiatingInstitutionACN { get; set; }
        /// <summary>
        /// Initiating Institution ARBN
        /// </summary>
        public string InitiatingInstitutionARBN { get; set; }
        /// <summary>
        /// Initiating Institution BIC
        /// </summary>
        public string InitiatingInstitutionBIC { get; set; }
        /// <summary>
        /// Initiating Institution City
        /// </summary>
        public string InitiatingInstitutionCity { get; set; }
        /// <summary>
        /// Initiating Institution Country Code
        /// </summary>
        public string InitiatingInstitutionCountryCode { get; set; }
        /// <summary>
        /// Initiating Institution Country Name
        /// </summary>
        public string InitiatingInstitutionCountryName { get; set; }
        /// <summary>
        /// Initiating Institution Name
        /// </summary>
        public string InitiatingInstitutionName { get; set; }
        /// <summary>
        /// Initiating Institution National Code
        /// </summary>
        public string InitiatingInstitutionNationalCode { get; set; }
        /// <summary>
        /// Initiating Institution National Code Type
        /// </summary>
        public string InitiatingInstitutionNationalCodeType { get; set; }
        /// <summary>
        /// Initiating Institution National Code Type Name
        /// </summary>
        public string InitiatingInstitutionNationalCodeTypeName { get; set; }
        /// <summary>
        /// Initiating Institution Postal Code
        /// </summary>
        public string InitiatingInstitutionPostalCode { get; set; }
        /// <summary>
        /// InitiatingInstitutionSameAsOrderingInstitution
        /// </summary>
        public bool InitiatingInstitutionSameAsOrderingInstitution { get; set; }
        /// <summary>
        /// Initiating Institution State Or Province
        /// </summary>
        public string InitiatingInstitutionStateOrProvince { get; set; }
        /// <summary>
        /// Initiating Institution State Or Province Text
        /// </summary>
        public string InitiatingInstitutionStateOrProvinceText { get; set; }
        /// <summary>
        /// Initiating Institution Street Address 1
        /// </summary>
        public string InitiatingInstitutionStreetAddress1 { get; set; }
        /// <summary>
        /// Initiating Institution Street Address 2
        /// </summary>
        public string InitiatingInstitutionStreetAddress2 { get; set; }
        /// <summary>
        /// Intermediary BankAddress 1
        /// </summary>
        public string IntermediaryBankAddress1 { get; set; }
        /// <summary>
        /// Intermediary Bank Address 2
        /// </summary>
        public string IntermediaryBankAddress2 { get; set; }
        /// <summary>
        /// Intermediary Bank Address 3
        /// </summary>
        public string IntermediaryBankAddress3 { get; set; }
        /// <summary>
        /// Intermediary Bank BIC
        /// </summary>
        public string IntermediaryBankBIC { get; set; }
        /// <summary>
        /// Intermediary Bank City
        /// </summary>
        public string IntermediaryBankCity { get; set; }
        /// <summary>
        /// Intermediary Bank Country Code
        /// </summary>
        public string IntermediaryBankCountryCode { get; set; }
        /// <summary>
        /// Intermediary Bank Country Name
        /// </summary>
        public string IntermediaryBankCountryName { get; set; }
        /// <summary>
        /// Intermediary Bank Name
        /// </summary>
        public string IntermediaryBankName { get; set; }
        /// <summary>
        /// Intermediary Bank National Code
        /// </summary>
        public string IntermediaryBankNationalCode { get; set; }
        /// <summary>
        /// Intermediary Bank National CodeType
        /// </summary>
        public string IntermediaryBankNationalCodeType { get; set; }
        /// <summary>
        /// Intermediary Bank National Code Type Name
        /// </summary>
        public string IntermediaryBankNationalCodeTypeName { get; set; }
        /// <summary>
        /// Intermediary Bank Postal Code
        /// </summary>
        public string IntermediaryBankPostalCode { get; set; }
        /// <summary>
        /// Intermediary Bank State Or Province
        /// </summary>
        public string IntermediaryBankStateOrProvince { get; set; }
        /// <summary>
        /// Intermediary Bank State Or Province Text
        /// </summary>
        public string IntermediaryBankStateOrProvinceText { get; set; }
        /// <summary>
        /// Intermediary Bank Street Address 1
        /// </summary>
        public string IntermediaryBankStreetAddress1 { get; set; }
        /// <summary>
        /// Intermediary Bank Street Address 2
        /// </summary>
        public string IntermediaryBankStreetAddress2 { get; set; }
        /// <summary>
        /// Is Deleted
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// Is Downloaded
        /// </summary>
        public bool IsDownloaded { get; set; }
        /// <summary>
        /// Is Reportable
        /// </summary>
        public bool IsReportable { get; set; }
        /// <summary>
        /// Is Transmitted
        /// </summary>
        public bool IsTransmitted { get; set; }
        /// <summary>
        /// Number Of Prior Customer Payments To Same Beneficiary Account
        /// </summary>
        public int NumberOfPriorCustomerPaymentsToSameBeneficiaryAccount { get; set; }
        /// <summary>
        /// OrderingCustomerAccountNumber
        /// </summary>
        public string OrderingCustomerAccountNumber { get; set; }
        /// <summary>
        /// Ordering Customer Address1
        /// </summary>
        public string OrderingCustomerAddress1 { get; set; }
        /// <summary>
        /// Ordering Customer Address 2
        /// </summary>
        public string OrderingCustomerAddress2 { get; set; }
        /// <summary>
        /// Ordering Customer Address 3
        /// </summary>
        public string OrderingCustomerAddress3 { get; set; }
        /// <summary>
        /// Ordering Customer Bank Address 1
        /// </summary>
        public string OrderingCustomerBankAddress1 { get; set; }
        /// <summary>
        /// Ordering Customer Bank Address 2
        /// </summary>
        public string OrderingCustomerBankAddress2 { get; set; }
        /// <summary>
        /// Ordering Customer Bank Address 3
        /// </summary>
        public string OrderingCustomerBankAddress3 { get; set; }
        /// <summary>
        /// Ordering Customer Bank BIC
        /// </summary>
        public string OrderingCustomerBankBIC { get; set; }
        /// <summary>
        /// Ordering Customer Bank City
        /// </summary>
        public string OrderingCustomerBankCity { get; set; }
        /// <summary>
        /// Ordering Customer Bank Country Code
        /// </summary>
        public string OrderingCustomerBankCountryCode { get; set; }
        /// <summary>
        /// Ordering Customer Bank Country Name
        /// </summary>
        public string OrderingCustomerBankCountryName { get; set; }
        /// <summary>
        /// Ordering Customer Bank National Code
        /// </summary>
        public string OrderingCustomerBankNationalCode { get; set; }
        /// <summary>
        /// Ordering Customer Bank National Code Type
        /// </summary>
        public string OrderingCustomerBankNationalCodeType { get; set; }
        /// <summary>
        /// Ordering Customer Bank National Code Type Name
        /// </summary>
        public string OrderingCustomerBankNationalCodeTypeName { get; set; }
        /// <summary>
        /// Ordering Customer Bank Postal Code
        /// </summary>
        public string OrderingCustomerBankPostalCode { get; set; }
        /// <summary>
        /// Ordering Customer Bank State Or Province
        /// </summary>
        public string OrderingCustomerBankStateOrProvince { get; set; }
        /// <summary>
        /// Ordering Customer Bank State Or Province Text
        /// </summary>
        public string OrderingCustomerBankStateOrProvinceText { get; set; }
        /// <summary>
        /// Ordering Customer Bank Street Address 1
        /// </summary>
        public string OrderingCustomerBankStreetAddress1 { get; set; }
        /// <summary>
        /// Ordering Customer Bank Street Address 2
        /// </summary>
        public string OrderingCustomerBankStreetAddress2 { get; set; }
        /// <summary>
        /// Ordering Customer Branch Code
        /// </summary>
        public string OrderingCustomerBranchCode { get; set; }
        /// <summary>
        /// Ordering Customer City
        /// </summary>
        public string OrderingCustomerCity { get; set; }
        /// <summary>
        /// Ordering Customer Company Registration Country Code
        /// </summary>
        public string OrderingCustomerCompanyRegistrationCountryCode { get; set; }
        /// <summary>
        /// Ordering Customer Company Registration Country Name
        /// </summary>
        public string OrderingCustomerCompanyRegistrationCountryName { get; set; }
        /// <summary>
        /// Ordering Customer Company Registration Number
        /// </summary>
        public string OrderingCustomerCompanyRegistrationNumber { get; set; }
        /// <summary>
        /// Ordering Customer Country Code
        /// </summary>
        public string OrderingCustomerCountryCode { get; set; }
        /// <summary>
        /// Ordering Customer Country Name
        /// </summary>
        public string OrderingCustomerCountryName { get; set; }
        /// <summary>
        /// Ordering Customer Country Of Birth Code
        /// </summary>
        public string OrderingCustomerCountryOfBirthCode { get; set; }
        /// <summary>
        /// Ordering Customer Country Of Birth Name
        /// </summary>
        public string OrderingCustomerCountryOfBirthName { get; set; }
        /// <summary>
        /// Ordering Customer Date Of Birth
        /// </summary>
        public string OrderingCustomerDateOfBirth { get; set; }
        /// <summary>
        /// Ordering Customer Email
        /// </summary>
        public string OrderingCustomerEmail { get; set; }
        /// <summary>
        /// Ordering Customer First Name
        /// </summary>
        public string OrderingCustomerFirstName { get; set; }
        /// <summary>
        /// Ordering Customer Id
        /// </summary>
        public string OrderingCustomerId { get; set; }
        /// <summary>
        /// Ordering Customer Identification Country Code
        /// </summary>
        public string OrderingCustomerIdentificationCountryCode { get; set; }
        /// <summary>
        /// Ordering Customer Identification Country Name
        /// </summary>
        public string OrderingCustomerIdentificationCountryName { get; set; }
        /// <summary>
        /// Ordering Customer Identification Number
        /// </summary>
        public string OrderingCustomerIdentificationNumber { get; set; }
        /// <summary>
        /// Ordering Customer Identification Type Id
        /// </summary>
        public int OrderingCustomerIdentificationTypeId { get; set; }
        /// <summary>
        /// Ordering Customer Identification Type Name
        /// </summary>
        public string OrderingCustomerIdentificationTypeName { get; set; }
        /// <summary>
        /// Ordering Customer Last Name
        /// </summary>
        public string OrderingCustomerLastName { get; set; }
        /// <summary>
        /// Ordering Customer Middle Name
        /// </summary>
        public string OrderingCustomerMiddleName { get; set; }
        /// <summary>
        /// Ordering Customer Name
        /// </summary>
        public string OrderingCustomerName { get; set; }
        /// <summary>
        /// Ordering Customer Phone
        /// </summary>
        public string OrderingCustomerPhone { get; set; }
        /// <summary>
        /// Ordering Customer Postal Code
        /// </summary>
        public string OrderingCustomerPostalCode { get; set; }
        /// <summary>
        /// Ordering Customer State Or Province
        /// </summary>
        public string OrderingCustomerStateOrProvince { get; set; }
        /// <summary>
        /// Ordering Customer State Or Province Text
        /// </summary>
        public string OrderingCustomerStateOrProvinceText { get; set; }
        /// <summary>
        /// Ordering Customer Street Address 1
        /// </summary>
        public string OrderingCustomerStreetAddress1 { get; set; }
        /// <summary>
        /// Ordering Customer Street Address 2
        /// </summary>
        public string OrderingCustomerStreetAddress2 { get; set; }
        /// <summary>
        /// Ordering Customer Type Id
        /// </summary>
        public int OrderingCustomerTypeId { get; set; }
        /// <summary>
        /// Ordering Customer Type Name
        /// </summary>
        public string OrderingCustomerTypeName { get; set; }
        /// <summary>
        /// Other Reference
        /// </summary>
        public string OtherReference { get; set; }
        /// <summary>
        /// Payment Reference
        /// </summary>
        public string PaymentReference { get; set; }
        /// <summary>
        /// Payment Sequence Number
        /// </summary>
        public int PaymentSequenceNumber { get; set; }
        /// <summary>
        /// Payment Source
        /// </summary>
        public string PaymentSource { get; set; }
        /// <summary>
        /// Payment Source Id
        /// </summary>
        public int PaymentSourceId { get; set; }
        /// <summary>
        /// Payment Status Type Id
        /// </summary>
        public int PaymentStatusTypeId { get; set; }
        /// <summary>
        /// Payment Status Type Name
        /// </summary>
        public string PaymentStatusTypeName { get; set; }
        /// <summary>
        /// Payment Value Type
        /// </summary>
        public string PaymentValueType { get; set; }
        /// <summary>
        /// Processing Branch Code
        /// </summary>
        public string ProcessingBranchCode { get; set; }
        /// <summary>
        /// Processing Branch Id
        /// </summary>
        public string ProcessingBranchId { get; set; }
        /// <summary>
        /// Processing Branch Name
        /// </summary>
        public string ProcessingBranchName { get; set; }
        /// <summary>
        /// Processing Branch Phone
        /// </summary>
        public string ProcessingBranchPhone { get; set; }
        /// <summary>
        /// Reason For Payment
        /// </summary>
        public string ReasonForPayment { get; set; }
        /// <summary>
        /// Reason For Payment Code
        /// </summary>
        public string ReasonForPaymentCode { get; set; }
        /// <summary>
        /// Reason For Payment Code Name
        /// </summary>
        public string ReasonForPaymentCodeName { get; set; }
        /// <summary>
        /// Receiver BIC
        /// </summary>
        public string ReceiverBIC { get; set; }
        /// <summary>
        /// Receiving Institution BIC
        /// </summary>
        public string ReceivingInstitutionBIC { get; set; }
        /// <summary>
        /// Receiving Institution City
        /// </summary>
        public string ReceivingInstitutionCity { get; set; }
        /// <summary>
        /// Receiving Institution Country Code
        /// </summary>
        public string ReceivingInstitutionCountryCode { get; set; }
        /// <summary>
        /// Receiving Institution Country Name
        /// </summary>
        public string ReceivingInstitutionCountryName { get; set; }
        /// <summary>
        /// Receiving Institution Name
        /// </summary>
        public string ReceivingInstitutionName { get; set; }
        /// <summary>
        /// Receiving Institution National Code
        /// </summary>
        public string ReceivingInstitutionNationalCode { get; set; }
        /// <summary>
        /// Receiving Institution National Code Type
        /// </summary>
        public string ReceivingInstitutionNationalCodeType { get; set; }
        /// <summary>
        /// Receiving Institution National Code Type Name
        /// </summary>
        public string ReceivingInstitutionNationalCodeTypeName { get; set; }
        /// <summary>
        /// Receiving Institution Postal Code
        /// </summary>
        public string ReceivingInstitutionPostalCode { get; set; }
        /// <summary>
        /// Receiving Institution State Or Province
        /// </summary>
        public string ReceivingInstitutionStateOrProvince { get; set; }
        /// <summary>
        /// Receiving Institution State Or Province Text
        /// </summary>
        public string ReceivingInstitutionStateOrProvinceText { get; set; }
        /// <summary>
        /// Receiving Institution Street Address 1
        /// </summary>
        public string ReceivingInstitutionStreetAddress1 { get; set; }
        /// <summary>
        /// Receiving Institution Street Address 2
        /// </summary>
        public string ReceivingInstitutionStreetAddress2 { get; set; }
        /// <summary>
        /// Released By
        /// </summary>
        public string ReleasedBy { get; set; }
        /// <summary>
        /// Released By Full Name
        /// </summary>
        public string ReleasedByFullName { get; set; }
        /// <summary>
        /// Released Time
        /// </summary>
        public string ReleasedTime { get; set; }
        /// <summary>
        /// Sender To Receiver Line 1
        /// </summary>
        public string SenderToReceiverInfo1 { get; set; }
        /// <summary>
        /// Sender To Receiver Line 2
        /// </summary>
        public string SenderToReceiverInfo2 { get; set; }
        /// <summary>
        /// Sender To Receiver Line 3
        /// </summary>
        public string SenderToReceiverInfo3 { get; set; }
        /// <summary>
        /// Sender To Receiver Line 4
        /// </summary>
        public string SenderToReceiverInfo4 { get; set; }
        /// <summary>
        /// Sender To Receiver Line 5
        /// </summary>
        public string SenderToReceiverInfo5 { get; set; }
        /// <summary>
        /// Sender To Receiver Line 6
        /// </summary>
        public string SenderToReceiverInfo6 { get; set; }
        /// <summary>
        /// Sending Institution ABN
        /// </summary>
        public string SendingInstitutionABN { get; set; }
        /// <summary>
        /// Sending Institution ACN
        /// </summary>
        public string SendingInstitutionACN { get; set; }
        /// <summary>
        /// Sending Institution ARBN
        /// </summary>
        public string SendingInstitutionARBN { get; set; }
        /// <summary>
        /// Sending Institution BIC
        /// </summary>
        public string SendingInstitutionBIC { get; set; }
        /// <summary>
        /// Sending Institution City
        /// </summary>
        public string SendingInstitutionCity { get; set; }
        /// <summary>
        /// SendingInstitution Country Code
        /// </summary>
        public string SendingInstitutionCountryCode { get; set; }
        /// <summary>
        /// Sending Institution Country Name
        /// </summary>
        public string SendingInstitutionCountryName { get; set; }
        /// <summary>
        /// Sending Institution Email
        /// </summary>
        public string SendingInstitutionEmail { get; set; }
        /// <summary>
        /// Sending Institution Name
        /// </summary>
        public string SendingInstitutionName { get; set; }
        /// <summary>
        /// Sending Institution National Code
        /// </summary>
        public string SendingInstitutionNationalCode { get; set; }
        /// <summary>
        /// Sending Institution National Code Type
        /// </summary>
        public string SendingInstitutionNationalCodeType { get; set; }
        /// <summary>
        /// Sending Institution National Code Type Name
        /// </summary>
        public string SendingInstitutionNationalCodeTypeName { get; set; }
        /// <summary>
        /// Sending Institution Occupation Code
        /// </summary>
        public string SendingInstitutionOccupationCode { get; set; }
        /// <summary>
        /// Sending Institution Occupation Description
        /// </summary>
        public string SendingInstitutionOccupationDescription { get; set; }
        /// <summary>
        /// Sending Institution Occupation Type Id
        /// </summary>
        public int SendingInstitutionOccupationTypeId { get; set; }
        /// <summary>
        /// Sending Institution Occupation Type Name
        /// </summary>
        public string SendingInstitutionOccupationTypeName { get; set; }
        /// <summary>
        /// Sending Institution Phone
        /// </summary>
        public string SendingInstitutionPhone { get; set; }
        /// <summary>
        /// Sending Institution Postal Code
        /// </summary>
        public string SendingInstitutionPostalCode { get; set; }
        /// <summary>
        /// Sending Institution Same As Ordering Institution
        /// </summary>
        public bool SendingInstitutionSameAsOrderingInstitution { get; set; }
        /// <summary>
        /// Sending Institution State Or Province
        /// </summary>
        public string SendingInstitutionStateOrProvince { get; set; }
        /// <summary>
        /// Sending Institution State Or Province Text
        /// </summary>
        public string SendingInstitutionStateOrProvinceText { get; set; }
        /// <summary>
        /// Sending Institution Street Address 1
        /// </summary>
        public string SendingInstitutionStreetAddress1 { get; set; }
        /// <summary>
        /// Sending Institution Street Address 2
        /// </summary>
        public string SendingInstitutionStreetAddress2 { get; set; }
        /// <summary>
        /// Settlement Message Type Id
        /// </summary>
        public int SettlementMessageTypeId { get; set; }
        /// <summary>
        /// Settlement Message Type Name
        /// </summary>
        public string SettlementMessageTypeName { get; set; }        
        /// <summary>
        /// Submitted By
        /// </summary>
        public string SubmittedBy { get; set; }
        /// <summary>
        /// Submitted By Full Name
        /// </summary>
        public string SubmittedByFullName { get; set; }
        /// <summary>
        /// Submitted Time
        /// </summary>
        public string SubmittedTime { get; set; }
        /// <summary>
        /// Swift UETR
        /// </summary>
        public string SwiftUETR { get; set; }
        /// <summary>
        /// Timestamp
        /// </summary>
        public string Timestamp { get; set; }
        /// <summary>
        /// Submitted Time
        /// </summary>
        public string ValueDate { get; set; }
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
        public string VerifiedTime { get; set; }
        /// <summary>
        /// WKYC Status Type Description
        /// </summary>
        public string WKYCStatusTypeDescription { get; set; }
        /// <summary>
        /// WKYC Status Type Id
        /// </summary>
        public int WKYCStatusTypeId { get; set; }
        /// <summary>
        /// WKYC Status Type Name
        /// </summary>
        public string WKYCStatusTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryExternalReference { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeneficiaryWKYCId { get; set; }
    }
}