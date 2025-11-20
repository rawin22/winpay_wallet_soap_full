using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models.Helpers;
using Tsg.UI.Main.Models.Security;

namespace Tsg.UI.Main.Models
{
    public class DashboardViewModel : BaseViewModel
    {
        public AccountBalancesListViewModel AccountBalancesModel { get; set; }

        public NewInstantPaymentViewModel NewInstantPaymentViewModel { get; set; }

        public IList<InstantPaymentViewModel> LatestPayments { get; set; }
        /// <summary>
        /// List of instant payments in created status
        /// </summary>
        public IList<InstantPaymentViewModel> CreatedStatusInstantPayments { get; set; }
        /// <summary>
        /// Payouts list
        /// </summary>
        public IList<PayoutViewModel> CreatedStatePayouts { get; set; }

        public DashboardViewModel()
        {            
            AccountBalancesModel = new AccountBalancesListViewModel();
            NewInstantPaymentViewModel = new NewInstantPaymentViewModel(null);
            LatestPayments = new List<InstantPaymentViewModel>();
            CreatedStatePayouts = new List<PayoutViewModel>();
            CreatedStatusInstantPayments = new List<InstantPaymentViewModel>();
        }

        public void PrepareAccountBalances()
        {
            var currenciesResponse = Service.GetPaymentCurrencies();
            CurrencyData [] currencies = new CurrencyData[0];
            if (!currenciesResponse.ServiceResponse.HasErrors)
            {
                currencies = currenciesResponse.Currencies;
            }

            var response = Service.GetAccountBalances();
            if (!response.ServiceResponse.HasErrors)
            {
                IList<AccountBalanceViewModel> allItems = new List<AccountBalanceViewModel>();
                var allowedCurrencies = AppSettingHelper.GetAllowedCurrencies();
                _logger.Debug(string.Format("PrepareAccountBalances, Allowed currencies, count: {0} items: {1}", allowedCurrencies.Count, JsonConvert.SerializeObject(allowedCurrencies)));
                if (allowedCurrencies.Count > 0 && !string.IsNullOrEmpty(allowedCurrencies[0]))
                {
                    response.Balances​ = response.Balances​.Where(b => allowedCurrencies.Any(ac => ac == b.CCY)).ToArray();
                    _logger.Debug(string.Format("PrepareAccountBalances, after filter allowed currencies, Balances: {0}", JsonConvert.SerializeObject(response.Balances)));
                }

                foreach (CustomerBalanceData data in response.Balances)
                {
                    var availableBalance = data.BalanceAvailable;
                    var currency = currencies.FirstOrDefault(c => c.CurrencyCode == data.CCY);
                    if (currency != null)
                    {
                        availableBalance = @Math.Round(data.BalanceAvailable, currency.CurrencyAmountScale);
                    }

                    var newItem = new AccountBalanceViewModel()
                        {
                            AccountId = new Guid(data.AccountId),
                            AccountNumber = data.AccountNumber,
                            Currency = data.CCY,
                            Balance = data.Balance,
                            ActiveHoldsTotal = data.ActiveHoldsTotal,
                            BalanceAvailable = availableBalance,
                            BaseCurrency = data.BaseCCY,
                            BalanceAvailableBase = data.BalanceAvailableBase
                        };
                    
                    if (data.Balance != 0)
                    {
                        this.AccountBalancesModel.Balances.Add(newItem);
                    }
                    allItems.Add(newItem);
                }
                SessionHelper.Set(Constants.TsgKeyBalances, allItems);
            }
        }        

        public void PrepareLatestInstantPayments()
        {
            var response = Service.GetLatestInstantPayments(100000000);
            int counter = 0;

            if (!response.ServiceResponse.HasErrors)
            {
                foreach (InstantPaymentSearchData data in response.Payments)
                {
                    if (data.Status.Equals("Posted") && counter<10)
                    {
                        var instantPayment = new InstantPaymentViewModel()
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
                            IsIncome = (AppSecurity.CurrentUser != null && AppSecurity.CurrentUser.OrganisationName != null && !AppSecurity.CurrentUser.OrganisationName.ToUpper().Equals(data.FromCustomerName.ToUpper()))
                        };

                        this.LatestPayments.Add(instantPayment);

                        //Get the last customer's alias name to display in the create instant payment form
                        //if (string.IsNullOrEmpty(this.NewInstantPaymentViewModel.ToCustomer) && !instantPayment.IsIncome)
                        //{
                        //    this.NewInstantPaymentViewModel.ToCustomer = instantPayment.ToCustomerAlias;
                        //}

                        counter++;
                    }

                }
            }
        }

