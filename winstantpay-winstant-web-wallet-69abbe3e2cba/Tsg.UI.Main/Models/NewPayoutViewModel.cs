using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;


namespace Tsg.UI.Main.Models
{
    /// <summary>
    /// New payout view model
    /// </summary>
    public class NewPayoutViewModel : BaseViewModel
    {

        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///
        /// </summary>
        public NewPayoutViewModel()
        {
            Service = new IgpService(AppSecurity.CurrentUser.UserName, AppSecurity.CurrentUser.Password, AppSecurity.CurrentUser.UserId);
            PrepareValues();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="customerId"></param>
        public NewPayoutViewModel(string customerId )
        {
            this.CustomerId = customerId;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="paymentId"></param>
        public NewPayoutViewModel(Guid? paymentId)
        : this()
        {
            if (paymentId.HasValue)
            {
                PaymentId = paymentId.Value;
                //PrepareDetails();
            }
            else
            {
                //GetLastCustomerAlias();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="ccy"></param>
        /// <param name="amount"></param>
        /// <param name="invoice"></param>
        /// <param name="memo"></param>
        public NewPayoutViewModel(string alias, string ccy, decimal amount, string invoice, string memo)
            : this()
        {
            // PrepareDetails(alias, ccy, amount, invoice, memo);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="ccy"></param>
        /// <param name="amount"></param>
        /// <param name="invoice"></param>
        /// <param name="memo"></param>
        /// <param name="receiveId"></param>
        public NewPayoutViewModel(string alias, string ccy, decimal amount, string invoice, string memo, Guid receiveId)
            : this()
        {
            // PrepareDetails(alias, ccy, amount, invoice, memo, receiveId);
        }


        /// <summary>
        /// Payment ID
        /// </summary>
        public Guid PaymentId { get; set; }

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
        public IList<SelectListItem> AvailableCurrencies { get; set; }
        /// <summary>
        ///
        /// </summary>
        public IList<SelectListItem> AccountAliases { get; set; }
        /// <summary>
        ///
        /// </summary>
        public IList<SelectListItem> BeneficiaryIdentificationTypes { get; set; }
        /// <summary>
        ///
        /// </summary>
        public IList<SelectListItem> BeneficiaryIdentificationCountries { get; set; }
        /// <summary>
        ///
        /// </summary>
        public IList<SelectListItem> BeneficiaryCountriesOfBirth { get; set; }
        /// <summary>
        ///
        /// </summary>
        public IList<SelectListItem> BeneficiaryOccupationTypes { get; set; }
        /// <summary>
        ///
        /// </summary>
        public IList<SelectListItem> BeneficiaryOccupationCodes { get; set; }
        /// <summary>
        ///
        /// </summary>
        public IList<SelectListItem> BeneficiaryAccountTypes{ get; set; }
        /// <summary>
        ///
        /// </summary>
        public IList<SelectListItem> BeneficiaryCountriesResidence { get; set; }
        /// <summary>
        ///
        /// </summary>
        public IList<SelectListItem> NationalCodes { get; set; }
        /// <summary>
        ///
        /// </summary>
        public IList<SelectListItem> CurrencyNationalCodes { get; set; }
        /// <summary>
        ///
        /// </summary>
        public IList<SelectListItem> AmountCurrencies { get; set; }
        /// <summary>
        ///
        /// </summary>
        public IList<SelectListItem> BeneficiaryTypes { get; set; }
        /// <summary>
        ///
        /// </summary>
        public IList<SelectListItem> BeneficiaryBankNationalCodeTypes { get; set; }

        /// <summary>
        ///
        /// </summary>
        public IList<SelectListItem> BeneficiaryBankCountries { get; set; }
        /// <summary>
        ///
        /// </summary>
        public IList<SelectListItem> DestinationCountries { get; set; }
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "[Payment Id=" + PaymentId + "],[Customer Id=" + CustomerId + "],[Value Date=" +
                ValueDate + "],[Beneficiary Name=" + BeneficiaryName + "],[Amount=" + Amount + "],[Currency Code=" + AmountCurrencyCode +
                "],[ReasonForPayment=" + ReasonForPayment + "]";
        }

        /// <summary>
        /// Create payout
        /// </summary>
        /// <param name="paymentId"></param>
        public void Create(out Guid? paymentId)
        {
            var response = Service.CreatePayment(BankOperationCode, BeneficiaryABN, BeneficiaryACN, BeneficiaryARBN, BeneficiaryAccountNumber, BeneficiaryAccountNumberPrefix, BeneficiaryAccountNumberSuffix, BeneficiaryAccountTypeCode, BeneficiaryAddress1, BeneficiaryAddress2, BeneficiaryAddress3, BeneficiaryBankAddress1, BeneficiaryBankAddress2, BeneficiaryBankAddress3, BeneficiaryBankBIC, BeneficiaryBankCity, BeneficiaryBankCode, BeneficiaryBankCountryCode, BeneficiaryBankName, BeneficiaryBankNationalCode, BeneficiaryBankNationalCodeType, BeneficiaryBankPostalCode, BeneficiaryBankStateOrProvince, BeneficiaryBankStreetAddress1, BeneficiaryBankStreetAddress2, BeneficiaryBranchBIC, BeneficiaryBranchCity, BeneficiaryBranchCode, BeneficiaryBranchCountryCode, BeneficiaryBranchId, BeneficiaryBranchName, BeneficiaryBranchNationalCode, BeneficiaryBranchNationalCodeType, BeneficiaryBranchPostalCode, BeneficiaryBranchStateOrProvince, BeneficiaryBranchStreetAddress1, BeneficiaryBranchStreetAddress2, BeneficiaryBusinessStructureTypeCode, BeneficiaryCellPhone, BeneficiaryCity, BeneficiaryCompanyRegistrationCountryCode, BeneficiaryCompanyRegistrationNumber, BeneficiaryCountryCode, BeneficiaryCountryOfBirthCode, BeneficiaryDateOfBirth, BeneficiaryEmail, BeneficiaryFirstName, BeneficiaryIdentificationCountryCode, BeneficiaryIdentificationNumber, BeneficiaryIdentificationTypeId, BeneficiaryInfoLine1, BeneficiaryInfoLine2, BeneficiaryInfoLine3, BeneficiaryInfoLine4, BeneficiaryLastName, BeneficiaryMiddleName, BeneficiaryName, BeneficiaryOccupationCode, BeneficiaryOccupationDescription, BeneficiaryOccupationTypeId, BeneficiaryPostalCode, BeneficiaryStateOrProvince, BeneficiaryStreetAddress1, BeneficiaryStreetAddress2, BeneficiaryTaxId, BeneficiaryTypeId, ChargeDetail, InitiatingInstitutionABN, InitiatingInstitutionACN, InitiatingInstitutionARBN, InitiatingInstitutionBIC, InitiatingInstitutionCity, InitiatingInstitutionCountryCode, InitiatingInstitutionName, InitiatingInstitutionNationalCode, InitiatingInstitutionNationalCodeType, InitiatingInstitutionPostalCode, InitiatingInstitutionSameAsOrderingInstitution, InitiatingInstitutionStateOrProvince, InitiatingInstitutionStreetAddress1, InitiatingInstitutionStreetAddress2, IntermediaryBankAddress1, IntermediaryBankAddress2, IntermediaryBankAddress3, IntermediaryBankBIC, IntermediaryBankCity, IntermediaryBankCountryCode, IntermediaryBankName, IntermediaryBankNationalCode, IntermediaryBankNationalCodeType, IntermediaryBankPostalCode, IntermediaryBankStateOrProvince, IntermediaryBankStreetAddress1, IntermediaryBankStreetAddress2, OrderingCustomerBankBIC, OrderingCustomerBankCity, OrderingCustomerBankCountryCode, OrderingCustomerBankName, OrderingCustomerBankNationalCode, OrderingCustomerBankNationalCodeType, OrderingCustomerBankPostalCode, OrderingCustomerBankStateOrProvince, OrderingCustomerBankStreetAddress1, OrderingCustomerBankStreetAddress2, ReasonForPayment, ReasonForPaymentCode, ReceiverBIC, ReceivingInstitutionBIC, ReceivingInstitutionCity, ReceivingInstitutionCountryCode, ReceivingInstitutionName, ReceivingInstitutionNationalCode, ReceivingInstitutionNationalCodeType, ReceivingInstitutionPostalCode, ReceivingInstitutionStateOrProvince, ReceivingInstitutionStreetAddress1, ReceivingInstitutionStreetAddress2, SenderToReceiverLine1, SenderToReceiverLine2, SenderToReceiverLine3, SenderToReceiverLine4, SenderToReceiverLine5, SenderToReceiverLine6, SendingInstitutionABN, SendingInstitutionACN, SendingInstitutionARBN, SendingInstitutionBIC, SendingInstitutionBusinessStructureTypeCode, SendingInstitutionCity, SendingInstitutionCountryCode, SendingInstitutionEmail, SendingInstitutionName, SendingInstitutionNationalCode, SendingInstitutionNationalCodeType, SendingInstitutionOccupationCode, SendingInstitutionOccupationDescription, SendingInstitutionOccupationTypeId, SendingInstitutionPhone, SendingInstitutionPostalCode, SendingInstitutionSameAsOrderingInstitution, SendingInstitutionStateOrProvince, SendingInstitutionStreetAddress1, SendingInstitutionStreetAddress2, CustomerId, FXDealId, Amount, AmountCurrencyCode, CountryCode, ValueDate, FeeAmount, FeeAmountCurrencyCode, PaymentValueType);

            paymentId = null;

            if (!response.ServiceResponse.HasErrors)
            {
                var model = new InstantPaymentPostResultViewModel(new Guid(response.Payment.PaymentId));
                paymentId = new Guid(response.Payment.PaymentId);

                PaymentId = paymentId.HasValue? paymentId.Value : Guid.Empty;
            }
            else
            {
                this.GetErrorMessages(response.ServiceResponse.Responses);
            }
        }

        /// <summary>
        /// Validate payment
        /// </summary>
        public bool Validate()
        {
            bool isValide = false;
            var response = Service.ValidatePayment(PreparePaymentValidateRequest());

            if (!response.ServiceResponse.HasErrors)
            {
                isValide = true;
            }
            else
            {
                this.GetErrorMessages(response.ServiceResponse.Responses);
            }

            return isValide;
        }

        //public void PrepareDetails()
        //{
        //    try
        //    {
        //        Tsg.Business.Model.TsgGPWebService.InstantPaymentGetSingleResponse response = Service.GetInstantPaymentDetails(this.PaymentId);

        //        if (!response.ServiceResponse.HasErrors)
        //        {
        //            this.PaymentId = new Guid(response.Payment.PaymentId);
        //            this.FromCustomer = response.Payment.FromCustomerAlias;
        //            this.ToCustomer = response.Payment.ToCustomerAlias;
        //            this.Amount = response.Payment.Amount;
        //            this.CurrencyCode = response.Payment.CCY;
        //            this.Memo = response.Payment.BankMemo;
        //        }
        //        else _logger.Error($"{response.ServiceResponse.Responses[0].Message}\n\r{response.ServiceResponse.Responses[0].MessageDetails}");
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.Error(e);
        //    }
        //}



        //public void PrepareDetails(string alias, string ccy, decimal amount, string invoice, string memo)
        //{
        //    try
        //    {
        //        this.ToCustomer = alias;
        //        this.Amount = amount;
        //        this.CurrencyCode = ccy;
        //        this.InstantPay = invoice;
        //        this.Memo = memo;
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.Error(e);
        //    }
        //}

        //public void PrepareDetails(string alias, string ccy, decimal amount, string invoice, string memo, Guid receiveId)
        //{
        //    try
        //    {
        //        this.ToCustomer = alias;
        //        this.Amount = amount;
        //        this.CurrencyCode = ccy;
        //        this.InstantPay = invoice;
        //        this.Memo = memo;
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.Error(e);
        //    }
        //}

        //public void GetLastCustomerAlias()
        //{
        //    var response = Service.GetLatestInstantPayments(10000000);

        //    if (!response.ServiceResponse.HasErrors)
        //    {
        //        foreach (InstantPaymentSearchData data in response.Payments)
        //        {
        //            InstantPaymentViewModel instantPayment = new InstantPaymentViewModel()
        //            {
        //                PaymentId = new Guid(data.PaymentId),
        //                PaymentReference = data.PaymentReference,
        //                Status = data.Status,
        //                FromCustomerAlias = data.FromCustomerAlias,
        //                ToCustomerAlias = data.ToCustomerAlias,
        //                FromCustomerName = data.FromCustomerName,
        //                ToCustomerName = data.ToCustomerName,
        //                Amount = data.Amount,
        //                Currency = data.CCY,
        //                ValueDate = data.ValueDate,
        //                CreatedTime = data.CreatedTime,
        //                IsIncome = (AppSecurity.CurrentUser != null && AppSecurity.CurrentUser.OrganisationName != null
        //                && !AppSecurity.CurrentUser.OrganisationName.ToUpper().Equals(data.FromCustomerName.ToUpper()))
        //            };

        //            //Get the last customer's alias name to display in the create instant payment form
        //            if (string.IsNullOrEmpty(this.ToCustomer) && !instantPayment.IsIncome)
        //            {
        //                this.ToCustomer = instantPayment.ToCustomerAlias;
        //            }
        //        }
        //    }
        //}

        private void PrepareValues()
        {
            CustomerId = Service.GetCustomerIdGuid.ToString();
            BeneficiaryTypes = PrepareBeneficiaryTypes();
            BeneficiaryIdentificationTypes = PrepareBeneficiaryIdentificationTypes();
            AvailableCurrencies = PrepareAvailablePaymentCurrencies();
            BeneficiaryCountriesResidence = PreparePaymentCountries();
            NationalCodes = PrepareNationalCodes();
            BeneficiaryIdentificationCountries = PreparePaymentCountries();
            AmountCurrencies = PrepareAvailablePaymentCurrencies();
            BeneficiaryOccupationTypes = PrepareBeneficiaryOccupationTypes();
            BeneficiaryAccountTypes = PrepareBeneficiaryAccountTypes();
            BeneficiaryBankNationalCodeTypes = PrepareNationalCodes();
            BeneficiaryBankCountries = PreparePaymentCountries();
            DestinationCountries = PreparePaymentCountries();
            //AccountAliases = PrepareAccountAliases();
            // PriorUsedAliases = PreparePriorUsedAliases();
        }

        /// <summary>
        /// Copy payout details from an existing one
        /// </summary>
        /// <param name="id">existing payment GUID</param>
        public void Copy(Guid id)
        {
            PaymentGetSingleResponse response = Service.GetPaymentDetails(id);

            if (!response.ServiceResponse.HasErrors)
            {
                //this.PaymentId = new Guid(response.Payment.PaymentId);
                //this.PaymentReference = response.Payment.PaymentReference;
                this.Amount = response.Payment.Amount;
                //this.AmountCurrencyScale = response.Payment.AmountCurrencyScale;
                this.AmountCurrencyCode = response.Payment.AmountCurrencyCode;
                //this.AmountCurrencyScale = response.Payment.AmountCurrencyScale;
                //this.AmountTextWithCurrencyCode = response.Payment.AmountTextWithCurrencyCode;
                this.BeneficiaryTypeId = response.Payment.BeneficiaryTypeId;
                //this.BeneficiaryTypeName = response.Payment.BeneficiaryTypeName;
                //this.BeneficiaryBankCountryName = response.Payment.BeneficiaryBankCountryName;
                this.BeneficiaryFirstName = response.Payment.BeneficiaryFirstName;
                this.BeneficiaryLastName = response.Payment.BeneficiaryLastName;
                this.BeneficiaryName = response.Payment.BeneficiaryName;
                this.BeneficiaryEmail = response.Payment.BeneficiaryEmail;

                this.BeneficiaryAccountTypeCode = response.Payment.BeneficiaryAccountTypeCode;
                //this.BeneficiaryAccountTypeName = response.Payment.BeneficiaryAccountTypeName;
                this.BeneficiaryAccountNumber = response.Payment.BeneficiaryAccountNumber;
                this.BeneficiaryStreetAddress1 = response.Payment.BeneficiaryStreetAddress1;
                this.BeneficiaryStreetAddress2 = response.Payment.BeneficiaryStreetAddress2;
                this.BeneficiaryCellPhone = response.Payment.BeneficiaryCellPhone;
                this.BeneficiaryCity = response.Payment.BeneficiaryCity;
                this.BeneficiaryCountryCode = response.Payment.BeneficiaryCountryCode;
                //this.BeneficiaryCountryName = response.Payment.BeneficiaryCountryName;

                this.BeneficiaryBankName = response.Payment.BeneficiaryBankName;
                this.BeneficiaryBankBIC = response.Payment.BeneficiaryBankBIC;
                this.BeneficiaryBankNationalCode = response.Payment.BeneficiaryBankNationalCode;
                this.BeneficiaryBankNationalCodeType = response.Payment.BeneficiaryBankNationalCodeType;
                //this.BeneficiaryBankNationalCodeTypeName = response.Payment.BeneficiaryBankNationalCodeTypeName;
                this.BeneficiaryBranchName = response.Payment.BeneficiaryBranchName;
                this.BeneficiaryBankStreetAddress1 = response.Payment.BeneficiaryBankStreetAddress1;
                this.BeneficiaryBankStreetAddress2 = response.Payment.BeneficiaryBankStreetAddress2;
                this.BeneficiaryBankCity = response.Payment.BeneficiaryBankCity;
                this.BeneficiaryBankStateOrProvince = response.Payment.BeneficiaryBankStateOrProvince;
                this.BeneficiaryBankPostalCode = response.Payment.BeneficiaryBankPostalCode;
                //this.CreatedTime = Convert.ToDateTime(response.Payment.CreatedTime);
                //this.PaymentStatusTypeName = response.Payment.PaymentStatusTypeName;
                this.ReasonForPayment = response.Payment.ReasonForPayment;
                this.ValueDate = response.Payment.ValueDate;
                this.CountryCode = response.Payment.DestinationCountryCode;
            }
            else
            {
                _logger.Info("NewPayoutViewModel - Copy - Getting Payment Details Error");
            }
        }

        private IList<SelectListItem> PrepareAvailablePaymentCurrencies()
        {

            var currencies = Service.GetPaymentCurrencies();
            FavoriteCurrencyRepository cr = new FavoriteCurrencyRepository();
            if (currencies.ServiceResponse.HasErrors)
                return new List<SelectListItem>();
            var favoritecurrency = cr.GetFavoriteCurrencyList();
            if (favoritecurrency.Count > 0)
            {
                var currenciesListItem = new List<SelectListItem>();

                currenciesListItem.AddRange(favoritecurrency.Join(currencies.Currencies, favorite => favorite.CurrencyCode, inner => inner.CurrencyCode,
                    (favorite, inner) => new SelectListItem
                    {
                        Text = inner.CurrencyCode,
                        Value = inner.CurrencyCode
                    }));
                currenciesListItem.AddRange(currencies.Currencies.Where(a => !currenciesListItem.Select(s => s.Value).Contains(a.CurrencyCode)).Select(s => new SelectListItem()
                {
                    Text = s.CurrencyCode,
                    Value = s.CurrencyCode
                }));
                return currenciesListItem;
            }
            return currencies.Currencies.Select(s => new SelectListItem { Text = s.CurrencyCode, Value = s.CurrencyCode }).ToList();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> PrepareAllAvailablePaymentCurrencies()
        {
            var currenciesListItem = new List<SelectListItem>();
            var currencies = Service.GetPaymentCurrencies();
            if (!currencies.ServiceResponse.HasErrors)
            {
                foreach (var item in currencies.Currencies)
                {
                    currenciesListItem.Add
                    (
                        new SelectListItem
                        {
                            Text = item.CurrencyCode,
                            Value = item.CurrencyCode
                        }
                    );
                }
            }
            return currenciesListItem;
        }

        private IList<SelectListItem> PrepareAccountAliases()
        {
            var aliasListItem = new List<SelectListItem>();
            var aliases = Service.GetAccountAliases();
            if (!aliases.ServiceResponse.HasErrors)
            {
                foreach (var item in aliases.Aliases)
                {
                    aliasListItem.Add
                    (
                        new SelectListItem
                        {
                            Text = item.AccountAlias,
                            Value = item.AccountAlias
                        }
                    );
                }
            }
            return aliasListItem;
        }

        private IList<SelectListItem> PreparePriorUsedAliases()
        {
            var aliasList = new List<SelectListItem>();
            aliasList.Add
            (
                new SelectListItem
                {
                    Text = GlobalRes.NewInstantPaymentViewModel_PreparePriorUsedAliases_OrSelectPrior,
                    Value = ""
                }
            );

            var response = Service.GetLatestInstantPayments(1000000);
            if (!response.ServiceResponse.HasErrors)
            {
                aliasList.AddRange(response.Payments.Where(w => w.Status == "Posted").OrderByDescending(ob => ob.CreatedTime).GroupBy(gb => gb.ToCustomerAlias).Take(5).Select(s => new SelectListItem()
                {
                    Text = s.Key,
                    Value = s.Key
                }));
            }

            return aliasList;
        }

        private IList<SelectListItem> PreparePaymentCountries()
        {
            var countries = new List<SelectListItem>();
            var response = Service.GetPaymentCountries();
            if (!response.ServiceResponse.HasErrors)
            {
                foreach (var item in response.Countries)
                {
                    countries.Add
                    (
                        new SelectListItem
                        {
                            Text = item.CountryName,
                            Value = item.CountryCode
                        }
                    );
                }
            }
            return countries;
        }

        private IList<SelectListItem> PrepareNationalCodes()
        {
            var nationalCodes = new List<SelectListItem>();
            var response = Service.GetPaymentNationalCodes();
            //nationalCodes.Add
            //(
            //    new SelectListItem
            //    {
            //        Text = "None",
            //        Value = ""
            //    }
            //);

            if (!response.ServiceResponse.HasErrors)
            {
                var i = 0;
                foreach (var item in response.NationalCodes)
                {
                    if (i == 0 && String.IsNullOrEmpty(item.NationalCode))
                    {
                        nationalCodes.Add
                        (
                            new SelectListItem
                            {
                                Text = "None",
                                Value = ""
                            }
                        );
                        i++;
                    }
                    else
                    {
                        nationalCodes.Add
                    (
                        new SelectListItem
                        {
                            Text = item.NationalCodeDescription,
                            Value = item.NationalCode
                        }
                    );
                    }
                }
            }

            return nationalCodes;
        }

        private IList<SelectListItem> PrepareNationalCodes(string currencyCode)
        {
            var nationalCodes = new List<SelectListItem>();
            var response = Service.GetPaymentNationalCodes(currencyCode);
            if (!response.ServiceResponse.HasErrors)
            {
                foreach (var item in response.NationalCodes)
                {
                    nationalCodes.Add
                    (
                        new SelectListItem
                        {
                            Text = item.NationalCodeName,
                            Value = item.NationalCode
                        }
                    );
                }
            }

            return nationalCodes;
        }

        private IList<SelectListItem> PrepareBeneficiaryTypes()
        {
            var beneficiaryTypes = new List<SelectListItem>();

            beneficiaryTypes.Add
            (
                new SelectListItem
                {
                    Text = "Individual",
                    Value = "1"
                }
            );

            beneficiaryTypes.Add
            (
                new SelectListItem
                {
                    Text = "Business",
                    Value = "2"
                }
            );


            return beneficiaryTypes;
        }

        private IList<SelectListItem> PrepareBeneficiaryIdentificationTypes()
        {
            var beneficiaryIDTypes = new List<SelectListItem>();

            beneficiaryIDTypes.Add
            (
                new SelectListItem
                {
                    Text = "None",
                    Value = "0"
                }
            );

            beneficiaryIDTypes.Add
            (
                new SelectListItem
                {
                    Text = "Account",
                    Value = "1"
                }
            );

            beneficiaryIDTypes.Add
            (
                new SelectListItem
                {
                    Text = "Credit Card, Debit Card",
                    Value = "2"
                }
            );

            beneficiaryIDTypes.Add
            (
                new SelectListItem
                {
                    Text = "Driver's License",
                    Value = "3"
                }
            );


            return beneficiaryIDTypes;
        }

        private IList<SelectListItem> PrepareBeneficiaryAccountTypes()
        {

            var beneficiaryAccountTypes = new List<SelectListItem>();

            beneficiaryAccountTypes.Add
            (
                new SelectListItem
                {
                    Text = "Checking",
                    Value = "1"
                }
            );

            beneficiaryAccountTypes.Add
            (
                new SelectListItem
                {
                    Text = "Savings",
                    Value = "2"
                }
            );


            return beneficiaryAccountTypes;
        }

        private IList<SelectListItem> PrepareBeneficiaryOccupationTypes()
        {

            var beneficiaryOccupationTypes = new List<SelectListItem>();

            beneficiaryOccupationTypes.Add
            (
                new SelectListItem
                {
                    Text = "Other",
                    Value = "0"
                }
            );

            beneficiaryOccupationTypes.Add
            (
                new SelectListItem
                {
                    Text = "Australian Standard Industry Code (ASIC)",
                    Value = "I"
                }
            );


            return beneficiaryOccupationTypes;
        }

        /// <summary>
        /// Check whether the payout details is valid
        /// </summary>
        /// <returns></returns>
        public bool IsPaymentValid()
        {
            bool isValid = false;

            //if (!String.IsNullOrEmpty(BeneficiaryFirstName) && !String.IsNullOrEmpty(BeneficiaryLastName) && !String.IsNullOrEmpty(BeneficiaryEmail) && !String.IsNullOrEmpty(BeneficiaryAccountNumber) && !String.IsNullOrEmpty(BeneficiaryCountryCode) && !String.IsNullOrEmpty(BeneficiaryStreetAddress1) && !String.IsNullOrEmpty(BeneficiaryCity) && !String.IsNullOrEmpty(BeneficiaryCellPhone) && !String.IsNullOrEmpty(BeneficiaryBankName) && (String.IsNullOrEmpty(BeneficiaryBankBIC) && !String.IsNullOrEmpty(BeneficiaryBankNationalCode)))
            if (!String.IsNullOrEmpty(BeneficiaryFirstName) && !String.IsNullOrEmpty(BeneficiaryLastName) && !String.IsNullOrEmpty(BeneficiaryEmail) && !String.IsNullOrEmpty(BeneficiaryAccountNumber) && !String.IsNullOrEmpty(BeneficiaryCountryCode) && !String.IsNullOrEmpty(BeneficiaryStreetAddress1) && !String.IsNullOrEmpty(BeneficiaryCity) && !String.IsNullOrEmpty(BeneficiaryCellPhone) && !String.IsNullOrEmpty(BeneficiaryBankName) && !String.IsNullOrEmpty(BeneficiaryBankBIC))
            {
                isValid = true;
            }
            else if(String.IsNullOrEmpty(BeneficiaryFirstName))
            {
                Errors.Add(new ErrorViewModel()
                {
                    MessageDetails = "Beneficiary First Name cannot be blank"
                });
            }
            else if(String.IsNullOrEmpty(BeneficiaryLastName))
            {
                Errors.Add(new ErrorViewModel()
                {
                    MessageDetails = "Beneficiary last Name cannot be blank"
                });
            }
            else if(String.IsNullOrEmpty(BeneficiaryEmail))
            {
                Errors.Add(new ErrorViewModel()
                {
                    MessageDetails = "Beneficiary Email cannot be blank"
                });
            }
            else if(String.IsNullOrEmpty(BeneficiaryAccountNumber))
            {
                Errors.Add(new ErrorViewModel()
                {
                    MessageDetails = "Beneficiary Account Number cannot be blank"
                });
            }
            else if(String.IsNullOrEmpty(BeneficiaryCountryCode))
            {
                Errors.Add(new ErrorViewModel()
                {
                    MessageDetails = "Beneficiary Country cannot be blank"
                });
            }
            else if (String.IsNullOrEmpty(BeneficiaryStreetAddress1))
            {
                Errors.Add(new ErrorViewModel()
                {
                    MessageDetails = "Beneficiary Street Address 1 cannot be blank"
                });
            }
            else if (String.IsNullOrEmpty(BeneficiaryCity))
            {
                Errors.Add(new ErrorViewModel()
                {
                    MessageDetails = "Beneficiary City cannot be blank"
                });
            }
            else if (String.IsNullOrEmpty(BeneficiaryCellPhone))
            {
                Errors.Add(new ErrorViewModel()
                {
                    MessageDetails = "Beneficiary Cell Phone cannot be blank"
                });
            }
            else if (String.IsNullOrEmpty(BeneficiaryBankName))
            {
                Errors.Add(new ErrorViewModel()
                {
                    MessageDetails = "Beneficiary Bank Name cannot be blank"
                });
            }
            else if (String.IsNullOrEmpty(BeneficiaryBankBIC))
            {
                Errors.Add(new ErrorViewModel()
                {
                    MessageDetails = "Beneficiary Bank BIC cannot be blank"
                });
            }

            return isValid;
        }

        /// <summary>
        /// Prepare PaymentValidateRequest
        /// </summary>
        /// <returns></returns>
        public PaymentValidateRequest PreparePaymentValidateRequest()
        {
            PaymentValidateRequest request = new PaymentValidateRequest()
            {
                BankOperationCode = BankOperationCode,
                BeneficiaryABN = BeneficiaryABN,
                BeneficiaryACN = BeneficiaryACN,
                BeneficiaryARBN = BeneficiaryARBN,
                BeneficiaryAccountNumber = BeneficiaryAccountNumber,
                BeneficiaryAccountNumberPrefix = BeneficiaryAccountNumberPrefix,
                BeneficiaryAccountNumberSuffix = BeneficiaryAccountNumberSuffix,
                BeneficiaryAccountTypeCode = BeneficiaryAccountTypeCode,
                BeneficiaryAddress1 = BeneficiaryAddress1,
                BeneficiaryAddress2 = BeneficiaryAddress2,
                BeneficiaryAddress3 = BeneficiaryAddress3,
                BeneficiaryBankAddress1 = BeneficiaryBankAddress1,
                BeneficiaryBankAddress2 = BeneficiaryBankAddress2,
                BeneficiaryBankAddress3 = BeneficiaryBankAddress3,
                BeneficiaryBankBIC = BeneficiaryBankBIC,
                BeneficiaryBankCity = BeneficiaryBankCity,
                BeneficiaryBankCode = BeneficiaryBankCode,
                BeneficiaryBankCountryCode = BeneficiaryBankCountryCode,
                BeneficiaryBankName = BeneficiaryBankName,
                BeneficiaryBankNationalCode = BeneficiaryBankNationalCode,
                BeneficiaryBankNationalCodeType = BeneficiaryBankNationalCodeType,
                BeneficiaryBankPostalCode = BeneficiaryBankPostalCode,
                BeneficiaryBankStateOrProvince = BeneficiaryBankStateOrProvince,
                BeneficiaryBankStreetAddress1 = BeneficiaryBankStreetAddress1,
                BeneficiaryBankStreetAddress2 = BeneficiaryBankStreetAddress2,
                BeneficiaryBranchBIC = BeneficiaryBranchBIC,
                BeneficiaryBranchCity = BeneficiaryBranchCity,
                BeneficiaryBranchCode = BeneficiaryBranchCode,
                BeneficiaryBranchCountryCode = BeneficiaryBranchCountryCode,
                BeneficiaryBranchId = BeneficiaryBranchId,
                BeneficiaryBranchName = BeneficiaryBranchName,
                BeneficiaryBranchNationalCode = BeneficiaryBranchNationalCode,
                BeneficiaryBranchNationalCodeType = BeneficiaryBranchNationalCodeType,
                BeneficiaryBranchPostalCode = BeneficiaryBranchPostalCode,
                BeneficiaryBranchStateOrProvince = BeneficiaryBranchStateOrProvince,
                BeneficiaryBranchStreetAddress1 = BeneficiaryBranchStreetAddress1,
                BeneficiaryBranchStreetAddress2 = BeneficiaryBranchStreetAddress2,
                BeneficiaryBusinessStructureTypeCode = BeneficiaryBusinessStructureTypeCode,
                BeneficiaryCellPhone = BeneficiaryCellPhone,
                BeneficiaryCity = BeneficiaryCity,
                BeneficiaryCompanyRegistrationCountryCode = BeneficiaryCompanyRegistrationCountryCode,
                BeneficiaryCompanyRegistrationNumber = BeneficiaryCompanyRegistrationNumber,
                BeneficiaryCountryCode = BeneficiaryCountryCode,
                BeneficiaryCountryOfBirthCode = BeneficiaryCountryOfBirthCode,
                BeneficiaryDateOfBirth = BeneficiaryDateOfBirth,
                BeneficiaryEmail = BeneficiaryEmail,
                BeneficiaryFirstName = BeneficiaryFirstName,
                BeneficiaryIdentificationCountryCode = BeneficiaryIdentificationCountryCode,
                BeneficiaryIdentificationNumber = BeneficiaryIdentificationNumber,
                BeneficiaryIdentificationTypeId = BeneficiaryIdentificationTypeId,
                BeneficiaryInfoLine1 = BeneficiaryInfoLine1,
                BeneficiaryInfoLine2 = BeneficiaryInfoLine2,
                BeneficiaryInfoLine3 = BeneficiaryInfoLine3,
                BeneficiaryInfoLine4 = BeneficiaryInfoLine4,
                BeneficiaryLastName = BeneficiaryLastName,
                BeneficiaryMiddleName = BeneficiaryMiddleName,
                BeneficiaryName = BeneficiaryName,
                BeneficiaryOccupationCode = BeneficiaryOccupationCode,
                BeneficiaryOccupationDescription = BeneficiaryOccupationDescription,
                BeneficiaryOccupationTypeId = BeneficiaryOccupationTypeId,
                BeneficiaryPostalCode = BeneficiaryPostalCode,
                BeneficiaryStateOrProvince = BeneficiaryStateOrProvince,
                BeneficiaryStreetAddress1 = BeneficiaryStreetAddress1,
                BeneficiaryStreetAddress2 = BeneficiaryStreetAddress2,
                BeneficiaryTaxId = BeneficiaryTaxId,
                BeneficiaryTypeId = BeneficiaryTypeId,
                ChargeDetail = ChargeDetail,
                InitiatingInstitutionABN = InitiatingInstitutionABN,
                InitiatingInstitutionACN = InitiatingInstitutionACN,
                InitiatingInstitutionARBN = InitiatingInstitutionARBN,
                InitiatingInstitutionBIC = InitiatingInstitutionBIC,
                InitiatingInstitutionCity = InitiatingInstitutionCity,
                InitiatingInstitutionCountryCode = InitiatingInstitutionCountryCode,
                InitiatingInstitutionName = InitiatingInstitutionName,
                InitiatingInstitutionNationalCode = InitiatingInstitutionNationalCode,
                InitiatingInstitutionNationalCodeType = InitiatingInstitutionNationalCodeType,
                InitiatingInstitutionPostalCode = InitiatingInstitutionPostalCode,
                InitiatingInstitutionSameAsOrderingInstitution = InitiatingInstitutionSameAsOrderingInstitution,
                InitiatingInstitutionStateOrProvince = InitiatingInstitutionStateOrProvince,
                InitiatingInstitutionStreetAddress1 = InitiatingInstitutionStreetAddress1,
                InitiatingInstitutionStreetAddress2 = InitiatingInstitutionStreetAddress2,
                IntermediaryBankAddress1 = IntermediaryBankAddress1,
                IntermediaryBankAddress2 = IntermediaryBankAddress2,
                IntermediaryBankAddress3 = IntermediaryBankAddress3,
                IntermediaryBankBIC = IntermediaryBankBIC,
                IntermediaryBankCity = IntermediaryBankCity,
                IntermediaryBankCountryCode = IntermediaryBankCountryCode,
                IntermediaryBankName = IntermediaryBankName,
                IntermediaryBankNationalCode = IntermediaryBankNationalCode,
                IntermediaryBankNationalCodeType = IntermediaryBankNationalCodeType,
                IntermediaryBankPostalCode = IntermediaryBankPostalCode,
                IntermediaryBankStateOrProvince = IntermediaryBankStateOrProvince,
                IntermediaryBankStreetAddress1 = IntermediaryBankStreetAddress1,
                IntermediaryBankStreetAddress2 = IntermediaryBankStreetAddress2,
                OrderingCustomerBankBIC = OrderingCustomerBankBIC,
                OrderingCustomerBankCity = OrderingCustomerBankCity,
                OrderingCustomerBankCountryCode = OrderingCustomerBankCountryCode,
                OrderingCustomerBankName = OrderingCustomerBankName,
                OrderingCustomerBankNationalCode = OrderingCustomerBankNationalCode,
                OrderingCustomerBankNationalCodeType = OrderingCustomerBankNationalCodeType,
                OrderingCustomerBankPostalCode = OrderingCustomerBankPostalCode,
                OrderingCustomerBankStateOrProvince = OrderingCustomerBankStateOrProvince,
                OrderingCustomerBankStreetAddress1 = OrderingCustomerBankStreetAddress1,
                OrderingCustomerBankStreetAddress2 = OrderingCustomerBankStreetAddress2,
                ReasonForPayment = ReasonForPayment,
                ReasonForPaymentCode = ReasonForPaymentCode,
                ReceiverBIC = ReceiverBIC,
                ReceivingInstitutionBIC = ReceivingInstitutionBIC,
                ReceivingInstitutionCity = ReceivingInstitutionCity,
                ReceivingInstitutionCountryCode = ReceivingInstitutionCountryCode,
                ReceivingInstitutionName = ReceivingInstitutionName,
                ReceivingInstitutionNationalCode = ReceivingInstitutionNationalCode,
                ReceivingInstitutionNationalCodeType = ReceivingInstitutionNationalCodeType,
                ReceivingInstitutionPostalCode = ReceivingInstitutionPostalCode,
                ReceivingInstitutionStateOrProvince = ReceivingInstitutionStateOrProvince,
                ReceivingInstitutionStreetAddress1 = ReceivingInstitutionStreetAddress1,
                ReceivingInstitutionStreetAddress2 = ReceivingInstitutionStreetAddress2,
                SenderToReceiverInfo1 = SenderToReceiverLine1,
                SenderToReceiverInfo2 = SenderToReceiverLine2,
                SenderToReceiverInfo3 = SenderToReceiverLine3,
                SenderToReceiverInfo4 = SenderToReceiverLine4,
                SenderToReceiverInfo5 = SenderToReceiverLine5,
                SenderToReceiverInfo6 = SenderToReceiverLine6,
                SendingInstitutionABN = SendingInstitutionABN,
                SendingInstitutionACN = SendingInstitutionACN,
                SendingInstitutionARBN = SendingInstitutionARBN,
                SendingInstitutionBIC = SendingInstitutionBIC,
                SendingInstitutionBusinessStructureTypeCode = SendingInstitutionBusinessStructureTypeCode,
                SendingInstitutionCity = SendingInstitutionCity,
                SendingInstitutionCountryCode = SendingInstitutionCountryCode,
                SendingInstitutionEmail = SendingInstitutionEmail,
                SendingInstitutionName = SendingInstitutionName,
                SendingInstitutionNationalCode = SendingInstitutionNationalCode,
                SendingInstitutionNationalCodeType = SendingInstitutionNationalCodeType,
                SendingInstitutionOccupationCode = SendingInstitutionOccupationCode,
                SendingInstitutionOccupationDescription = SendingInstitutionOccupationDescription,
                SendingInstitutionOccupationTypeId = SendingInstitutionOccupationTypeId,
                SendingInstitutionPhone = SendingInstitutionPhone,
                SendingInstitutionPostalCode = SendingInstitutionPostalCode,
                SendingInstitutionSameAsOrderingInstitution = SendingInstitutionSameAsOrderingInstitution,
                SendingInstitutionStateOrProvince = SendingInstitutionStateOrProvince,
                SendingInstitutionStreetAddress1 = SendingInstitutionStreetAddress1,
                SendingInstitutionStreetAddress2 = SendingInstitutionStreetAddress2,
                CustomerId = CustomerId,
                FXDealId = FXDealId,
                Amount = Amount,
                AmountCurrencyCode = AmountCurrencyCode,
                DestinationCountryCode = CountryCode,
                ValueDate = ValueDate,
                FeeAmount = FeeAmount,
                FeeAmountCurrencyCode = FeeAmountCurrencyCode,
                PaymentValueType = PaymentValueType,
            };

            return request;
        }
    }
}