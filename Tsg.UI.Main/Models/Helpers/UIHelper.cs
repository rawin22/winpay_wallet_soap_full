using Autofac.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Controllers;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Helpers;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;


public static class UiHelper
{
	static readonly log4net.ILog Logger =
		log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	public static string FormatPrice(this decimal amount)
	{
		return amount.ToTrimmedString();
		//return amount< 0.01M ? amount.ToString("F9", new System.Globalization.CultureInfo("en-US")).TrimEnd('0'): amount.ToString("G29", new System.Globalization.CultureInfo("en-US")).TrimEnd('0');
	}
	public static string ToTrimmedString(this decimal target)
	{
		string strValue = target.ToString(); //Get the stock string

		//If there is a decimal point present
		if (strValue.Contains("."))
		{
			//Remove all trailing zeros
			strValue = strValue.TrimEnd('0');

			//If all we are left with is a decimal point
			if (strValue.EndsWith(".")) //then remove it
				strValue = strValue.TrimEnd('.');
		}

		return strValue;
	}

	public static IList<SelectListItem> PrepareAvailableBankCurrencies()
	{
		var bankCurrenciesList = new List<SelectListItem>();
		bankCurrenciesList.Add
		(
			new SelectListItem
			{
				Text = GlobalRes.Funding_FundingController_PrepareAvalBanks_SelectBank,
				Value = ""
			}
		);

		CurrencyRepository curRepo = new CurrencyRepository();
		var bankCurrencies = curRepo.GetBankCurrencies();
		foreach (var item in bankCurrencies)
		{
			bankCurrenciesList.Add
			(
				new SelectListItem
				{
					Text = item.BankCurrencyName,
					Value = item.BankCurrencyId.ToString()
				}
			);
		}
		return bankCurrenciesList;
	}
	public static IList<SelectListItem> PrepareAvailableCurrencies()
	{
		var currenciesList = new List<SelectListItem>();
		currenciesList.Add
		(
			new SelectListItem
			{
				Text = GlobalRes.Shared_NewInstantPaymentFormPage_SelectCurrency,
				Value = ""
			}
		);

		CurrencyRepository curRepo = new CurrencyRepository();
		var currencies = curRepo.GetCurrencies();
		foreach (var item in currencies)
		{
			currenciesList.Add
			(
				new SelectListItem
				{
					Text = item.CurrencyCode,
					Value = item.CurrencyId.ToString()
				}
			);
		}
		return currenciesList;
	}
	public static IList<SelectListItem> PrepareAvailableCurrenciesByService(string userName, string password)
	{
		var currenciesListItem = new List<SelectListItem>();
		try
		{
			IgpService service = new IgpService();
			var resAuth = service.GetUserData(userName, password);
			if (resAuth.ServiceResponse.HasErrors)
				return new List<SelectListItem>();
			service.SetUserCredentials(userName, password, resAuth.UserSettings.UserId);
			var currencies = service.GetPaymentCurrencies();

			//currenciesListItem.Add
			//(
			//    new SelectListItem
			//    {
			//        Text = "Select",
			//        Value = ""
			//    }
			//);
			FavoriteCurrencyRepository cr = new FavoriteCurrencyRepository();
			var favoritecurrency = cr.GetFavoriteCurrencyList(resAuth.UserSettings.UserId);
			if (favoritecurrency.Count > 0)
			{
				foreach (var item in favoritecurrency)
				{
					var currCurrency =
						currencies.Currencies.FirstOrDefault(a => a.CurrencyCode == item.CurrencyCode);

					if (currCurrency != null)
					{
						currenciesListItem.Add
						(
							new SelectListItem
							{
								Text = currCurrency.CurrencyCode,
								Value = currCurrency.CurrencyCode
							}
						);
					}

				}
			}
			else
			{
				if (!currencies.ServiceResponse.HasErrors)
				{
					foreach (var item in currencies.Currencies)
					{
						currenciesListItem.Add
						(
							new SelectListItem
							{
								Text = item.CurrencyCode,
								Value = item.CurrencyCode
							}
						);
					}
				}
			}
		}
		catch (Exception ex) { Logger.Error(ex); }

		return currenciesListItem;
	}
	public static IList<SelectListItem> PrepareAvailableCurrenciesByService()
	{
		var currenciesListItem = new List<SelectListItem>();
		try
		{
			IgpService service = new IgpService();
			service.SetUserCredentials(AppSecurity.CurrentUser.UserName, AppSecurity.CurrentUser.Password,
				AppSecurity.CurrentUser.UserId);

			var currencies = service.GetPaymentCurrencies();

			//currenciesListItem.Add
			//(
			//    new SelectListItem
			//    {
			//        Text = "Select",
			//        Value = ""
			//    }
			//);
			FavoriteCurrencyRepository cr = new FavoriteCurrencyRepository();
			var favoritecurrency = cr.GetFavoriteCurrencyList();
			if (favoritecurrency.Count > 0)
			{
				foreach (var item in favoritecurrency)
				{
					var currCurrency =
						currencies.Currencies.FirstOrDefault(a => a.CurrencyCode == item.CurrencyCode);

					if (currCurrency != null)
					{
						currenciesListItem.Add
						(
							new SelectListItem
							{
								Text = currCurrency.CurrencyCode,
								Value = currCurrency.CurrencyCode
							}
						);
					}

				}
			}
			else
			{
				if (!currencies.ServiceResponse.HasErrors)
				{
					foreach (var item in currencies.Currencies)
					{
						currenciesListItem.Add
						(
							new SelectListItem
							{
								Text = item.CurrencyCode,
								Value = item.CurrencyCode
							}
						);
					}
				}
			}
		}
		catch (Exception ex) { Logger.Error(ex); }

		return currenciesListItem;
	}
	public static IList<SelectListItem> PrepareAvailableBank()
	{
		var bankCurrenciesList = new List<SelectListItem>();
		bankCurrenciesList.Add
		(
			new SelectListItem
			{
				Text = GlobalRes.UIHelper_PrepareAvaliableBank_SelectBank,
				Value = ""
			}
		);

		BankRepository curRepo = new BankRepository();
		var bankCurrencies = curRepo.GetBanks();
		foreach (var item in bankCurrencies)
		{
			bankCurrenciesList.Add
			(
				new SelectListItem
				{
					Text = item.BankName + " - " + item.BankCountry,
					Value = item.BankId.ToString()
				}
			);
		}
		return bankCurrenciesList;
	}

