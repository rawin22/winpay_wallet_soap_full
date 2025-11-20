using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.ApiMethods.Payments;
using TSG.Models.APIModels;
using TSG.Models.ServiceModels.AutomaticExchange;

namespace Tsg.UI.Main.ApiMethods.AutomaticExchange
{
    public class AutomaticExchangeCommonMethods : BaseApiMethods
    {
        private readonly UserInfo _userInfo;
        private static readonly string cryptoCurrency = ConfigurationManager.AppSettings["cryptoCurrencies"];

        public AutomaticExchangeCommonMethods(UserInfo ui) : base(ui)
        {
            _userInfo = ui;
        }

        public ExtendedStandartResponse CommonChekingTotalCurrencies(List<DependencyLiquidForUserSo> liquidCcyList, GetCustomerAccountBalancesResponse getAllBalances, CustomerBalanceData baseCcy, CustomerBalanceData workingCcy, decimal insufficientAmount)
        {
            var res = new ExtendedStandartResponse();

            var liquidsCcys = string.Join(", ", liquidCcyList.Where(w => w.DependencyLiquidForUser_LiquidCcyList.LiquidCcyList_CurrencyCode != workingCcy.CCY).OrderBy(ob => ob.DependencyLiquidForUser_LiquidOrder)
                    .Select(s => s.DependencyLiquidForUser_LiquidCcyList.LiquidCcyList_CurrencyCode).ToList());

            var totalBalanceAvailableBaseByAll = getAllBalances.Balances.Sum(s => s.BalanceAvailableBase);

            var checkingPossibilities = Service.FxDealQuoteCreate(workingCcy.CCY, baseCcy.CCY,
                cryptoCurrency.Contains(workingCcy.CCY) ? Decimal.Round(insufficientAmount, 8, MidpointRounding.AwayFromZero) :
                    Decimal.Round(insufficientAmount, 2, MidpointRounding.AwayFromZero), workingCcy.CCY, false, "SPOT");
            if (checkingPossibilities.ServiceResponse.HasErrors)
            {
                string errors = string.Join(Environment.NewLine, checkingPossibilities.ServiceResponse.Responses.Select(s => s.Message));
                _logger.Error(errors);
                res.IsWarning = true;
                res.InfoBlock = new InfoBlock(checkingPossibilities.ServiceResponse.Responses[0].MessageDetails, errors);
            }
            else
            {
                var totalCurrencyValue = Decimal.Parse(checkingPossibilities.Quote.SellAmount.Replace(checkingPossibilities.Quote.SellCurrencyCode, "").Trim());
                if (totalBalanceAvailableBaseByAll < totalCurrencyValue)
                {
                    res.IsWarning = true;
                    res.InfoBlock = new InfoBlock("Sorry, you do not have sufficient funds based on your selected liquidation preferences in your user profile.");
                    return res;
                }

                res.Success = true;
            }

            return res;
        }

