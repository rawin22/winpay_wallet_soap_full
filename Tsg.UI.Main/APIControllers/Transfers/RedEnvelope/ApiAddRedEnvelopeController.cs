using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Formatting;
using System.Security.AccessControl;
using System.Text;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Tsg.UI.Main.ApiMethods.AutomaticExchange;
using Tsg.UI.Main.ApiMethods.FavoriteCurrency;
using Tsg.UI.Main.ApiMethods.Payments;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Repository;
using TSG.Dal.RedEnvelope;
using TSG.Models.APIModels;
using TSG.Models.APIModels.InstantPayment;
using TSG.Models.APIModels.TransfersApiModel;
using TSG.Models.DTO.Transfers;
using TSG.Models.Enums;
using TSG.Models.ServiceModels.Transfers;
using TSG.Models.ServiceModels.Transfers.RedEnvelope;
using TSG.ServiceLayer.AutomaticExchange.DependencyLiquidForUserServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidCcyListServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidOverDraftUserServiceMethods;
using TSG.ServiceLayer.RedEnvelope;
using TSG.ServiceLayer.Transfers;
using TSG.ServiceLayer.Users;

namespace Tsg.UI.Main.APIControllers.Transfers.RedEnvelope
{
    [ApiFilter]
    public class ApiAddRedEnvelopeController : ApiController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IUsersServiceMethods _usersServiceMethods;
        private readonly IDependencyLiquidForUserServiceMethods _dependencyLiquidForUserService;
        private readonly ILiquidCcyListServiceMethods _liquidCcyListServiceMethods;
        private readonly ILiquidOverDraftUserServiceMethods _liquidOverDraftUserServiceMethods;
        private readonly IRedEnvelopeServiceMethods _envelopeServiceMethods;
        private readonly ITransfersServiceMethods _transfersServiceMethods;

        private static readonly string cryptoCurrency = ConfigurationManager.AppSettings["cryptoCurrencies"];

