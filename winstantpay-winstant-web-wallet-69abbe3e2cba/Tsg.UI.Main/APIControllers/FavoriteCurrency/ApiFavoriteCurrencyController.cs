using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Tsg.UI.Main.ApiMethods.FavoriteCurrency;
using TSG.Models.APIModels;
using TSG.Models.APIModels.FavoriteCurrencyModels;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Repository;
using CurrencyViewModel = TSG.Models.APIModels.CurrencyViewModel;

namespace Tsg.UI.Main.APIControllers.FavoriteCurrency
{
    [ApiFilter]
    public class ApiFavoriteCurrencyController : ApiController
    {
        [System.Web.Http.HttpGet]
        public IHttpActionResult Get()
        {
            var result = new FavoriteCurrenciesModels.FavoriteCurrencyList
            {
                Success = false,
                InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty },
                FavoriteCurrencies = new List<CurrencyViewModel>()
            };

            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    var favoriteCurrMethods = new FavoriteCurrencyMethods(ui);
                    result.FavoriteCurrencies = favoriteCurrMethods.PrepareCurrencies();
                    result.Success = true;
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK", UserMessage = "Data for instant payment selected correct" };
                }
                else
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.CheckUserError, DeveloperMessage = "Data for user not correct", UserMessage = "Data for user not correct" };
                }
            }
            catch (ApiException apiException)
            {
                result.InfoBlock = apiException.CustomInfoBlock;
            }
            catch (Exception e)
            {
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
            }
            return Ok(result);
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult Post(string currencyCode)
        {
            var result = new FavoriteCurrenciesModels.FavoriteCurrencyUpdateResult
            {
                Success = false,
                InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty }
            };
            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();

            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    var favoriteCurrMethods = new FavoriteCurrencyMethods(ui);
                    var freps = new FavoriteCurrencyRepository();
                    var favoriteCurrencies = favoriteCurrMethods.PrepareCurrencies();


                    if (favoriteCurrencies.FirstOrDefault(f => f.CurrencyCode == currencyCode) == null)
                        throw new ApiException(new InfoBlock()
                        {
                            Code = ApiErrors.ErrorCodeState.EmptyResult,
                            DeveloperMessage = "Doesn't find sended currency",
                            UserMessage= "Doesn't find sended currency"
                        });

                    result.CurrencyCode = currencyCode;
                    result.IsFavoriteCurrency = favoriteCurrencies.Any(a => a.CurrencyCode == currencyCode && a.IsFavorite);
                    string outRes = String.Empty;
                    
                    result.Success = favoriteCurrencies.Any(a => a.CurrencyCode == currencyCode && a.IsFavorite)
                        ? freps.DeleteFavoriteCurrency(currencyCode, ui.UserId, out outRes)
                        : freps.AddFavoriteCurrency(
                            new FavoriteCurrencyModel() { CurrencyCode = currencyCode, IdUser = ui.UserId},
                            out outRes);
                    if (result.Success)
                    {
                        result.IsFavoriteCurrency = !result.IsFavoriteCurrency;
                        result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK", UserMessage = "Data for favorite currency updated successfully" };
                    }
                    else if(!result.Success)
                        result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.CheckUserError, DeveloperMessage = outRes, UserMessage = "Data for favorite currency updated unsuccessfully" };
                }
                else
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.CheckUserError, DeveloperMessage = "Data for user not correct", UserMessage = "Data for user not correct" };
                }
            }
            catch (ApiException apiException)
            {
                result.InfoBlock = apiException.CustomInfoBlock;
            }
            catch (Exception e)
            {
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
            }
            return Ok(result);
        }
    }
}