using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Tsg.UI.Main.Extensions
{
	///<Summary>
	/// Get settings from Web.config
	///</Summary>
	public class AppSettingHelper
	{
		private static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		///<Summary>
		/// Get white listed functions from Web.config
		///</Summary>
		public static string[] GetWhiteListedFunctions()
		{
			return Convert.ToString(ConfigurationManager.AppSettings["WhiteListedFunctions"]).Split(';');
		}

		///<Summary>
		/// Get white listed functions from Web.config
		///</Summary>
		public static string GetWhiteListedFunctionsString()
		{
			return Convert.ToString(ConfigurationManager.AppSettings["WhiteListedFunctions"]);
		}

		///<Summary>
		/// Whether the Instant Payment function is white listed.
		///</Summary>
		public static bool IsInstantPaymentWhiteListed()
		{
			// return (GetWhiteListedFunctions().Contains("InstantPayment"));
			return SearchWhiteListedFunction("InstantPayment");
		}


		///<Summary>
		/// Whether the Instant Payment > Pay Now function is white listed.
		///</Summary>
		public static bool IsPayNowWhiteListed()
		{
			// return (GetWhiteListedFunctions().Contains("PayNow"));
			return SearchWhiteListedFunction("PayNow");
		}

		///<Summary>
		/// Whether the Instant Payment > Receive function is white listed.
		///</Summary>
		public static bool IsReceiveWhiteListed()
		{
			//return (GetWhiteListedFunctions().Contains("Receive"));
			return SearchWhiteListedFunction("Receive");
		}

		///<Summary>
		/// Whether the Instant Payment > Receive QR Code creation function is white listed.
		///</Summary>
		public static bool IsReceiveQrCodeWhiteListed()
		{
			return SearchWhiteListedFunction("ReceiveQrCode");
		}

		///<Summary>
		/// Whether the Instant Payment > Receive History function is white listed.
		///</Summary>
		public static bool IsReceiveHistoryWhiteListed()
		{
			return SearchWhiteListedFunction("ReceiveHistory");
		}


		///<Summary>
		/// Whether the History > Convert function is white listed.
		///</Summary>
		public static bool IsConvertHistoryWhiteListed()
		{
			return SearchWhiteListedFunction("ConvertHistory");
		}

		///<Summary>
		/// Whether the Instant Payment > History function is white listed.
		///</Summary>
		public static bool IsHistoryWhiteListed()
		{
			//return (GetWhiteListedFunctions().Contains("Receive"));
			return SearchWhiteListedFunction("History");
		}

		///<Summary>
		/// Whether the Accounts function is white listed.
		///</Summary>
		public static bool IsAccountsWhiteListed()
		{
			//return (GetWhiteListedFunctions().Contains("Receive"));
			return SearchWhiteListedFunction("Accounts");
		}

		///<Summary>
		/// Whether the Accounts > Balances function is white listed.
		///</Summary>
		public static bool IsBalancesWhiteListed()
		{
			//return (GetWhiteListedFunctions().Contains("Receive"));
			return SearchWhiteListedFunction("Balances");
		}

		///<Summary>
		/// Whether the Accounts > Statements function is white listed.
		///</Summary>
		public static bool IsStatementsWhiteListed()
		{
			//return (GetWhiteListedFunctions().Contains("Receive"));
			return SearchWhiteListedFunction("Statements");
		}

		///<Summary>
		/// Whether the Add Funds function is white listed.
		///</Summary>
		public static bool IsAddFundsWhiteListed()
		{
			return SearchWhiteListedFunction("AddFunds");
		}

		///<Summary>
		/// Whether the Add Funds > Add Funds via Wire Transfer function is white listed.
		///</Summary>
		public static bool IsWireTransferWhiteListed()
		{
			return SearchWhiteListedFunction("WireTransfer");
		}

		///<Summary>
		/// Whether the Add Funds > Load Crypto function is white listed.
		///</Summary>
		public static bool IsLoadCryptoWhiteListed()
		{
			return SearchWhiteListedFunction("LoadCrypto");
		}

		///<Summary>
		/// Whether the Add Funds > Add Funds via PIP function is white listed.
		///</Summary>
		public static bool IsPIPWhiteListed()
		{
			return SearchWhiteListedFunction("PIP");
		}

		///<Summary>
		/// Whether the Add Funds > Upload Deposit Proof function is white listed.
		///</Summary>
		public static bool IsUploadDepositProofWhiteListed()
		{
			return SearchWhiteListedFunction("UploadDepositProof");
		}

		///<Summary>
		/// Whether the Add Funds > Deposit History function is white listed.
		///</Summary>
		public static bool IsDepositHistoryWhiteListed()
		{
			return SearchWhiteListedFunction("DepositHistory");
		}

		///<Summary>
		/// Whether the Convert & Exchange function is white listed.
		///</Summary>
		public static bool IsConvertExchangeWhiteListed()
		{
			return SearchWhiteListedFunction("ConvertExchange");
		}

		///<Summary>
		/// Whether the User Profile function is white listed.
		///</Summary>
		public static bool IsUserProfileWhiteListed()
		{
			return SearchWhiteListedFunction("UserProfile");
		}

		///<Summary>
		/// Whether the User Profile > Liquidation Preferences function is white listed.
		///</Summary>
		public static bool IsLiquidationPreferencesWhiteListed()
		{
			return SearchWhiteListedFunction("LiquidationPreferences");
		}

		///<Summary>
		/// Whether the User Profile > View Profile / KYC status function is white listed.
		///</Summary>
		public static bool IsViewProfileWhiteListed()
		{
			return SearchWhiteListedFunction("ViewProfile");
		}

		///<Summary>
		/// Whether the User Profile > Change Password function is white listed.
		///</Summary>
		public static bool IsChangePasswordWhiteListed()
		{
			return SearchWhiteListedFunction("ChangePassword");
		}

		///<Summary>
		/// Whether the User Profile > Reset Password function is white listed.
		///</Summary>
		public static bool IsResetPasswordWhiteListed()
		{
			return SearchWhiteListedFunction("ResetPassword");
		}

		///<Summary>
		/// Whether the User Profile > Currency Settings function is white listed.
		///</Summary>
		public static bool IsCurrencySettingsWhiteListed()
		{
			return SearchWhiteListedFunction("CurrencySettings");
		}

		///<Summary>
		/// Whether the Invitation Link function is white listed.
		///</Summary>
		public static bool IsInvitationLinkWhiteListed()
		{
			return SearchWhiteListedFunction("InvitationLink");
		}

		///<Summary>
		/// Whether the Contact Form function is white listed.
		///</Summary>
		public static bool IsContactFormWhiteListed()
		{
			return SearchWhiteListedFunction("ContactForm");
		}

		///<Summary>
		/// Whether the MARKETPLACE function is white listed.
		///</Summary>
		public static bool IsMarketPlaceMainWhiteListed()
		{
			return SearchWhiteListedFunction("MarketplaceMain");
		}

		///<Summary>
		/// Whether the MARKETPLACE > Marketplace function is white listed.
		///</Summary>
		public static bool IsMarketPlaceWhiteListed()
		{
			return SearchWhiteListedFunction("Marketplace");
		}

		///<Summary>
		/// Whether the MARKETPLACE > Orders function is white listed.
		///</Summary>
		public static bool IsMarketplaceOrdersWhiteListed()
		{
			return SearchWhiteListedFunction("MarketplaceOrders");
		}


		///<Summary>
		/// Whether the Trusted Token function is white listed.
		///</Summary>
		public static bool IsTrustedTokenWhiteListed()
		{
			return SearchWhiteListedFunction("TrustedToken");
		}

		///<Summary>
		/// Whether the Red Envelope function is white listed.
		///</Summary>
		public static bool IsRedEnvelopeWhiteListed()
		{
			return SearchWhiteListedFunction("RedEnvelope");
		}

		///<Summary>
		/// Whether the Payout function is white listed.
		///</Summary>
		public static bool IsPayoutWhiteListed()
		{
			return SearchWhiteListedFunction("Payout");
		}

		///<Summary>
		/// Whether the Payout function is white listed.
		///</Summary>
		public static bool IsPayoutNewWhiteListed()
		{
			return SearchWhiteListedFunction("PayoutNew");
		}

		///<Summary>
		/// Whether the Payout Details function is white listed.
		///</Summary>
		public static bool IsPayoutDetailsWhiteListed()
		{
			return SearchWhiteListedFunction("PayoutDetails");
		}

		///<Summary>
		/// Whether the Payout Details function is white listed.
		///</Summary>
		public static bool IsPayoutHistoryWhiteListed()
		{
			return SearchWhiteListedFunction("PayoutHistory");
		}

		///<Summary>
		/// Search whether white listed string contains a specific string.
		///</Summary>
		public static bool SearchWhiteListedFunction(string function)
		{
			bool isFound = false;
			_logger.InfoFormat("SearchWhiteListedFunction, function: {0} ", function);
			_logger.InfoFormat("SearchWhiteListedFunction, GetWhiteListedFunctions: {0} ", JsonConvert.SerializeObject(GetWhiteListedFunctions()));
			var isEnabled = GetWhiteListedFunctions().Any(s => s.Equals(function, StringComparison.CurrentCultureIgnoreCase));
			_logger.InfoFormat("SearchWhiteListedFunction, isEnabled: {0} ", isEnabled);
			if (GetWhiteListedFunctions().Any(s => s.Equals(function, StringComparison.CurrentCultureIgnoreCase)))
			{
				isFound = true;
			}

			return isFound;
		}
		///<Summary>
		/// Get URL Shortener Base Address
		///</Summary>
		public static string GetUrlShortenerBaseAddress()
		{
			return Convert.ToString(ConfigurationManager.AppSettings["UrlShortenerBaseAddress"]);
		}

		///<Summary>
		/// Get allowed currencies from Web.config
		///</Summary>
		public static List<string> GetAllowedCurrencies()
		{
			return Convert.ToString(ConfigurationManager.AppSettings["AllowedCurrencies"]).Split(';').ToList();
		}

		///<Summary>
		/// Whether the Withdraw function is white listed.
		///</Summary>
		public static bool IsWithdrawWhiteListed()
		{
			return SearchWhiteListedFunction("Withdraw");
		}

		///<Summary>
		/// Whether the Deposit function is white listed.
		///</Summary>
		public static bool IsDepositWhiteListed()
		{
			return SearchWhiteListedFunction("Deposit");
		}

		///<Summary>
		/// Get website
		///</Summary>
		public static string GetWebsiteUrl()
		{
			return Convert.ToString(ConfigurationManager.AppSettings["Website"]);
		}

		///<Summary>
		/// Get Login Page Logo
		///</Summary>
		public static string GetLoginPageLogo()
		{
			return Convert.ToString(ConfigurationManager.AppSettings["LoginPageLogo"]);
		}
	}
}