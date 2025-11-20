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
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.APIModels.Fundings;
using TSG.Models.APIModels.InstantPayment;
using TSG.Models.APIModels.Payment;
using TSG.Models.APIModels.Wire_Instructions;

namespace Tsg.UI.Main.ApiMethods.Fundings
{
    /// <summary>
    /// Model for deposit API
    /// </summary>
    public class DepositMethods : BaseApiMethods
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
        public DepositMethods(UserInfo ui) : base(ui)
        {
            _userInfo = ui;
            _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            Errors = new List<ApiErrorModel>();
        }

        /// <summary>
        /// Create deposit
        /// </summary>
        /// <param name="request">Create Payout Request</param>
        public ApiDepositCreateResponse Create(ApiDepositCreateRequest request)
        {

            var response = Service.CreateDeposit(PrepareDepositCreateRequest(request));

            return (PrepareApiDepositCreateResponse(response));
            
        }

        /// <summary>
        /// Prepare DepositCreateRequest from ApiDepositCreateRequest
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DepositCreateRequest PrepareDepositCreateRequest(ApiDepositCreateRequest request)
        {
            return (new DepositCreateRequest()
            {
                CustomerId = request.CustomerId,
                Amount = request.Amount,
                FXDealId = request.FXDealId,
                AmountCurrencyCode = request.AmountCurrencyCode,
                FeeAmount = request.FeeAmount,
                FeeAmountCurrencyCode = request.FeeAmountCurrencyCode,
                ValueDate = request.ValueDate,
                Reference = request.Reference,
                CustomerMemo = request.CustomerMemo,
                BankMemo = request.BankMemo
            });
        }

        /// <summary>
        /// Prepare ApiDepositCreateResponse from response of DepositCreate
        /// </summary>
        /// <param name="response">DepositCreateResponse of DepositCreate</param>
        /// <returns></returns>
        public ApiDepositCreateResponse PrepareApiDepositCreateResponse(DepositCreateResponse response)
        {
            var depositInfo = new ApiCreatedDepositInformation();

            if (!response.ServiceResponse.HasErrors)
            {
                depositInfo.DepositId = response.DepositInformation.DepositId;
                depositInfo.DepositReference = response.DepositInformation.DepositReference;
                depositInfo.Timestamp = response.DepositInformation.Timestamp;
            }
            else
            {
                GetErrorMessages(response.ServiceResponse.Responses);
            }

            return (new ApiDepositCreateResponse()
            {
                DepositInformation = depositInfo,
                Errors = this.Errors
            });
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
    }
}