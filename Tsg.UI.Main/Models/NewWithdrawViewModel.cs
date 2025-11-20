using Newtonsoft.Json;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;


namespace Tsg.UI.Main.Models
{
    public class NewWithdrawViewModel : BaseViewModel
    {

        //readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public NewWithdrawViewModel()
        {
            Service = new IgpService(AppSecurity.CurrentUser.AccessToken, AppSecurity.CurrentUser.UserId);
            PrepareValues();
        }

        public NewWithdrawViewModel(Guid? paymentId)
            : this()
        {
            if (paymentId.HasValue)
            {
                PaymentId = paymentId.Value;
                PrepareDetails();
            }
            else
            {
                //GetLastCustomerAlias();
            }
        }

        public NewWithdrawViewModel(string alias, string ccy, decimal amount, string invoice, string memo)
            : this()
        {
            PrepareDetails(alias, ccy, amount, invoice, memo);
        }

        public NewWithdrawViewModel(string alias, string ccy, decimal amount, string invoice, string memo, Guid receiveId)
            : this()
        {
            PrepareDetails(alias, ccy, amount, invoice, memo, receiveId);
        }

        public Guid CustomerId { get; set; }
        public Guid PaymentId { get; set; }
        public string PaymentReference{ get; set; }
        public Guid MerchantId { get; set; }
        public string MerchantAlias { get; set; }
        public string CustomerAlias { get; set; }
        public string ReceiveId { get; set; }

        //public string FromAccount { get; set; }
        //public string ToCustomer { get; set; }
        //public IList<SelectListItem> PriorUsedAliases { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Amount { get; set; }
        
        //public Guid InstantPaymentReceiveId { get; set; }

        //public IList<SelectListItem> AvailableCurrencies { get; set; }
        //public IList<CurrencyData> CustomerAvailableCurrencies { get; set; }
        //public IList<CustomerBalanceData> CustomerAvailableBalances { get; set; }

        public IList<SelectListItem> AccountAliases { get; set; }

        public override string ToString()
        {
            return "[Payment Id=" + PaymentId + "],[Merchant Id=" + MerchantId + "],[Merchant Alias=" + MerchantAlias + "],[Customer Alias=" + CustomerAlias + "],[Amount=" + Amount + "],[Currency Code=" + CurrencyCode + "]";
        }

        /// <summary>
        /// Create withdraw
        /// </summary>
        /// <param name="paymentId"></param>
        public void Create(out Guid paymentId, out string paymentReference)
        {
            var response = Service.CreateInstantPayment(CustomerAlias, MerchantAlias, Amount, CurrencyCode, 3);
            _logger.Debug(string.Format("create new instant payment, response:{0}", JsonConvert.SerializeObject(response)));
            paymentId = Guid.Empty;
            paymentReference = string.Empty;
            if (!response.ServiceResponse.HasErrors)
            {
                var model = new InstantPaymentPostResultViewModel(new Guid(response.PaymentInformation.PaymentId));
                paymentId = new Guid(response.PaymentInformation.PaymentId);
                paymentReference = response.PaymentInformation.PaymentReference;
                PaymentId = paymentId;
                PaymentReference = paymentReference;
            }
            else
            {
                this.GetErrorMessages(response.ServiceResponse.Responses);
            }
        }

        /// <summary>
        /// Confirm withdraw
        /// </summary>
        public bool Confirm()
        {
            var response = Service.PostInstantPayment(PaymentId);
            _logger.Debug(string.Format("Confirm withdraw, post instant payment, response:{0}", JsonConvert.SerializeObject(response)));
            
            if (!response.ServiceResponse.HasErrors)
            {
                return true;
            }
            return false;
        }

        public void PrepareDetails()
        {
            try
            {
                Tsg.Business.Model.TsgGPWebService.InstantPaymentGetSingleResponse response = Service.GetInstantPaymentDetails(this.PaymentId);

                if (!response.ServiceResponse.HasErrors)
                {
                    this.PaymentId = new Guid(response.Payment.PaymentId);
                    this.CustomerAlias = response.Payment.FromCustomerAlias;
                    this.MerchantAlias = response.Payment.ToCustomerAlias;
                    this.Amount = response.Payment.Amount;
                    this.CurrencyCode = response.Payment.CCY;
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
                this.MerchantAlias = alias;
                this.Amount = amount;
                this.CurrencyCode = ccy;
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
                this.MerchantAlias = alias;
                this.Amount = amount;
                this.CurrencyCode = ccy;

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

                    ////Get the last customer's alias name to display in the create instant payment form
                    //if (string.IsNullOrEmpty(this.ToCustomer) && !instantPayment.IsIncome)
                    //{
                    //    this.ToCustomer = instantPayment.ToCustomerAlias;
                    //}
                }
            }
        }

        private void PrepareValues()
        {
            CustomerId = Service.GetCustomerIdGuid;
            //AvailableCurrencies = PrepareAvailablePaymentCurrencies();
            //CustomerAvailableCurrencies = PrepareCustomerAvailablePaymentCurrencies();
            //CustomerAvailableBalances = PrepareCustomerAvailableBalances();
            AccountAliases = PrepareAccountAliases();
            //PriorUsedAliases = PreparePriorUsedAliases();
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
            _logger.Debug(string.Format("Allowed currencies, count: {0} items: {1}", allowedCurrencies.Count,JsonConvert.SerializeObject(allowedCurrencies)));
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
                            Value = item.AccountAlias
                        }
                    );
                }
            }
            return aliasListItem;
        }

        //private IList<SelectListItem> PreparePriorUsedAliases()
        //{
        //    var aliasList = new List<SelectListItem>();
        //    aliasList.Add
        //    (
        //        new SelectListItem
        //        {
        //            Text = GlobalRes.NewInstantPaymentViewModel_PreparePriorUsedAliases_OrSelectPrior,
        //            Value = ""
        //        }
        //    );

        //    var response = Service.GetLatestInstantPayments(1000000);
        //    if (!response.ServiceResponse.HasErrors)
        //    {
        //        aliasList.AddRange(response.Payments.Where(w => w.Status == "Posted").OrderByDescending(ob => ob.CreatedTime).GroupBy(gb => gb.ToCustomerAlias).Take(5).Select(s => new SelectListItem()
        //        {
        //            Text = s.Key,
        //            Value = s.Key
        //        }));
        //    }

        //    return aliasList;
        //}

        /// <summary>
        /// Is the customer is a merchant
        /// </summary>
        /// <returns>True: customer is a merchant</returns>
        public bool IsMerchant()
        {
            var data = this.GetCustomerDetails();
            if (data.IsCashOut)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get customer details
        /// </summary>
        /// <returns>Customer data</returns>
        public CustomerGetSingleData GetCustomerDetails()
        {
            var response = Service.GetCustomerGetSingleFromAlias(MerchantAlias);
            if (!response.ServiceResponse.HasErrors && response.CustomerGetSingleData.IsCashOut)
            {
                return response.CustomerGetSingleData;
            }

            return new CustomerGetSingleData();
        }
    }
}