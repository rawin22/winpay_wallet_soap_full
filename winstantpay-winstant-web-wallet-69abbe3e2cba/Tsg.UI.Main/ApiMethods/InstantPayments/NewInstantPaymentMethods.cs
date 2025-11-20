using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.APIModels.InstantPayment;

namespace Tsg.UI.Main.ApiMethods.Payments
{
    public class NewInstantPaymentMethods : BaseApiMethods
    {
        private readonly UserInfo _userInfo;
        public NewInstantPaymentMethods(UserInfo ui) : base(ui)
        {
            _userInfo = ui;
        }

        public InstantPaymentCreateResponse Create(ApiNewInstantPaymentViewModel model)
        {
            return Service.CreateNewPayment(model.FromCustomer, model.ToCustomer, model.Amount, model.CurrencyCode, model.Memo, model.Invoice, model.ReasonForPayment);
        }

        public InstantPaymentGetSingleResponse PrepareDetails(Guid paymentId)
        {
            return Service.GetInstantPaymentDetails(paymentId);
        }

        public InstantPaymentPostResponse Post(Guid paymentId)
        {
            return Service.PostInstantPayment(paymentId);
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

                currenciesListItem.AddRange(favoritecurrency.Join(currencies.Currencies, favorite => favorite.CurrencyCode, inner => inner.CurrencyCode.ToString(),
                    (favorite, inner) => new SelectListItem
                    {
                        Text = inner.CurrencyCode,
                        Value = inner.CurrencyName
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

        public IList<SelectListItem> PrepareAllAvailablePaymentCurrencies()
        {
            var currenciesListItem = new List<SelectListItem>();
            var currencies = Service.GetPaymentCurrencies();
            if (!currencies.ServiceResponse.HasErrors)
            {
                return currencies.Currencies.Select(s =>
                    new SelectListItem
                    {
                        Text = s.CurrencyCode,
                        Value = s.CurrencyCode
                    }).ToList();
            }
            return currenciesListItem;
        }

        public IList<SelectListItem> PrepareAccountAliases()
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

        public IList<SelectListItem> PreparePriorUsedAliases()
        {
            var aliasList = new List<SelectListItem>();
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

        public List<InstantPaymentViewModel> PrepareLatestInstantPayments(InstantPaymentStatus status = InstantPaymentStatus.All)
        {
            var response = Service.GetLatestInstantPayments(100000000, status);
            int counter = 0;


            if (!response.ServiceResponse.HasErrors)
            {
                return response.Payments.Select(data => new InstantPaymentViewModel
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
                    IsIncome = (_userInfo != null && _userInfo.OrganisationName != null && !_userInfo.OrganisationName.ToUpper().Equals(data.FromCustomerName.ToUpper()))

                }).ToList();
            }
            return new List<InstantPaymentViewModel>();
        }

        public GetCustomerAccountBalancesResponse PrepareAccountBalances()
        {
            return Service.GetAccountBalances();
        }

    }
}