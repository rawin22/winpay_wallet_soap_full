using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using TSG.Models.APIModels;
using TSG.Models.APIModels.Common;
using TSG.Models.App_LocalResources;
using TSG.Models.ServiceModels;
using TSG.Models.ServiceModels.LimitPayment;

namespace Tsg.UI.Main.ApiMethods.Common
{
    /// <summary>
    /// View model for payment national codes
    /// </summary>
    public class PaymentNationalCodeMethods : BaseApiMethods
    {

        public PaymentNationalCodeMethods(UserInfo ui) : base(ui)
        {
            _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            UserInfo = ui;
        }

        /// <summary>
        /// Get all payment countries
        /// </summary>
        /// <returns></returns>
        public IList<ApiPaymentNationalCodeDetailsModel> All()
        {
            var countries = new List<ApiPaymentNationalCodeDetailsModel>();

            var response = Service.GetPaymentNationalCodes();
            if (!response.ServiceResponse.HasErrors)
            {
                countries = PreparePaymentNationalCodesList(response);
            }
            else
            {
                _logger.Error("PaymentNationalCodeMethods: getting payment national codes list failed: Error code : " + response.ServiceResponse.Responses[0].MessageDetails);
            }

            return countries;
        }

        /// <summary>
        /// Get all payment countries
        /// </summary>
        /// <returns></returns>
        public IList<ApiPaymentNationalCodeDetailsModel> ByCurrency(string currencyCode)
        {
            var countries = new List<ApiPaymentNationalCodeDetailsModel>();

            var response = Service.GetPaymentNationalCodes(currencyCode);
            if (!response.ServiceResponse.HasErrors)
            {
                countries = PreparePaymentNationalCodesList(response);
            }
            else
            {
                _logger.Error("PaymentNationalCodeMethods: getting payment national codes list failed: Error code : " + response.ServiceResponse.Responses[0].MessageDetails);
            }

            return countries;
        }

        /// <summary>
        /// Prepare list of payment countries
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public List<ApiPaymentNationalCodeDetailsModel> PreparePaymentNationalCodesList(NationalCodeListGetAllResponse response)
        {
            var nationalCodes = new List<ApiPaymentNationalCodeDetailsModel>();

            foreach (var nationalCodeData in response.NationalCodes)
            {
                var nationalCode = PreparePaymentNationalCodeDetails(nationalCodeData);
                nationalCodes.Add(nationalCode);
            }
            return nationalCodes;

        }

        /// <summary>
        /// Prepare list of payment countries
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public List<ApiPaymentNationalCodeDetailsModel> PreparePaymentNationalCodesList(NationalCodeListGetAllForCurrencyResponse response)
        {
            var nationalCodes = new List<ApiPaymentNationalCodeDetailsModel>();

            foreach (var nationalCodeData in response.NationalCodes)
            {
                var nationalCode = PreparePaymentNationalCodeDetails(nationalCodeData);
                nationalCodes.Add(nationalCode);
            }
            return nationalCodes;

        }

        /// <summary>
        /// Prepare payment country details
        /// </summary>
        /// <param name="nationalCodeData"></param>
        /// <returns></returns>
        public ApiPaymentNationalCodeDetailsModel PreparePaymentNationalCodeDetails(NationalCodeData nationalCodeData)
        {

            return new ApiPaymentNationalCodeDetailsModel
            {
                NationalCode = nationalCodeData.NationalCode,
                NationalCodeDescription = nationalCodeData.NationalCodeDescription,
                NationalCodeName = nationalCodeData.NationalCodeName
            };
        }

        private UserInfo UserInfo { get; set; }
    }
}