	public static IList<SelectListItem> PrepareAvailableCountries()
	{
		var bankCurrenciesList = new List<SelectListItem>();
		bankCurrenciesList.Add
		(
			new SelectListItem
			{
				Text = "Select country",
				Value = ""
			}
		);

		CountryRepository countryRepo = new CountryRepository();
		var bankCurrencies = countryRepo.GetCountryList();
		foreach (var item in bankCurrencies)
		{
			bankCurrenciesList.Add
			(
				new SelectListItem
				{
					Text = item.CountryName,
					Value = item.CountryId.ToString()
				}
			);
		}
		return bankCurrenciesList;
	}

	public static IList<SelectListItem> PrepareAllAvailablePaymentCurrencies()
	{
		var currenciesListItem = new List<SelectListItem>();
		var instantpaymentModel = new NewInstantPaymentViewModel();

		var currencies = instantpaymentModel.PrepareAllAvailablePaymentCurrencies();
		foreach (var item in currencies)
		{
			currenciesListItem.Add
			(
				new SelectListItem
				{
					Text = item.Text,
					Value = item.Value
				}
			);
		}
		return currenciesListItem;
	}

	/// <summary>
	/// Prepare a list of all available currencies
	/// </summary>
	/// <returns></returns>
	public static IList<CurrencyData> PrepareAllAvailableCurrencies()
	{
		var instantpaymentModel = new NewInstantPaymentViewModel();
		return instantpaymentModel.PrepareAllAvailableCurrencies();
	}

	/// <summary>
	/// Get FX buy currencies
	/// </summary>
	/// <returns></returns>
	public static IList<CurrencyData> GetFXBuyCurrencies()
	{
		var instantpaymentModel = new NewInstantPaymentViewModel();
		return instantpaymentModel.GetFXBuyCurrencies();
	}

	/// <summary>
	/// Get FX sell currencies
	/// </summary>
	/// <returns></returns>
	public static IList<CurrencyData> GetFXSellCurrencies()
	{
		var instantpaymentModel = new NewInstantPaymentViewModel();
		return instantpaymentModel.GetFXSellCurrencies();
	}

