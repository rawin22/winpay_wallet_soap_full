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
using TSG.Models.APIModels.ExchangeModels;
using TSG.Models.APIModels.InstantPayment;
using TSG.Models.APIModels.Payment;

namespace Tsg.UI.Main.ApiMethods.FX
{
    /// <summary>
    /// Model for payout API
    /// </summary>
    public class FxQuoteMethods : BaseApiMethods
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
        public FxQuoteMethods(UserInfo ui) : base(ui)
        {
            _userInfo = ui;
            _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            Errors = new List<ApiErrorModel>();
        }

        /// <summary>
        /// Create payout
        /// </summary>
        /// <param name="request">Create Payout Request</param>
        public ApiFxQuoteCreateResponse Create(ApiFxQuoteCreateRequest request)
        {

            var response = Service.FxDealQuoteCreate(PrepareFXDealQuoteCreateRequest(request));

            return (PrepareApiFxQuoteCreateResponse(response));
            
        }

        /// <summary>
        /// Create payout
        /// </summary>
        /// <param name="request">Create Payout Request</param>
        public ApiFxQuoteBookResponse Book(ApiFxQuoteBookRequest request)
        {

            var response = Service.FxDealQuoteBook(request.QuoteId);

            return (PrepareApiFxQuoteBookResponse(response));

        }

        public FXDealQuoteCreateRequest PrepareFXDealQuoteCreateRequest(ApiFxQuoteCreateRequest request)
        {
            return (new FXDealQuoteCreateRequest()
            {
                BuyCCY = request.BuyCCY,
                CustomerId = request.CustomerId,
                SellCCY = request.SellCCY,
                Amount = request.Amount,
                AmountCCY = request.AmountCCY,
                DealType = request.DealType,
                WindowOpenDate = request.WindowOpenDate,
                FinalValueDate = request.FinalValueDate,
                IsForCurrencyCalculator = request.IsForCurrencyCalculator,                                
            });
        }

        /// <summary>
        /// Prepare ApiFxQuoteCreateResponse from response of FXDealQuoteCreate
        /// </summary>
        /// <param name="response">FXDealQuoteCreateResponse of FXDealQuoteCreate</param>
        /// <returns></returns>
        public ApiFxQuoteCreateResponse PrepareApiFxQuoteCreateResponse(FXDealQuoteCreateResponse response)
        {
            var quote = new ApiFxQuote();

            if (!response.ServiceResponse.HasErrors)
            {
                quote.QuoteId = response.Quote.QuoteId;
                quote.QuoteReference = response.Quote.QuoteReference;
                quote.QuoteSequenceNumber = response.Quote.QuoteSequenceNumber;
                quote.CustomerAccountNumber = response.Quote.CustomerAccountNumber;
                quote.DealType = response.Quote.DealType;
                quote.BuyAmount = response.Quote.BuyAmount;
                quote.BuyCurrencyCode = response.Quote.BuyCurrencyCode;
                quote.SellAmount = response.Quote.SellAmount;
                quote.SellCurrencyCode = response.Quote.SellCurrencyCode;
                quote.Rate = response.Quote.Rate;
                quote.RateFormat = response.Quote.RateFormat;
                quote.DealDate = response.Quote.DealDate;
                quote.ValueDate = response.Quote.ValueDate;
                quote.QuoteTime = response.Quote.QuoteTime;
                quote.ExpirationTime = response.Quote.ExpirationTime;
                quote.IsForCurrencyCalculator = response.Quote.IsForCurrencyCalculator;
            }
            else
            {
                GetErrorMessages(response.ServiceResponse.Responses);
            }

            return (new ApiFxQuoteCreateResponse()
            {
                Quote = quote,
                Errors = this.Errors
            });
        }


        /// <summary>
        /// Prepare ApiFxQuoteCreateResponse from response of FXDealQuoteCreate
        /// </summary>
        /// <param name="response">FXDealQuoteCreateResponse of FXDealQuoteCreate</param>
        /// <returns></returns>
        public ApiFxQuoteBookResponse PrepareApiFxQuoteBookResponse(FXDealQuoteBookResponse response)
        {
            var fxDeal = new apiFxQuoteBookFxDeal();

            if (!response.ServiceResponse.HasErrors)
            {
                fxDeal.FXDealId = response.FXDeal.FXDealId;
                fxDeal.FXDealReference = response.FXDeal.FXDealReference;
                fxDeal.FXDealSequenceNumber = response.FXDeal.FXDealSequenceNumber;
            }
            else
            {
                GetErrorMessages(response.ServiceResponse.Responses);
            }

            return (new ApiFxQuoteBookResponse()
            {
                FxDeal = fxDeal,
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