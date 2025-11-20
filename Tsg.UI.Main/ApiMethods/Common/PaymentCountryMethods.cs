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
    /// View model for Bank directory
    /// </summary>
    public class PaymentCountryMethods : BaseApiMethods
    {

        public PaymentCountryMethods(UserInfo ui) : base(ui)
        {
            _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            UserInfo = ui;
        }

        /// <summary>
        /// Get all payment countries
        /// </summary>
        /// <returns></returns>
        public IList<ApiPaymentCountryDetailsModel> All()
        {
            var countries = new List<ApiPaymentCountryDetailsModel>();

            var response = Service.GetPaymentCountries();
            if (!response.ServiceResponse.HasErrors)
            {
                countries = PreparePaymentCountriesList(response);
            }
            else
            {
                _logger.Error("PaymentCountryMethods: getting payment countries list failed: Error code : " + response.ServiceResponse.Responses[0].MessageDetails);
            }

            return countries;
        }

        /// <summary>
        /// Prepare list of payment countries
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public List<ApiPaymentCountryDetailsModel> PreparePaymentCountriesList(CountryListGetPaymentCountriesResponse response)
        {
            var countries = new List<ApiPaymentCountryDetailsModel>();

            foreach (var countryData in response.Countries)
            {
                var country = PreparePaymentCountryDetails(countryData);
                countries.Add(country);
            }
            return countries;

        }

        /// <summary>
        /// Prepare payment country details
        /// </summary>
        /// <param name="countryData"></param>
        /// <returns></returns>
        public ApiPaymentCountryDetailsModel PreparePaymentCountryDetails(CountryData countryData)
        {

            return new ApiPaymentCountryDetailsModel
            {
                BlockStatus = countryData.BlockStatus,
                CountryCode = countryData.CountryCode,
                CountryName = countryData.CountryName,
                DefaultCurrencyCode = countryData.DefaultCurrencyCode,
                IsBlocked = countryData.IsBlocked,
                IsCustomerPostalCodeRequired = countryData.IsCustomerPostalCodeRequired,
                IsEnabled = countryData.IsEnabled,
                IsIbanCountry = countryData.IsIbanCountry,
                Memo = countryData.Memo,
                SortOrder = countryData.SortOrder,
            };
        }

        private UserInfo UserInfo { get; set; }
    }
}