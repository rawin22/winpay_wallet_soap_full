using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;


namespace Tsg.UI.Main.Models
{
	public class NewInstantPaymentViewModel : BaseViewModel
	{

		readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public NewInstantPaymentViewModel()
		{
			Service = new IgpService(AppSecurity.CurrentUser.AccessToken, AppSecurity.CurrentUser.UserId);
			PrepareValues();
		}

		public NewInstantPaymentViewModel(Guid? paymentId)
			: this()
		{
			if (paymentId.HasValue)
			{
				PaymentId = paymentId.Value;
				PrepareDetails();
			}
			else
			{
				CustomerBaseCurrencyCode = GetCustomerBaseCurrencyCode();
				CurrencyCode = CustomerBaseCurrencyCode;
				//GetLastCustomerAlias();
			}
		}

		public NewInstantPaymentViewModel(string alias, string ccy, decimal amount, string invoice, string memo)
			: this()
		{
			PrepareDetails(alias, ccy, amount, invoice, memo);
		}

		public NewInstantPaymentViewModel(string alias, string ccy, decimal amount, string invoice, string memo, Guid receiveId)
			: this()
		{
			PrepareDetails(alias, ccy, amount, invoice, memo, receiveId);
		}

		public Guid PaymentId { get; set; }
		public Guid CustomerId { get; set; }
		[AllowHtml]
		public string FromCustomer { get; set; }
		public string FromAccount { get; set; }
		public string ToCustomer { get; set; }
		public IList<SelectListItem> PriorUsedAliases { get; set; }
		public string CurrencyCode { get; set; }
		public decimal Amount { get; set; }
		public string Memo { get; set; }
		public bool IsAutoExchangeAvaliable { get; set; }
		public Guid ResultPaymentId { get; set; }
		public string ResultPaymentReference { get; set; }
		public string ReasonForPayment { get; set; }
		public string InstantPay { get; set; }

		public string Action { get; set; }

		public Guid InstantPaymentReceiveId { get; set; }

		public IList<SelectListItem> AvailableCurrencies { get; set; }
		public IList<CurrencyData> CustomerAvailableCurrencies { get; set; }
		public IList<CustomerBalanceData> CustomerAvailableBalances { get; set; }

		public IList<SelectListItem> AccountAliases { get; set; }
		public string CustomerBaseCurrencyCode { get; set; }

		public override string ToString()
		{
			return "[Payment Id=" + PaymentId + "],[Customer Id=" + CustomerId + "],[From Customer=" +
				FromCustomer + "],[To Customer=" + ToCustomer + "],[Amount=" + Amount + "],[Currency Code=" + CurrencyCode +
				"],[Memo=" + Memo + "]";
		}

		/// <summary>
		/// Create instant payment
		/// </summary>
		/// <param name="paymentId"></param>
		public void Create(out Guid? paymentId)
		{
			var response = Service.CreateNewPayment(FromCustomer, ToCustomer, Amount, CurrencyCode, Memo, InstantPay, reasonForPayment: ReasonForPayment);
			_logger.Debug(string.Format("create new payment, response:{0}", JsonConvert.SerializeObject(response)));
			paymentId = null;
			if (!response.ServiceResponse.HasErrors)
			{
				var model = new InstantPaymentPostResultViewModel(new Guid(response.PaymentInformation.PaymentId));
				paymentId = new Guid(response.PaymentInformation.PaymentId);
				if (Action == "post" || Action == "exchangeConfirmed")
				{
					model.Post();
					if (model.Errors.Any())
					{
						_logger.ErrorFormat("Posting payment error, payment Id: {0} :{1}", paymentId, JsonConvert.SerializeObject(response));
						//this.HasError = true;
						//this.Errors = model.Errors;
					}
				}
			}
			else
			{
				this.GetErrorMessages(response.ServiceResponse.Responses);
			}
		}

		public void PrepareDetails()
		{
			try
			{
				Tsg.Business.Model.TsgGPWebService.InstantPaymentGetSingleResponse response = Service.GetInstantPaymentDetails(this.PaymentId);

				if (!response.ServiceResponse.HasErrors)
				{
					this.PaymentId = new Guid(response.Payment.PaymentId);
					this.FromCustomer = response.Payment.FromCustomerAlias;
					this.ToCustomer = response.Payment.ToCustomerAlias;
					this.Amount = response.Payment.Amount;
					this.CurrencyCode = response.Payment.CCY;
					this.Memo = response.Payment.BankMemo;
				}
				else _logger.Error($"{response.ServiceResponse.Responses[0].Message}\n\r{response.ServiceResponse.Responses[0].MessageDetails}");
			}
			catch (Exception e)
			{
				_logger.Error(e);
			}
		}