	/// <summary>
	/// Prepare customer account balances
	/// </summary>
	/// <returns></returns>
	public static IList<CustomerBalanceData> PrepareCustomerBalances()
	{
		var instantpaymentModel = new NewInstantPaymentViewModel();
		return instantpaymentModel.PrepareCustomerAvailableBalances();
	}


	public static IEnumerable<SelectListItem> GetAccountBalancesForDa(bool needClearData = false)
	{
		var list = new List<SelectListItem>();
		if (!needClearData)
		{
			list.Add(new SelectListItem()
			{
				Text = GlobalRes.Shared_NewInstantPaymentFormPage_SelectCurrency,
				Value = String.Empty
			});
		}

		try
		{
			IgpService service = new IgpService(AppSecurity.CurrentUser.AccessToken, AppSecurity.CurrentUser.UserId);
			var response = service.GetPaymentCurrencies();
			var listCustomerBalanceData = new List<CustomerBalanceData>();
			FavoriteCurrencyRepository cr = new FavoriteCurrencyRepository();
			var favoritecurrency = cr.GetFavoriteCurrencyList(AppSecurity.CurrentUser.UserId).GroupBy(gb => gb.CurrencyCode).ToList();
			listCustomerBalanceData = service.GetAccountBalances().Balances.ToList();

			if (response.ServiceResponse.HasErrors)
				favoritecurrency.Clear();

			var currencies = response.Currencies.Join(favoritecurrency, currency => currency.CurrencyCode,
					favoritecurrencies => favoritecurrencies.Key,
					(currency, favoritecurrencies) => new
					{
						favoritecurrencies.Key,
						currency.CurrencyName,
						currency.CurrencyCode
					}).ToList();

			var allowedCurrencies = AppSettingHelper.GetAllowedCurrencies();
			if (allowedCurrencies.Count > 0 && !string.IsNullOrEmpty(allowedCurrencies[0]))
			{
				currencies = currencies.Where(c => allowedCurrencies.Any(ac => ac == c.CurrencyCode)).ToList();
				listCustomerBalanceData = listCustomerBalanceData.Where(c => allowedCurrencies.Any(ac => ac == c.CCY)).ToList();
			}

			var cryptoCurrency = new[] { "BTC", "ETH" };
			foreach (var currency in currencies.Where(w => listCustomerBalanceData.Select(s => s.CCY).Contains(w.CurrencyCode)))
			{
				var item = listCustomerBalanceData.FirstOrDefault(f => f.CCY == currency.CurrencyCode);
				if (item != null)
				{
					list.Add(
						new SelectListItem
						{
							Selected = item.CCY == item.BaseCCY,
							Text = item.CCY,
							//needClearData ? item.AccountNumber : String.Format(
							//    "{0} {1} {2}", item.CCY, GlobalRes.UIHelper_GetAccountBalances____Available_,
							//                                           cryptoCurrency.Contains(item.CCY) ?
							//                                               String.Format(new CultureInfo("en-US"), "{0:N8}", item.BalanceAvailable)
							//                                               :
							//                                               String.Format(new CultureInfo("en-US"), "{0:N2}", item.BalanceAvailable)),
							Value = item.CCY
						});
				}
			}
			foreach (var item in listCustomerBalanceData.Where(w => !currencies.Select(s => s.CurrencyCode).Contains(w.CCY)))
			{
				list.Add(new SelectListItem
				{
					Selected = item.CCY == item.BaseCCY,
					Text = item.CCY,
					//needClearData ? item.AccountNumber : String.Format(
					//    "{0} {1} {2}", item.CCY, GlobalRes.UIHelper_GetAccountBalances____Available_,
					//    cryptoCurrency.Contains(item.CCY) ?
					//        String.Format(new CultureInfo("en-US"), "{0:N8}", item.BalanceAvailable)
					//        :
					//        String.Format(new CultureInfo("en-US"), "{0:N2}", item.BalanceAvailable)),
					Value = item.CCY
				});
			}
		}
		catch (Exception e)
		{
			Logger.Error(e.Message);
		}

		return list;
	}

