using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Security;
using Tsg.Business.Model.Classes;
using Tsg.UI.Main.Models.Security;
using System.Security.Cryptography;
using Tsg.UI.Main.Repository;
using System.Text;
using Tsg.UI.Main.Models.Attributes;
using Autofac.Core;
using Tsg.Business.Model.TSGgpwsbeta;
using TSG.Models.APIModels.ExchangeModels;
using Newtonsoft.Json;
using System.Configuration;

namespace Tsg.UI.Main.Models
{
    /// <summary>
    /// User forgot password model
    /// </summary>
    public class UserForgotPasswordModel
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IgpService _service;
        /// <summary>
        /// Forgot password user name
        /// </summary>
        [Required]
        public string forgotPasswordUserName { get; set; }
        
        /// <summary>
        /// Forgot password email
        /// </summary>
        [Required]
        public string forgotPasswordEmail { get; set; }

        /// <summary>
        /// User forgot password constructor
        /// </summary>
        public UserForgotPasswordModel()
        {
            //_service = new IgpService(ConfigurationManager.AppSettings["kycLogin"] ?? string.Empty, ConfigurationManager.AppSettings["kycPassword"] ?? string.Empty, "");
            _service = new IgpService();
        }

        public bool? IsSuccess { get; set; }
        public string ResultText { get; set; }
        /// <summary>
        /// Reset user password
        /// </summary>
        public void ResetPassword()
        {
            // Search user by user name
            _logger.Debug("forgotPasswordUserName: " + JsonConvert.SerializeObject(forgotPasswordUserName));
            _logger.Debug(string.Format("kycLogin: {0}, kycPassword: {1} ", ConfigurationManager.AppSettings["kycLogin"], ConfigurationManager.AppSettings["kycPassword"]));
            //var authenticateResponse = _service.Authenticate(ConfigurationManager.AppSettings["kycLogin"], ConfigurationManager.AppSettings["kycPassword"]);
            //_logger.Debug("authenticateResponse: " + JsonConvert.SerializeObject(authenticateResponse));
            var user_search_response = _service.GetCustomerUsersSearch(forgotPasswordUserName);
            _logger.Debug("user_search_response: " + JsonConvert.SerializeObject(user_search_response));
            var userId = string.Empty;

            if (!user_search_response.ServiceResponse.HasErrors)
            {
                // If user found, compare the email to get the user ID
                userId = user_search_response.Users.FirstOrDefault(u => u.Email.Equals(forgotPasswordEmail, StringComparison.OrdinalIgnoreCase))?.UserId;
                _logger.Debug("userId: " + userId);
            }
            
            if(!string.IsNullOrEmpty(userId))
            {
                var reset_password_response = _service.ResetPassword(userId);
                _logger.Debug("reset_password_response: " + JsonConvert.SerializeObject(reset_password_response));
                if (!reset_password_response.ServiceResponse.HasErrors)
                {
                    // If no error
                    IsSuccess = true;
                    ResultText = "Password is successfully reset";
                }
            }
            else
            {
                _logger.Debug("User ID is null. User not found");
                IsSuccess = false;
                ResultText = "Password reset failed";
                //this.Errors.Add(new ErrorViewModel
                //{
                //    Message = "User not found"
                //});
            }
        }
    }
}