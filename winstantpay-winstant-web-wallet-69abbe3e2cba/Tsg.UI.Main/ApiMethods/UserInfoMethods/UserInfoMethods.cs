using System;
using System.Collections.Generic;
using System.Linq;
using Tsg.Business.Model.TsgGPWebService;
using TSG.Models.APIModels.UserInformation;
using Tsg.UI.Main.Models.Security;
using TSG.Models.APIModels;
using Newtonsoft.Json;

namespace Tsg.UI.Main.ApiMethods.UserInfoMethods
{
    public class UserInfoMethods : BaseApiMethods
    {
        private UserInfo UserInfo { get; set; }
        public UserInfoMethods(UserInfo ui) : base(ui)
        {
            _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            UserInfo = ui;
        }

        public List<string> GetUserAliases()
        {
            var res = new List<string>();
            try
            {
                res = Service.GetAccountAliases()?.Aliases.Select(s => s.AccountAlias).ToList();
                _logger.Info("List of WPayIds getted successfully");
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            return res;
        }

        public void GetUserData(ViewProfileModel ui)
        {
            try
            {
                var res = Service.GetUserData(UserInfo.UserName, UserInfo.Password);
                ui.Customer = res.UserSettings.OrganizationName;
                ui.Email = res.UserSettings.EmailAddress;
                ui.FirstName = res.UserSettings.FirstName;
                ui.LastName = res.UserSettings.LastName;
                ui.Username = res.UserSettings.UserName;
                _logger.Info("User data getted successfully");
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        public bool AddNewAlias(string newAliasName, out string res)
        {
            try
            {
                var addNewAliasRes = Service.AddNewUserAliases(newAliasName);
                res = addNewAliasRes.ServiceResponse.Responses[0]?.Message;
                return !addNewAliasRes.ServiceResponse.HasErrors;
            }
            catch (Exception e)
            {
                res = e.Message;
                return false;
            }
        }


        public UserPasswordChangeResponse ChangePassword(string oldPassword, string newPassword)
        {
            return  Service.ChangePassword(oldPassword, newPassword);
        }


        public List<string> GetLimitTabs()
        {
            var res = new List<string>();
            try
            {
                res = Service.GetAccountAliases()?.Aliases.Select(s => s.AccountAlias).ToList();
                _logger.Info("List of WPayIds getted successfully");
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            return res;
        }

        /// <summary>
        /// Delete user alias
        /// </summary>
        /// <param name="alias">Alias to add</param>
        /// <param name="res">output message</param>
        /// <returns></returns>
        public bool DeleteAlias(string alias, out string res)
        {
            try
            {
                var deleteAliasRes = Service.DeleteUserAlias(alias);
                res = deleteAliasRes.ServiceResponse.Responses[0]?.Message;                
                return !deleteAliasRes.ServiceResponse.HasErrors;
            }
            catch (Exception e)
            {
                res = e.Message;
                return false;
            }
        }
    }
}