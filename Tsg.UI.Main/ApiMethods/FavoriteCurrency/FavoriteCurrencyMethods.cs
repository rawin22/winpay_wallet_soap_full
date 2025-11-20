using System.Collections.Generic;
using System.Linq;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;

namespace Tsg.UI.Main.ApiMethods.FavoriteCurrency
{
    public class FavoriteCurrencyMethods : BaseApiMethods
    {
        private UserInfo _userInfo { get; set; }
        public FavoriteCurrencyMethods(UserInfo ui) : base(ui)
        {
            _userInfo = ui;
            _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }


        public List<CurrencyViewModel> PrepareCurrencies()
        {
            var response = Service.GetPaymentCurrencies();
            if (response.ServiceResponse.HasErrors) return new List<CurrencyViewModel>();

            Repository.FavoriteCurrencyRepository fr = new FavoriteCurrencyRepository();
            var allfavoriteCurrencies = fr.GetFavoriteCurrencyList(_userInfo.UserId);

            return response.Currencies.Select(s => new CurrencyViewModel()
            {
                //CurrencyId = s.CurrencyId,
                CurrencyCode = s.CurrencyCode,
                CurrencyName = s.CurrencyName,
                CurrencyAmountScale = s.CurrencyAmountScale,
                CurrencyRateScale = s.CurrencyRateScale,
                Symbol = s.Symbol,
                PaymentCutoffTime = s.PaymentCutoffTime,
                SettlementDaysToAdd = s.SettlementDaysToAdd,
                IsFavorite = allfavoriteCurrencies.Any(a => a.CurrencyCode == s.CurrencyCode.ToString())
            }).ToList();
        }

    }
}