	/// <summary>
	/// Get customer's account balances list
	/// </summary>
	/// <param name="needClearData"></param>
	/// <returns></returns>
	public static IEnumerable<SelectListItem> GetAccountBalances(bool needClearData = false)
	{
		var list = new List<SelectListItem>();
		if (!needClearData)
		{
			list.Add(new SelectListItem()
			{
				Text = GlobalRes.Shared_NewInstantPaymentFormPage_SelectCurrency,
				Value = null
			});
		}

		try
		{
			IgpService service = new IgpService(AppSecurity.CurrentUser.AccessToken, AppSecurity.CurrentUser.UserId);
			var response = service.GetPaymentCurrencies();
			var listCustomerBalanceData = new List<CustomerBalanceData>();
			FavoriteCurrencyRepository cr = new FavoriteCurrencyRepository();
			var favoritecurrency = cr.GetFavoriteCurrencyList(AppSecurity.CurrentUser.UserId).GroupBy(gb => gb.CurrencyCode).ToList();
			listCustomerBalanceData = service.GetAccountBalances().Balances.ToList();

			if (response.ServiceResponse.HasErrors)
				favoritecurrency.Clear();
			var currenciesList = response.Currencies;
			var currencies = response.Currencies.Join(favoritecurrency, currency => currency.CurrencyCode,
					favoritecurrencies => favoritecurrencies.Key,
					(currency, favoritecurrencies) => new
					{
						favoritecurrencies.Key,
						currency.CurrencyName,
						currency.CurrencyCode
					}).ToList();

			var allowedCurrencies = AppSettingHelper.GetAllowedCurrencies();
			if (allowedCurrencies.Count > 0 && !string.IsNullOrEmpty(allowedCurrencies[0]))
			{
				currencies = currencies.Where(c => allowedCurrencies.Any(ac => ac == c.CurrencyCode)).ToList();
				listCustomerBalanceData = listCustomerBalanceData.Where(c => allowedCurrencies.Any(ac => ac == c.CCY)).ToList();
			}

			foreach (var currency in currencies.Where(w => listCustomerBalanceData.Select(s => s.CCY).Contains(w.CurrencyCode)))
			{
				var item = listCustomerBalanceData.FirstOrDefault(f => f.CCY == currency.CurrencyCode);
				if (item != null)
				{
					var currencyDetails = currenciesList.FirstOrDefault(c => c.CurrencyCode == item.CCY);
					var amount = item.BalanceAvailable;
					if (currencyDetails != null)
					{
						amount = @Math.Round(item.BalanceAvailable, currencyDetails.CurrencyAmountScale);
					}

					list.Add(
						new SelectListItem
						{
							Text = needClearData ? item.AccountNumber : String.Format(
								"{0} {1} {2}", item.CCY, GlobalRes.UIHelper_GetAccountBalances____Available_, amount),
							Value = needClearData ? item.AccountNumber : item.AccountId
						});
				}
			}
			foreach (var item in listCustomerBalanceData.Where(w => !currencies.Select(s => s.CurrencyCode).Contains(w.CCY)))
			{
				var currency = currenciesList.FirstOrDefault(c => c.CurrencyCode == item.CCY);
				var amount = item.BalanceAvailable;
				if (currency != null)
				{
					amount = @Math.Round(item.BalanceAvailable, currency.CurrencyAmountScale);
				}
				list.Add(new SelectListItem
				{
					Text = needClearData ? item.AccountNumber : String.Format(
						"{0} {1} {2}", item.CCY, GlobalRes.UIHelper_GetAccountBalances____Available_, amount),
					Value = needClearData ? item.AccountNumber : item.AccountId
				});
			}
		}
		catch (Exception e)
		{
			Logger.Error(e.Message);
		}

		return list;
	}

