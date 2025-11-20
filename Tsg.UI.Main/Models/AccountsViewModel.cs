using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models.Helpers;
using Tsg.UI.Main.Repository;

namespace Tsg.UI.Main.Models
{
	public class AccountsViewModel : BaseViewModel
	{
		public AccountBalancesListViewModel AccountBalancesModel { get; set; }

		public AccountsViewModel()
		{
			AccountBalancesModel = new AccountBalancesListViewModel();
		}

		public void PrepareAccountBalances()
		{
			var currenciesResponse = Service.GetPaymentCurrencies();
			CurrencyData[] currencies = new CurrencyData[0];
			if (!currenciesResponse.ServiceResponse.HasErrors)
			{
				currencies = currenciesResponse.Currencies;
			}


			var response = Service.GetAccountBalances();


			if (!response.ServiceResponse.HasErrors)
			{
				var allowedCurrencies = AppSettingHelper.GetAllowedCurrencies();
				_logger.Debug(string.Format("Allowed currencies, count: {0} items: {1}", allowedCurrencies.Count, JsonConvert.SerializeObject(allowedCurrencies)));
				if (allowedCurrencies.Count > 0 && !string.IsNullOrEmpty(allowedCurrencies[0]))
				{
					response.Balances = response.Balances.Where(c => allowedCurrencies.Any(ac => ac == c.CCY)).ToArray();
					_logger.Debug(string.Format("after filter allowed currencies, Balances: {0}", JsonConvert.SerializeObject(response.Balances)));
				}

				foreach (CustomerBalanceData data in response.Balances)
				{
					var availableBalance = data.BalanceAvailable;
					var balance = data.Balance;

					var currency = currencies.FirstOrDefault(c => c.CurrencyCode == data.CCY);
					if (currency != null)
					{
						availableBalance = @Math.Round(data.BalanceAvailable, currency.CurrencyAmountScale);
						balance = @Math.Round(data.Balance, currency.CurrencyAmountScale);

					}
					this.AccountBalancesModel.Balances.Add(new AccountBalanceViewModel()
					{
						AccountId = new Guid(data.AccountId),
						AccountNumber = data.AccountNumber,
						Currency = data.CCY,
						Balance = balance,
						ActiveHoldsTotal = data.ActiveHoldsTotal,
						BalanceAvailable = availableBalance,
						BaseCurrency = data.BaseCCY,
						BalanceAvailableBase = data.BalanceAvailableBase
					});
				}
				SessionHelper.Set(Constants.TsgKeyBalances, this.AccountBalancesModel.Balances);
			}
		}
		public void PrepareFavoriteCurrency()
		{
			PaymentCurrenciesListViewModel model = new PaymentCurrenciesListViewModel();
			model.PrepareCurrencies();
			this.AccountBalancesModel.FavoriteCurrecy = string.Join("|", model.Currencies.Where(w => w.IsFavorite).Select(s => s.CurrencyCode));
		}

		public void PrepareAccountStatements()
		{
			var response = Service.GetAccontStatements(new Guid("3d15c8a4-fa28-e211-a5e3-002590067645"), DateTime.Now, DateTime.Now);
			Console.WriteLine();
		}

	}
}