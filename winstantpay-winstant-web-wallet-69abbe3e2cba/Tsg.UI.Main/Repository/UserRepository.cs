using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using TSG.Models.APIModels;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;
using Crypto = WinstantPay.Common.CryptDecriptInfo.Crypto;


namespace Tsg.UI.Main.Repository
{
    public class UserRepository
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private SqlConnection _con;

        public UserInfo CurrentUserInfo { get; set; }

        private void Connection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["ewalletDbCon"].ToString();
            _con = new SqlConnection(conStr);
        }

        public bool AddAdmin(AdminModel obj)
        {
            Connection();
            SqlCommand com = new SqlCommand("AddOrUpdateAdmin", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@admUserName", obj.Username);
            com.Parameters.AddWithValue("@admFirstName", obj.FirstName);
            com.Parameters.AddWithValue("@admLastName", obj.LastName);
            com.Parameters.AddWithValue("@admEmail", obj.MailAddress);
            com.Parameters.AddWithValue("@admLinkAddress", obj.LinkText);
            com.Parameters.AddWithValue("@admLinkOutId", DbType.Int32).Direction = ParameterDirection.Output;

            try
            {
                _logger.Info("Attempting to add user info to database. Username=" + obj.Username);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();

                if (i >= 1)
                {
                    _logger.Info("User info is added to database. Username=" + obj.Username);
                    return true;
                }
                else
                {
                    _logger.Error("Could not add user info to database");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return false;
        }

        public bool AddAdmin(AdminModel obj, string pass)
        {
            Connection();
            SqlCommand com = new SqlCommand("AddOrUpdateAdmin", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@admLinkId", obj.Id);
            com.Parameters.AddWithValue("@admUserName", obj.Username);
            com.Parameters.AddWithValue("@admFirstName", obj.FirstName);
            com.Parameters.AddWithValue("@admLastName", obj.LastName);
            com.Parameters.AddWithValue("@admEmail", obj.MailAddress);
            com.Parameters.AddWithValue("@admLinkAddress", obj.LinkText);
            com.Parameters.AddWithValue("@admLinkPassword", pass);
            com.Parameters.AddWithValue("@admLinkOutId", DbType.Int32).Direction = ParameterDirection.Output;

            try
            {
                _logger.Info("Attempting to add user info to database. Username=" + obj.Username);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();

                if (i >= 1)
                {
                    _logger.Info("User info is added to database. Username=" + obj.Username);
                    return true;
                }
                else
                {
                    _logger.Error("Could not add user info to database");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return false;
        }
        
        public DateTime GetLastLoginDate(string username)
        {
            Connection();
            SqlCommand com = new SqlCommand("GetUserLastLoginDate", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@username", username);
            SqlParameter returnValue = com.Parameters.Add("@lastLoginDate", SqlDbType.DateTime);
            returnValue.Direction = ParameterDirection.ReturnValue;

            try
            {
                _logger.Info("Attempting to get user last login date from database. Username=" + username);
                _con.Open();
                com.ExecuteNonQuery();
                _con.Close();
                if (returnValue.Value != DBNull.Value)
                {
                    _logger.Info("Successfully read last login date. Last login date=" + (DateTime)returnValue.Value + ", Username=" + username);
                    return (DateTime)returnValue.Value;
                }
                else
                {
                    _logger.Error("Could not read last login date. Returning current date=" + DateTime.Now);
                    return DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

            return DateTime.Now;
        }

        public List<UserModel> GetUsers()
        {
            Connection();
            List<UserModel> userList = new List<UserModel>();
            SqlCommand com = new SqlCommand("GetUsers", _con);
            com.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();
            try
            {
                _logger.Info("Attempting to get all users from database");
                _con.Open();
                da.Fill(dt);
                _con.Close();
                _logger.Info("List of all users successfully obtained from database");

                userList = (from DataRow dr in dt.Rows
                            select new UserModel()
                            {
                                Username = Convert.ToString(dr["username"]),
                                UserPassword = dr["password"] != DBNull.Value ? dr["password"].ToString() : String.Empty,
                                FirstName = dr["firstName"] != DBNull.Value ? dr["firstName"].ToString() : String.Empty,
                                LastName = dr["lastName"] != DBNull.Value ? dr["lastName"].ToString() : String.Empty,
                                WelcomeMessageId = dr["wlcMsgId"] != DBNull.Value ? Convert.ToInt32(dr["wlcMsgId"]) : 0,
                                DefWlcMsgAssigned = dr["wlcMsgId"] != DBNull.Value && Convert.ToInt32(dr["wlcMsgId"]) > 0,
                                IsLocal = Convert.ToBoolean(dr["isLocal"]),
                                RoleId = dr["roleId"] != DBNull.Value ? Convert.ToInt32(dr["roleId"]) : 0,
                                Role = dr["roleName"] != DBNull.Value ? dr["roleName"].ToString() : "",
                                RoleName = dr["roleName"] != DBNull.Value ? dr["roleName"].ToString() : "",
                                LastLoginDate = dr["lastLoginDate"] != DBNull.Value ? Convert.ToDateTime(dr["lastLoginDate"]) : DateTime.Now,
                                WelcomeMessageText = dr["wlcMsgText"] != DBNull.Value ? dr["wlcMsgText"].ToString() : "",
                                UserMail = dr["userMail"] != DBNull.Value ? dr["userMail"].ToString() : "",
                                IsNeedToShowWm = dr["needToSearchWelcomeMessage"] == DBNull.Value || Convert.ToBoolean(dr["needToSearchWelcomeMessage"]),
                                IsBlockLocal = false
                            }).ToList();
                return userList;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return null;
        }

        public List<AdminModel> GetAdminLink()
        {
            Connection();
            List<AdminModel> userList = new List<AdminModel>();
            SqlCommand com = new SqlCommand("GetAdminLink", _con);
            com.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();
            try
            {
                _logger.Info("Attempting to get all users from database");
                _con.Open();
                da.Fill(dt);
                _con.Close();
                _logger.Info("List of all users successfully obtained from database");

                userList = (from DataRow dr in dt.Rows
                            select new AdminModel()
                            {
                                Id = Convert.ToInt32(dr["id"]),
                                Username = Convert.ToString(dr["username"]),
                                FirstName = dr["firstName"] != DBNull.Value ? dr["firstName"].ToString() : String.Empty,
                                LastName = dr["lastName"] != DBNull.Value ? dr["lastName"].ToString() : String.Empty,
                                LinkText = dr["LinkAddress"] != DBNull.Value ? dr["LinkAddress"].ToString() : String.Empty,
                                ActivationDate = dr["ActivationDate"] != DBNull.Value ? Convert.ToDateTime(dr["ActivationDate"].ToString()) : default(DateTime),
                                LinkStatus = dr["StatusLink"] != DBNull.Value ? Convert.ToInt32(dr["StatusLink"].ToString()) : -1,
                                CreationDate = dr["CreationDate"] != DBNull.Value ? Convert.ToDateTime(dr["CreationDate"].ToString()) : default(DateTime)
                            }).ToList();
                return userList;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return null;
        }

        public string GetUserWelcomeMessage(string username)
        {
            Connection();
            SqlCommand com = new SqlCommand("GetUserWelcomeMessage", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@username", username);
            com.Parameters.AddWithValue("@lang", CultureInfo.CurrentCulture.TwoLetterISOLanguageName);

            SqlParameter returnValue = com.Parameters.Add("@wlcMsgText", SqlDbType.NVarChar);
            returnValue.Direction = ParameterDirection.ReturnValue;
            try
            {
                _logger.Info("Attempting to get welcome message. Username=" + username);
                _con.Open();
                com.ExecuteNonQuery();
                _con.Close();
                _logger.Info("Welcome message is successfully obtained. Username=" + username);
                return returnValue.Value.ToString();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return null;
        }

        public bool GetWelcomeMessageStatus(string username)
        {
            if (String.IsNullOrEmpty(username))
                return true;

            Connection();
            SqlCommand com = new SqlCommand("IsNeedToShowWelcMessShow", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@username", username);
            
            SqlParameter returnValue = com.Parameters.Add("@isNeedToShow", SqlDbType.Bit);
            returnValue.Direction = ParameterDirection.Output;
            try
            {
                _logger.Info("Attempting to get welcome message. Username=" + username);
                _con.Open();
                com.ExecuteNonQuery();
                _con.Close();
                _logger.Info("Welcome message is successfully obtained. Username=" + username);
                return returnValue.IsNullable || Convert.ToBoolean(returnValue.Value);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return true;
        }

        public string GetHashedPassword(string username)
        {
            Connection();
            SqlCommand com = new SqlCommand("GetUserHashedPassword", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@username", username);
            SqlParameter returnValue = com.Parameters.Add("@hashedPasswd", SqlDbType.VarChar);
            returnValue.Direction = ParameterDirection.ReturnValue;

            try
            {
                _con.Open();
                com.ExecuteNonQuery();
                _con.Close();
                return returnValue.Value.ToString();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return null;
        }

        public bool ChangePassword(UserLoginModel obj, string hashedPassword)
        {
            Connection();
            SqlCommand com = new SqlCommand("ChangePassword", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@username", obj.Username);
            com.Parameters.AddWithValue("@hashedPassword", hashedPassword);
            try
            {

                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    _logger.Info("Password changed");
                    return true;
                }
                else
                {
                    _logger.Info("Password doesn't changed");
                    return false;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return false;
            }
        }

        public bool ChangeWmStatuses(UserInfo ui, bool statuses)
        {
            var result = false;
            try
            {
                if (ui == null) { _logger.Error($"UserInfo is null"); return false; }
                Connection();
                SqlCommand com = new SqlCommand("ChangeWMStatuses", _con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@userId", ui.UserName);
                com.Parameters.AddWithValue("@status", statuses);

                try
                {
                    _logger.Info($"Welcome message for username={ui.UserName}" + (statuses ? "activation" : "deactivation") + " begin");
                    _con.Open();
                    int res = com.ExecuteNonQuery();
                    _con.Close();
                    _logger.Info($"Welcome message for username={ui.UserName}" + (statuses ? "activated" : "deactivated") + (res > 0 ? "success" : "unsuccess"));
                    result = res > 0;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                }

                result = true;
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            return result;
        }


        public bool ChangeWmStatuses(bool statuses)
        {
            var result = false;
            try
            {
                Connection();
                SqlCommand com = new SqlCommand("ChangeWMStatuses", _con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@userId", AppSecurity.CurrentUser.UserName);
                com.Parameters.AddWithValue("@status", statuses);

                try
                {
                    _logger.Info($"Welcome message for username={AppSecurity.CurrentUser.UserName}" +  (statuses ? "activation":"deactivation")+" begin" );
                    _con.Open();
                    int res = com.ExecuteNonQuery();
                    _con.Close();
                    _logger.Info($"Welcome message for username={AppSecurity.CurrentUser.UserName}" + (statuses ? "activated" : "deactivated") + (res > 0 ? "success": "unsuccess"));
                    result = res > 0;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                }

                result = true;
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            return result;
        }
       
        #region ApiMethods

        public bool CreateUserToken(ref ExtendedLoginParameters ulm)
        {
            Connection();
            SqlCommand com = new SqlCommand("AddUserToken", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@UserName", ulm.UserInformation.UserName);
            com.Parameters.AddWithValue("@Password", ulm.UserInformation.Password);
            com.Parameters.AddWithValue("@UserId", ulm.UserInformation.UserId);
            com.Parameters.AddWithValue("@TokenKey", ulm.TokenKey);
            com.Parameters.AddWithValue("@TokenId", ulm.TokenId);
            com.Parameters.AddWithValue("@CreationDate", ulm.CreationDate);
            com.Parameters.AddWithValue("@MerchantAppIdentificator", ulm.MerchatAppIdentificator);
            SqlParameter returnValue = com.Parameters.Add("@ExpiredDate", SqlDbType.DateTime);
            returnValue.Direction = ParameterDirection.Output;
            ulm.ExpiredDate = default(DateTime);
            try
            {
                _logger.Info(String.Format("Start generation new user token by Username [{0}]", ulm.UserInformation.UserName));
                _con.Open();
                com.ExecuteNonQuery();
                _con.Close();
                _logger.Info(String.Format("Generated new user token by Username [{0}]", ulm.UserInformation.UserName));
                if (returnValue.IsNullable)
                    return false;
                ulm.ExpiredDate = Convert.ToDateTime(returnValue.Value);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return false;
            }
            return true;
        }

        public bool IsVerificatedUser(string tokenKey)
        {
            var currentUserInfo = new UserInfo();
            return IsVerificatedUser(tokenKey, out currentUserInfo);
        }

        public bool IsVerificatedUser(string tokenKey, out UserInfo userLogin)
        {
            Connection();
            userLogin = new UserInfo();

            SqlCommand com = new SqlCommand("GetApiUserById", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@TokenKey", tokenKey);
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();

            try
            {
                _logger.Info("Start method for getting user parameters");
                _con.Open();
                da.Fill(dt);
                _con.Close();
                _logger.Info("Finish method for getting user parameters");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }


            var userLoginEx = (from DataRow dr in dt.Rows
                               select new ExtendedLoginParameters()
                               {
                                   UserInformation = new UserInfo()
                                   {
                                       UserName = dr["UserName"].ToString(),
                                       Password = dr["Password"].ToString(),
                                       UserId = dr["UserId"].ToString()
                                   },
                                   TokenKey = dr["TokenKey"].ToString(),
                                   TokenId = dr["TokenId"].ToString(),
                                   ExpiredDate = dr["ExpiredDate"] != DBNull.Value ? Convert.ToDateTime(dr["ExpiredDate"].ToString()) : default(DateTime)
                               }).ToList();
            var currUser = userLoginEx.FirstOrDefault();
            if (currUser == null || currUser.ExpiredDate < DateTime.Now)
                return false;
            userLogin = JsonConvert.DeserializeObject<UserInfo>(Crypto.Decrypt(currUser.TokenKey, currUser.TokenId));
            userLogin.Password = Crypto.Decrypt(currUser.UserInformation.Password, currUser.TokenId);

            return true;
        }

        #endregion

        #region Get sys values by user

        public List<SettingModel> SysValues()
        {
            Connection();
            List<SettingModel> sysProps = new List<SettingModel>();
            SqlCommand com = new SqlCommand("GetSysProperties", _con);
            com.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();
            try
            {
                _logger.Info("Attempting to get all users from database");
                _con.Open();
                da.Fill(dt);
                _con.Close();
                _logger.Info("List of all users successfully obtained from database");
                sysProps = (from DataRow dr in dt.Rows
                            select new SettingModel
                            {
                                Id = Convert.ToInt32(dr["Id"]),
                                PropertyName = dr["PropertyName"].ToString(),
                                PropertyValue = dr["PropertyValue"] != DBNull.Value ? dr["PropertyValue"] : null
                            }).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

            return sysProps;
        }

        public string GetUiInfo()
        {
            var res = SysValues().FirstOrDefault(f => f.PropertyName == "ui_version");
            if(res==null || res.PropertyValue == null) return String.Empty;
            return res.PropertyValue.ToString();
        }

        #endregion
    }
}