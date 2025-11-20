using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using TSG.Models.APIModels.Common;
using TSG.Models.App_LocalResources;
using TSG.Models.ServiceModels;
using TSG.Models.ServiceModels.LimitPayment;

namespace Tsg.UI.Main.Models
{
    /// <summary>
    /// View model for Bank directory
    /// </summary>
    public class BankDirectoryViewModel : BaseViewModel
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Constructor
        /// </summary>
        public BankDirectoryViewModel()
        {
            _logger.Info("BankDirectoryViewModel, constructor");
            // BankDetails = new ApiBankDetailsModel();
        }

        /// <summary>
        /// Searching for bank details
        /// </summary>
        /// <param name="bankCode">Bank code</param>
        /// <param name="codeType">Bank code type</param>
        /// <returns></returns>
        public ApiBankDetailsModel Search(string bankCode, string codeType)
        {
            var bankDetails = new ApiBankDetailsModel();

            var response = Service.BankDirectorySearch(bankCode, codeType);
            if (!response.ServiceResponse.HasErrors)
            {
                HasError = false;
                bankDetails = PrepareBankDetails(response);
            }
            else
            {
                this.GetErrorMessages(response.ServiceResponse.Responses);
                _logger.Error(JsonConvert.SerializeObject(Errors));
            }

            return bankDetails;
        }

        /// <summary>
        /// Prepare ApiBankDetailsModel from BankDirectorySearchResponse
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public ApiBankDetailsModel PrepareBankDetails(BankDirectorySearchResponse response)
        {
            return new ApiBankDetailsModel
            {
                BIC = response.BankInformation.BIC,
                BankCode = response.BankInformation.BankCode,
                BankName = response.BankInformation.BankName,
                BranchName = response.BankInformation.BranchName,
                City = response.BankInformation.City,
                CountryCode = response.BankInformation.CountryCode,
                CountryName = response.BankInformation.CountryName,
                IsACH = response.BankInformation.IsACH,
                StateOrProvince = response.BankInformation.StateOrProvince,
                StateOrProvinceName = response.BankInformation.StateOrProvinceName,
                StreetAddress1 = response.BankInformation.StreetAddress1,
                StreetAddress2 = response.BankInformation.StreetAddress2,
                SwiftAddress1 = response.BankInformation.SwiftAddress1,
                SwiftAddress2 = response.BankInformation.SwiftAddress2,
                SwiftAddress3 = response.BankInformation.SwiftAddress3
            };
        }

        /// <summary>
        /// Bank information
        /// </summary>
        public ApiBankDetailsModel BankInformation { get; set; }
    }
}