        public ApiAddRedEnvelopeController(IDependencyLiquidForUserServiceMethods dependencyLiquidForUserService,
            ILiquidCcyListServiceMethods liquidCcyListServiceMethods,
            ILiquidOverDraftUserServiceMethods liquidOverDraftUserServiceMethods, IUsersServiceMethods usersServiceMethods,
            IRedEnvelopeServiceMethods envelopeServiceMethods, ITransfersServiceMethods transfersServiceMethods)
        {
            _usersServiceMethods = usersServiceMethods;
            _dependencyLiquidForUserService = dependencyLiquidForUserService;
            _liquidCcyListServiceMethods = liquidCcyListServiceMethods;
            _liquidOverDraftUserServiceMethods = liquidOverDraftUserServiceMethods;
            _envelopeServiceMethods = envelopeServiceMethods;
            _transfersServiceMethods = transfersServiceMethods;
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            var result = new RedEnvelopeCreatePaymentResponse
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
                    NewInstantPaymentMethods m = new NewInstantPaymentMethods(ui);

                    result.CurrencyList = m.PrepareAllAvailablePaymentCurrencies().Select(s => s.Value).ToList();
                    result.WPayIdList = m.PrepareAccountAliases().Select(s => s.Value).ToList();
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


        [HttpPost]
        public IHttpActionResult Post()
        {
            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();
            IEnumerable<string> outerUserToken;
            var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
            try
            {
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    var file = HttpContext.Current.Request.Files.Count > 0 ?
                    HttpContext.Current.Request.Files[0] : null;
                    var json = HttpContext.Current.Request.Form.Get("json");
                    var convertedJson = JsonConvert.DeserializeObject<NewRedEnvelopeModel>(json);
                    if (convertedJson != null)
                    {
                        var favoriteCurrMethods = new FavoriteCurrencyMethods(ui);
                        var currencies = favoriteCurrMethods.PrepareCurrencies();

                        if(currencies.All(a=>a.CurrencyCode!=convertedJson.CurrencyCode))
                            return Content(HttpStatusCode.BadRequest, new StandartResponse($"Can not find requested currency {convertedJson.CurrencyCode}"), new JsonMediaTypeFormatter(), mediaType: "application/json");

                        if (convertedJson.Amount <= Decimal.Zero)
                            return Content(HttpStatusCode.BadRequest, new StandartResponse($"Amount \"{convertedJson.Amount}\" can't be zero or negative"), new JsonMediaTypeFormatter(), mediaType: "application/json");

                        if(String.IsNullOrEmpty(convertedJson.WpayIdFrom) || String.IsNullOrEmpty(convertedJson.WpayIdTo))
                            return Content(HttpStatusCode.BadRequest, new StandartResponse($"User WPayId to \"{convertedJson.WpayIdTo}\" or WPayId from \"{convertedJson.WpayIdFrom}\"  can't be empty"), new JsonMediaTypeFormatter(), mediaType: "application/json");

                        if(convertedJson.IsKycCreated && String.IsNullOrEmpty(convertedJson.KycUserName))
                            return Content(HttpStatusCode.BadRequest, new StandartResponse($"User WPayId to \"{convertedJson.WpayIdTo}\" or WPayId from \"{convertedJson.WpayIdFrom}\"  can't be empty"), new JsonMediaTypeFormatter(), mediaType: "application/json");

                        else if(!convertedJson.IsKycCreated && String.IsNullOrEmpty(convertedJson.WpayIdTo))
                            return Content(HttpStatusCode.BadRequest, new StandartResponse($"User WPayId to \"{convertedJson.WpayIdTo}\" or WPayId from \"{convertedJson.WpayIdFrom}\"  can't be empty"), new JsonMediaTypeFormatter(), mediaType: "application/json");

                        string userNameTo = convertedJson.KycUserName;

                        if (!convertedJson.IsKycCreated)
                        {
                            var resCkeckToAlias = _usersServiceMethods.ExistedUserByAliasName(convertedJson.WpayIdTo);

                            if (!resCkeckToAlias.Success || resCkeckToAlias.Obj == null)
                                return Content(HttpStatusCode.BadRequest,
                                    new StandartResponse("Not found user by this WPayId."), new JsonMediaTypeFormatter(), mediaType: "application/json");
                            if (convertedJson.WpayIdTo == ConfigurationManager.AppSettings["redEnvelopeLogin"])
                                return Json(new StandartResponse<bool>(false, "You can't use system WPayId for transfer"));
                            if (resCkeckToAlias.Obj.Wpay_UserName == ui.UserName)
                                throw new Exception("Please select other WPayId. You can't use yourself WPayId for red envelope");

                            userNameTo = resCkeckToAlias.Obj.Wpay_UserName;
                        }
                        if(String.IsNullOrEmpty(userNameTo))
                            return Content(HttpStatusCode.BadRequest, new StandartResponse($"User WPayId to \"{convertedJson.WpayIdTo}\" doesn't find in system"), new JsonMediaTypeFormatter(), mediaType: "application/json");

                        string serverPath = "";
                        if (file != null && file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            serverPath = $"~/RedEnvelops/sender-{ui.UserName}/{Guid.NewGuid().ToString("N").Substring(0, 5)}{fileName}";
                            var path = HttpContext.Current.Server.MapPath(serverPath);
                            System.IO.FileInfo fileInfo = new System.IO.FileInfo(path);
                            if (fileInfo.Directory != null)
                            {
                                fileInfo.Directory.Create();
                                file.SaveAs(path);
                            }
                        }

                        ApiNewInstantPaymentViewModel apiNewInstantPayment = new ApiNewInstantPaymentViewModel();
                        apiNewInstantPayment.FromCustomer = convertedJson.WpayIdFrom;
                        apiNewInstantPayment.ToCustomer = ConfigurationManager.AppSettings["redEnvelopeWPayIdInTheMiddle"];
                        apiNewInstantPayment.Amount = Convert.ToDecimal(convertedJson.Amount);
                        apiNewInstantPayment.CurrencyCode = convertedJson.CurrencyCode;
                        apiNewInstantPayment.Memo = $"Red Envelope to {convertedJson.WpayIdTo} [{convertedJson.CurrencyCode} {convertedJson.Amount}]";
                        
                        #region Payment to Red envelope service process 
                        var getAllBalances = new NewInstantPaymentMethods(ui).PrepareAccountBalances();
                        var liquidCcyList = _dependencyLiquidForUserService.GetAllSoByUser(Guid.Parse(ui.UserId)).Obj;
                        AutomaticExchangeCommonMethods automaticExchangeCommonMethods = new AutomaticExchangeCommonMethods(ui);
                        var selectedBalanceByCcy = getAllBalances.Balances.FirstOrDefault(f => f.CCY == convertedJson.CurrencyCode);
                        var baseCcy = getAllBalances.Balances.FirstOrDefault(f => f.CCY == (getAllBalances.Balances.FirstOrDefault()?.BaseCCY ?? "USD"));
                        if (selectedBalanceByCcy != null)
                        {
                            var overdraftUser = _liquidOverDraftUserServiceMethods.GetAllSo().Obj.FirstOrDefault(f =>
                                f.LiquidOverDraftUserList_UserId == Guid.Parse(ui.UserId));

                            convertedJson.Amount = cryptoCurrency.Contains(selectedBalanceByCcy.CCY)
                                ? Decimal.Round(convertedJson.Amount, 8, MidpointRounding.AwayFromZero)
                                : Decimal.Round(convertedJson.Amount, 2, MidpointRounding.AwayFromZero);

                            var insufficientAmount = selectedBalanceByCcy.Balance - convertedJson.Amount;
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

                                        var checkedLiquids = automaticExchangeCommonMethods.CheckingPaymentSize(convertedJson.CurrencyCode,
                                            convertedJson.Amount, overdraftUsers, getAllBalances, liquidCcyList.OrderBy(ob => ob.DependencyLiquidForUser_LiquidOrder).ToList(), true);


                                        if (checkedLiquids.Success && checkedLiquids.IsAutomaticExchange)
                                        {
                                            var allExchangeCcy = checkedLiquids.AutomaticExchngeListCcy.Where(w => !w.IsPaymentableCurrency).ToList();
                                            StringBuilder sb = new StringBuilder();
                                            string s = "";
                                            allExchangeCcy.ForEach(f =>
                                            {
                                                sb.Append(String.Format(" {0} {1} at a rate of {2}{3} per {0} and", f.CurrencyCode, f.CurrencyAmount, f.Rate, convertedJson.CurrencyCode));
                                            });

                                            if (sb.Length > 4)
                                                s = sb.ToString().Remove(sb.Length - 4, 4);
                                            if (!String.IsNullOrEmpty(s) && !String.IsNullOrWhiteSpace(s))
                                            {
                                                var message =
                                                    $"There was not enough {convertedJson.CurrencyCode} in your account to complete the transaction " +
                                                    $"so automatic FX exchanged {convertedJson.CurrencyCode} {(cryptoCurrency.Contains(selectedBalanceByCcy.CCY) ? convertedJson.Amount.ToString("N8") : convertedJson.Amount.ToString("N2"))}" +
                                                    $", selling " +
                                                    $"{s}.";

                                                apiNewInstantPayment.Memo += $"{Environment.NewLine} {message}";
                                            }
                                        }
                                        else
                                        {
                                            return Content(HttpStatusCode.BadRequest, (StandartResponse)checkedLiquids, new JsonMediaTypeFormatter(), mediaType: "application/json");
                                        }
                                    }
                                    else
                                    {
                                        return Content(HttpStatusCode.BadRequest, new StandartResponse(false, "Sorry, you do not have sufficient funds based on your selected liquidation preferences in your user profile."), new JsonMediaTypeFormatter(), mediaType: "application/json");
                                    }
                                }
                                else
                                {
                                    return Content(HttpStatusCode.BadRequest, new StandartResponse(false, "Sorry, you do not have sufficient funds based on your selected liquidation preferences in your user profile."), new JsonMediaTypeFormatter(), mediaType: "application/json");
                                }
                                /***************************************/
                            }
                        }

                        NewInstantPaymentMethods m = new NewInstantPaymentMethods(ui);
                        var res = m.Create(apiNewInstantPayment);
                        if (res.ServiceResponse.HasErrors)
                            return Content(HttpStatusCode.BadRequest, new StandartResponse(StringExtensions.ConvertServiceResponseToSingleString(res.ServiceResponse.Responses)), new JsonMediaTypeFormatter(), mediaType: "application/json");

                        var paymentRedEnvelopeResult = m.Post(Guid.Parse(res.PaymentInformation.PaymentId));
                        if (paymentRedEnvelopeResult.ServiceResponse.HasErrors)
                            return Content(HttpStatusCode.BadRequest, new StandartResponse(StringExtensions.ConvertServiceResponseToSingleString(paymentRedEnvelopeResult.ServiceResponse.Responses)), new JsonMediaTypeFormatter(), mediaType: "application/json");
                        
                        #endregion
                        RedEnvelopeSo redEnvelope = new RedEnvelopeSo();    
                        redEnvelope.RedEnvelope_Amount = convertedJson.Amount;
                        redEnvelope.RedEnvelope_CurrencyCode = convertedJson.CurrencyCode;
                        redEnvelope.RedEnvelope_DateTransferedToRedEnvelope = DateTime.Now;
                        redEnvelope.RedEnvelope_IsSuccessTransferToRedEnvelopeAcc = true;
                        redEnvelope.RedEnvelope_FilePath = serverPath;
                        redEnvelope.RedEnvelope_Note = convertedJson.Note;
                        redEnvelope.RedEnvelope_WPayIdTo = convertedJson.WpayIdTo;
                        redEnvelope.RedEnvelope_WPayIdFrom = convertedJson.WpayIdFrom;
                        redEnvelope.RedEnvelope_RedEnvelopePaymentId = Guid.Parse(res.PaymentInformation.PaymentId);

                        var resToInsertEnvelope = _envelopeServiceMethods.Insert(redEnvelope);
                        if (!resToInsertEnvelope.Success || resToInsertEnvelope.Obj==null)
                        {
                            _logger.Error($"DB save red envelope error: {resToInsertEnvelope.Message}");
                            return Content(HttpStatusCode.BadRequest, new StandartResponse($"DB save red envelope error: {resToInsertEnvelope.Message}"), new JsonMediaTypeFormatter(), mediaType: "application/json");
                        }
                        var transfers = new TransfersSo()
                        {
                            Transfers_LinkToSourceRow = resToInsertEnvelope.Obj.RedEnvelope_Id,
                            Transfers_IsKycCreated = convertedJson.IsKycCreated,
                            Transfers_KycLinkId = convertedJson.KycId,
                            Transfers_Source = "Red Envelope",
                            Transfers_SourceType = TransfersSourceTypeEnum.RedEnvelope,
                            Transfers_TransferParent = ui.UserName,
                            Transfers_TransferRecipient = userNameTo
                        };
                        var querySaveTransfer = _transfersServiceMethods.Insert(transfers);
                        if (querySaveTransfer.Success)
                            return Content(HttpStatusCode.OK, new StandartResponse(true, "Ok"), new JsonMediaTypeFormatter() ,mediaType: "application/json");
                        return Content(HttpStatusCode.BadRequest, new StandartResponse(querySaveTransfer.Message), new JsonMediaTypeFormatter(), mediaType: "application/json");
                    }

                    return Content(HttpStatusCode.BadRequest, new StandartResponse("Invalid model"), new JsonMediaTypeFormatter(), mediaType: "application/json");
                }
                else return Content(HttpStatusCode.Unauthorized, new StandartResponse("Undefined user"), new JsonMediaTypeFormatter(), mediaType: "application/json");
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Content(HttpStatusCode.BadRequest, new StandartResponse($"{e.Message}", $"{e.Message} {Environment.NewLine} {e.InnerException?.Message}"), new JsonMediaTypeFormatter(), mediaType: "application/json");
            }
        }
    }
}