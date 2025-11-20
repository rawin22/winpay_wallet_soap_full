using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using Tsg.Business.Model.TSGgpwsbeta;
using Tsg.UI.Main.ApiMethods.Payments;
using TSG.Models.APIModels;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels.InstantPayment;
using Tsg.UI.Main.ApiMethods.AutomaticExchange;
using TSG.ServiceLayer.AutomaticExchange.DependencyLiquidForUserServiceMethods;
using TSG.Models.ServiceModels.AutomaticExchange;
using TSG.ServiceLayer.AutomaticExchange.LiquidCcyListServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidOverDraftUserServiceMethods;

namespace Tsg.UI.Main.APIControllers
{
    /// <summary>
    /// API for creating instant payment
    /// </summary>
    [ApiFilter]
    public class ApiCreateInstantPaymentController : ApiController
    {
        private readonly IDependencyLiquidForUserServiceMethods _dependencyLiquidForUserService;
        private readonly ILiquidCcyListServiceMethods _liquidCcyListServiceMethods;
        private readonly ILiquidOverDraftUserServiceMethods _liquidOverDraftUserServiceMethods;
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly string cryptoCurrency = ConfigurationManager.AppSettings["cryptoCurrencies"];

        /// <summary>
        /// controller
        /// </summary>
        /// <param name="dependencyLiquidForUserService"></param>
        /// <param name="liquidCcyListServiceMethods"></param>
        /// <param name="liquidOverDraftUserServiceMethods"></param>
        public ApiCreateInstantPaymentController(IDependencyLiquidForUserServiceMethods dependencyLiquidForUserService, 
            ILiquidCcyListServiceMethods liquidCcyListServiceMethods,
            ILiquidOverDraftUserServiceMethods liquidOverDraftUserServiceMethods) {
            _dependencyLiquidForUserService = dependencyLiquidForUserService;
            _liquidCcyListServiceMethods = liquidCcyListServiceMethods;
            _liquidOverDraftUserServiceMethods = liquidOverDraftUserServiceMethods;
        }

        /// <summary>
        /// Get data for instant payment
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get()
        {
            var result = new ApiInstantPaymentModel.InstantPaymentPageModelResponse
            {
                Success = false,
                InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty },
                Data = new ApiInstantPaymentModel.SendedDataByInstantPayment()
            };

            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    NewInstantPaymentMethods m = new NewInstantPaymentMethods(ui);
                    
                    result.Data.CurrencyList = m.PrepareAllAvailablePaymentCurrencies().Select(s => s.Value).ToList();
                    result.Data.ToAlias = m.PreparePriorUsedAliases().Select(s => s.Value).ToList();
                    result.Data.FromAlias = m.PrepareAccountAliases().Select(s => s.Value).ToList();
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

