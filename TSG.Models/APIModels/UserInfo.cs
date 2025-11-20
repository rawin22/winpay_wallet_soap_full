using System;

namespace TSG.Models.APIModels
{
	public class UserInfo
	{
		public string UserId { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string OrganisationId { get; set; }
		public string OrganisationName { get; set; }
		public bool IsActive { get; set; }
		public bool IsBlockLocal { get; set; }
		public bool ShowWelcomeMessage { get; set; }
		public DateTime LastLoginDate { get; set; }
		public string EmailAddress { get; set; }
		public string WelcomeMessage { get; set; }
		public UserRoleType Role { get; set; }
		public string RoleName { get; set; }
		public string Fax { get; set; }
		public string Phone { get; set; }
		public string BaseCurrencyCode { get; set; }
		public DateTime CurrentLoginDate { get; set; }
		public string AccessToken { get; set; }
		public string BankHomePageMessage { get; set; }
	}
}