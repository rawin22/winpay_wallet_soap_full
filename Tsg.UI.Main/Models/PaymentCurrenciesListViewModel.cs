using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Repository;

namespace Tsg.UI.Main.Models
{
    public class PaymentCurrenciesListViewModel : BaseViewModel
    {
        readonly Dictionary<string, string> currencySymbol = new Dictionary<string, string>()
        {
            {"AED", "AED"},
            {"AUD", "AU$"},
            {"CAD", "CA$"},
            {"HKD", "HK$"},
            {"INR", "Indian Rupee"},
            {"KWD", "ك"},
            {"PHP", "₱"},
            {"QAR", "ر.ق"},
            {"RUB", "₽"},
            {"SGD", "S$"},
            {"TOP", "T$"},
            {"TZS", "TSh"},
            {"UGX", "USh"},
            {"VUV", "VT"},
            {"XAF", "FCFA"},
            {"XOF", "CFA"},
            {"XCD", "EC$"},
            {"AUT", "1 AUG"},
            {"BTC", "₿"},
            {"ETH", "Ξ"},
            {"AHO", "AHO"},
            {"USDT", "₮"},
            {"XAU", "Au"},
        };

        public PaymentCurrenciesListViewModel()
        {            
            Currencies = new List<CurrencyViewModel>();            
        }
        public IList<CurrencyViewModel> Currencies { get; set; }

        public void PrepareCurrencies()
        {
            var response = Service.GetPaymentCurrencies();
            if (!response.ServiceResponse.HasErrors)
            {
                Repository.FavoriteCurrencyRepository fr = new FavoriteCurrencyRepository();
                var allfavoriteCurrencies = fr.GetFavoriteCurrencyList();

                var allowedCurrencies = AppSettingHelper.GetAllowedCurrencies();
                _logger.Debug(string.Format("Allowed currencies, count: {0} items: {1}", allowedCurrencies.Count, JsonConvert.SerializeObject(allowedCurrencies)));
                if (allowedCurrencies.Count > 0 && !string.IsNullOrEmpty(allowedCurrencies[0]))
                {
                    response.Currencies = response.Currencies.Where(c => allowedCurrencies.Any(ac => ac == c.CurrencyCode)).ToArray();
                    _logger.Debug(string.Format("after filter allowed currencies, currencies: {0}", JsonConvert.SerializeObject(response.Currencies)));
                }

                foreach (CurrencyData data in response.Currencies)
                {
                    this.Currencies.Add(new CurrencyViewModel
                    {
                        // CurrencyId = data.CurrencyId,
                        CurrencyCode = data.CurrencyCode,
                        CurrencyName = data.CurrencyName,
                        CurrencyAmountScale = data.CurrencyAmountScale,
                        CurrencyRateScale = data.CurrencyRateScale,
                        Symbol = GetCurrencySymbol(data),
                        PaymentCutoffTime = data.PaymentCutoffTime,
                        SettlementDaysToAdd = data.SettlementDaysToAdd,
                        IsFavorite = allfavoriteCurrencies.Any(a=>a.CurrencyCode == data.CurrencyCode)
                    });
                }
            }
        }

        private string GetCurrencySymbol(CurrencyData data)
        {
            var symbol = data.Symbol;
            if (string.IsNullOrEmpty(symbol) || symbol == "?")
            {
                if (currencySymbol.ContainsKey(data.CurrencyCode))
                {
                    symbol = currencySymbol[data.CurrencyCode];
                }
                else 
                {
                    symbol = data.CurrencyCode;
                }
            }

            return symbol;
        }

    }
}