	/// <summary>
	/// Get a string contains customer account number, amount, and currency code of a specific currency
	/// </summary>
	/// <param name="currencyCode"></param>
	/// <param name="needClearData"></param>
	/// <returns>string contains customer account number, amount, and currency code</returns>
	public static string GetAccountBalance(string currencyCode, bool needClearData = false)
	{
		string balanceDetails = "Empty";
		var list = new List<SelectListItem>();
		if (!needClearData)
		{
			list.Add(new SelectListItem()
			{
				Text = GlobalRes.Shared_NewInstantPaymentFormPage_SelectCurrency,
				Value = null
			});
		}

		try
		{
			IgpService service = new IgpService();
			service.SetUserCredentials(AppSecurity.CurrentUser.UserName, AppSecurity.CurrentUser.Password, AppSecurity.CurrentUser.UserId);
			var response = service.GetPaymentCurrencies();
			var customerBalanceData = new CustomerBalanceData();

			if (response.ServiceResponse.HasErrors)
				customerBalanceData = service.GetAccountBalances().Balances.FirstOrDefault(B => B.CCY.Contains(currencyCode));

			balanceDetails = customerBalanceData.AccountNumber + "/" + customerBalanceData.BalanceAvailable + "/" + customerBalanceData.CCY;
		}
		catch (Exception e)
		{
			Logger.Error(e.Message);
		}

		return balanceDetails;
	}

	/// <summary>
	/// Get customer account of a specific currency
	/// </summary>
	/// <param name="currencyCode"></param>
	/// <returns>CustomerBalanceData</returns>
	public static CustomerBalanceData GetAccountBalance(string currencyCode)
	{
		var customerBalanceData = new CustomerBalanceData();
		try
		{
			IgpService service = new IgpService();
			service.SetUserCredentials(AppSecurity.CurrentUser.UserName, AppSecurity.CurrentUser.Password, AppSecurity.CurrentUser.UserId);
			var response = service.GetPaymentCurrencies();

			if (!response.ServiceResponse.HasErrors)
				customerBalanceData = service.GetAccountBalances().Balances.FirstOrDefault(B => B.CCY.Contains(currencyCode));
		}
		catch (Exception e)
		{
			Logger.Error(e.Message);
		}

		return customerBalanceData;
	}

	public static IEnumerable<SelectListItem> ApiGetAccountBalances(UserInfo ui)
	{
		var list = new List<SelectListItem>();

		try
		{
			IgpService service = new IgpService();
			service.SetUserCredentials(ui.UserName, ui.Password, ui.UserId);
			var response = service.GetPaymentCurrencies();
			var listCustomerBalanceData = new List<CustomerBalanceData>();
			FavoriteCurrencyRepository cr = new FavoriteCurrencyRepository();
			var favoritecurrency = cr.GetFavoriteCurrencyList(ui.UserId);
			listCustomerBalanceData = service.GetAccountBalances().Balances.ToList();

			if (response.ServiceResponse.HasErrors)
				throw new Exception(response.ServiceResponse.Responses[0].Message);
			if (favoritecurrency.Count == 0)
			{
				foreach (var item in listCustomerBalanceData)
				{
					list.Add(
						new SelectListItem
						{
							Selected = item.CCY == item.BaseCCY,
							Text = item.CCY + GlobalRes.UIHelper_GetAccountBalances____Available_ +
								   String.Format(new CultureInfo("en-US"), "{0:F}", item.BalanceAvailable)
									   .Replace(".", ","),
							Value = item.AccountId
						});
				}
			}
			else
			{
				foreach (var item in favoritecurrency)
				{
					var ccyName = response.Currencies.FirstOrDefault(f => f.CurrencyCode == item.CurrencyCode);
					if (ccyName != null)
					{
						var ccyBalance = listCustomerBalanceData.FirstOrDefault(f => f.CCY == ccyName.CurrencyCode);
						if (ccyBalance != null)
						{
							list.Add(new SelectListItem
							{
								Selected = ccyBalance.CCY == ccyBalance.BaseCCY,
								Text = ccyBalance.CCY + GlobalRes.UIHelper_GetAccountBalances____Available_ +
										   String.Format(new CultureInfo("en-US"), "{0:F}",
											   ccyBalance.BalanceAvailable),
								Value = ccyBalance.AccountId
							}
							);
						}
					}
				}

				foreach (var customerBalanceData in listCustomerBalanceData)
				{
					if (list.FirstOrDefault(f => f.Value == customerBalanceData.AccountId) == null)
					{
						list.Add(
							new SelectListItem
							{
								Selected = customerBalanceData.CCY == customerBalanceData.BaseCCY,
								Text = customerBalanceData.CCY + GlobalRes.UIHelper_GetAccountBalances____Available_ +
									   String.Format(new CultureInfo("en-US"), "{0:F}", customerBalanceData.BalanceAvailable)
										   .Replace(".", ","),
								Value = customerBalanceData.AccountId
							});
					}
				}
			}


		}
		catch (Exception e)
		{
			Logger.Error(e.Message);
		}

		return list;
	}