        public ExtendedStandartResponse CommonChekingBeforeAutomaticExchange(List<DependencyLiquidForUserSo> liquidCcyList, GetCustomerAccountBalancesResponse getAllBalances, CustomerBalanceData baseCcy, CustomerBalanceData workingCcy, decimal insufficientAmount)
        {
            var res = new ExtendedStandartResponse();

            var liquidsCcys = string.Join(", ", liquidCcyList.Where(w => w.DependencyLiquidForUser_LiquidCcyList.LiquidCcyList_CurrencyCode != workingCcy.CCY).OrderBy(ob => ob.DependencyLiquidForUser_LiquidOrder)
                    .Select(s => s.DependencyLiquidForUser_LiquidCcyList.LiquidCcyList_CurrencyCode).ToList());

            var totalBalanceAvailableBaseByLiquids = getAllBalances.Balances.Where(w => w.BaseCCY == baseCcy.CCY
                && liquidsCcys.Contains(w.CCY))
                .ToList()
                .Sum(s => s.BalanceAvailableBase);

            var totalBalanceAvailableBaseByAll = getAllBalances.Balances.Sum(s => s.BalanceAvailableBase);
            insufficientAmount = (-1) * insufficientAmount;
            if (baseCcy.CCY == workingCcy.CCY)
            {
                bool resCheck = (totalBalanceAvailableBaseByAll - insufficientAmount) > Decimal.Zero;
                return new ExtendedStandartResponse(resCheck, 
                    !resCheck ? "Sorry, you do not have sufficient funds based on your selected liquidation preferences in your user profile."
                        : String.Empty);
            }

            var checkingPossibilities = Service.FxDealQuoteCreate(workingCcy.CCY, baseCcy.CCY, cryptoCurrency.Contains(workingCcy.CCY) ? Decimal.Round(insufficientAmount, 8, MidpointRounding.AwayFromZero) :
                Decimal.Round(insufficientAmount, 2, MidpointRounding.AwayFromZero), workingCcy.CCY, false, "SPOT");
            if (checkingPossibilities.ServiceResponse.HasErrors)
            {
                string errors = string.Join(Environment.NewLine, checkingPossibilities.ServiceResponse.Responses.Select(s => s.Message));
                _logger.Error(errors);
                res.IsWarning = true;
                res.InfoBlock = new InfoBlock(checkingPossibilities.ServiceResponse.Responses[0].MessageDetails, errors);
            }
            else
            {
                var totalCurrencyValue = Decimal.Parse(checkingPossibilities.Quote.SellAmount.Replace(checkingPossibilities.Quote.SellCurrencyCode, "").Trim());
                if (totalBalanceAvailableBaseByLiquids < totalCurrencyValue)
                {
                    res.IsWarning = true;
                    res.InfoBlock = new InfoBlock("Sorry, you do not have sufficient funds based on your selected liquidation preferences in your user profile.");
                    return res;
                }

                res.Success = true;
            }

            return res;
        }