		public void PrepareDetails(string alias, string ccy, decimal amount, string invoice, string memo)
		{
			try
			{
				this.ToCustomer = alias;
				this.Amount = amount;
				this.CurrencyCode = string.IsNullOrEmpty(ccy) ? CustomerBaseCurrencyCode : ccy;
				this.InstantPay = invoice;
				this.Memo = memo;
			}
			catch (Exception e)
			{
				_logger.Error(e);
			}
		}

		public void PrepareDetails(string alias, string ccy, decimal amount, string invoice, string memo, Guid receiveId)
		{
			try
			{
				this.ToCustomer = alias;
				this.Amount = amount;
				this.CurrencyCode = ccy;
				this.InstantPay = invoice;
				this.Memo = memo;
			}
			catch (Exception e)
			{
				_logger.Error(e);
			}
		}

		public void GetLastCustomerAlias()
		{
			var response = Service.GetLatestInstantPayments(10000000);

			if (!response.ServiceResponse.HasErrors)
			{
				foreach (InstantPaymentSearchData data in response.Payments)
				{
					InstantPaymentViewModel instantPayment = new InstantPaymentViewModel()
					{
						PaymentId = new Guid(data.PaymentId),
						PaymentReference = data.PaymentReference,
						Status = data.Status,
						FromCustomerAlias = data.FromCustomerAlias,
						ToCustomerAlias = data.ToCustomerAlias,
						FromCustomerName = data.FromCustomerName,
						ToCustomerName = data.ToCustomerName,
						Amount = data.Amount,
						Currency = data.CCY,
						ValueDate = Convert.ToDateTime(data.ValueDate),
						CreatedTime = Convert.ToDateTime(data.CreatedTime),
						IsIncome = (AppSecurity.CurrentUser != null && AppSecurity.CurrentUser.OrganisationName != null
						&& !AppSecurity.CurrentUser.OrganisationName.ToUpper().Equals(data.FromCustomerName.ToUpper()))
					};

					//Get the last customer's alias name to display in the create instant payment form
					if (string.IsNullOrEmpty(this.ToCustomer) && !instantPayment.IsIncome)
					{
						this.ToCustomer = instantPayment.ToCustomerAlias;
					}
				}
			}
		}

		private void PrepareValues()
		{
			CustomerId = Service.GetCustomerIdGuid;
			AvailableCurrencies = PrepareAvailablePaymentCurrencies();
			CustomerAvailableCurrencies = PrepareCustomerAvailablePaymentCurrencies();
			CustomerAvailableBalances = PrepareCustomerAvailableBalances();
			AccountAliases = PrepareAccountAliases();
			PriorUsedAliases = PreparePriorUsedAliases();
			CustomerBaseCurrencyCode = GetCustomerBaseCurrencyCode();
		}

		private IList<SelectListItem> PrepareAvailablePaymentCurrencies()
		{
			var currencies = Service.GetPaymentCurrencies();
			FavoriteCurrencyRepository cr = new FavoriteCurrencyRepository();
			if (currencies.ServiceResponse.HasErrors)
				return new List<SelectListItem>();
			var favoritecurrency = cr.GetFavoriteCurrencyList();
			if (favoritecurrency.Count > 0)
			{
				var currenciesListItem = new List<SelectListItem>();

				currenciesListItem.AddRange(favoritecurrency.Join(currencies.Currencies, favorite => favorite.CurrencyCode, inner => inner.CurrencyCode,
					(favorite, inner) => new SelectListItem
					{
						Text = inner.CurrencyCode,
						Value = inner.CurrencyCode
					}));
				currenciesListItem.AddRange(currencies.Currencies.Where(a => !currenciesListItem.Select(s => s.Value).Contains(a.CurrencyCode)).Select(s => new SelectListItem()
				{
					Text = s.CurrencyCode,
					Value = s.CurrencyCode
				}));
				return currenciesListItem;
			}
			return currencies.Currencies.Select(s => new SelectListItem { Text = s.CurrencyCode, Value = s.CurrencyCode }).ToList();
		}

