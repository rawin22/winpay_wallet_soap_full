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
    public class BankDirectoryMethods : BaseApiMethods
    {

        public BankDirectoryMethods(UserInfo ui) : base(ui)
        {
            _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            UserInfo = ui;
        }

        public ApiBankDetailsModel Search(string bankCode, string codeType)
        {
            var bankDetails = new ApiBankDetailsModel();

            var response = Service.BankDirectorySearch(bankCode, codeType);
            if (!response.ServiceResponse.HasErrors)
            {
                bankDetails = PrepareBankDetails(response);
            }
            else
            {
                _logger.Error("BankDirectoryMethods: Searching for bank details failed: Error code : " + response.ServiceResponse.Responses[0].MessageDetails);
            }

            return bankDetails;
        }

        //public void GetUserData(ViewProfileModel ui)
        //{
        //    try
        //    {
        //        var res = Service.GetUserData(UserInfo.UserName, UserInfo.Password);
        //        ui.Customer = res.UserSettings.OrganizationName;
        //        ui.Email = res.UserSettings.EmailAddress;
        //        ui.FirstName = res.UserSettings.FirstName;
        //        ui.LastName = res.UserSettings.LastName;
        //        ui.Username = res.UserSettings.UserName;
        //        _logger.Info("User data getted successfully");
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.Error(e);
        //    }
        //}

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

        private UserInfo UserInfo { get; set; }
        public ApiBankDetailsModel BankDetails { get; set; }
    }
}