using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using Tsg.Business.Model.Classes;
using Tsg.UI.Main.Models.Security;
using System.Security.Cryptography;
using Tsg.UI.Main.Repository;
using System.Text;
using Tsg.UI.Main.Models.Attributes;
using TSG.Models.APIModels;
using Newtonsoft.Json;
using Swashbuckle.Swagger;

namespace Tsg.UI.Main.Models
{
	public class UserApiResult
	{
		public bool IsSuccess { get; set; }
		public UserInfo UserInfo { get; set; }
	}

	public class UserLoginModel
	{
		private IgpService _service;
		readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		[Required]
		[StringLength(100, ErrorMessage = "The user name must be less than 100 characters")]
		public string Username { get; set; }

		[DataType(DataType.Password)]
		public string OldPassword { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		public string RepeatPassword { get; set; }
		public string ReturnUrl { get; set; }
		public int ErrorCode { get; set; }
		public string ErrorMessage { get; set; }
		public string ErrorMessageDetails { get; set; }


		public UserLoginModel()
		{
			_service = new IgpService();
		}

		public bool ServiceLogin()
		{
			var response = _service.GetUserData(this.Username, this.Password);

			if (!response.ServiceResponse.HasErrors)
			{
				_service.SetUserCredentials(this.Username, this.Password, response.UserSettings.UserId);

				UserInfo userInfo = new UserInfo()
				{
					UserId = response.UserSettings.UserId,
					Password = this.Password,
					UserName = response.UserSettings.UserName,
					FirstName = response.UserSettings.FirstName,
					LastName = response.UserSettings.LastName,
					OrganisationName = response.UserSettings.OrganizationName,
					OrganisationId = response.UserSettings.OrganizationId,
					IsActive = response.UserSettings.IsEnabled,
					EmailAddress = response.UserSettings.EmailAddress,
					ShowWelcomeMessage = true,
					LastLoginDate = DateTime.Now,
					Role = UserRoleType.User,
					RoleName = Role.User,
					Fax = response.UserSettings.Fax,
					Phone = response.UserSettings.Phone,
					BaseCurrencyCode = response.UserSettings.BaseCurrencyCode

				};
				AppSecurity.CurrentUser = userInfo;
				FormsAuthentication.SetAuthCookie(this.Username, false);

				return true;
			}
			else
			{
				return false;
			}
		}


		/// <summary>
		/// Authenticate by the Core
		/// </summary>
		/// <returns></returns>
		public bool ServiceAuthenticate()
		{
			var authenticationResponse = _service.Authenticate(this.Username, this.Password);
			_logger.Debug(string.Format("ServiceAuthenticate, authenticationResponse: {0}", JsonConvert.SerializeObject(authenticationResponse)));

			if (authenticationResponse.ServiceResponse.HasErrors)
			{
				ErrorCode = authenticationResponse.ServiceResponse.Responses[0].ResponseCode;
				ErrorMessage = authenticationResponse.ServiceResponse.Responses[0].Message;
				ErrorMessageDetails = authenticationResponse.ServiceResponse.Responses[0].MessageDetails;

				_logger.DebugFormat("Authenticate ErrorMessage: {0}", ErrorMessage);
				return false;
			}
			else
			{
				//var response = _service.GetUserData(this.Username, this.Password);
				var response = _service.GetUserData();

				if (!response.ServiceResponse.HasErrors)
				{
					_service.SetUserCredentials(this.Username, this.Password, response.UserSettings.UserId);

					UserInfo userInfo = new UserInfo()
					{
						UserId = response.UserSettings.UserId,
						Password = this.Password,
						UserName = response.UserSettings.UserName,
						FirstName = response.UserSettings.FirstName,
						LastName = response.UserSettings.LastName,
						OrganisationName = response.UserSettings.OrganizationName,
						OrganisationId = response.UserSettings.OrganizationId,
						IsActive = response.UserSettings.IsEnabled,
						EmailAddress = response.UserSettings.EmailAddress,
						ShowWelcomeMessage = true,
						LastLoginDate = DateTime.Now,
						Role = UserRoleType.User,
						RoleName = Role.User,
						Fax = response.UserSettings.Fax,
						Phone = response.UserSettings.Phone,
						BaseCurrencyCode = response.UserSettings.BaseCurrencyCode,
						AccessToken = authenticationResponse.AccessToken,
						BankHomePageMessage = response.UserSettings.BankHomePageMessage,
					};
					AppSecurity.CurrentUser = userInfo;
					FormsAuthentication.SetAuthCookie(this.Username, false);

					return true;
				}
				else
				{
					ErrorMessage = response.ServiceResponse.Responses[0].MessageDetails;
					_logger.DebugFormat("ErrorMessage: {0}", ErrorMessage);
					return false;
				}
			}
		}

		/// <summary>
		/// Authenticate by the Core for API
		/// </summary>
		/// <returns></returns>
		public UserApiResult ApiServiceLogin()
		{
			var result = new UserApiResult() { IsSuccess = false, UserInfo = null };
			var authenticationResponse = _service.Authenticate(this.Username, this.Password);
			_logger.Debug(string.Format("ApiServiceLogin, authenticationResponse: {0}", JsonConvert.SerializeObject(authenticationResponse)));

			var response = _service.GetUserData();

			if (!response.ServiceResponse.HasErrors)
			{
				_service.SetUserCredentials(this.Username, this.Password, response.UserSettings.UserId);

				result.UserInfo = new UserInfo()
				{
					UserId = response.UserSettings.UserId,
					Password = this.Password,
					UserName = response.UserSettings.UserName,
					FirstName = response.UserSettings.FirstName,
					LastName = response.UserSettings.LastName,
					OrganisationName = response.UserSettings.OrganizationName,
					OrganisationId = response.UserSettings.OrganizationId,
					IsActive = response.UserSettings.IsEnabled,
					EmailAddress = response.UserSettings.EmailAddress,
					ShowWelcomeMessage = true,
					LastLoginDate = DateTime.Now,
					Role = UserRoleType.User,
					RoleName = Role.User
				};
				result.IsSuccess = true;
			}
			return result;
		}

		public bool LocalUserLogin()
		{
			UserRepository ur = new UserRepository();
			var allUsers = ur.GetUsers();

			var currentUser =
				allUsers?.FirstOrDefault(f => f.Username == this.Username && CheckPassword(Username, Password));
			if (currentUser != null)
			{
				_service.SetUserCredentials(this.Username, this.Password, ConfigurationManager.AppSettings["callerId"].ToString());
				UserInfo userInfo = new UserInfo()
				{
					UserId = ConfigurationManager.AppSettings["callerId"].ToString(),
					Password = this.Password,
					UserName = this.Username,
					FirstName = currentUser.FirstName,
					LastName = currentUser.LastName,
					OrganisationName = "",
					IsActive = true,
					EmailAddress = currentUser.UserMail,
					ShowWelcomeMessage = true,
					LastLoginDate = currentUser.LastLoginDate,
					WelcomeMessage = currentUser.WelcomeMessageText,
					Role = (UserRoleType)currentUser.RoleId,
					RoleName = currentUser.RoleName,
					IsBlockLocal = currentUser.IsBlockLocal
				};
				AppSecurity.CurrentUser = userInfo;
				//FormsAuthentication.SetAuthCookie(userInfo.RoleName, false);
				CreateTicket(Username, new string[] { userInfo.RoleName });
				if (userInfo.Role == UserRoleType.Admin)
				{
					//var cookies = service.LoginInPipIt();
					//foreach(var cookie in cookies)
					//    HttpContext.Current.Response.Cookies.Add(cookie);
				}


				return true;
			}
			return false;
		}
		public UserApiResult ApiLocalUserLogin()
		{
			var result = new UserApiResult() { IsSuccess = false, UserInfo = null };
			UserRepository ur = new UserRepository();
			var allUsers = ur.GetUsers();

			var currentUser =
				allUsers.FirstOrDefault(f => f.Username == this.Username && CheckPassword(Username, Password));
			if (currentUser != null)
			{
				_service.SetUserCredentials(this.Username, this.Password, ConfigurationManager.AppSettings["callerId"].ToString());
				result.UserInfo = new UserInfo()
				{
					UserId = ConfigurationManager.AppSettings["callerId"].ToString(),
					Password = this.Password,
					UserName = this.Username,
					FirstName = currentUser.FirstName,
					LastName = currentUser.LastName,
					OrganisationName = "",
					IsActive = true,
					EmailAddress = currentUser.UserMail,
					ShowWelcomeMessage = true,
					LastLoginDate = currentUser.LastLoginDate,
					WelcomeMessage = currentUser.WelcomeMessageText,
					Role = (UserRoleType)currentUser.RoleId,
					RoleName = currentUser.RoleName,
					IsBlockLocal = currentUser.IsBlockLocal
				};
				result.IsSuccess = true;
			}
			return result;
		}

		private void CreateTicket(string userName, string[] arrayOfRoles)
		{
			var ticket = new FormsAuthenticationTicket(
				version: 1,
				name: userName,
				issueDate: DateTime.Now,
				expiration: DateTime.Now.AddMinutes(HttpContext.Current.Session.Timeout),
				isPersistent: false,
				userData: String.Join(",", arrayOfRoles));

			var encryptedTicket = FormsAuthentication.Encrypt(ticket);
			var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

			HttpContext.Current.Response.AppendCookie(cookie);
			//HttpContext.Current.Response.Cookies.Add(cookie);
		}

		private bool CheckPassword(string username, string plainPassword)
		{
			string localPlainPassword = "";
			if (!String.IsNullOrEmpty(plainPassword))
				localPlainPassword = plainPassword;
			string hashedPassword = GetSha1(localPlainPassword);

			UserRepository userRepo = new UserRepository();
			if (hashedPassword.Equals(userRepo.GetHashedPassword(username).ToUpper()))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static string GetSha1(string value)
		{
			SHA1 algorithm = SHA1.Create();
			byte[] data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
			return data.Aggregate("", (current, t) => current + t.ToString("x2").ToUpperInvariant());
		}

		public void SetLastLoginDate(DateTime lastLoginDate)
		{
			AppSecurity.CurrentUser.LastLoginDate = lastLoginDate;
		}

		public void SetWelcomeMessage(string wlcMsgText)
		{
			AppSecurity.CurrentUser.WelcomeMessage = wlcMsgText;
		}


	}
}