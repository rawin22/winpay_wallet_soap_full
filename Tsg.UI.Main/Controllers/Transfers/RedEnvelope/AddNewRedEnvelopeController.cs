using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Tsg.UI.Main.ApiMethods.AutomaticExchange;
using Tsg.UI.Main.ApiMethods.FavoriteCurrency;
using Tsg.UI.Main.ApiMethods.Payments;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;
using TSG.Models.APIModels.InstantPayment;
using TSG.Models.Enums;
using TSG.Models.ServiceModels.Transfers;
using TSG.Models.ServiceModels.Transfers.RedEnvelope;
using TSG.ServiceLayer.AutomaticExchange.DependencyLiquidForUserServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidCcyListServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidOverDraftUserServiceMethods;
using TSG.ServiceLayer.RedEnvelope;
using TSG.ServiceLayer.Transfers;
using TSG.ServiceLayer.Users;

namespace Tsg.UI.Main.Controllers.Transfers.RedEnvelope
{
    public class AddNewRedEnvelopeController : Controller
    {
        // GET: AddNewRedEnvelope
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IUsersServiceMethods _usersServiceMethods;
        private readonly IDependencyLiquidForUserServiceMethods _dependencyLiquidForUserService;
        private readonly ILiquidCcyListServiceMethods _liquidCcyListServiceMethods;
        private readonly ILiquidOverDraftUserServiceMethods _liquidOverDraftUserServiceMethods;
        private readonly IRedEnvelopeServiceMethods _envelopeServiceMethods;
        private readonly ITransfersServiceMethods _transfersServiceMethods;

        private static readonly string cryptoCurrency = ConfigurationManager.AppSettings["cryptoCurrencies"];

