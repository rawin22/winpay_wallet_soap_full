using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using Newtonsoft.Json;
using StaticExtensions;
using Swashbuckle.Swagger;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.APIModels.InstantPayment;
using TSG.Models.APIModels.Payment;

namespace Tsg.UI.Main.ApiMethods.Payouts
{
    /// <summary>
    /// Model for payout API
    /// </summary>
    public class PaymentMethods : BaseApiMethods
    {
        private UserInfo _userInfo { get; set; }
        /// <summary>
        /// List of validation errors
        /// </summary>
        public IList<ApiErrorModel> Errors { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ui"></param>
        public PaymentMethods(UserInfo ui) : base(ui)
        {
            _userInfo = ui;
            _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            Errors = new List<ApiErrorModel>();
        }

        /// <summary>
        /// Create payout
        /// </summary>
        /// <param name="request">Create Payout Request</param>
        public ApiCreatePaymentResponse Create(ApiCreatePaymentRequest request)
        {
            var isValid = Validate(PrepareApiValidatePaymentRequest(request));
            if (isValid)
            {
                _logger.Info("isValid: " + JsonConvert.SerializeObject(request));
                var paymentCreateRequest = PreparePaymentCreateRequest(request);
                _logger.Info("paymentCreateRequest: " + JsonConvert.SerializeObject(paymentCreateRequest));
                var serviceResponse = Service.CreatePayment(paymentCreateRequest);
                // var serviceResponse = Service.CreatePayment(PreparePaymentCreateRequest(request));
                _logger.Info("serviceResponse: " + JsonConvert.SerializeObject(serviceResponse));
                return PrepareApiCreatePaymentResponse(serviceResponse);
            }
            else
            {
                _logger.Info("Not Valid: " + JsonConvert.SerializeObject(this.Errors));
                return (new ApiCreatePaymentResponse()
                {
                    Errors = this.Errors
                });
            }
        }

        /// <summary>
        /// Submit pay out
        /// </summary>
        public ApiSubmitPaymentResponse Submit(ApiSubmitPaymentResquest request)
        {
            var serviceResponse = Service.SubmitPayment(PreparePaymentSubmitRequest(request));

            return (PrepareApiSubmitPaymentResponse(serviceResponse));
        }

        /// <summary>
        /// Submit pay out
        /// </summary>
        public ApiSubmitPaymentResponse Submit(Guid id)
        {
            var paymentResponse = Service.GetPaymentDetails(id);
            var serviceResponse = new PaymentSubmitResponse();
            if (!paymentResponse.ServiceResponse.HasErrors && paymentResponse.Payment.PaymentId != null && paymentResponse.Payment.PaymentId != default)
            {
                var submitRequest = new PaymentSubmitRequest()
                {
                    PaymentId = paymentResponse.Payment.PaymentId,
                    Timestamp = paymentResponse.Payment.Timestamp
                };

                serviceResponse = Service.SubmitPayment(submitRequest);
            }

            return (PrepareApiSubmitPaymentResponse(serviceResponse));
        }

        /// <summary>
        /// Delete a payment
        /// </summary>
        public ApiDeletePaymentResponse Delete(ApiDeletePaymentResquest request)
        {
            var serviceResponse = Service.DeletePayment(PreparePaymentDeleteRequest(request));

            return (PrepareApiDeletePaymentResponse(serviceResponse));

        }

        /// <summary>
        /// Validate payment
        /// </summary>
        public bool Validate(ApiValidatePaymentRequest request)
        {
            ApiValidatePaymentResponse apiResponse = new ApiValidatePaymentResponse();

            bool isValide = false;
            var response = Service.ValidatePayment(PreparePaymentValidateRequest(request));

            if (!response.ServiceResponse.HasErrors)
            {
                isValide = true;
            }
            else
            {
                GetErrorMessages(response.ServiceResponse.Responses);
            }

            return isValide;
        }

        /// <summary>
        /// Get error messages from web service response
        /// </summary>
        /// <returns></returns>
        public void GetErrorMessages(ServiceResponseData[] responses)
        {
            foreach (var error in responses)
            {
                this.Errors.Add(new ApiErrorModel
                {
                    Code = error.ResponseCode,
                    Type = (ApiErrorType) error.ResponseType,
                    Message = error.Message,
                    MessageDetails = error.MessageDetails,
                    FieldName = error.FieldName,
                    FieldValue = error.FieldValue
                });
            }
        }

        /// <summary>
        /// Prepare PaymentValidateRequest
        /// </summary>
        /// <returns>PaymentValidateRequest</returns>
        public PaymentValidateRequest PreparePaymentValidateRequest(ApiValidatePaymentRequest apiRequest)
        {
            PaymentValidateRequest request = new PaymentValidateRequest()
            {
                BankOperationCode = apiRequest.BankOperationCode,
                BeneficiaryABN = apiRequest.BeneficiaryABN,
                BeneficiaryACN = apiRequest.BeneficiaryACN,
                BeneficiaryARBN = apiRequest.BeneficiaryARBN,
                BeneficiaryAccountNumber = apiRequest.BeneficiaryAccountNumber,
                BeneficiaryAccountNumberPrefix = apiRequest.BeneficiaryAccountNumberPrefix,
                BeneficiaryAccountNumberSuffix = apiRequest.BeneficiaryAccountNumberSuffix,
                BeneficiaryAccountTypeCode = apiRequest.BeneficiaryAccountTypeCode,
                BeneficiaryAddress1 = apiRequest.BeneficiaryAddress1,
                BeneficiaryAddress2 = apiRequest.BeneficiaryAddress2,
                BeneficiaryAddress3 = apiRequest.BeneficiaryAddress3,
                BeneficiaryBankAddress1 = apiRequest.BeneficiaryBankAddress1,
                BeneficiaryBankAddress2 = apiRequest.BeneficiaryBankAddress2,
                BeneficiaryBankAddress3 = apiRequest.BeneficiaryBankAddress3,
                BeneficiaryBankBIC = apiRequest.BeneficiaryBankBIC,
                BeneficiaryBankCity = apiRequest.BeneficiaryBankCity,
                BeneficiaryBankCode = apiRequest.BeneficiaryBankCode,
                BeneficiaryBankCountryCode = apiRequest.BeneficiaryBankCountryCode,
                BeneficiaryBankName = apiRequest.BeneficiaryBankName,
                BeneficiaryBankNationalCode = apiRequest.BeneficiaryBankNationalCode,
                BeneficiaryBankNationalCodeType = apiRequest.BeneficiaryBankNationalCodeType,
                BeneficiaryBankPostalCode = apiRequest.BeneficiaryBankPostalCode,
                BeneficiaryBankStateOrProvince = apiRequest.BeneficiaryBankStateOrProvince,
                BeneficiaryBankStreetAddress1 = apiRequest.BeneficiaryBankStreetAddress1,
                BeneficiaryBankStreetAddress2 = apiRequest.BeneficiaryBankStreetAddress2,
                BeneficiaryBranchBIC = apiRequest.BeneficiaryBranchBIC,
                BeneficiaryBranchCity = apiRequest.BeneficiaryBranchCity,
                BeneficiaryBranchCode = apiRequest.BeneficiaryBranchCode,
                BeneficiaryBranchCountryCode = apiRequest.BeneficiaryBranchCountryCode,
                BeneficiaryBranchId = apiRequest.BeneficiaryBranchId,
                BeneficiaryBranchName = apiRequest.BeneficiaryBranchName,
                BeneficiaryBranchNationalCode = apiRequest.BeneficiaryBranchNationalCode,
                BeneficiaryBranchNationalCodeType = apiRequest.BeneficiaryBranchNationalCodeType,
                BeneficiaryBranchPostalCode = apiRequest.BeneficiaryBranchPostalCode,
                BeneficiaryBranchStateOrProvince = apiRequest.BeneficiaryBranchStateOrProvince,
                BeneficiaryBranchStreetAddress1 = apiRequest.BeneficiaryBranchStreetAddress1,
                BeneficiaryBranchStreetAddress2 = apiRequest.BeneficiaryBranchStreetAddress2,
                BeneficiaryBusinessStructureTypeCode = apiRequest.BeneficiaryBusinessStructureTypeCode,
                BeneficiaryCellPhone = apiRequest.BeneficiaryCellPhone,
                BeneficiaryCity = apiRequest.BeneficiaryCity,
                BeneficiaryCompanyRegistrationCountryCode = apiRequest.BeneficiaryCompanyRegistrationCountryCode,
                BeneficiaryCompanyRegistrationNumber = apiRequest.BeneficiaryCompanyRegistrationNumber,
                BeneficiaryCountryCode = apiRequest.BeneficiaryCountryCode,
                BeneficiaryCountryOfBirthCode = apiRequest.BeneficiaryCountryOfBirthCode,
                BeneficiaryDateOfBirth = apiRequest.BeneficiaryDateOfBirth,
                BeneficiaryEmail = apiRequest.BeneficiaryEmail,
                BeneficiaryFirstName = apiRequest.BeneficiaryFirstName,
                BeneficiaryIdentificationCountryCode = apiRequest.BeneficiaryIdentificationCountryCode,
                BeneficiaryIdentificationNumber = apiRequest.BeneficiaryIdentificationNumber,
                BeneficiaryIdentificationTypeId = apiRequest.BeneficiaryIdentificationTypeId,
                BeneficiaryInfoLine1 = apiRequest.BeneficiaryInfoLine1,
                BeneficiaryInfoLine2 = apiRequest.BeneficiaryInfoLine2,
                BeneficiaryInfoLine3 = apiRequest.BeneficiaryInfoLine3,
                BeneficiaryInfoLine4 = apiRequest.BeneficiaryInfoLine4,
                BeneficiaryLastName = apiRequest.BeneficiaryLastName,
                BeneficiaryMiddleName = apiRequest.BeneficiaryMiddleName,
                BeneficiaryName = apiRequest.BeneficiaryName,
                BeneficiaryOccupationCode = apiRequest.BeneficiaryOccupationCode,
                BeneficiaryOccupationDescription = apiRequest.BeneficiaryOccupationDescription,
                BeneficiaryOccupationTypeId = apiRequest.BeneficiaryOccupationTypeId,
                BeneficiaryPostalCode = apiRequest.BeneficiaryPostalCode,
                BeneficiaryStateOrProvince = apiRequest.BeneficiaryStateOrProvince,
                BeneficiaryStreetAddress1 = apiRequest.BeneficiaryStreetAddress1,
                BeneficiaryStreetAddress2 = apiRequest.BeneficiaryStreetAddress2,
                BeneficiaryTaxId = apiRequest.BeneficiaryTaxId,
                BeneficiaryTypeId = apiRequest.BeneficiaryTypeId,
                ChargeDetail = apiRequest.ChargeDetail,
                InitiatingInstitutionABN = apiRequest.InitiatingInstitutionABN,
                InitiatingInstitutionACN = apiRequest.InitiatingInstitutionACN,
                InitiatingInstitutionARBN = apiRequest.InitiatingInstitutionARBN,
                InitiatingInstitutionBIC = apiRequest.InitiatingInstitutionBIC,
                InitiatingInstitutionCity = apiRequest.InitiatingInstitutionCity,
                InitiatingInstitutionCountryCode = apiRequest.InitiatingInstitutionCountryCode,
                InitiatingInstitutionName = apiRequest.InitiatingInstitutionName,
                InitiatingInstitutionNationalCode = apiRequest.InitiatingInstitutionNationalCode,
                InitiatingInstitutionNationalCodeType = apiRequest.InitiatingInstitutionNationalCodeType,
                InitiatingInstitutionPostalCode = apiRequest.InitiatingInstitutionPostalCode,
                InitiatingInstitutionSameAsOrderingInstitution = apiRequest.InitiatingInstitutionSameAsOrderingInstitution,
                InitiatingInstitutionStateOrProvince = apiRequest.InitiatingInstitutionStateOrProvince,
                InitiatingInstitutionStreetAddress1 = apiRequest.InitiatingInstitutionStreetAddress1,
                InitiatingInstitutionStreetAddress2 = apiRequest.InitiatingInstitutionStreetAddress2,
                IntermediaryBankAddress1 = apiRequest.IntermediaryBankAddress1,
                IntermediaryBankAddress2 = apiRequest.IntermediaryBankAddress2,
                IntermediaryBankAddress3 = apiRequest.IntermediaryBankAddress3,
                IntermediaryBankBIC = apiRequest.IntermediaryBankBIC,
                IntermediaryBankCity = apiRequest.IntermediaryBankCity,
                IntermediaryBankCountryCode = apiRequest.IntermediaryBankCountryCode,
                IntermediaryBankName = apiRequest.IntermediaryBankName,
                IntermediaryBankNationalCode = apiRequest.IntermediaryBankNationalCode,
                IntermediaryBankNationalCodeType = apiRequest.IntermediaryBankNationalCodeType,
                IntermediaryBankPostalCode = apiRequest.IntermediaryBankPostalCode,
                IntermediaryBankStateOrProvince = apiRequest.IntermediaryBankStateOrProvince,
                IntermediaryBankStreetAddress1 = apiRequest.IntermediaryBankStreetAddress1,
                IntermediaryBankStreetAddress2 = apiRequest.IntermediaryBankStreetAddress2,
                OrderingCustomerBankBIC = apiRequest.OrderingCustomerBankBIC,
                OrderingCustomerBankCity = apiRequest.OrderingCustomerBankCity,
                OrderingCustomerBankCountryCode = apiRequest.OrderingCustomerBankCountryCode,
                OrderingCustomerBankName = apiRequest.OrderingCustomerBankName,
                OrderingCustomerBankNationalCode = apiRequest.OrderingCustomerBankNationalCode,
                OrderingCustomerBankNationalCodeType = apiRequest.OrderingCustomerBankNationalCodeType,
                OrderingCustomerBankPostalCode = apiRequest.OrderingCustomerBankPostalCode,
                OrderingCustomerBankStateOrProvince = apiRequest.OrderingCustomerBankStateOrProvince,
                OrderingCustomerBankStreetAddress1 = apiRequest.OrderingCustomerBankStreetAddress1,
                OrderingCustomerBankStreetAddress2 = apiRequest.OrderingCustomerBankStreetAddress2,
                ReasonForPayment = apiRequest.ReasonForPayment,
                ReasonForPaymentCode = apiRequest.ReasonForPaymentCode,
                ReceiverBIC = apiRequest.ReceiverBIC,
                ReceivingInstitutionBIC = apiRequest.ReceivingInstitutionBIC,
                ReceivingInstitutionCity = apiRequest.ReceivingInstitutionCity,
                ReceivingInstitutionCountryCode = apiRequest.ReceivingInstitutionCountryCode,
                ReceivingInstitutionName = apiRequest.ReceivingInstitutionName,
                ReceivingInstitutionNationalCode = apiRequest.ReceivingInstitutionNationalCode,
                ReceivingInstitutionNationalCodeType = apiRequest.ReceivingInstitutionNationalCodeType,
                ReceivingInstitutionPostalCode = apiRequest.ReceivingInstitutionPostalCode,
                ReceivingInstitutionStateOrProvince = apiRequest.ReceivingInstitutionStateOrProvince,
                ReceivingInstitutionStreetAddress1 = apiRequest.ReceivingInstitutionStreetAddress1,
                ReceivingInstitutionStreetAddress2 = apiRequest.ReceivingInstitutionStreetAddress2,
                SenderToReceiverInfo1 = apiRequest.SenderToReceiverLine1,
                SenderToReceiverInfo2 = apiRequest.SenderToReceiverLine2,
                SenderToReceiverInfo3 = apiRequest.SenderToReceiverLine3,
                SenderToReceiverInfo4 = apiRequest.SenderToReceiverLine4,
                SenderToReceiverInfo5 = apiRequest.SenderToReceiverLine5,
                SenderToReceiverInfo6 = apiRequest.SenderToReceiverLine6,
                SendingInstitutionABN = apiRequest.SendingInstitutionABN,
                SendingInstitutionACN = apiRequest.SendingInstitutionACN,
                SendingInstitutionARBN = apiRequest.SendingInstitutionARBN,
                SendingInstitutionBIC = apiRequest.SendingInstitutionBIC,
                SendingInstitutionBusinessStructureTypeCode = apiRequest.SendingInstitutionBusinessStructureTypeCode,
                SendingInstitutionCity = apiRequest.SendingInstitutionCity,
                SendingInstitutionCountryCode = apiRequest.SendingInstitutionCountryCode,
                SendingInstitutionEmail = apiRequest.SendingInstitutionEmail,
                SendingInstitutionName = apiRequest.SendingInstitutionName,
                SendingInstitutionNationalCode = apiRequest.SendingInstitutionNationalCode,
                SendingInstitutionNationalCodeType = apiRequest.SendingInstitutionNationalCodeType,
                SendingInstitutionOccupationCode = apiRequest.SendingInstitutionOccupationCode,
                SendingInstitutionOccupationDescription = apiRequest.SendingInstitutionOccupationDescription,
                SendingInstitutionOccupationTypeId = apiRequest.SendingInstitutionOccupationTypeId,
                SendingInstitutionPhone = apiRequest.SendingInstitutionPhone,
                SendingInstitutionPostalCode = apiRequest.SendingInstitutionPostalCode,
                SendingInstitutionSameAsOrderingInstitution = apiRequest.SendingInstitutionSameAsOrderingInstitution,
                SendingInstitutionStateOrProvince = apiRequest.SendingInstitutionStateOrProvince,
                SendingInstitutionStreetAddress1 = apiRequest.SendingInstitutionStreetAddress1,
                SendingInstitutionStreetAddress2 = apiRequest.SendingInstitutionStreetAddress2,
                CustomerId = apiRequest.CustomerId,
                FXDealId = apiRequest.FXDealId,
                Amount = apiRequest.Amount,
                AmountCurrencyCode = apiRequest.AmountCurrencyCode,
                DestinationCountryCode = apiRequest.CountryCode,
                ValueDate = apiRequest.ValueDate,
                FeeAmount = apiRequest.FeeAmount,
                FeeAmountCurrencyCode = apiRequest.FeeAmountCurrencyCode,
                PaymentValueType = apiRequest.PaymentValueType,
                BeneficiaryExternalReference = apiRequest.BeneficiaryExternalReference,
                BeneficiaryWKYCId = apiRequest.BeneficiaryWKYCId,
                SwiftUETR = apiRequest.SwiftUETR,
                BeneficiaryStreetName = apiRequest.BeneficiaryStreetAddress1
            };

            return request;
        }

        /// <summary>
        /// Prepare PaymentValidateRequest
        /// </summary>
        /// <returns>PaymentValidateRequest</returns>
        public PaymentValidateRequest PreparePaymentValidateRequest(ApiCreatePaymentRequest apiRequest)
        {
            PaymentValidateRequest request = new PaymentValidateRequest()
            {
                BankOperationCode = apiRequest.BankOperationCode,
                BeneficiaryABN = apiRequest.BeneficiaryABN,
                BeneficiaryACN = apiRequest.BeneficiaryACN,
                BeneficiaryARBN = apiRequest.BeneficiaryARBN,
                BeneficiaryAccountNumber = apiRequest.BeneficiaryAccountNumber,
                BeneficiaryAccountNumberPrefix = apiRequest.BeneficiaryAccountNumberPrefix,
                BeneficiaryAccountNumberSuffix = apiRequest.BeneficiaryAccountNumberSuffix,
                BeneficiaryAccountTypeCode = apiRequest.BeneficiaryAccountTypeCode,
                BeneficiaryAddress1 = apiRequest.BeneficiaryAddress1,
                BeneficiaryAddress2 = apiRequest.BeneficiaryAddress2,
                BeneficiaryAddress3 = apiRequest.BeneficiaryAddress3,
                BeneficiaryBankAddress1 = apiRequest.BeneficiaryBankAddress1,
                BeneficiaryBankAddress2 = apiRequest.BeneficiaryBankAddress2,
                BeneficiaryBankAddress3 = apiRequest.BeneficiaryBankAddress3,
                BeneficiaryBankBIC = apiRequest.BeneficiaryBankBIC,
                BeneficiaryBankCity = apiRequest.BeneficiaryBankCity,
                BeneficiaryBankCode = apiRequest.BeneficiaryBankCode,
                BeneficiaryBankCountryCode = apiRequest.BeneficiaryBankCountryCode,
                BeneficiaryBankName = apiRequest.BeneficiaryBankName,
                BeneficiaryBankNationalCode = apiRequest.BeneficiaryBankNationalCode,
                BeneficiaryBankNationalCodeType = apiRequest.BeneficiaryBankNationalCodeType,
                BeneficiaryBankPostalCode = apiRequest.BeneficiaryBankPostalCode,
                BeneficiaryBankStateOrProvince = apiRequest.BeneficiaryBankStateOrProvince,
                BeneficiaryBankStreetAddress1 = apiRequest.BeneficiaryBankStreetAddress1,
                BeneficiaryBankStreetAddress2 = apiRequest.BeneficiaryBankStreetAddress2,
                BeneficiaryBranchBIC = apiRequest.BeneficiaryBranchBIC,
                BeneficiaryBranchCity = apiRequest.BeneficiaryBranchCity,
                BeneficiaryBranchCode = apiRequest.BeneficiaryBranchCode,
                BeneficiaryBranchCountryCode = apiRequest.BeneficiaryBranchCountryCode,
                BeneficiaryBranchId = apiRequest.BeneficiaryBranchId,
                BeneficiaryBranchName = apiRequest.BeneficiaryBranchName,
                BeneficiaryBranchNationalCode = apiRequest.BeneficiaryBranchNationalCode,
                BeneficiaryBranchNationalCodeType = apiRequest.BeneficiaryBranchNationalCodeType,
                BeneficiaryBranchPostalCode = apiRequest.BeneficiaryBranchPostalCode,
                BeneficiaryBranchStateOrProvince = apiRequest.BeneficiaryBranchStateOrProvince,
                BeneficiaryBranchStreetAddress1 = apiRequest.BeneficiaryBranchStreetAddress1,
                BeneficiaryBranchStreetAddress2 = apiRequest.BeneficiaryBranchStreetAddress2,
                BeneficiaryBusinessStructureTypeCode = apiRequest.BeneficiaryBusinessStructureTypeCode,
                BeneficiaryCellPhone = apiRequest.BeneficiaryCellPhone,
                BeneficiaryCity = apiRequest.BeneficiaryCity,
                BeneficiaryCompanyRegistrationCountryCode = apiRequest.BeneficiaryCompanyRegistrationCountryCode,
                BeneficiaryCompanyRegistrationNumber = apiRequest.BeneficiaryCompanyRegistrationNumber,
                BeneficiaryCountryCode = apiRequest.BeneficiaryCountryCode,
                BeneficiaryCountryOfBirthCode = apiRequest.BeneficiaryCountryOfBirthCode,
                BeneficiaryDateOfBirth = apiRequest.BeneficiaryDateOfBirth,
                BeneficiaryEmail = apiRequest.BeneficiaryEmail,
                BeneficiaryFirstName = apiRequest.BeneficiaryFirstName,
                BeneficiaryIdentificationCountryCode = apiRequest.BeneficiaryIdentificationCountryCode,
                BeneficiaryIdentificationNumber = apiRequest.BeneficiaryIdentificationNumber,
                BeneficiaryIdentificationTypeId = apiRequest.BeneficiaryIdentificationTypeId,
                BeneficiaryInfoLine1 = apiRequest.BeneficiaryInfoLine1,
                BeneficiaryInfoLine2 = apiRequest.BeneficiaryInfoLine2,
                BeneficiaryInfoLine3 = apiRequest.BeneficiaryInfoLine3,
                BeneficiaryInfoLine4 = apiRequest.BeneficiaryInfoLine4,
                BeneficiaryLastName = apiRequest.BeneficiaryLastName,
                BeneficiaryMiddleName = apiRequest.BeneficiaryMiddleName,
                BeneficiaryName = apiRequest.BeneficiaryName,
                BeneficiaryOccupationCode = apiRequest.BeneficiaryOccupationCode,
                BeneficiaryOccupationDescription = apiRequest.BeneficiaryOccupationDescription,
                BeneficiaryOccupationTypeId = apiRequest.BeneficiaryOccupationTypeId,
                BeneficiaryPostalCode = apiRequest.BeneficiaryPostalCode,
                BeneficiaryStateOrProvince = apiRequest.BeneficiaryStateOrProvince,
                BeneficiaryStreetAddress1 = apiRequest.BeneficiaryStreetAddress1,
                BeneficiaryStreetAddress2 = apiRequest.BeneficiaryStreetAddress2,
                BeneficiaryTaxId = apiRequest.BeneficiaryTaxId,
                BeneficiaryTypeId = apiRequest.BeneficiaryTypeId,
                ChargeDetail = apiRequest.ChargeDetail,
                InitiatingInstitutionABN = apiRequest.InitiatingInstitutionABN,
                InitiatingInstitutionACN = apiRequest.InitiatingInstitutionACN,
                InitiatingInstitutionARBN = apiRequest.InitiatingInstitutionARBN,
                InitiatingInstitutionBIC = apiRequest.InitiatingInstitutionBIC,
                InitiatingInstitutionCity = apiRequest.InitiatingInstitutionCity,
                InitiatingInstitutionCountryCode = apiRequest.InitiatingInstitutionCountryCode,
                InitiatingInstitutionName = apiRequest.InitiatingInstitutionName,
                InitiatingInstitutionNationalCode = apiRequest.InitiatingInstitutionNationalCode,
                InitiatingInstitutionNationalCodeType = apiRequest.InitiatingInstitutionNationalCodeType,
                InitiatingInstitutionPostalCode = apiRequest.InitiatingInstitutionPostalCode,
                InitiatingInstitutionSameAsOrderingInstitution = apiRequest.InitiatingInstitutionSameAsOrderingInstitution,
                InitiatingInstitutionStateOrProvince = apiRequest.InitiatingInstitutionStateOrProvince,
                InitiatingInstitutionStreetAddress1 = apiRequest.InitiatingInstitutionStreetAddress1,
                InitiatingInstitutionStreetAddress2 = apiRequest.InitiatingInstitutionStreetAddress2,
                IntermediaryBankAddress1 = apiRequest.IntermediaryBankAddress1,
                IntermediaryBankAddress2 = apiRequest.IntermediaryBankAddress2,
                IntermediaryBankAddress3 = apiRequest.IntermediaryBankAddress3,
                IntermediaryBankBIC = apiRequest.IntermediaryBankBIC,
                IntermediaryBankCity = apiRequest.IntermediaryBankCity,
                IntermediaryBankCountryCode = apiRequest.IntermediaryBankCountryCode,
                IntermediaryBankName = apiRequest.IntermediaryBankName,
                IntermediaryBankNationalCode = apiRequest.IntermediaryBankNationalCode,
                IntermediaryBankNationalCodeType = apiRequest.IntermediaryBankNationalCodeType,
                IntermediaryBankPostalCode = apiRequest.IntermediaryBankPostalCode,
                IntermediaryBankStateOrProvince = apiRequest.IntermediaryBankStateOrProvince,
                IntermediaryBankStreetAddress1 = apiRequest.IntermediaryBankStreetAddress1,
                IntermediaryBankStreetAddress2 = apiRequest.IntermediaryBankStreetAddress2,
                OrderingCustomerBankBIC = apiRequest.OrderingCustomerBankBIC,
                OrderingCustomerBankCity = apiRequest.OrderingCustomerBankCity,
                OrderingCustomerBankCountryCode = apiRequest.OrderingCustomerBankCountryCode,
                OrderingCustomerBankName = apiRequest.OrderingCustomerBankName,
                OrderingCustomerBankNationalCode = apiRequest.OrderingCustomerBankNationalCode,
                OrderingCustomerBankNationalCodeType = apiRequest.OrderingCustomerBankNationalCodeType,
                OrderingCustomerBankPostalCode = apiRequest.OrderingCustomerBankPostalCode,
                OrderingCustomerBankStateOrProvince = apiRequest.OrderingCustomerBankStateOrProvince,
                OrderingCustomerBankStreetAddress1 = apiRequest.OrderingCustomerBankStreetAddress1,
                OrderingCustomerBankStreetAddress2 = apiRequest.OrderingCustomerBankStreetAddress2,
                ReasonForPayment = apiRequest.ReasonForPayment,
                ReasonForPaymentCode = apiRequest.ReasonForPaymentCode,
                ReceiverBIC = apiRequest.ReceiverBIC,
                ReceivingInstitutionBIC = apiRequest.ReceivingInstitutionBIC,
                ReceivingInstitutionCity = apiRequest.ReceivingInstitutionCity,
                ReceivingInstitutionCountryCode = apiRequest.ReceivingInstitutionCountryCode,
                ReceivingInstitutionName = apiRequest.ReceivingInstitutionName,
                ReceivingInstitutionNationalCode = apiRequest.ReceivingInstitutionNationalCode,
                ReceivingInstitutionNationalCodeType = apiRequest.ReceivingInstitutionNationalCodeType,
                ReceivingInstitutionPostalCode = apiRequest.ReceivingInstitutionPostalCode,
                ReceivingInstitutionStateOrProvince = apiRequest.ReceivingInstitutionStateOrProvince,
                ReceivingInstitutionStreetAddress1 = apiRequest.ReceivingInstitutionStreetAddress1,
                ReceivingInstitutionStreetAddress2 = apiRequest.ReceivingInstitutionStreetAddress2,
                SenderToReceiverInfo1 = apiRequest.SenderToReceiverLine1,
                SenderToReceiverInfo2 = apiRequest.SenderToReceiverLine2,
                SenderToReceiverInfo3 = apiRequest.SenderToReceiverLine3,
                SenderToReceiverInfo4 = apiRequest.SenderToReceiverLine4,
                SenderToReceiverInfo5 = apiRequest.SenderToReceiverLine5,
                SenderToReceiverInfo6 = apiRequest.SenderToReceiverLine6,
                SendingInstitutionABN = apiRequest.SendingInstitutionABN,
                SendingInstitutionACN = apiRequest.SendingInstitutionACN,
                SendingInstitutionARBN = apiRequest.SendingInstitutionARBN,
                SendingInstitutionBIC = apiRequest.SendingInstitutionBIC,
                SendingInstitutionBusinessStructureTypeCode = apiRequest.SendingInstitutionBusinessStructureTypeCode,
                SendingInstitutionCity = apiRequest.SendingInstitutionCity,
                SendingInstitutionCountryCode = apiRequest.SendingInstitutionCountryCode,
                SendingInstitutionEmail = apiRequest.SendingInstitutionEmail,
                SendingInstitutionName = apiRequest.SendingInstitutionName,
                SendingInstitutionNationalCode = apiRequest.SendingInstitutionNationalCode,
                SendingInstitutionNationalCodeType = apiRequest.SendingInstitutionNationalCodeType,
                SendingInstitutionOccupationCode = apiRequest.SendingInstitutionOccupationCode,
                SendingInstitutionOccupationDescription = apiRequest.SendingInstitutionOccupationDescription,
                SendingInstitutionOccupationTypeId = apiRequest.SendingInstitutionOccupationTypeId,
                SendingInstitutionPhone = apiRequest.SendingInstitutionPhone,
                SendingInstitutionPostalCode = apiRequest.SendingInstitutionPostalCode,
                SendingInstitutionSameAsOrderingInstitution = apiRequest.SendingInstitutionSameAsOrderingInstitution,
                SendingInstitutionStateOrProvince = apiRequest.SendingInstitutionStateOrProvince,
                SendingInstitutionStreetAddress1 = apiRequest.SendingInstitutionStreetAddress1,
                SendingInstitutionStreetAddress2 = apiRequest.SendingInstitutionStreetAddress2,
                CustomerId = apiRequest.CustomerId,
                FXDealId = apiRequest.FXDealId,
                Amount = apiRequest.Amount,
                AmountCurrencyCode = apiRequest.AmountCurrencyCode,
                DestinationCountryCode = apiRequest.CountryCode,
                ValueDate = apiRequest.ValueDate,
                FeeAmount = apiRequest.FeeAmount,
                FeeAmountCurrencyCode = apiRequest.FeeAmountCurrencyCode,
                PaymentValueType = apiRequest.PaymentValueType,
                BeneficiaryExternalReference = apiRequest.BeneficiaryExternalReference,
                BeneficiaryWKYCId = apiRequest.BeneficiaryWKYCId,
                SwiftUETR = apiRequest.SwiftUETR
            };

            return request;
        }

        /// <summary>
        /// Prepare PaymentValidateRequest
        /// </summary>
        /// <returns>PaymentValidateRequest</returns>
        public ApiValidatePaymentRequest PrepareApiValidatePaymentRequest(ApiCreatePaymentRequest apiRequest)
        {
            ApiValidatePaymentRequest request = new ApiValidatePaymentRequest()
            {
                BankOperationCode = apiRequest.BankOperationCode,
                BeneficiaryABN = apiRequest.BeneficiaryABN,
                BeneficiaryACN = apiRequest.BeneficiaryACN,
                BeneficiaryARBN = apiRequest.BeneficiaryARBN,
                BeneficiaryAccountNumber = apiRequest.BeneficiaryAccountNumber,
                BeneficiaryAccountNumberPrefix = apiRequest.BeneficiaryAccountNumberPrefix,
                BeneficiaryAccountNumberSuffix = apiRequest.BeneficiaryAccountNumberSuffix,
                BeneficiaryAccountTypeCode = apiRequest.BeneficiaryAccountTypeCode,
                BeneficiaryAddress1 = apiRequest.BeneficiaryAddress1,
                BeneficiaryAddress2 = apiRequest.BeneficiaryAddress2,
                BeneficiaryAddress3 = apiRequest.BeneficiaryAddress3,
                BeneficiaryBankAddress1 = apiRequest.BeneficiaryBankAddress1,
                BeneficiaryBankAddress2 = apiRequest.BeneficiaryBankAddress2,
                BeneficiaryBankAddress3 = apiRequest.BeneficiaryBankAddress3,
                BeneficiaryBankBIC = apiRequest.BeneficiaryBankBIC,
                BeneficiaryBankCity = apiRequest.BeneficiaryBankCity,
                BeneficiaryBankCode = apiRequest.BeneficiaryBankCode,
                BeneficiaryBankCountryCode = apiRequest.BeneficiaryBankCountryCode,
                BeneficiaryBankName = apiRequest.BeneficiaryBankName,
                BeneficiaryBankNationalCode = apiRequest.BeneficiaryBankNationalCode,
                BeneficiaryBankNationalCodeType = apiRequest.BeneficiaryBankNationalCodeType,
                BeneficiaryBankPostalCode = apiRequest.BeneficiaryBankPostalCode,
                BeneficiaryBankStateOrProvince = apiRequest.BeneficiaryBankStateOrProvince,
                BeneficiaryBankStreetAddress1 = apiRequest.BeneficiaryBankStreetAddress1,
                BeneficiaryBankStreetAddress2 = apiRequest.BeneficiaryBankStreetAddress2,
                BeneficiaryBranchBIC = apiRequest.BeneficiaryBranchBIC,
                BeneficiaryBranchCity = apiRequest.BeneficiaryBranchCity,
                BeneficiaryBranchCode = apiRequest.BeneficiaryBranchCode,
                BeneficiaryBranchCountryCode = apiRequest.BeneficiaryBranchCountryCode,
                BeneficiaryBranchId = apiRequest.BeneficiaryBranchId,
                BeneficiaryBranchName = apiRequest.BeneficiaryBranchName,
                BeneficiaryBranchNationalCode = apiRequest.BeneficiaryBranchNationalCode,
                BeneficiaryBranchNationalCodeType = apiRequest.BeneficiaryBranchNationalCodeType,
                BeneficiaryBranchPostalCode = apiRequest.BeneficiaryBranchPostalCode,
                BeneficiaryBranchStateOrProvince = apiRequest.BeneficiaryBranchStateOrProvince,
                BeneficiaryBranchStreetAddress1 = apiRequest.BeneficiaryBranchStreetAddress1,
                BeneficiaryBranchStreetAddress2 = apiRequest.BeneficiaryBranchStreetAddress2,
                BeneficiaryBusinessStructureTypeCode = apiRequest.BeneficiaryBusinessStructureTypeCode,
                BeneficiaryCellPhone = apiRequest.BeneficiaryCellPhone,
                BeneficiaryCity = apiRequest.BeneficiaryCity,
                BeneficiaryCompanyRegistrationCountryCode = apiRequest.BeneficiaryCompanyRegistrationCountryCode,
                BeneficiaryCompanyRegistrationNumber = apiRequest.BeneficiaryCompanyRegistrationNumber,
                BeneficiaryCountryCode = apiRequest.BeneficiaryCountryCode,
                BeneficiaryCountryOfBirthCode = apiRequest.BeneficiaryCountryOfBirthCode,
                BeneficiaryDateOfBirth = apiRequest.BeneficiaryDateOfBirth,
                BeneficiaryEmail = apiRequest.BeneficiaryEmail,
                BeneficiaryFirstName = apiRequest.BeneficiaryFirstName,
                BeneficiaryIdentificationCountryCode = apiRequest.BeneficiaryIdentificationCountryCode,
                BeneficiaryIdentificationNumber = apiRequest.BeneficiaryIdentificationNumber,
                BeneficiaryIdentificationTypeId = apiRequest.BeneficiaryIdentificationTypeId,
                BeneficiaryInfoLine1 = apiRequest.BeneficiaryInfoLine1,
                BeneficiaryInfoLine2 = apiRequest.BeneficiaryInfoLine2,
                BeneficiaryInfoLine3 = apiRequest.BeneficiaryInfoLine3,
                BeneficiaryInfoLine4 = apiRequest.BeneficiaryInfoLine4,
                BeneficiaryLastName = apiRequest.BeneficiaryLastName,
                BeneficiaryMiddleName = apiRequest.BeneficiaryMiddleName,
                BeneficiaryName = apiRequest.BeneficiaryName,
                BeneficiaryOccupationCode = apiRequest.BeneficiaryOccupationCode,
                BeneficiaryOccupationDescription = apiRequest.BeneficiaryOccupationDescription,
                BeneficiaryOccupationTypeId = apiRequest.BeneficiaryOccupationTypeId,
                BeneficiaryPostalCode = apiRequest.BeneficiaryPostalCode,
                BeneficiaryStateOrProvince = apiRequest.BeneficiaryStateOrProvince,
                BeneficiaryStreetAddress1 = apiRequest.BeneficiaryStreetAddress1,
                BeneficiaryStreetAddress2 = apiRequest.BeneficiaryStreetAddress2,
                BeneficiaryTaxId = apiRequest.BeneficiaryTaxId,
                BeneficiaryTypeId = apiRequest.BeneficiaryTypeId,
                ChargeDetail = apiRequest.ChargeDetail,
                InitiatingInstitutionABN = apiRequest.InitiatingInstitutionABN,
                InitiatingInstitutionACN = apiRequest.InitiatingInstitutionACN,
                InitiatingInstitutionARBN = apiRequest.InitiatingInstitutionARBN,
                InitiatingInstitutionBIC = apiRequest.InitiatingInstitutionBIC,
                InitiatingInstitutionCity = apiRequest.InitiatingInstitutionCity,
                InitiatingInstitutionCountryCode = apiRequest.InitiatingInstitutionCountryCode,
                InitiatingInstitutionName = apiRequest.InitiatingInstitutionName,
                InitiatingInstitutionNationalCode = apiRequest.InitiatingInstitutionNationalCode,
                InitiatingInstitutionNationalCodeType = apiRequest.InitiatingInstitutionNationalCodeType,
                InitiatingInstitutionPostalCode = apiRequest.InitiatingInstitutionPostalCode,
                InitiatingInstitutionSameAsOrderingInstitution = apiRequest.InitiatingInstitutionSameAsOrderingInstitution,
                InitiatingInstitutionStateOrProvince = apiRequest.InitiatingInstitutionStateOrProvince,
                InitiatingInstitutionStreetAddress1 = apiRequest.InitiatingInstitutionStreetAddress1,
                InitiatingInstitutionStreetAddress2 = apiRequest.InitiatingInstitutionStreetAddress2,
                IntermediaryBankAddress1 = apiRequest.IntermediaryBankAddress1,
                IntermediaryBankAddress2 = apiRequest.IntermediaryBankAddress2,
                IntermediaryBankAddress3 = apiRequest.IntermediaryBankAddress3,
                IntermediaryBankBIC = apiRequest.IntermediaryBankBIC,
                IntermediaryBankCity = apiRequest.IntermediaryBankCity,
                IntermediaryBankCountryCode = apiRequest.IntermediaryBankCountryCode,
                IntermediaryBankName = apiRequest.IntermediaryBankName,
                IntermediaryBankNationalCode = apiRequest.IntermediaryBankNationalCode,
                IntermediaryBankNationalCodeType = apiRequest.IntermediaryBankNationalCodeType,
                IntermediaryBankPostalCode = apiRequest.IntermediaryBankPostalCode,
                IntermediaryBankStateOrProvince = apiRequest.IntermediaryBankStateOrProvince,
                IntermediaryBankStreetAddress1 = apiRequest.IntermediaryBankStreetAddress1,
                IntermediaryBankStreetAddress2 = apiRequest.IntermediaryBankStreetAddress2,
                OrderingCustomerBankBIC = apiRequest.OrderingCustomerBankBIC,
                OrderingCustomerBankCity = apiRequest.OrderingCustomerBankCity,
                OrderingCustomerBankCountryCode = apiRequest.OrderingCustomerBankCountryCode,
                OrderingCustomerBankName = apiRequest.OrderingCustomerBankName,
                OrderingCustomerBankNationalCode = apiRequest.OrderingCustomerBankNationalCode,
                OrderingCustomerBankNationalCodeType = apiRequest.OrderingCustomerBankNationalCodeType,
                OrderingCustomerBankPostalCode = apiRequest.OrderingCustomerBankPostalCode,
                OrderingCustomerBankStateOrProvince = apiRequest.OrderingCustomerBankStateOrProvince,
                OrderingCustomerBankStreetAddress1 = apiRequest.OrderingCustomerBankStreetAddress1,
                OrderingCustomerBankStreetAddress2 = apiRequest.OrderingCustomerBankStreetAddress2,
                ReasonForPayment = apiRequest.ReasonForPayment,
                ReasonForPaymentCode = apiRequest.ReasonForPaymentCode,
                ReceiverBIC = apiRequest.ReceiverBIC,
                ReceivingInstitutionBIC = apiRequest.ReceivingInstitutionBIC,
                ReceivingInstitutionCity = apiRequest.ReceivingInstitutionCity,
                ReceivingInstitutionCountryCode = apiRequest.ReceivingInstitutionCountryCode,
                ReceivingInstitutionName = apiRequest.ReceivingInstitutionName,
                ReceivingInstitutionNationalCode = apiRequest.ReceivingInstitutionNationalCode,
                ReceivingInstitutionNationalCodeType = apiRequest.ReceivingInstitutionNationalCodeType,
                ReceivingInstitutionPostalCode = apiRequest.ReceivingInstitutionPostalCode,
                ReceivingInstitutionStateOrProvince = apiRequest.ReceivingInstitutionStateOrProvince,
                ReceivingInstitutionStreetAddress1 = apiRequest.ReceivingInstitutionStreetAddress1,
                ReceivingInstitutionStreetAddress2 = apiRequest.ReceivingInstitutionStreetAddress2,
                SenderToReceiverLine1 = apiRequest.SenderToReceiverLine1,
                SenderToReceiverLine2 = apiRequest.SenderToReceiverLine2,
                SenderToReceiverLine3 = apiRequest.SenderToReceiverLine3,
                SenderToReceiverLine4 = apiRequest.SenderToReceiverLine4,
                SenderToReceiverLine5 = apiRequest.SenderToReceiverLine5,
                SenderToReceiverLine6 = apiRequest.SenderToReceiverLine6,
                SendingInstitutionABN = apiRequest.SendingInstitutionABN,
                SendingInstitutionACN = apiRequest.SendingInstitutionACN,
                SendingInstitutionARBN = apiRequest.SendingInstitutionARBN,
                SendingInstitutionBIC = apiRequest.SendingInstitutionBIC,
                SendingInstitutionBusinessStructureTypeCode = apiRequest.SendingInstitutionBusinessStructureTypeCode,
                SendingInstitutionCity = apiRequest.SendingInstitutionCity,
                SendingInstitutionCountryCode = apiRequest.SendingInstitutionCountryCode,
                SendingInstitutionEmail = apiRequest.SendingInstitutionEmail,
                SendingInstitutionName = apiRequest.SendingInstitutionName,
                SendingInstitutionNationalCode = apiRequest.SendingInstitutionNationalCode,
                SendingInstitutionNationalCodeType = apiRequest.SendingInstitutionNationalCodeType,
                SendingInstitutionOccupationCode = apiRequest.SendingInstitutionOccupationCode,
                SendingInstitutionOccupationDescription = apiRequest.SendingInstitutionOccupationDescription,
                SendingInstitutionOccupationTypeId = apiRequest.SendingInstitutionOccupationTypeId,
                SendingInstitutionPhone = apiRequest.SendingInstitutionPhone,
                SendingInstitutionPostalCode = apiRequest.SendingInstitutionPostalCode,
                SendingInstitutionSameAsOrderingInstitution = apiRequest.SendingInstitutionSameAsOrderingInstitution,
                SendingInstitutionStateOrProvince = apiRequest.SendingInstitutionStateOrProvince,
                SendingInstitutionStreetAddress1 = apiRequest.SendingInstitutionStreetAddress1,
                SendingInstitutionStreetAddress2 = apiRequest.SendingInstitutionStreetAddress2,
                CustomerId = apiRequest.CustomerId,
                FXDealId = apiRequest.FXDealId,
                Amount = apiRequest.Amount,
                AmountCurrencyCode = apiRequest.AmountCurrencyCode,
                CountryCode = apiRequest.CountryCode,
                ValueDate = apiRequest.ValueDate,
                FeeAmount = apiRequest.FeeAmount,
                FeeAmountCurrencyCode = apiRequest.FeeAmountCurrencyCode,
                PaymentValueType = apiRequest.PaymentValueType,
                BeneficiaryExternalReference = apiRequest.BeneficiaryExternalReference,
                BeneficiaryWKYCId = apiRequest.BeneficiaryWKYCId,
                SwiftUETR = apiRequest.SwiftUETR,
                BeneficiaryStreetName = apiRequest.BeneficiaryAddress1,
            };

            return request;
        }

        /// <summary>
        /// Prepare PaymentCreateRequest from ApiCreatePaymentRequest
        /// </summary>
        /// <returns>PaymentCreateRequest</returns>
        public PaymentCreateRequest PreparePaymentCreateRequest(ApiCreatePaymentRequest apiRequest)
        {
            PaymentCreateRequest request = new PaymentCreateRequest()
            {
                BankOperationCode = apiRequest.BankOperationCode,
                BeneficiaryABN = apiRequest.BeneficiaryABN,
                BeneficiaryACN = apiRequest.BeneficiaryACN,
                BeneficiaryARBN = apiRequest.BeneficiaryARBN,
                BeneficiaryAccountNumber = apiRequest.BeneficiaryAccountNumber,
                BeneficiaryAccountNumberPrefix = apiRequest.BeneficiaryAccountNumberPrefix,
                BeneficiaryAccountNumberSuffix = apiRequest.BeneficiaryAccountNumberSuffix,
                BeneficiaryAccountTypeCode = apiRequest.BeneficiaryAccountTypeCode,
                BeneficiaryAddress1 = apiRequest.BeneficiaryAddress1,
                BeneficiaryAddress2 = apiRequest.BeneficiaryAddress2,
                BeneficiaryAddress3 = apiRequest.BeneficiaryAddress3,
                BeneficiaryBankAddress1 = apiRequest.BeneficiaryBankAddress1,
                BeneficiaryBankAddress2 = apiRequest.BeneficiaryBankAddress2,
                BeneficiaryBankAddress3 = apiRequest.BeneficiaryBankAddress3,
                BeneficiaryBankBIC = apiRequest.BeneficiaryBankBIC,
                BeneficiaryBankCity = apiRequest.BeneficiaryBankCity,
                BeneficiaryBankCode = apiRequest.BeneficiaryBankCode,
                BeneficiaryBankCountryCode = apiRequest.BeneficiaryBankCountryCode,
                BeneficiaryBankName = apiRequest.BeneficiaryBankName,
                BeneficiaryBankNationalCode = apiRequest.BeneficiaryBankNationalCode,
                BeneficiaryBankNationalCodeType = apiRequest.BeneficiaryBankNationalCodeType,
                BeneficiaryBankPostalCode = apiRequest.BeneficiaryBankPostalCode,
                BeneficiaryBankStateOrProvince = apiRequest.BeneficiaryBankStateOrProvince,
                BeneficiaryBankStreetAddress1 = apiRequest.BeneficiaryBankStreetAddress1,
                BeneficiaryBankStreetAddress2 = apiRequest.BeneficiaryBankStreetAddress2,
                BeneficiaryBranchBIC = apiRequest.BeneficiaryBranchBIC,
                BeneficiaryBranchCity = apiRequest.BeneficiaryBranchCity,
                BeneficiaryBranchCode = apiRequest.BeneficiaryBranchCode,
                BeneficiaryBranchCountryCode = apiRequest.BeneficiaryBranchCountryCode,
                BeneficiaryBranchId = apiRequest.BeneficiaryBranchId,
                BeneficiaryBranchName = apiRequest.BeneficiaryBranchName,
                BeneficiaryBranchNationalCode = apiRequest.BeneficiaryBranchNationalCode,
                BeneficiaryBranchNationalCodeType = apiRequest.BeneficiaryBranchNationalCodeType,
                BeneficiaryBranchPostalCode = apiRequest.BeneficiaryBranchPostalCode,
                BeneficiaryBranchStateOrProvince = apiRequest.BeneficiaryBranchStateOrProvince,
                BeneficiaryBranchStreetAddress1 = apiRequest.BeneficiaryBranchStreetAddress1,
                BeneficiaryBranchStreetAddress2 = apiRequest.BeneficiaryBranchStreetAddress2,
                BeneficiaryBusinessStructureTypeCode = apiRequest.BeneficiaryBusinessStructureTypeCode,
                BeneficiaryCellPhone = apiRequest.BeneficiaryCellPhone,
                BeneficiaryCity = apiRequest.BeneficiaryCity,
                BeneficiaryCompanyRegistrationCountryCode = apiRequest.BeneficiaryCompanyRegistrationCountryCode,
                BeneficiaryCompanyRegistrationNumber = apiRequest.BeneficiaryCompanyRegistrationNumber,
                BeneficiaryCountryCode = apiRequest.BeneficiaryCountryCode,
                BeneficiaryCountryOfBirthCode = apiRequest.BeneficiaryCountryOfBirthCode,
                BeneficiaryDateOfBirth = apiRequest.BeneficiaryDateOfBirth,
                BeneficiaryEmail = apiRequest.BeneficiaryEmail,
                BeneficiaryFirstName = apiRequest.BeneficiaryFirstName,
                BeneficiaryIdentificationCountryCode = apiRequest.BeneficiaryIdentificationCountryCode,
                BeneficiaryIdentificationNumber = apiRequest.BeneficiaryIdentificationNumber,
                BeneficiaryIdentificationTypeId = apiRequest.BeneficiaryIdentificationTypeId,
                BeneficiaryInfoLine1 = apiRequest.BeneficiaryInfoLine1,
                BeneficiaryInfoLine2 = apiRequest.BeneficiaryInfoLine2,
                BeneficiaryInfoLine3 = apiRequest.BeneficiaryInfoLine3,
                BeneficiaryInfoLine4 = apiRequest.BeneficiaryInfoLine4,
                BeneficiaryLastName = apiRequest.BeneficiaryLastName,
                BeneficiaryMiddleName = apiRequest.BeneficiaryMiddleName,
                BeneficiaryName = apiRequest.BeneficiaryName,
                BeneficiaryOccupationCode = apiRequest.BeneficiaryOccupationCode,
                BeneficiaryOccupationDescription = apiRequest.BeneficiaryOccupationDescription,
                BeneficiaryOccupationTypeId = apiRequest.BeneficiaryOccupationTypeId,
                BeneficiaryPostalCode = apiRequest.BeneficiaryPostalCode,
                BeneficiaryStateOrProvince = apiRequest.BeneficiaryStateOrProvince,
                BeneficiaryStreetAddress1 = apiRequest.BeneficiaryStreetAddress1,
                BeneficiaryStreetAddress2 = apiRequest.BeneficiaryStreetAddress2,
                BeneficiaryTaxId = apiRequest.BeneficiaryTaxId,
                BeneficiaryTypeId = apiRequest.BeneficiaryTypeId,
                ChargeDetail = apiRequest.ChargeDetail,
                InitiatingInstitutionABN = apiRequest.InitiatingInstitutionABN,
                InitiatingInstitutionACN = apiRequest.InitiatingInstitutionACN,
                InitiatingInstitutionARBN = apiRequest.InitiatingInstitutionARBN,
                InitiatingInstitutionBIC = apiRequest.InitiatingInstitutionBIC,
                InitiatingInstitutionCity = apiRequest.InitiatingInstitutionCity,
                InitiatingInstitutionCountryCode = apiRequest.InitiatingInstitutionCountryCode,
                InitiatingInstitutionName = apiRequest.InitiatingInstitutionName,
                InitiatingInstitutionNationalCode = apiRequest.InitiatingInstitutionNationalCode,
                InitiatingInstitutionNationalCodeType = apiRequest.InitiatingInstitutionNationalCodeType,
                InitiatingInstitutionPostalCode = apiRequest.InitiatingInstitutionPostalCode,
                InitiatingInstitutionSameAsOrderingInstitution = apiRequest.InitiatingInstitutionSameAsOrderingInstitution,
                InitiatingInstitutionStateOrProvince = apiRequest.InitiatingInstitutionStateOrProvince,
                InitiatingInstitutionStreetAddress1 = apiRequest.InitiatingInstitutionStreetAddress1,
                InitiatingInstitutionStreetAddress2 = apiRequest.InitiatingInstitutionStreetAddress2,
                IntermediaryBankAddress1 = apiRequest.IntermediaryBankAddress1,
                IntermediaryBankAddress2 = apiRequest.IntermediaryBankAddress2,
                IntermediaryBankAddress3 = apiRequest.IntermediaryBankAddress3,
                IntermediaryBankBIC = apiRequest.IntermediaryBankBIC,
                IntermediaryBankCity = apiRequest.IntermediaryBankCity,
                IntermediaryBankCountryCode = apiRequest.IntermediaryBankCountryCode,
                IntermediaryBankName = apiRequest.IntermediaryBankName,
                IntermediaryBankNationalCode = apiRequest.IntermediaryBankNationalCode,
                IntermediaryBankNationalCodeType = apiRequest.IntermediaryBankNationalCodeType,
                IntermediaryBankPostalCode = apiRequest.IntermediaryBankPostalCode,
                IntermediaryBankStateOrProvince = apiRequest.IntermediaryBankStateOrProvince,
                IntermediaryBankStreetAddress1 = apiRequest.IntermediaryBankStreetAddress1,
                IntermediaryBankStreetAddress2 = apiRequest.IntermediaryBankStreetAddress2,
                OrderingCustomerBankBIC = apiRequest.OrderingCustomerBankBIC,
                OrderingCustomerBankCity = apiRequest.OrderingCustomerBankCity,
                OrderingCustomerBankCountryCode = apiRequest.OrderingCustomerBankCountryCode,
                OrderingCustomerBankName = apiRequest.OrderingCustomerBankName,
                OrderingCustomerBankNationalCode = apiRequest.OrderingCustomerBankNationalCode,
                OrderingCustomerBankNationalCodeType = apiRequest.OrderingCustomerBankNationalCodeType,
                OrderingCustomerBankPostalCode = apiRequest.OrderingCustomerBankPostalCode,
                OrderingCustomerBankStateOrProvince = apiRequest.OrderingCustomerBankStateOrProvince,
                OrderingCustomerBankStreetAddress1 = apiRequest.OrderingCustomerBankStreetAddress1,
                OrderingCustomerBankStreetAddress2 = apiRequest.OrderingCustomerBankStreetAddress2,
                ReasonForPayment = apiRequest.ReasonForPayment,
                ReasonForPaymentCode = apiRequest.ReasonForPaymentCode,
                ReceiverBIC = apiRequest.ReceiverBIC,
                ReceivingInstitutionBIC = apiRequest.ReceivingInstitutionBIC,
                ReceivingInstitutionCity = apiRequest.ReceivingInstitutionCity,
                ReceivingInstitutionCountryCode = apiRequest.ReceivingInstitutionCountryCode,
                ReceivingInstitutionName = apiRequest.ReceivingInstitutionName,
                ReceivingInstitutionNationalCode = apiRequest.ReceivingInstitutionNationalCode,
                ReceivingInstitutionNationalCodeType = apiRequest.ReceivingInstitutionNationalCodeType,
                ReceivingInstitutionPostalCode = apiRequest.ReceivingInstitutionPostalCode,
                ReceivingInstitutionStateOrProvince = apiRequest.ReceivingInstitutionStateOrProvince,
                ReceivingInstitutionStreetAddress1 = apiRequest.ReceivingInstitutionStreetAddress1,
                ReceivingInstitutionStreetAddress2 = apiRequest.ReceivingInstitutionStreetAddress2,
                SenderToReceiverInfo1 = apiRequest.SenderToReceiverLine1,
                SenderToReceiverInfo2 = apiRequest.SenderToReceiverLine2,
                SenderToReceiverInfo3 = apiRequest.SenderToReceiverLine3,
                SenderToReceiverInfo4 = apiRequest.SenderToReceiverLine4,
                SenderToReceiverInfo5 = apiRequest.SenderToReceiverLine5,
                SenderToReceiverInfo6 = apiRequest.SenderToReceiverLine6,
                SendingInstitutionABN = apiRequest.SendingInstitutionABN,
                SendingInstitutionACN = apiRequest.SendingInstitutionACN,
                SendingInstitutionARBN = apiRequest.SendingInstitutionARBN,
                SendingInstitutionBIC = apiRequest.SendingInstitutionBIC,
                SendingInstitutionBusinessStructureTypeCode = apiRequest.SendingInstitutionBusinessStructureTypeCode,
                SendingInstitutionCity = apiRequest.SendingInstitutionCity,
                SendingInstitutionCountryCode = apiRequest.SendingInstitutionCountryCode,
                SendingInstitutionEmail = apiRequest.SendingInstitutionEmail,
                SendingInstitutionName = apiRequest.SendingInstitutionName,
                SendingInstitutionNationalCode = apiRequest.SendingInstitutionNationalCode,
                SendingInstitutionNationalCodeType = apiRequest.SendingInstitutionNationalCodeType,
                SendingInstitutionOccupationCode = apiRequest.SendingInstitutionOccupationCode,
                SendingInstitutionOccupationDescription = apiRequest.SendingInstitutionOccupationDescription,
                SendingInstitutionOccupationTypeId = apiRequest.SendingInstitutionOccupationTypeId,
                SendingInstitutionPhone = apiRequest.SendingInstitutionPhone,
                SendingInstitutionPostalCode = apiRequest.SendingInstitutionPostalCode,
                SendingInstitutionSameAsOrderingInstitution = apiRequest.SendingInstitutionSameAsOrderingInstitution,
                SendingInstitutionStateOrProvince = apiRequest.SendingInstitutionStateOrProvince,
                SendingInstitutionStreetAddress1 = apiRequest.SendingInstitutionStreetAddress1,
                SendingInstitutionStreetAddress2 = apiRequest.SendingInstitutionStreetAddress2,
                CustomerId = apiRequest.CustomerId,
                FXDealId = apiRequest.FXDealId,
                Amount = apiRequest.Amount,
                AmountCurrencyCode = apiRequest.AmountCurrencyCode,
                DestinationCountryCode = apiRequest.CountryCode,
                ValueDate = apiRequest.ValueDate,
                FeeAmount = apiRequest.FeeAmount,
                FeeAmountCurrencyCode = apiRequest.FeeAmountCurrencyCode,
                PaymentValueType = apiRequest.PaymentValueType,
                BeneficiaryExternalReference = apiRequest.BeneficiaryExternalReference,
                BeneficiaryWKYCId = apiRequest.BeneficiaryWKYCId,
                SwiftUETR = apiRequest.SwiftUETR,
                BeneficiaryStreetName = apiRequest.BeneficiaryAddress1
            };

            return request;
        }
        /// <summary>
        /// Read payment details response and assign it to respective fields
        /// </summary>
        /// <param name="id">payment ID</param>
        /// <returns></returns>
        public ApiPaymentDetailsModel PrepareDetails(Guid id)
        {
            var response = Service.GetPaymentDetails(id);

            if (!response.ServiceResponse.HasErrors)
            {
                return new ApiPaymentDetailsModel()
                {
                    PaymentId = response.Payment.PaymentId,
                    Amount = Math.Round(response.Payment.Amount, response.Payment.AmountCurrencyScale),
                    AmountCurrencyCode = response.Payment.AmountCurrencyCode,
                    AmountCurrencyScale = response.Payment.AmountCurrencyScale,
                    AmountTextBare = response.Payment.AmountTextBare,
                    AmountTextBareSWIFT = response.Payment.AmountTextBareSWIFT,
                    AmountTextWithCurrencyCode = response.Payment.AmountTextWithCurrencyCode,
                    ApplicationId = response.Payment.ApplicationId,
                    ApprovedBy = response.Payment.ApprovedBy,
                    ApprovedByFullName = response.Payment.ApprovedByFullName,
                    ApprovedTime = response.Payment.ApprovedTime,
                    BankId = response.Payment.BankId,
                    BankOperationCode = response.Payment.BankOperationCode,
                    BeneficiaryABN = response.Payment.BeneficiaryABN,
                    BeneficiaryACN = response.Payment.BeneficiaryACN,
                    BeneficiaryARBN = response.Payment.BeneficiaryARBN,
                    BeneficiaryAccountNumber = response.Payment.BeneficiaryAccountNumber,
                    BeneficiaryAccountNumberPrefix = response.Payment.BeneficiaryAccountNumberPrefix,
                    BeneficiaryAccountNumberSuffix = response.Payment.BeneficiaryAccountNumberSuffix,
                    BeneficiaryAccountTypeCode = response.Payment.BeneficiaryAccountTypeCode,
                    BeneficiaryAccountTypeName = response.Payment.BeneficiaryAccountTypeName,
                    BeneficiaryAddress1 = response.Payment.BeneficiaryAddress1,
                    BeneficiaryAddress2 = response.Payment.BeneficiaryAddress2,
                    BeneficiaryAddress3 = response.Payment.BeneficiaryAddress3,
                    BeneficiaryBankAddress1 = response.Payment.BeneficiaryBankAddress1,
                    BeneficiaryBankAddress2 = response.Payment.BeneficiaryBankAddress2,
                    BeneficiaryBankAddress3 = response.Payment.BeneficiaryBankAddress3,
                    BeneficiaryBankBIC = response.Payment.BeneficiaryBankBIC,
                    BeneficiaryBankCity = response.Payment.BeneficiaryBankCity,
                    BeneficiaryBankCode = response.Payment.BeneficiaryBankCode,
                    BeneficiaryBankCountryCode = response.Payment.BeneficiaryBankCountryCode,
                    BeneficiaryBankCountryName = response.Payment.BeneficiaryBankCountryName,
                    BeneficiaryBankName = response.Payment.BeneficiaryBankName,
                    BeneficiaryBankNationalCode = response.Payment.BeneficiaryBankNationalCode,
                    BeneficiaryBankNationalCodeType = response.Payment.BeneficiaryBankNationalCodeType,
                    BeneficiaryBankNationalCodeTypeName = response.Payment.BeneficiaryBankNationalCodeTypeName,
                    BeneficiaryBankPostalCode = response.Payment.BeneficiaryBankPostalCode,
                    BeneficiaryBankStateOrProvince = response.Payment.BeneficiaryBankStateOrProvince,
                    BeneficiaryBankStateOrProvinceText = response.Payment.BeneficiaryBankStateOrProvinceText,
                    BeneficiaryBankStreetAddress1 = response.Payment.BeneficiaryBankStreetAddress1,
                    BeneficiaryBankStreetAddress2 = response.Payment.BeneficiaryBankStreetAddress2,
                    BeneficiaryBranchBIC = response.Payment.BeneficiaryBranchBIC,
                    BeneficiaryBranchCity = response.Payment.BeneficiaryBranchCity,
                    BeneficiaryBranchCode = response.Payment.BeneficiaryBranchCode,
                    BeneficiaryBranchCountryCode = response.Payment.BeneficiaryBranchCountryCode,
                    BeneficiaryBranchCountryName = response.Payment.BeneficiaryBranchCountryName,
                    BeneficiaryBranchId = response.Payment.BeneficiaryBranchId,
                    BeneficiaryBranchName = response.Payment.BeneficiaryBranchName,
                    BeneficiaryBranchNationalCode = response.Payment.BeneficiaryBranchNationalCode,
                    BeneficiaryBranchNationalCodeType = response.Payment.BeneficiaryBranchNationalCodeType,
                    BeneficiaryBranchNationalCodeTypeName = response.Payment.BeneficiaryBranchNationalCodeTypeName,
                    BeneficiaryBranchPostalCode = response.Payment.BeneficiaryBranchPostalCode,
                    BeneficiaryBranchStateOrProvince = response.Payment.BeneficiaryBranchStateOrProvince,
                    BeneficiaryBranchStateOrProvinceText = response.Payment.BeneficiaryBranchStateOrProvinceText,
                    BeneficiaryBranchStreetAddress1 = response.Payment.BeneficiaryBranchStreetAddress1,
                    BeneficiaryBranchStreetAddress2 = response.Payment.BeneficiaryBranchStreetAddress2,
                    BeneficiaryBusinessStructureTypeCode = response.Payment.BeneficiaryBusinessStructureTypeCode,
                    BeneficiaryBusinessStructureTypeName = response.Payment.BeneficiaryBusinessStructureTypeName,
                    BeneficiaryCellPhone = response.Payment.BeneficiaryCellPhone,
                    BeneficiaryCity = response.Payment.BeneficiaryCity,
                    BeneficiaryCompanyRegistrationCountryCode = response.Payment.BeneficiaryCompanyRegistrationCountryCode,
                    BeneficiaryCompanyRegistrationCountryName = response.Payment.BeneficiaryCompanyRegistrationCountryName,
                    BeneficiaryCompanyRegistrationNumber = response.Payment.BeneficiaryCompanyRegistrationNumber,
                    BeneficiaryCountryCode = response.Payment.BeneficiaryCountryCode,
                    BeneficiaryCountryName = response.Payment.BeneficiaryCountryName,
                    BeneficiaryCountryOfBirthCode = response.Payment.BeneficiaryCountryOfBirthCode,
                    BeneficiaryCountryOfBirthName = response.Payment.BeneficiaryCountryOfBirthName,
                    BeneficiaryDateOfBirth = response.Payment.BeneficiaryDateOfBirth,
                    BeneficiaryEmail = response.Payment.BeneficiaryEmail,
                    BeneficiaryFirstName = response.Payment.BeneficiaryFirstName,
                    BeneficiaryIdentificationCountryCode = response.Payment.BeneficiaryIdentificationCountryCode,
                    BeneficiaryIdentificationCountryName = response.Payment.BeneficiaryIdentificationCountryName,
                    BeneficiaryIdentificationNumber = response.Payment.BeneficiaryIdentificationNumber,
                    BeneficiaryIdentificationTypeId = response.Payment.BeneficiaryIdentificationTypeId,
                    BeneficiaryIdentificationTypeName = response.Payment.BeneficiaryIdentificationTypeName,
                    BeneficiaryInfoLine1 = response.Payment.BeneficiaryInfoLine1,
                    BeneficiaryInfoLine2 = response.Payment.BeneficiaryInfoLine2,
                    BeneficiaryInfoLine3 = response.Payment.BeneficiaryInfoLine3,
                    BeneficiaryInfoLine4 = response.Payment.BeneficiaryInfoLine4,
                    BeneficiaryLastName = response.Payment.BeneficiaryLastName,
                    BeneficiaryMiddleName = response.Payment.BeneficiaryMiddleName,
                    BeneficiaryName = response.Payment.BeneficiaryName,
                    BeneficiaryOccupationCode = response.Payment.BeneficiaryOccupationCode,
                    BeneficiaryOccupationDescription = response.Payment.BeneficiaryOccupationDescription,
                    BeneficiaryOccupationTypeId = response.Payment.BeneficiaryOccupationTypeId,
                    BeneficiaryOccupationTypeName = response.Payment.BeneficiaryOccupationTypeName,
                    BeneficiaryPostalCode = response.Payment.BeneficiaryPostalCode,
                    BeneficiaryStateOrProvince = response.Payment.BeneficiaryStateOrProvince,
                    BeneficiaryStateOrProvinceText = response.Payment.BeneficiaryStateOrProvinceText,
                    BeneficiaryStreetAddress1 = response.Payment.BeneficiaryStreetAddress1,
                    BeneficiaryStreetAddress2 = response.Payment.BeneficiaryStreetAddress2,
                    BeneficiaryTaxId = response.Payment.BeneficiaryTaxId,
                    BeneficiaryTypeId = response.Payment.BeneficiaryTypeId,
                    BeneficiaryTypeName = response.Payment.BeneficiaryTypeName,
                    ChargeDetail = response.Payment.ChargeDetail,
                    CreatedBy = response.Payment.CreatedBy,
                    CreatedByFullName = response.Payment.CreatedByFullName,
                    CreatedTime = response.Payment.CreatedTime,
                    DeletedBy = response.Payment.DeletedBy,
                    DeletedByFullName = response.Payment.DeletedByFullName,
                    DeletedTime = response.Payment.DeletedTime,
                    DestinationCountryCode = response.Payment.DestinationCountryCode,
                    DestinationCountryName = response.Payment.DestinationCountryName,
                    FXCoverDealExecutionId = response.Payment.FXCoverDealExecutionId,
                    FXCoverDealTradeId = response.Payment.FXCoverDealTradeId,
                    FXDealId = response.Payment.FXDealId,
                    FXDealReference = response.Payment.FXDealReference,
                    FXSellAmountEquivalent = response.Payment.FXSellAmountEquivalent,
                    FXSellAmountEquivalentCurrencyCode = response.Payment.FXSellAmountEquivalentCurrencyCode,
                    FXSellAmountEquivalentCurrencyScale = response.Payment.FXSellAmountEquivalentCurrencyScale,
                    FXSellAmountEquivalentTextBare = response.Payment.FXSellAmountEquivalentTextBare,
                    FXSellAmountEquivalentTextBareSWIFT = response.Payment.FXSellAmountEquivalentTextBareSWIFT,
                    FXSellAmountEquivalentTextWithCurrencyCode = response.Payment.FXSellAmountEquivalentTextWithCurrencyCode,
                    FeeAmount = response.Payment.FeeAmount,
                    FeeAmountCurrencyCode = response.Payment.FeeAmountCurrencyCode,
                    FeeAmountCurrencyScale = response.Payment.FeeAmountCurrencyScale,
                    FeeAmountTextBare = response.Payment.FeeAmountTextBare,
                    FeeAmountTextWithCurrencyCode = response.Payment.FeeAmountTextWithCurrencyCode,
                    InitiatingInstitutionABN = response.Payment.InitiatingInstitutionABN,
                    InitiatingInstitutionACN = response.Payment.InitiatingInstitutionACN,
                    InitiatingInstitutionARBN = response.Payment.InitiatingInstitutionARBN,
                    InitiatingInstitutionBIC = response.Payment.InitiatingInstitutionBIC,
                    InitiatingInstitutionCity = response.Payment.InitiatingInstitutionCity,
                    InitiatingInstitutionCountryCode = response.Payment.InitiatingInstitutionCountryCode,
                    InitiatingInstitutionCountryName = response.Payment.InitiatingInstitutionCountryName,
                    InitiatingInstitutionName = response.Payment.InitiatingInstitutionName,
                    InitiatingInstitutionNationalCode = response.Payment.InitiatingInstitutionNationalCode,
                    InitiatingInstitutionNationalCodeType = response.Payment.InitiatingInstitutionNationalCodeType,
                    InitiatingInstitutionNationalCodeTypeName = response.Payment.InitiatingInstitutionNationalCodeTypeName,
                    InitiatingInstitutionPostalCode = response.Payment.InitiatingInstitutionPostalCode,
                    InitiatingInstitutionSameAsOrderingInstitution = response.Payment.InitiatingInstitutionSameAsOrderingInstitution,
                    InitiatingInstitutionStateOrProvince = response.Payment.InitiatingInstitutionStateOrProvince,
                    InitiatingInstitutionStateOrProvinceText = response.Payment.InitiatingInstitutionStateOrProvinceText,
                    InitiatingInstitutionStreetAddress1 = response.Payment.InitiatingInstitutionStreetAddress1,
                    InitiatingInstitutionStreetAddress2 = response.Payment.InitiatingInstitutionStreetAddress2,
                    IntermediaryBankAddress1 = response.Payment.IntermediaryBankAddress1,
                    IntermediaryBankAddress2 = response.Payment.IntermediaryBankAddress2,
                    IntermediaryBankAddress3 = response.Payment.IntermediaryBankAddress3,
                    IntermediaryBankBIC = response.Payment.IntermediaryBankBIC,
                    IntermediaryBankCity = response.Payment.IntermediaryBankCity,
                    IntermediaryBankCountryCode = response.Payment.IntermediaryBankCountryCode,
                    IntermediaryBankCountryName = response.Payment.IntermediaryBankCountryName,
                    IntermediaryBankName = response.Payment.IntermediaryBankName,
                    IntermediaryBankNationalCode = response.Payment.IntermediaryBankNationalCode,
                    IntermediaryBankNationalCodeType = response.Payment.IntermediaryBankNationalCodeType,
                    IntermediaryBankNationalCodeTypeName = response.Payment.IntermediaryBankNationalCodeTypeName,
                    IntermediaryBankPostalCode = response.Payment.IntermediaryBankPostalCode,
                    IntermediaryBankStateOrProvince = response.Payment.IntermediaryBankStateOrProvince,
                    IntermediaryBankStateOrProvinceText = response.Payment.IntermediaryBankStateOrProvinceText,
                    IntermediaryBankStreetAddress1 = response.Payment.IntermediaryBankStreetAddress1,
                    IntermediaryBankStreetAddress2 = response.Payment.IntermediaryBankStreetAddress2,
                    IsDeleted = response.Payment.IsDeleted,
                    IsDownloaded = response.Payment.IsDownloaded,
                    IsReportable = response.Payment.IsReportable,
                    IsTransmitted = response.Payment.IsTransmitted,
                    NumberOfPriorCustomerPaymentsToSameBeneficiaryAccount = response.Payment.NumberOfPriorCustomerPaymentsToSameBeneficiaryAccount,
                    OrderingCustomerAccountNumber = response.Payment.OrderingCustomerAccountNumber,
                    OrderingCustomerAddress1 = response.Payment.OrderingCustomerAddress1,
                    OrderingCustomerAddress2 = response.Payment.OrderingCustomerAddress2,
                    OrderingCustomerAddress3 = response.Payment.OrderingCustomerAddress3,
                    OrderingCustomerBankAddress1 = response.Payment.OrderingCustomerBankAddress1,
                    OrderingCustomerBankAddress2 = response.Payment.OrderingCustomerBankAddress2,
                    OrderingCustomerBankAddress3 = response.Payment.OrderingCustomerBankAddress3,
                    OrderingCustomerBankBIC = response.Payment.OrderingCustomerBankBIC,
                    OrderingCustomerBankCity = response.Payment.OrderingCustomerBankCity,
                    OrderingCustomerBankCountryCode = response.Payment.OrderingCustomerBankCountryCode,
                    OrderingCustomerBankCountryName = response.Payment.OrderingCustomerBankCountryName,
                    OrderingCustomerBankNationalCode = response.Payment.OrderingCustomerBankNationalCode,
                    OrderingCustomerBankNationalCodeType = response.Payment.OrderingCustomerBankNationalCodeType,
                    OrderingCustomerBankNationalCodeTypeName = response.Payment.OrderingCustomerBankNationalCodeTypeName,
                    OrderingCustomerBankPostalCode = response.Payment.OrderingCustomerBankPostalCode,
                    OrderingCustomerBankStateOrProvince = response.Payment.OrderingCustomerBankStateOrProvince,
                    OrderingCustomerBankStateOrProvinceText = response.Payment.OrderingCustomerBankStateOrProvinceText,
                    OrderingCustomerBankStreetAddress1 = response.Payment.OrderingCustomerBankStreetAddress1,
                    OrderingCustomerBankStreetAddress2 = response.Payment.OrderingCustomerBankStreetAddress2,
                    OrderingCustomerBranchCode = response.Payment.OrderingCustomerBranchCode,
                    OrderingCustomerCity = response.Payment.OrderingCustomerCity,
                    OrderingCustomerCompanyRegistrationCountryCode = response.Payment.OrderingCustomerCompanyRegistrationCountryCode,
                    OrderingCustomerCompanyRegistrationCountryName = response.Payment.OrderingCustomerCompanyRegistrationCountryName,
                    OrderingCustomerCompanyRegistrationNumber = response.Payment.OrderingCustomerCompanyRegistrationNumber,
                    OrderingCustomerCountryCode = response.Payment.OrderingCustomerCountryCode,
                    OrderingCustomerCountryName = response.Payment.OrderingCustomerCountryName,
                    OrderingCustomerCountryOfBirthCode = response.Payment.OrderingCustomerCountryOfBirthCode,
                    OrderingCustomerCountryOfBirthName = response.Payment.OrderingCustomerCountryOfBirthName,
                    OrderingCustomerDateOfBirth = response.Payment.OrderingCustomerDateOfBirth,
                    OrderingCustomerEmail = response.Payment.OrderingCustomerEmail,
                    OrderingCustomerFirstName = response.Payment.OrderingCustomerFirstName,
                    OrderingCustomerId = response.Payment.OrderingCustomerId,
                    OrderingCustomerIdentificationCountryCode = response.Payment.OrderingCustomerIdentificationCountryCode,
                    OrderingCustomerIdentificationCountryName = response.Payment.OrderingCustomerIdentificationCountryName,
                    OrderingCustomerIdentificationNumber = response.Payment.OrderingCustomerIdentificationNumber,
                    OrderingCustomerIdentificationTypeId = response.Payment.OrderingCustomerIdentificationTypeId,
                    OrderingCustomerIdentificationTypeName = response.Payment.OrderingCustomerIdentificationTypeName,
                    OrderingCustomerLastName = response.Payment.OrderingCustomerLastName,
                    OrderingCustomerMiddleName = response.Payment.OrderingCustomerMiddleName,
                    OrderingCustomerName = response.Payment.OrderingCustomerName,
                    OrderingCustomerPhone = response.Payment.OrderingCustomerPhone,
                    OrderingCustomerPostalCode = response.Payment.OrderingCustomerPostalCode,
                    OrderingCustomerStateOrProvince = response.Payment.OrderingCustomerStateOrProvince,
                    OrderingCustomerStateOrProvinceText = response.Payment.OrderingCustomerStateOrProvinceText,
                    OrderingCustomerStreetAddress1 = response.Payment.OrderingCustomerStreetAddress1,
                    OrderingCustomerStreetAddress2 = response.Payment.OrderingCustomerStreetAddress2,
                    OrderingCustomerTypeId = response.Payment.OrderingCustomerTypeId,
                    OrderingCustomerTypeName = response.Payment.OrderingCustomerTypeName,
                    OtherReference = response.Payment.OtherReference,
                    PaymentReference = response.Payment.PaymentReference,
                    PaymentSequenceNumber = response.Payment.PaymentSequenceNumber,
                    PaymentSource = response.Payment.PaymentSource,
                    PaymentSourceId = response.Payment.PaymentSourceId,
                    PaymentStatusTypeId = response.Payment.PaymentStatusTypeId,
                    PaymentStatusTypeName = response.Payment.PaymentStatusTypeName,
                    PaymentValueType = response.Payment.PaymentValueType,
                    ProcessingBranchCode = response.Payment.ProcessingBranchCode,
                    ProcessingBranchId = response.Payment.ProcessingBranchId,
                    ProcessingBranchName = response.Payment.ProcessingBranchName,
                    ProcessingBranchPhone = response.Payment.ProcessingBranchPhone,
                    ReasonForPayment = response.Payment.ReasonForPayment,
                    ReasonForPaymentCode = response.Payment.ReasonForPaymentCode,
                    ReasonForPaymentCodeName = response.Payment.ReasonForPaymentCodeName,
                    ReceiverBIC = response.Payment.ReceiverBIC,
                    ReceivingInstitutionBIC = response.Payment.ReceivingInstitutionBIC,
                    ReceivingInstitutionCity = response.Payment.ReceivingInstitutionCity,
                    ReceivingInstitutionCountryCode = response.Payment.ReceivingInstitutionCountryCode,
                    ReceivingInstitutionCountryName = response.Payment.ReceivingInstitutionCountryName,
                    ReceivingInstitutionName = response.Payment.ReceivingInstitutionName,
                    ReceivingInstitutionNationalCode = response.Payment.ReceivingInstitutionNationalCode,
                    ReceivingInstitutionNationalCodeType = response.Payment.ReceivingInstitutionNationalCodeType,
                    ReceivingInstitutionNationalCodeTypeName = response.Payment.ReceivingInstitutionNationalCodeTypeName,
                    ReceivingInstitutionPostalCode = response.Payment.ReceivingInstitutionPostalCode,
                    ReceivingInstitutionStateOrProvince = response.Payment.ReceivingInstitutionStateOrProvince,
                    ReceivingInstitutionStateOrProvinceText = response.Payment.ReceivingInstitutionStateOrProvinceText,
                    ReceivingInstitutionStreetAddress1 = response.Payment.ReceivingInstitutionStreetAddress1,
                    ReceivingInstitutionStreetAddress2 = response.Payment.ReceivingInstitutionStreetAddress2,
                    ReleasedBy = response.Payment.ReleasedBy,
                    ReleasedByFullName = response.Payment.ReleasedByFullName,
                    ReleasedTime = response.Payment.ReleasedTime,
                    SenderToReceiverInfo1 = response.Payment.SenderToReceiverInfo1,
                    SenderToReceiverInfo2 = response.Payment.SenderToReceiverInfo2,
                    SenderToReceiverInfo3 = response.Payment.SenderToReceiverInfo3,
                    SenderToReceiverInfo4 = response.Payment.SenderToReceiverInfo4,
                    SenderToReceiverInfo5 = response.Payment.SenderToReceiverInfo5,
                    SenderToReceiverInfo6 = response.Payment.SenderToReceiverInfo6,
                    SendingInstitutionABN = response.Payment.SendingInstitutionABN,
                    SendingInstitutionACN = response.Payment.SendingInstitutionACN,
                    SendingInstitutionARBN = response.Payment.SendingInstitutionARBN,
                    SendingInstitutionBIC = response.Payment.SendingInstitutionBIC,
                    SendingInstitutionCity = response.Payment.SendingInstitutionCity,
                    SendingInstitutionCountryCode = response.Payment.SendingInstitutionCountryCode,
                    SendingInstitutionCountryName = response.Payment.SendingInstitutionCountryName,
                    SendingInstitutionEmail = response.Payment.SendingInstitutionEmail,
                    SendingInstitutionName = response.Payment.SendingInstitutionName,
                    SendingInstitutionNationalCode = response.Payment.SendingInstitutionNationalCode,
                    SendingInstitutionNationalCodeType = response.Payment.SendingInstitutionNationalCodeType,
                    SendingInstitutionNationalCodeTypeName = response.Payment.SendingInstitutionNationalCodeTypeName,
                    SendingInstitutionOccupationCode = response.Payment.SendingInstitutionOccupationCode,
                    SendingInstitutionOccupationDescription = response.Payment.SendingInstitutionOccupationDescription,
                    SendingInstitutionOccupationTypeId = response.Payment.SendingInstitutionOccupationTypeId,
                    SendingInstitutionOccupationTypeName = response.Payment.SendingInstitutionOccupationTypeName,
                    SendingInstitutionPhone = response.Payment.SendingInstitutionPhone,
                    SendingInstitutionPostalCode = response.Payment.SendingInstitutionPostalCode,
                    SendingInstitutionSameAsOrderingInstitution = response.Payment.SendingInstitutionSameAsOrderingInstitution,
                    SendingInstitutionStateOrProvince = response.Payment.SendingInstitutionStateOrProvince,
                    SendingInstitutionStateOrProvinceText = response.Payment.SendingInstitutionStateOrProvinceText,
                    SendingInstitutionStreetAddress1 = response.Payment.SendingInstitutionStreetAddress1,
                    SendingInstitutionStreetAddress2 = response.Payment.SendingInstitutionStreetAddress2,
                    SettlementMessageTypeId = response.Payment.SettlementMessageTypeId,
                    SettlementMessageTypeName = response.Payment.SettlementMessageTypeName,
                    SubmittedBy = response.Payment.SubmittedBy,
                    SubmittedByFullName = response.Payment.SubmittedByFullName,
                    SubmittedTime = response.Payment.SubmittedTime,
                    SwiftUETR = response.Payment.SwiftUETR,
                    Timestamp = response.Payment.Timestamp,
                    ValueDate = response.Payment.ValueDate,
                    VerifiedBy = response.Payment.VerifiedBy,
                    VerifiedByFullName = response.Payment.VerifiedByFullName,
                    VerifiedTime = response.Payment.VerifiedTime,
                    WKYCStatusTypeDescription = response.Payment.WKYCStatusTypeDescription,
                    WKYCStatusTypeId = response.Payment.WKYCStatusTypeId,
                    WKYCStatusTypeName = response.Payment.WKYCStatusTypeName,
                    BeneficiaryWKYCId = response.Payment.BeneficiaryWKYCId,
                    BeneficiaryExternalReference = response.Payment.BeneficiaryExternalReference
                };
            }

            return null;
        }

        /// <summary>
        /// Prepare payment search result
        /// </summary>
        public IList<ApiPayoutSearchDataModel> PreparePayoutsList()
        {
            var Payments = new List<ApiPayoutSearchDataModel>();
            var response = Service.PaymentSearch(100000000);

            if (!response.ServiceResponse.HasErrors)
            {
                // INFO: eWallet September bug edited --Payment History default is showing oldest transactions first
                //foreach (InstantPaymentSearchData data in response.Payments.OrderBy(ob => ob.CreatedTime))
                var payments = response.Payments.OrderBy(p => p.PaymentStatusTypeName).OrderByDescending(p => p.PaymentReference).OrderByDescending(p => p.CreatedTime);

                foreach (PaymentSearchData data in payments)
                {
                    ApiPayoutSearchDataModel payout = new ApiPayoutSearchDataModel()
                    {
                        PaymentId = data.PaymentId,
                        Amount = data.Amount,
                        AmountCurrencyCode = data.AmountCurrencyCode,
                        // AmountCurrencyId = data.AmountCurrencyId,
                        AmountTextBare = data.AmountTextBare,
                        AmountTextBareSWIFT = data.AmountTextBareSWIFT,
                        AmountTextWithCurrencyCode = data.AmountTextWithCurrencyCode,
                        ApprovedBy = data.ApprovedBy,
                        ApprovedByFullName = data.ApprovedByFullName,
                        ApprovedTime = data.ApprovedTime,
                        BeneficiaryName = data.BeneficiaryName,
                        CreatedBy = data.CreatedBy,
                        CreatedByFullName = data.CreatedByFullName,
                        CustomerId = data.CustomerId,
                        CustomerName = data.CustomerName,
                        DeletedBy = data.DeletedBy,
                        DeletedByFullName = data.DeletedByFullName,
                        DeletedTime = data.DeletedTime,
                        FXDealId = data.FXDealId,
                        FXDealReference = data.FXDealReference,
                        FeeAmount = data.FeeAmount,
                        FeeAmountCurrencyCode = data.FeeAmountCurrencyCode,
                        // FeeAmountCurrencyId = data.FeeAmountCurrencyId,
                        FeeAmountTextBare = data.FeeAmountTextBare,
                        FeeAmountTextWithCurrencyCode = data.FeeAmountTextWithCurrencyCode,
                        IsDeleted = data.IsDeleted,
                        NumberOfPriorPaymentsToSameBeneficiary = data.NumberOfPriorPaymentsToSameBeneficiary,
                        PaymentReference = data.PaymentReference,
                        PaymentStatusTypeId = data.PaymentStatusTypeId,
                        PaymentStatusTypeName = data.PaymentStatusTypeName,
                        ProcessingBranchId = data.ProcessingBranchId,
                        ProcessingBranchName = data.ProcessingBranchName,
                        ReleasedBy = data.ReleasedBy,
                        ReleasedByFullName = data.ReleasedByFullName,
                        ReleasedTime = data.ReleasedTime,
                        SubmittedBy = data.SubmittedBy,
                        SubmittedByFullName = data.SubmittedByFullName,
                        SubmittedTime = data.SubmittedTime,
                        SwiftUETR = data.SwiftUETR,
                        ValueDate = data.ValueDate,
                        VerifiedBy = data.VerifiedBy,
                        VerifiedByFullName = data.VerifiedByFullName,
                        VerifiedTime = data.VerifiedTime,
                        WKYCStatusTypeId = data.WKYCStatusTypeId,
                        WKYCStatusTypeName = data.WKYCStatusTypeName
                    };
                    Payments.Add(payout);
                }
            }

            return Payments;
        }

        /// <summary>
        /// Prepare ApiCreatePaymentResponse from response of PaymentCreateResponse
        /// </summary>
        /// <param name="response">DepositCreateResponse of DepositCreate</param>
        /// <returns></returns>
        public ApiCreatePaymentResponse PrepareApiCreatePaymentResponse(PaymentCreateResponse response)
        {
            var createdPayment = new ApiCreatedPayment();

            if (!response.ServiceResponse.HasErrors)
            {
                createdPayment.PaymentId = response.Payment.PaymentId;
                createdPayment.PaymentReference = response.Payment.PaymentReference;
                createdPayment.Timestamp = response.Payment.Timestamp;
            }
            else
            {
                GetErrorMessages(response.ServiceResponse.Responses);
            }

            _logger.Info("createdPayment: " + JsonConvert.SerializeObject(createdPayment));

            return (new ApiCreatePaymentResponse()
            {
                Payment = createdPayment,
                Errors = this.Errors
            });
        }

        /// <summary>
        /// Prepare PaymentDeleteRequest from ApiDeletePaymentResquest
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PaymentSubmitRequest PreparePaymentSubmitRequest(ApiSubmitPaymentResquest request)
        {
            return (new PaymentSubmitRequest()
            {
                PaymentId = request.PaymentId,
                Timestamp = request.Timestamp,
            });
        }

        /// <summary>
        /// Prepare ApiDepositCreateResponse from response of DepositCreate
        /// </summary>
        /// <param name="response">DepositCreateResponse of DepositCreate</param>
        /// <returns></returns>
        public ApiSubmitPaymentResponse PrepareApiSubmitPaymentResponse(PaymentSubmitResponse response)
        {
            var sumittedPayment = new ApiSumittedPayment();

            if (!response.ServiceResponse.HasErrors)
            {
                sumittedPayment.PaymentId = response.Payment.PaymentId;
                sumittedPayment.PaymentReference = response.Payment.PaymentReference;
                sumittedPayment.Timestamp = response.Payment.Timestamp;
            }
            else
            {
                GetErrorMessages(response.ServiceResponse.Responses);
            }

            _logger.Info("sumittedPayment: " + JsonConvert.SerializeObject(sumittedPayment));

            return (new ApiSubmitPaymentResponse()

            {
                Payment = sumittedPayment,
                Errors = this.Errors
            });
        }

        /// <summary>
        /// Prepare PaymentDeleteRequest from ApiDeletePaymentResquest
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PaymentDeleteRequest PreparePaymentDeleteRequest(ApiDeletePaymentResquest request)
        {
            return (new PaymentDeleteRequest()
            {
                PaymentId = request.PaymentId,
                Timestamp = request.Timestamp,
            });
        }

        /// <summary>
        /// Prepare ApiDepositCreateResponse from response of DepositCreate
        /// </summary>
        /// <param name="response">DepositCreateResponse of DepositCreate</param>
        /// <returns></returns>
        public ApiDeletePaymentResponse PrepareApiDeletePaymentResponse(PaymentDeleteResponse response)
        {
            var deletedPayment = new ApiDeletedPayment();

            if (!response.ServiceResponse.HasErrors)
            {
                deletedPayment.PaymentId = response.Payment.PaymentId;
                deletedPayment.PaymentReference = response.Payment.PaymentReference;
                deletedPayment.Timestamp = response.Payment.Timestamp;
            }
            else
            {
                GetErrorMessages(response.ServiceResponse.Responses);
            }

            return (new ApiDeletePaymentResponse()
            {
                Payment = deletedPayment,
                Errors = this.Errors
            });
        }
    }
}