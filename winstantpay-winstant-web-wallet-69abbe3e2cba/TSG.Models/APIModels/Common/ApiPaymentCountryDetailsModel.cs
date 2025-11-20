using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.Common
{
    public class ApiPaymentCountryDetailsModel
    {
        /// <summary>
        /// Block status
        /// </summary>
        public int BlockStatus { get; set; }
        /// <summary>
        /// Country code
        /// </summary>
        public string CountryCode { get; set; }
        /// <summary>
        /// Country name
        /// </summary>
        public string CountryName { get; set; }
        /// <summary>
        /// Default currency code
        /// </summary>
        public string DefaultCurrencyCode { get; set; }
        /// <summary>
        /// Is blocked
        /// </summary>
        public bool IsBlocked { get; set; }
        /// <summary>
        /// Is customer postal code required
        /// </summary>
        public bool IsCustomerPostalCodeRequired { get; set; }
        /// <summary>
        /// Is enabled
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// Is IBAN Country
        /// </summary>
        public bool IsIbanCountry { get; set; }
        /// <summary>
        /// Memo
        /// </summary>
        public string Memo { get; set; }
        /// <summary>
        /// Sort order
        /// </summary>
        public int SortOrder { get; set; }
    }
}