	public static IEnumerable<SelectListItem> ApiGetAccountBalancesNoAvailable(UserInfo ui)
	{
		var list = new List<SelectListItem>();

		try
		{
			IgpService service = new IgpService();
			service.SetUserCredentials(ui.UserName, ui.Password, ui.UserId);
			var response = service.GetPaymentCurrencies();
			var listCustomerBalanceData = new List<CustomerBalanceData>();
			FavoriteCurrencyRepository cr = new FavoriteCurrencyRepository();
			var favoritecurrency = cr.GetFavoriteCurrencyList(ui.UserId);
			listCustomerBalanceData = service.GetAccountBalances().Balances.ToList();

			if (response.ServiceResponse.HasErrors)
				throw new Exception(response.ServiceResponse.Responses[0].Message);
			if (favoritecurrency.Count == 0)
			{
				foreach (var item in listCustomerBalanceData)
				{
					list.Add(
						new SelectListItem
						{
							Selected = item.CCY == item.BaseCCY,
							// Text = item.CCY + GlobalRes.UIHelper_GetAccountBalances____Available_ + String.Format(new CultureInfo("en-US"), "{0:F}", item.BalanceAvailable).Replace(".", ","),
							Text = item.CCY + " " + String.Format(new CultureInfo("en-US"), "{0:#,0.####}", item.BalanceAvailable),
							Value = item.AccountId
						});
				}
			}
			else
			{
				foreach (var item in favoritecurrency)
				{
					var ccyName = response.Currencies.FirstOrDefault(f => f.CurrencyCode == item.CurrencyCode);
					if (ccyName != null)
					{
						var ccyBalance = listCustomerBalanceData.FirstOrDefault(f => f.CCY == ccyName.CurrencyCode);
						if (ccyBalance != null)
						{
							list.Add(new SelectListItem
							{
								Selected = ccyBalance.CCY == ccyBalance.BaseCCY,
								Text = ccyBalance.CCY + " " + String.Format(new CultureInfo("en-US"), "{0:#,0.####}", ccyBalance.BalanceAvailable),
								//Text = ccyBalance.CCY + GlobalRes.UIHelper_GetAccountBalances____Available_ + String.Format(new CultureInfo("en-US"), "{0:F}", ccyBalance.BalanceAvailable),
								Value = ccyBalance.AccountId
							}
							);
						}
					}
				}

				foreach (var customerBalanceData in listCustomerBalanceData)
				{
					if (list.FirstOrDefault(f => f.Value == customerBalanceData.AccountId) == null)
					{
						list.Add(
							new SelectListItem
							{
								Selected = customerBalanceData.CCY == customerBalanceData.BaseCCY,
								// Text = customerBalanceData.CCY + GlobalRes.UIHelper_GetAccountBalances____Available_ +
								//       String.Format(new CultureInfo("en-US"), "{0:#,0.####}", customerBalanceData.BalanceAvailable)
								//           .Replace(".", ","),
								Text = customerBalanceData.CCY + " " + String.Format(new CultureInfo("en-US"), "{0:#,0.####}", customerBalanceData.BalanceAvailable),
								Value = customerBalanceData.AccountId
							});
					}
				}
			}


		}
		catch (Exception e)
		{
			Logger.Error(e.Message);
		}

		return list;
	}
	public static IEnumerable<SelectListItem> ApiGetAccountBalancesForDa(UserInfo ui, bool needClearData = false)
	{
		var list = new List<SelectListItem>();
		if (!needClearData)
		{
			list.Add(new SelectListItem()
			{
				Text = GlobalRes.Shared_NewInstantPaymentFormPage_SelectCurrency,
				Value = String.Empty
			});
		}

		try
		{
			IgpService service = new IgpService();
			service.SetUserCredentials(ui.UserName, ui.Password, ui.UserId);
			var response = service.GetPaymentCurrencies();
			var listCustomerBalanceData = new List<CustomerBalanceData>();
			FavoriteCurrencyRepository cr = new FavoriteCurrencyRepository();
			var favoritecurrency = cr.GetFavoriteCurrencyList(ui.UserId)
				.GroupBy(gb => gb.CurrencyCode).ToList();
			listCustomerBalanceData = service.GetAccountBalances().Balances.ToList();

			if (response.ServiceResponse.HasErrors)
				favoritecurrency.Clear();

			var currencies = response.Currencies.Join(favoritecurrency, currency => currency.CurrencyCode,
				favoritecurrencies => favoritecurrencies.Key,
				(currency, favoritecurrencies) => new
				{
					favoritecurrencies.Key,
					currency.CurrencyName,
					currency.CurrencyCode
				}).ToList();
			var cryptoCurrency = new[] { "BTC", "ETH" };
			foreach (var currency in currencies.Where(w =>
				listCustomerBalanceData.Select(s => s.CCY).Contains(w.CurrencyCode)))
			{
				var item = listCustomerBalanceData.FirstOrDefault(f => f.CCY == currency.CurrencyCode);
				if (item != null)
				{
					list.Add(
						new SelectListItem
						{
							Selected = item.CCY == item.BaseCCY,
							Text = item.CCY,
							//needClearData ? item.AccountNumber : String.Format(
							//    "{0} {1} {2}", item.CCY, GlobalRes.UIHelper_GetAccountBalances____Available_,
							//                                           cryptoCurrency.Contains(item.CCY) ?
							//                                               String.Format(new CultureInfo("en-US"), "{0:N8}", item.BalanceAvailable)
							//                                               :
							//                                               String.Format(new CultureInfo("en-US"), "{0:N2}", item.BalanceAvailable)),
							Value = item.CCY
						});
				}
			}

			foreach (var item in listCustomerBalanceData.Where(w =>
				!currencies.Select(s => s.CurrencyCode).Contains(w.CCY)))
			{
				list.Add(new SelectListItem
				{
					Selected = item.CCY == item.BaseCCY,
					Text = item.CCY,
					//needClearData ? item.AccountNumber : String.Format(
					//    "{0} {1} {2}", item.CCY, GlobalRes.UIHelper_GetAccountBalances____Available_,
					//    cryptoCurrency.Contains(item.CCY) ?
					//        String.Format(new CultureInfo("en-US"), "{0:N8}", item.BalanceAvailable)
					//        :
					//        String.Format(new CultureInfo("en-US"), "{0:N2}", item.BalanceAvailable)),
					Value = item.CCY
				});
			}
		}
		catch (Exception e)
		{
			Logger.Error(e.Message);
		}
		return list;
	}