        public AutomaticExchangeResponse CheckingPaymentSize(string currencyCode, decimal amount, List<LiquidOverDraftUserListSo> overdraftUsers, Tsg.Business.Model.TsgGPWebService.GetCustomerAccountBalancesResponse getAllBalances, List<DependencyLiquidForUserSo> liquidCcyList, bool isConfirmedExchange)
        {
            try
            {
                if (liquidCcyList.Count == 0)
                    return new AutomaticExchangeResponse() { InfoBlock = new InfoBlock("Empty liquids") };
                // Get all balances
                if (getAllBalances == null || getAllBalances.Balances == null || getAllBalances.Balances.Length == 0) //|| getAllBalances.Balances.All(a => a.CCY != currencyCode))
                    return new AutomaticExchangeResponse() { InfoBlock = new InfoBlock("Invalid balance data") };

                var baseCcy = getAllBalances.Balances.FirstOrDefault(f => f.CCY == (getAllBalances.Balances.FirstOrDefault()?.BaseCCY ?? "USD"));
                if (baseCcy == null)
                    return new AutomaticExchangeResponse() { InfoBlock = new InfoBlock("Undefined base currency") };

                var liquidsCcys = string.Join(", ", liquidCcyList.Where(w => w.DependencyLiquidForUser_LiquidCcyList.LiquidCcyList_CurrencyCode != currencyCode)
                    .Select(s => s.DependencyLiquidForUser_LiquidCcyList.LiquidCcyList_CurrencyCode));

                var totalBalanceAvailableBaseByLiquids = getAllBalances.Balances.Where(w => w.BaseCCY == baseCcy.CCY
                    && liquidsCcys.Contains(w.CCY))
                    .ToList()
                    .Sum(s => s.BalanceAvailableBase);

                var totalBalanceAvailableBaseByAll = getAllBalances.Balances.Sum(s => s.BalanceAvailableBase);

                var selectedBalanceByCcy = getAllBalances.Balances.FirstOrDefault(f => f.CCY == currencyCode);

                decimal totalAmount = 0;
                if (selectedBalanceByCcy != null)
                {
                    var res = new AutomaticExchangeResponse();

                    if (selectedBalanceByCcy.Balance >= amount)
                        return new AutomaticExchangeResponse() { InfoBlock = new InfoBlock("Balance amount is positive"), IsAutomaticExchange = true, Success = true };

                    // Checking if user have amount by current account
                    var insufficientAmount = (-1) * (selectedBalanceByCcy.Balance - amount);
                    res.AutomaticExchngeListCcy.Add(new AutomaticExchangeChekingModel()
                    {
                        CurrencyCode = selectedBalanceByCcy.CCY,
                        IsLiquidCurrency = liquidCcyList.Select(s => s.DependencyLiquidForUser_LiquidCcyList.LiquidCcyList_CurrencyCode).Contains(selectedBalanceByCcy.CCY),
                        AmountForExchange = insufficientAmount,
                        IsPaymentableCurrency = true,
                        CurrencyAmount = amount,
                        PrintString = $"{amount.FormatPrice()} {currencyCode}"
                    });

                    // Check global possibilities
                    //var checkingPossibilities =  Service.FxDealQuoteCreate(currencyCode, baseCcy.CCY, insufficientAmount, currencyCode, false, "SPOT");

                    //if (checkingPossibilities.ServiceResponse.HasErrors)
                    //{
                    //    string errors = string.Join(Environment.NewLine, checkingPossibilities.ServiceResponse.Responses.Select(s => s.Message));
                    //    _logger.Error(errors);
                    //    return new AutomaticExchangeResponse() { InfoBlock = new InfoBlock("You don't have sufficient funds to pay for this transaction, please confirm if you want to use automatic exchange to proceed.", errors)};
                    //}
                    //var totalCurrencyValue = Decimal.Parse(checkingPossibilities.Quote.SellAmount.Replace(checkingPossibilities.Quote.SellCurrencyCode, "").Trim());
                    //if (totalBalanceAvailableBaseByLiquids < totalCurrencyValue && totalBalanceAvailableBaseByAll >= totalCurrencyValue )
                    //    return new AutomaticExchangeResponse() { InfoBlock = new InfoBlock("You don't have sufficient funds to pay for this transaction, please confirm if you want to use automatic exchange to proceed."), IsNeedManualExchange = true };
                    //else if (totalBalanceAvailableBaseByLiquids < totalCurrencyValue && totalBalanceAvailableBaseByAll < totalCurrencyValue)
                    //{
                    //    return new AutomaticExchangeResponse() { InfoBlock = new InfoBlock("You don't have sufficient funds to pay for this transaction, please confirm if you want to use automatic exchange to proceed.") };
                    //}

                    insufficientAmount = cryptoCurrency.Contains(selectedBalanceByCcy.CCY)
                        ? Decimal.Round(insufficientAmount, 8, MidpointRounding.AwayFromZero)
                        : Decimal.Round(insufficientAmount, 2, MidpointRounding.AwayFromZero);

                    /*****************  Generate quotes for exchange  *********************/

                    foreach (var dependencyLiquidForUserSo in liquidCcyList.Where(w => w.DependencyLiquidForUser_LiquidCcyList.LiquidCcyList_CurrencyCode != currencyCode).ToList())
                    {
                        if (insufficientAmount <= 0) break;

                        var balance = getAllBalances.Balances.FirstOrDefault(f =>
                            f.CCY == dependencyLiquidForUserSo.DependencyLiquidForUser_LiquidCcyList
                                .LiquidCcyList_CurrencyCode);
                        //var overdraftBydependencyCcy = overdraftUsers.FirstOrDefault(f => f.LiquidOverDraftUserList_LiquidCcyList
                        //                                              .LiquidCcyList_CurrencyCode == balance.CCY)?
                        //                                    .LiquidOverDraftUserList_Amount ?? 0;
                        if (balance == null || balance.Balance <= 0) continue;

                        var quotes = Service.FxDealQuoteCreate(currencyCode, balance.CCY, insufficientAmount, currencyCode, false, "SPOT");
                        if (quotes.Quote == null) continue;

                        var currentSellAmount = Decimal.Parse(quotes.Quote.SellAmount.Replace(quotes.Quote.SellCurrencyCode, "").Trim());
                        var currentBuyAmount = Decimal.Parse(quotes.Quote.BuyAmount.Replace(quotes.Quote.BuyCurrencyCode, "").Trim());
                        if (balance.Balance - currentSellAmount < 0)
                        {
                            quotes = Service.FxDealQuoteCreate(currencyCode, balance.CCY, balance.Balance, balance.CCY, false, "SPOT");
                            currentSellAmount = Decimal.Parse(quotes.Quote.SellAmount.Replace(quotes.Quote.SellCurrencyCode, "").Trim());
                            currentBuyAmount = Decimal.Parse(quotes.Quote.BuyAmount.Replace(quotes.Quote.BuyCurrencyCode, "").Trim());
                        };


                        insufficientAmount -= currentBuyAmount;
                        res.AutomaticExchngeListCcy.Add(new AutomaticExchangeChekingModel()
                        {
                            CurrencyCode = quotes.Quote.SellCurrencyCode,
                            IsLiquidCurrency = true,
                            CurrencyAmount = currentSellAmount,
                            PrintString = quotes.Quote.SellAmount,
                            AmountForExchange = currentBuyAmount,
                            Rate = quotes.Quote.Rate,
                            QuoteId = quotes.Quote.QuoteId
                        });
                    }
                    if (insufficientAmount > 0)
                        return new AutomaticExchangeResponse() { InfoBlock = new InfoBlock("Sorry, you do not have sufficient funds based on your selected liquidation preferences in your user profile.") };
                    else
                    {
                        res.Success = true;
                        res.IsAutomaticExchange = true;
                        if (isConfirmedExchange)
                        {
                            List<Tuple<string, bool>> keyValuePair = new List<Tuple<string, bool>>();
                            bool r = false;
                            foreach (var automaticExchangeChekingModel in res.AutomaticExchngeListCcy)
                            {
                                var checkQuote = Service.FxDealQuoteBookAndInstantDeposit(automaticExchangeChekingModel.QuoteId);
                                if (checkQuote.ServiceResponse.HasErrors)
                                    keyValuePair.Add(new Tuple<string, bool>(string.Join(Environment.NewLine, checkQuote.ServiceResponse.Responses.Select(s => s.MessageDetails)), checkQuote.ServiceResponse.HasErrors));
                            }
                            r = keyValuePair.All(a => a.Item2);

                            return new AutomaticExchangeResponse() { Success = r, AutomaticExchngeListCcy = res.AutomaticExchngeListCcy, IsAutomaticExchange = r, InfoBlock = new InfoBlock(r ? "Ok" : $"Error { string.Join("", keyValuePair.Select(s => s.Item1)) }") };
                        }

                        return res;
                    }

                    /**********************************************************************/
                }
                else
                {
                    return new AutomaticExchangeResponse() { InfoBlock = new InfoBlock("Currency does not found") };
                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
                return new AutomaticExchangeResponse() { InfoBlock = new InfoBlock("Sorry, you do not have sufficient funds based on your selected liquidation preferences in your user profile.", exception.Message) };
            }
        }


        public FXDealQuoteCreateResponse CreateQuoteForExchange(string toCcy, string fromCcy, string saleCcy, decimal amount)
        {

            return Service.FxDealQuoteCreate(toCcy, fromCcy,
                cryptoCurrency.Contains(toCcy) ? Decimal.Round(amount, 8, MidpointRounding.AwayFromZero) :
                    Decimal.Round(amount, 2, MidpointRounding.AwayFromZero), toCcy, false, "SPOT");
        }
    }
}