        /// <summary>
        /// Get payouts in created status
        /// </summary>
        public void PrepareCreatedStatusPayouts()
        {
            var response = Service.PaymentSearch(100000000);

            if (!response.ServiceResponse.HasErrors)
            {
                // INFO: eWallet September bug edited --Payment History default is showing oldest transactions first 
                //foreach (InstantPaymentSearchData data in response.Payments.OrderBy(ob => ob.CreatedTime))
                var payments = response.Payments.Where(p => p.PaymentStatusTypeName== "Created").OrderByDescending(p => p.CreatedTime);

                foreach (PaymentSearchData data in payments)
                {
                    PayoutViewModel instantPayment = new PayoutViewModel()
                    {
                        PaymentId = new Guid(data.PaymentId),
                        PaymentReference = data.PaymentReference,
                        Status = data.PaymentStatusTypeName,
                        CustomerName = data.CustomerName,
                        Amount = data.Amount,
                        AmountCurrency = data.AmountCurrencyCode,
                        AmountTextWithCurrencyCode = data.AmountTextWithCurrencyCode,
                        BeneficiaryName = data.BeneficiaryName,
                        ValueDate = Convert.ToDateTime(data.ValueDate).Date,
                        CreatedTime = Convert.ToDateTime(data.CreatedTime),
                        SumittedTime = String.IsNullOrEmpty(data.SubmittedTime) ? (DateTime?)null : Convert.ToDateTime(data.SubmittedTime)
                    };
                    this.CreatedStatePayouts.Add(instantPayment);
                }
            }
        }

        /// <summary>
        /// Prepare a list of instant payment in created status
        /// </summary>        
        public void PrepareCreatedStatusInstantPayments()
        {
            var response = Service.GetLatestInstantPayments(100000000);

            if (!response.ServiceResponse.HasErrors)
            {
                var instantPayments = response.Payments.Where(p => p.Status == "Created").OrderByDescending(p => p.CreatedTime);
                // var instantPayments = response.Payments.OrderByDescending(p => p.CreatedTime);

                foreach (InstantPaymentSearchData data in instantPayments)
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
                        IsIncome = (AppSecurity.CurrentUser != null && AppSecurity.CurrentUser.OrganisationName != null && !AppSecurity.CurrentUser.OrganisationName.ToUpper().Equals(data.FromCustomerName.ToUpper()))
                    };
                    this.CreatedStatusInstantPayments.Add(instantPayment);
                }
            }
        }

        /// <summary>
        /// Get white label logo
        /// </summary>
        public byte[] GetWhiteLabelProfileLogo()
        {
            //byte[] whiteLabelProfileResponse = Array.Empty<byte>();
            var response = Service.GetUserData();
            Debug.WriteLine("GetWhiteLabelProfileLogo, response: " + Json.Encode(response));

            if (!response.ServiceResponse.HasErrors && !string.IsNullOrEmpty(response.UserSettings.WhiteLabelProfileId))
            {
                var whiteLabelProfileResponse = Service.WhiteLabelProfileGetSingle(response.UserSettings.WhiteLabelProfileId);
                Debug.WriteLine("GetWhiteLabelProfileLogo, whiteLabelProfileResponse: " + Json.Encode(whiteLabelProfileResponse));
                return whiteLabelProfileResponse.Profile.ReportLogo;
            }

            return null;
        }

        /// <summary>
        /// Get white label logo
        /// </summary>
        public string GetWhiteLabelProfileEmailLogoUrl()
        {
            var response = Service.GetUserData();
            Debug.WriteLine("GetWhiteLabelProfileEmailLogoUrl, response: " + Json.Encode(response));

            if (!response.ServiceResponse.HasErrors && !string.IsNullOrEmpty(response.UserSettings.WhiteLabelProfileId))
            {
                var whiteLabelProfileResponse = Service.WhiteLabelProfileGetSingle(response.UserSettings.WhiteLabelProfileId);
                Debug.WriteLine("GetWhiteLabelProfileEmailLogoUrl, whiteLabelProfileResponse: " + Json.Encode(whiteLabelProfileResponse));
                if (!whiteLabelProfileResponse.ServiceResponse.HasErrors)
                {
                    return whiteLabelProfileResponse.Profile.EmailLogoUrl;
                }
            }

            return null;
        }
    }
}