	public static IEnumerable<SelectListItem> GetDatePeriods()
	{
		var list = new List<SelectListItem>();
		list.Add(new SelectListItem { Text = GlobalRes.UIHelper_GetDatePeriods_Today, Value = DatePeriod.Today.ToString() });
		list.Add(new SelectListItem { Text = GlobalRes.UIHelper_GetDatePeriods_Last_Week, Value = DatePeriod.LastWeek.ToString() });
		list.Add(new SelectListItem { Text = GlobalRes.UIHelper_GetDatePeriods_Last_Month, Value = DatePeriod.LastMonth.ToString() });
		list.Add(new SelectListItem { Text = GlobalRes.UIHelper_GetDatePeriods_Last_3_Months, Value = DatePeriod.Last3Months.ToString() });
		list.Add(new SelectListItem { Text = GlobalRes.UIHelper_GetDatePeriods_Last_6_Months, Value = DatePeriod.Last6Months.ToString() });
		return list;
	}
	public static IEnumerable<SelectListItem> PrepareAccountAliases()
	{
		var aliasListItem = new List<SelectListItem>();
		IgpService service = new IgpService(AppSecurity.CurrentUser.AccessToken, AppSecurity.CurrentUser.UserId);
		var aliases = service.GetAccountAliases();
		Logger.Debug(string.Format("aliases response: {0}", JsonConvert.SerializeObject(aliases)));
		if (!aliases.ServiceResponse.HasErrors)
		{

			aliasListItem.AddRange(aliases.Aliases.Select(s => new SelectListItem()
			{
				Text = s.AccountAlias,
				Value = s.AccountAlias,
				Selected = s.IsDefault
			}));
			//foreach (var item in aliases.Aliases)
			//{
			//    aliasListItem.Add
			//    (
			//        new SelectListItem
			//        {
			//            Text = item.AccountAlias,
			//            Value = item.AccountAlias
			//        }
			//    );
			//}
		}

		return aliasListItem;
	}

