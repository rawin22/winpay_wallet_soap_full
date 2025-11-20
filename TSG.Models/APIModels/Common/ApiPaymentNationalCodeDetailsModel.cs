using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.Common
{
    public class ApiPaymentNationalCodeDetailsModel
    {
        /// <summary>
        /// National code
        /// </summary>
        public string NationalCode { get; set; }
        /// <summary>
        /// National code description
        /// </summary>
        public string NationalCodeDescription { get; set; }
        /// <summary>
        /// National code name
        /// </summary>
        public string NationalCodeName { get; set; }

    }
}