        public AddNewRedEnvelopeController(IDependencyLiquidForUserServiceMethods dependencyLiquidForUserService,
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

        public ActionResult NewEnvelope()
        {
            NewInstantPaymentViewModel instantPayment = new NewInstantPaymentViewModel();
            AddNewRedEnvelope model = new AddNewRedEnvelope();

            model.AccountAliases = instantPayment.AccountAliases;
            model.AvailableCurrencies = instantPayment.AvailableCurrencies;
            model.PriorUsedAliases = instantPayment.PriorUsedAliases;

            return View(model);
        }

        [HttpPost]
        public ActionResult NewEnvelope(AddNewRedEnvelope model)
        {
            NewInstantPaymentViewModel instantPayment = new NewInstantPaymentViewModel();
            model.AccountAliases = instantPayment.AccountAliases;
            model.AvailableCurrencies = instantPayment.AvailableCurrencies;
            model.PriorUsedAliases = instantPayment.PriorUsedAliases;
            var favoriteCurrMethods = new FavoriteCurrencyMethods(AppSecurity.CurrentUser);

            try
            {
                var currencies = favoriteCurrMethods.PrepareCurrencies();

                if (currencies.All(a => a.CurrencyCode != model.CurrencyCode))
                    throw new Exception($"Can not find requested currency {model.CurrencyCode}");

                if (model.Amount <= Decimal.Zero)
                    throw new Exception($"Amount \"{model.Amount}\" can't be zero or negative");

                if (String.IsNullOrEmpty(model.WpayIdFrom) || String.IsNullOrEmpty(model.WpayIdTo))
                    throw new Exception($"User WPayId to \"{model.WpayIdTo}\" or WPayId from \"{model.WpayIdFrom}\"  can't be empty");

                if (model.IsKycCreated && String.IsNullOrEmpty(model.KycUserName))
                    throw new Exception($"User WPayId to \"{model.WpayIdTo}\" or WPayId from \"{model.WpayIdFrom}\"  can't be empty");

                else if (!model.IsKycCreated && String.IsNullOrEmpty(model.WpayIdTo))
                    throw new Exception($"User WPayId to \"{model.WpayIdTo}\" or WPayId from \"{model.WpayIdFrom}\"  can't be empty");

                string userNameTo = model.KycUserName;

                if (!model.IsKycCreated)
                {
                    var resCkeckToAlias = _usersServiceMethods.ExistedUserByAliasName(model.WpayIdTo);

                    if (!resCkeckToAlias.Success || resCkeckToAlias.Obj == null)
                        throw new Exception("Not found user by this WPayId.");
                    if(resCkeckToAlias.Obj.Wpay_UserName == AppSecurity.CurrentUser.UserName)
                        throw new Exception("Please select other WPayId. You can't use yourself WPayId for red envelope");

                    userNameTo = resCkeckToAlias.Obj.Wpay_UserName;
                }
                if (String.IsNullOrEmpty(userNameTo))
                    throw new Exception($"User WPayId to \"{model.WpayIdTo}\" doesn't find in system");

                string serverPath = "";
                if (model.RedEnvelope_PostedFile != null && model.RedEnvelope_PostedFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(model.RedEnvelope_PostedFile.FileName);
                    serverPath = $"~/RedEnvelops/sender-{AppSecurity.CurrentUser.UserName}/{Guid.NewGuid().ToString("N").Substring(0, 5)}{fileName}";
                    var path = Server.MapPath(serverPath);
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(path);
                    if (fileInfo.Directory != null)
                    {
                        fileInfo.Directory.Create();
                        model.RedEnvelope_PostedFile.SaveAs(path);
                    }
                }

                ApiNewInstantPaymentViewModel apiNewInstantPayment = new ApiNewInstantPaymentViewModel();
                apiNewInstantPayment.FromCustomer = model.WpayIdFrom;
                apiNewInstantPayment.ToCustomer = ConfigurationManager.AppSettings["redEnvelopeWPayIdInTheMiddle"];
                apiNewInstantPayment.Amount = Convert.ToDecimal(model.Amount);
                apiNewInstantPayment.CurrencyCode = model.CurrencyCode;
                apiNewInstantPayment.Memo = $"Red Envelope to {model.WpayIdTo} [{model.CurrencyCode} {model.Amount}]";

                #region Payment to Red envelope service process 
                var getAllBalances = new NewInstantPaymentMethods(AppSecurity.CurrentUser).PrepareAccountBalances();
                var liquidCcyList = _dependencyLiquidForUserService.GetAllSoByUser(Guid.Parse(AppSecurity.CurrentUser.UserId)).Obj;
                AutomaticExchangeCommonMethods automaticExchangeCommonMethods = new AutomaticExchangeCommonMethods(AppSecurity.CurrentUser);
                var selectedBalanceByCcy = getAllBalances.Balances.FirstOrDefault(f => f.CCY == model.CurrencyCode);
                var baseCcy = getAllBalances.Balances.FirstOrDefault(f => f.CCY == (getAllBalances.Balances.FirstOrDefault()?.BaseCCY ?? "USD"));
                if (selectedBalanceByCcy != null)
                {
                    var overdraftUser = _liquidOverDraftUserServiceMethods.GetAllSo().Obj.FirstOrDefault(f =>
                        f.LiquidOverDraftUserList_UserId == Guid.Parse(AppSecurity.CurrentUser.UserId));

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
                                    f.LiquidOverDraftUserList_UserId == Guid.Parse(AppSecurity.CurrentUser.UserId)).ToList();

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
                                    throw new Exception(checkedLiquids?.InfoBlock?.UserMessage);
                                }
                            }
                            else
                            {
                                throw new Exception("Sorry, you do not have sufficient funds based on your selected liquidation preferences in your user profile.");
                            }
                        }
                        else
                        {
                            throw new Exception("Sorry, you do not have sufficient funds based on your selected liquidation preferences in your user profile.");
                        }
                        /***************************************/
                    }
                }

                NewInstantPaymentMethods m = new NewInstantPaymentMethods(AppSecurity.CurrentUser);
                var res = m.Create(apiNewInstantPayment);
                if (res.ServiceResponse.HasErrors)
                    throw new Exception(StringExtensions.ConvertServiceResponseToSingleString(res.ServiceResponse.Responses));

                var paymentRedEnvelopeResult = m.Post(Guid.Parse(res.PaymentInformation.PaymentId));
                if (paymentRedEnvelopeResult.ServiceResponse.HasErrors)
                    throw new Exception(StringExtensions.ConvertServiceResponseToSingleString(paymentRedEnvelopeResult.ServiceResponse.Responses));

                #endregion
                RedEnvelopeSo redEnvelope = new RedEnvelopeSo();
                redEnvelope.RedEnvelope_Amount = model.Amount;
                redEnvelope.RedEnvelope_CurrencyCode = model.CurrencyCode;
                redEnvelope.RedEnvelope_DateTransferedToRedEnvelope = DateTime.Now;
                redEnvelope.RedEnvelope_IsSuccessTransferToRedEnvelopeAcc = true;
                redEnvelope.RedEnvelope_FilePath = serverPath;
                redEnvelope.RedEnvelope_Note = model.Memo;
                redEnvelope.RedEnvelope_WPayIdTo = model.WpayIdTo;
                redEnvelope.RedEnvelope_WPayIdFrom = model.WpayIdFrom;
                redEnvelope.RedEnvelope_RedEnvelopePaymentId = Guid.Parse(res.PaymentInformation.PaymentId);

                var resToInsertEnvelope = _envelopeServiceMethods.Insert(redEnvelope);
                if (!resToInsertEnvelope.Success || resToInsertEnvelope.Obj == null)
                {
                    _logger.Error($"DB save red envelope error: {resToInsertEnvelope.Message}");
                    throw new Exception($"DB save red envelope error: {resToInsertEnvelope.Message}");
                }
                var transfers = new TransfersSo()
                {
                    Transfers_LinkToSourceRow = resToInsertEnvelope.Obj.RedEnvelope_Id,
                    Transfers_IsKycCreated = model.IsKycCreated,
                    Transfers_KycLinkId = model.KycId,
                    Transfers_Source = "Red Envelope",
                    Transfers_SourceType = TransfersSourceTypeEnum.RedEnvelope,
                    Transfers_TransferParent = AppSecurity.CurrentUser.UserName,
                    Transfers_TransferRecipient = userNameTo
                };
                var querySaveTransfer = _transfersServiceMethods.Insert(transfers);
                if (querySaveTransfer.Success)
                    return RedirectToAction("Index", "Home");
                throw new Exception(querySaveTransfer.Message);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                ViewBag.ErrorServer = e.Message;
                ViewBag.IsServerError = true;
            }

            return View(model);
        }
    }
}