        /// <summary>
        /// Create new instant payment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Post([FromBody]ApiInstantPaymentModel.ApiInstantPaymentResponse model)
        {
            var result = new ApiInstantPaymentModel.CreatePaymentResponse()
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
                if (!(isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui)))
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.CheckUserError, DeveloperMessage = "Data for user not correct", UserMessage = "Data for user not correct" };
                    return Ok(result);
                }
                bool isDefValue = false;
                var listSpecParams = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(w => Attribute.IsDefined(w, typeof(RequiredAttribute)));
                foreach (var propertyInfo in listSpecParams)
                {
                    var x = CheckValueByType.GetDefault(propertyInfo.PropertyType);
                    var @equals = propertyInfo.GetValue(model) is string ? String.IsNullOrEmpty(propertyInfo.GetValue(model).ToString()) : Equals(x, propertyInfo.GetValue(model));
                    isDefValue |= @equals;
                }
                if (isDefValue)
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "InfoBlock. can't not be value is null", UserMessage = "InfoBlock. Value is null" };
                    return Ok(result);
                }
                Guid? paymentId;
                ApiNewInstantPaymentViewModel apiNewInstantPayment = new ApiNewInstantPaymentViewModel();
                apiNewInstantPayment.FromCustomer = model.FromCustomer;
                apiNewInstantPayment.ToCustomer = model.ToCustomer;
                apiNewInstantPayment.Amount = Convert.ToDecimal(model.Amount);
                apiNewInstantPayment.CurrencyCode = model.CurrencyCode;
                apiNewInstantPayment.Memo = model.Memo;
                apiNewInstantPayment.Invoice = model.Invoice;
                apiNewInstantPayment.ReasonForPayment = model.ReasonForPayment;

                var getAllBalances = new NewInstantPaymentMethods(ui).PrepareAccountBalances();
                var liquidCcyList = _dependencyLiquidForUserService.GetAllSoByUser(Guid.Parse(ui.UserId)).Obj;
                AutomaticExchangeCommonMethods automaticExchangeCommonMethods = new AutomaticExchangeCommonMethods(ui);
                var selectedBalanceByCcy = getAllBalances.Balances.FirstOrDefault(f => f.CCY == model.CurrencyCode);
                var baseCcy = getAllBalances.Balances.FirstOrDefault(f => f.CCY == (getAllBalances.Balances.FirstOrDefault()?.BaseCCY ?? "USD"));
                if (selectedBalanceByCcy != null)
                {
                    var overdraftUser = _liquidOverDraftUserServiceMethods.GetAllSo().Obj.FirstOrDefault(f =>
                        f.LiquidOverDraftUserList_UserId == Guid.Parse(ui.UserId));

                    model.Amount = cryptoCurrency.Contains(selectedBalanceByCcy.CCY)
                        ? Decimal.Round(model.Amount, 8, MidpointRounding.AwayFromZero)
                        : Decimal.Round(model.Amount, 2, MidpointRounding.AwayFromZero);

                    var insufficientAmount = selectedBalanceByCcy.Balance - model.Amount;
                    if (insufficientAmount > 0 || overdraftUser != null)
                    {
                        //var check = automaticExchangeCommonMethods.CommonChekingTotalCurrencies(liquidCcyList, getAllBalances, baseCcy, selectedBalanceByCcy, model.Amount);
                        //if (!check.Success)
                        //    return Content(HttpStatusCode.BadRequest, (StandartResponse)check);
                    }
                    else
                    {
                        /********** Automatic Exhange **********/
                        var check = automaticExchangeCommonMethods.CommonChekingBeforeAutomaticExchange(liquidCcyList, getAllBalances, baseCcy, selectedBalanceByCcy, insufficientAmount);

                        if (check.Success && !check.IsWarning)
                        {
                            if (liquidCcyList.Count > 0)
                            {
                                var overdraftUsers = _liquidOverDraftUserServiceMethods.GetAllSo().Obj.Where(f =>
                                    f.LiquidOverDraftUserList_UserId == Guid.Parse(ui.UserId)).ToList();

                                var checkedLiquids = automaticExchangeCommonMethods.CheckingPaymentSize(model.CurrencyCode,
                                    model.Amount, overdraftUsers, getAllBalances, liquidCcyList.OrderBy(ob => ob.DependencyLiquidForUser_LiquidOrder).ToList(), true);


                                if (checkedLiquids.Success && checkedLiquids.IsAutomaticExchange)
                                {
                                    var allExchangeCcy = checkedLiquids.AutomaticExchngeListCcy.Where(w => !w.IsPaymentableCurrency).ToList();
                                    StringBuilder sb = new StringBuilder();
                                    string s = "";
                                    allExchangeCcy.ForEach(f =>
                                    {
                                        sb.Append(String.Format(" {0} {1} at a rate of {2}{3} per {0} and", f.CurrencyCode, f.CurrencyAmount, f.Rate, model.CurrencyCode));
                                    });
                                   
                                    if (sb.Length > 4)
                                        s = sb.ToString().Remove(sb.Length - 4, 4);
                                    if (!String.IsNullOrEmpty(s) && !String.IsNullOrWhiteSpace(s))
                                    {
                                        var message =
                                            $"There was not enough {model.CurrencyCode} in your account to complete the transaction " +
                                            $"so automatic FX exchanged {model.CurrencyCode} {(cryptoCurrency.Contains(selectedBalanceByCcy.CCY) ? model.Amount.ToString("N8") : model.Amount.ToString("N2"))}" +
                                            $", selling " +
                                            $"{s}.";

                                        apiNewInstantPayment.Memo += $"{Environment.NewLine} {message}";
                                    }
                                }
                                else
                                {
                                    return Content(HttpStatusCode.BadRequest, (StandartResponse) checkedLiquids);
                                }
                            }
                            else
                            {
                                return Content(HttpStatusCode.BadRequest, new StandartResponse(false, "Sorry, you do not have sufficient funds based on your selected liquidation preferences in your user profile."));
                            }
                        }
                        else
                        {
                            return Content(HttpStatusCode.BadRequest, new StandartResponse(false, "Sorry, you do not have sufficient funds based on your selected liquidation preferences in your user profile."));
                        }
                        /***************************************/
                    }
                }

                NewInstantPaymentMethods m = new NewInstantPaymentMethods(ui);
                var res = m.Create(apiNewInstantPayment);
                if (res.ServiceResponse.HasErrors)
                    throw new Exception(StringExtensions.ConvertServiceResponseToSingleString(res.ServiceResponse.Responses));



                model.PaymentId = res.PaymentInformation.PaymentId;
                result.CreationInfo = model;
                result.Success = true;
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK", UserMessage = "Instant payment will be creatad succesifully" };
            }
            catch (Exception e)
            {
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
            }
            return Ok(result);
        }
    }
}