	public static IEnumerable<SelectListItem> ListOfCryptoCurrency()
	{
		var ccy = new List<SelectListItem>();
		ccy.Add(new SelectListItem() { Text = "BTC", Value = "BTC" });
		ccy.Add(new SelectListItem() { Text = "DASH", Value = "DASH" });
		return ccy;
	}

	public static IList<SelectListItem> PrepareAvailableFavoriteCurrencies()
	{
		var currenciesListItem = new List<SelectListItem>();
		try
		{
			IgpService Service = new IgpService(AppSecurity.CurrentUser.UserName, AppSecurity.CurrentUser.Password, AppSecurity.CurrentUser.UserId);

			var currencies = Service.GetPaymentCurrencies();

			//currenciesListItem.Add
			//(
			//    new SelectListItem
			//    {
			//        Text = "Select",
			//        Value = ""
			//    }
			//);
			FavoriteCurrencyRepository cr = new FavoriteCurrencyRepository();
			var favoritecurrency = cr.GetFavoriteCurrencyList();
			if (favoritecurrency.Count > 0)
			{
				foreach (var item in favoritecurrency)
				{
					var currCurrency =
						currencies.Currencies.FirstOrDefault(a => a.CurrencyCode == item.CurrencyCode);

					if (currCurrency != null)
					{
						currenciesListItem.Add
						(
							new SelectListItem
							{
								Text = currCurrency.CurrencyCode,
								Value = currCurrency.CurrencyCode
							}
						);
					}

				}
			}
			else
			{
				if (!currencies.ServiceResponse.HasErrors)
				{
					foreach (var item in currencies.Currencies)
					{
						currenciesListItem.Add
						(
							new SelectListItem
							{
								Text = item.CurrencyCode,
								Value = item.CurrencyCode
							}
						);
					}
				}
			}
		}
		catch (Exception ex) { Logger.Error(ex); }

		return currenciesListItem;
	}

	public static IList<Tsg.UI.Main.Models.CurrencyViewModel> PrepareCurrencies()
	{
		var Currencies = new List<Tsg.UI.Main.Models.CurrencyViewModel>();

		// IgpService Service = new IgpService();
		IgpService Service = new IgpService(AppSecurity.CurrentUser.UserName, AppSecurity.CurrentUser.Password, AppSecurity.CurrentUser.UserId);

		var response = Service.GetPaymentCurrencies();
		if (!response.ServiceResponse.HasErrors)
		{
			FavoriteCurrencyRepository fr = new FavoriteCurrencyRepository();
			var allfavoriteCurrencies = fr.GetFavoriteCurrencyList();
			foreach (CurrencyData data in response.Currencies)
			{
				Currencies.Add(new Tsg.UI.Main.Models.CurrencyViewModel
				{
					// CurrencyId = data.CurrencyId,
					CurrencyCode = data.CurrencyCode,
					CurrencyName = data.CurrencyName,
					CurrencyAmountScale = data.CurrencyAmountScale,
					CurrencyRateScale = data.CurrencyRateScale,
					Symbol = data.Symbol,
					PaymentCutoffTime = data.PaymentCutoffTime,
					SettlementDaysToAdd = data.SettlementDaysToAdd,
					IsFavorite = allfavoriteCurrencies.Any(a => a.CurrencyCode == data.CurrencyCode)
				});
			}
		}

		return Currencies;
	}

	/// <summary>
	/// Get Customer base currency from the Core
	/// </summary>
	/// <returns></returns>
	public static string GetCustomerBaseCurrencyCode()
	{
		var baseCurrency = string.Empty;
		IgpService Service = new IgpService(AppSecurity.CurrentUser.UserName, AppSecurity.CurrentUser.Password, AppSecurity.CurrentUser.UserId);
		var customerGetSingleResponse = Service.CustomerGetSingle();
		Logger.Debug(string.Format("customerGetSingleResponse: {0}", JsonConvert.SerializeObject(customerGetSingleResponse)));
		if (!customerGetSingleResponse.ServiceResponse.HasErrors)
		{
			baseCurrency = customerGetSingleResponse.CustomerGetSingleData.BaseCurrencyCode;
		}

		return baseCurrency;
	}
}
