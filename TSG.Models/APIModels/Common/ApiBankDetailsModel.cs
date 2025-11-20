using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.Common
{
    public class ApiBankDetailsModel
    {
        /// <summary>
        /// BIC
        /// </summary>
        public string BIC { get; set; }
        /// <summary>
        /// Bank code
        /// </summary>
        public string BankCode { get; set; }
        /// <summary>
        /// Bank name
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// Branch name
        /// </summary>
        public string BranchName { get; set; }
        /// <summary>
        /// City
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Country code
        /// </summary>
        public string CountryCode { get; set; }
        /// <summary>
        /// Country name
        /// </summary>
        public string CountryName { get; set; }
        /// <summary>
        /// Is ACH
        /// </summary>
        public bool IsACH { get; set; }
        /// <summary>
        /// State or province
        /// </summary>
        public string StateOrProvince { get; set; }
        /// <summary>
        /// State or province name
        /// </summary>
        public string StateOrProvinceName { get; set; }
        /// <summary>
        /// Street address 1
        /// </summary>
        public string StreetAddress1 { get; set; }
        /// <summary>
        /// Street address 2
        /// </summary>
        public string StreetAddress2 { get; set; }
        /// <summary>
        /// Swift address 1
        /// </summary>
        public string SwiftAddress1 { get; set; }
        /// <summary>
        /// Swift address 2
        /// </summary>
        public string SwiftAddress2 { get; set; }
        /// <summary>
        /// Swift address 3
        /// </summary>
        public string SwiftAddress3 { get; set; }
    }
}