		private IList<CurrencyData> PrepareCustomerAvailablePaymentCurrencies()
		{
			var currencies = Service.GetPaymentCurrencies();
			FavoriteCurrencyRepository cr = new FavoriteCurrencyRepository();
			if (currencies.ServiceResponse.HasErrors)
				return new List<CurrencyData>();

			var allowedCurrencies = AppSettingHelper.GetAllowedCurrencies();
			_logger.Debug(string.Format("Allowed currencies, count: {0} items: {1}", allowedCurrencies.Count, JsonConvert.SerializeObject(allowedCurrencies)));
			if (allowedCurrencies.Count > 0 && !string.IsNullOrEmpty(allowedCurrencies[0]))
			{
				currencies.Currencies = currencies.Currencies.Where(c => allowedCurrencies.Any(ac => ac == c.CurrencyCode)).ToArray();
				_logger.Debug(string.Format("after filter allowed currencies, currencies: {0}", JsonConvert.SerializeObject(currencies.Currencies)));
			}

			var favoritecurrency = cr.GetFavoriteCurrencyList();
			if (favoritecurrency.Count > 0)
			{
				var currenciesListItem = new List<CurrencyData>();

				currenciesListItem.AddRange(currencies.Currencies.Where(c => favoritecurrency.Any(f => f.CurrencyCode == c.CurrencyCode)));
				currenciesListItem.AddRange(currencies.Currencies.Where(c => !favoritecurrency.Any(f => f.CurrencyCode == c.CurrencyCode)));
				return currenciesListItem;
			}
			return currencies.Currencies;
		}

		public IList<CustomerBalanceData> PrepareCustomerAvailableBalances()
		{
			var balances = Service.GetAccountBalances();
			if (balances.ServiceResponse.HasErrors)
				return new List<CustomerBalanceData>();

			return balances.Balances;
		}

		public IList<SelectListItem> PrepareAllAvailablePaymentCurrencies()
		{
			var currenciesListItem = new List<SelectListItem>();
			var currencies = Service.GetPaymentCurrencies();
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
			return currenciesListItem;
		}

		/// <summary>
		/// Prepare a list of all available currencies
		/// </summary>
		/// <returns></returns>
		public IList<CurrencyData> PrepareAllAvailableCurrencies()
		{
			var currencies = new List<CurrencyData>();
			var currenciesResponse = Service.GetPaymentCurrencies();
			if (!currenciesResponse.ServiceResponse.HasErrors)
			{
				currencies = currenciesResponse.Currencies.ToList();
			}

			var allowedCurrencies = AppSettingHelper.GetAllowedCurrencies();
			_logger.Debug(string.Format("Allowed currencies, count: {0} items: {1}", allowedCurrencies.Count, JsonConvert.SerializeObject(allowedCurrencies)));
			if (allowedCurrencies.Count > 0 && !string.IsNullOrEmpty(allowedCurrencies[0]))
			{
				currencies = currencies.Where(c => allowedCurrencies.Any(ac => ac == c.CurrencyCode)).ToList();
				_logger.Debug(string.Format("after filter allowed currencies, currencies: {0}", JsonConvert.SerializeObject(currencies)));
			}

			return currencies;
		}

		private IList<SelectListItem> PrepareAccountAliases()
		{
			var aliasListItem = new List<SelectListItem>();
			var aliases = Service.GetAccountAliases();
			if (!aliases.ServiceResponse.HasErrors)
			{
				foreach (var item in aliases.Aliases)
				{
					aliasListItem.Add
					(
						new SelectListItem
						{
							Text = item.AccountAlias,
							Value = item.AccountAlias,
							Selected = item.IsDefault
						}
					);
				}
			}
			return aliasListItem;
		}

		private IList<SelectListItem> PreparePriorUsedAliases()
		{
			var aliasList = new List<SelectListItem>();
			aliasList.Add
			(
				new SelectListItem
				{
					Text = GlobalRes.NewInstantPaymentViewModel_PreparePriorUsedAliases_OrSelectPrior,
					Value = ""
				}
			);

			var response = Service.GetLatestInstantPayments(1000000);
			if (!response.ServiceResponse.HasErrors)
			{
				aliasList.AddRange(response.Payments.Where(w => w.Status == "Posted").OrderByDescending(ob => ob.CreatedTime).GroupBy(gb => gb.ToCustomerAlias).Take(5).Select(s => new SelectListItem()
				{
					Text = s.Key,
					Value = s.Key
				}));
			}

			return aliasList;
		}

		/// <summary>
		/// Get Customer base currency from the Core
		/// </summary>
		/// <returns></returns>
		private string GetCustomerBaseCurrencyCode()
		{
			var baseCurrency = string.Empty;

			var customerGetSingleResponse = Service.CustomerGetSingle();
			_logger.Debug(string.Format("customerGetSingleResponse: {0}", JsonConvert.SerializeObject(customerGetSingleResponse)));
			if (!customerGetSingleResponse.ServiceResponse.HasErrors)
			{
				baseCurrency = customerGetSingleResponse.CustomerGetSingleData.BaseCurrencyCode;
			}

			return baseCurrency;
		}
	}
}

public class AliasComparer : IEqualityComparer<SelectListItem>
{
	public bool Equals(SelectListItem x, SelectListItem y)
	{
		return (x.Value == y.Value);
	}

	public int GetHashCode(SelectListItem obj)
	{
		return obj.Value.ToString().GetHashCode();
	}
}