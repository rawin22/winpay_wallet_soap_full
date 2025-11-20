using System;
using System.Collections.Generic;
using System.Net;
using TSG.Models.APIModels;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models.Merchants;
using Tsg.UI.Main.Models.NopCommerce;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.NopCommerceChekingRules;
using Tsg.UI.Main.Repository;

namespace Tsg.UI.Main.APIControllers
{
    /// <summary>
    /// 
    /// </summary>
    public static class NopCommerceApiLogic
    {
        static readonly log4net.ILog _logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void NopCommercePreparePaymentProcess(string token, string order, ref NopCommerceModel model, out bool checker, out List<Exception> errorsList)
        {
            errorsList = new List<Exception>();
            checker = true;

            try
            {
                // Check rules
                // Check token (Check existing Merchant by Id)
                var resCheckToken = NopCommerceRules.CheckToken(token, out var checkTokenExc);
                checker &= resCheckToken.IsSuccess;
                if (checkTokenExc != null)
                {
                    errorsList.Add(checkTokenExc);
                    throw new Exception(checkTokenExc.Message);
                }

                model.Token = token;

                if (IsParsedData(order, resCheckToken, ref model, ref checker, ref errorsList))
                {
                    NopCommerceRepository reps = new NopCommerceRepository();
                    if (errorsList.Count > 0)
                    {
                        throw new Exception();
                    }

                    var checkOrderIfExist = ApiLogic.CheckOrder(model.Custom);

                    if (!checkOrderIfExist.IsExist)
                    {
                        try
                        {
                            var tokenGuid = ApiLogic.GetNewOrderKey(
                                Guid.Parse(resCheckToken.ReturnedObject.MerchantGuid),
                                model.UrlReturn, model.CurrencyCode, model.Custom, model.IsSandbox);
                            checker &= tokenGuid.ResultAnswer.ResultCode == HttpStatusCode.OK;
                            if (!checker)
                                throw new Exception(tokenGuid.ResultAnswer.ResultText);
                            model.OrderTokenInEwalletSystem = tokenGuid.KeyOfTokenGuid;
                            model.Account = UiHelper.GetAccountBalance(model.CurrencyCode).AccountNumber;
                            model.AccountSingleText = UiHelper.GetAccountBalance(model.CurrencyCode, true);
                            model.AccountSingleNumber = UiHelper.GetAccountBalance(model.CurrencyCode).AccountNumber;
                            
                            var confirmOrder = reps.ConfirmOrder(tokenGuid.KeyOfTokenGuid);
                            if (confirmOrder.IsSuccess != null && !Convert.ToBoolean(confirmOrder.IsSuccess))
                            {
                                throw new Exception(confirmOrder.Message);
                            }

                            var prepareOrder = reps.PrepareOrderToPay(Guid.Parse(tokenGuid.KeyOfTokenGuid));
                            if (!prepareOrder.IsSuccess)
                                throw new Exception(prepareOrder.Message);
                        }
                        catch (Exception internalEx)
                        {
                            _logger.Error(internalEx);
                            errorsList.Add(internalEx);
                            checker = false;
                        }
                    }
                    else if (checkOrderIfExist.Order.CustomerOrderStatus == (int)OrderStatuses.SuccessifulPayment)
                    {
                        model.NeedToRedirect = true;
                        return;
                    }
                    else
                    {
                        try
                        {
                            checkOrderIfExist.Order.Currency = model.CurrencyCode;
                            checkOrderIfExist.Order.Quantity = model.Amount;
                            checkOrderIfExist.Order.OrderExpiredDate = DateTime.Now.AddMinutes(30);
                            checker &= ApiLogic.UpdateOrder(checkOrderIfExist.Order);
                            model.OrderTokenInEwalletSystem = checkOrderIfExist.Order.CustomerOrderId.ToString();
                            
                            var prepareOrder = reps.PrepareOrderToPay(checkOrderIfExist.Order.CustomerOrderId);
                            if (!prepareOrder.IsSuccess)
                                throw new Exception(prepareOrder.Message);
                        }
                        catch (Exception internalEx)
                        {
                            _logger.Error(internalEx);
                            errorsList.Add(internalEx);
                            checker = false;
                        }
                    }


                    if (checker && errorsList.Count == 0)
                    {
                        model.IsSuccess = true;
                        model.Message = "Please confirm your payment";
                    }
                    else
                    {
                        foreach (var exception in errorsList)
                        {
                            _logger.Error(exception);
                        }

                        throw new Exception("Cummulative exception");
                    }
                }
                else
                {
                    throw new Exception("Data corrupted");
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                checker = false;
                errorsList.Add(e);
            }
        }

        static bool IsParsedData(string order, NopCommerceRepository.ResultOfDbOperation<MerchantModel> mm, ref NopCommerceModel model, ref bool checker, ref List<Exception> errorsList)
        {
            try
            {
                NopCommerceRepository reps = new NopCommerceRepository();

                var ord = order.Decrypt(mm.ReturnedObject.MerchantPublicTokenKey,
                    mm.ReturnedObject.MerchantPrivateTokenKey);
                var splittedData = ord.Split('&');

                if (splittedData.Length > 0)
                {
                    model = reps.ParseData(model, splittedData, out checker, out errorsList);
                    model.QueryString = ord;
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static void NopCommerceDoPaymentProcess(string token, string order, ref NopCommerceModel model, out bool checker, out List<Exception> errorsList)
        {
            errorsList = new List<Exception>();
            checker = true;
            try
            {
                // Check rules
                // Check token (Check existing Merchant by Id)
                var resCheckToken = NopCommerceRules.CheckToken(token, out var checkTokenExc);
                checker &= resCheckToken.IsSuccess;
                if (checkTokenExc != null)
                {
                    errorsList.Add(checkTokenExc);
                }

                model.Token = token;
                NopCommerceRepository reps = new NopCommerceRepository();

                if (IsParsedData(order, resCheckToken, ref model, ref checker, ref errorsList))
                {
                    try
                    {
                        var checkOrderIfExist = ApiLogic.CheckOrder(model.Custom);
                        if (checkOrderIfExist.IsExist && checkOrderIfExist.Order.CustomerOrderStatus ==
                            (int)OrderStatuses.SuccessifulPayment)
                        {
                            model.NeedToRedirect = true;
                            return;
                        }
                        var prepareOrder = reps.PrepareOrderToPay(Guid.Parse(model.OrderTokenInEwalletSystem));
                        if (!prepareOrder.IsSuccess)
                            throw new Exception(prepareOrder.Message);
                        if (String.IsNullOrEmpty(model.Alias) || String.IsNullOrEmpty(model.Account))
                        {
                            throw new Exception("Please select other account and WPayId");
                        }

                        if (prepareOrder.IsSuccess)
                        {
                            prepareOrder.ReturnedObject.Account = model.Account;
                            prepareOrder.ReturnedObject.Alias = model.Alias;
                            prepareOrder.ReturnedObject.ReturnUrl = model.UrlReturn;
                            PaymentRepository paymentRepository = new PaymentRepository();
                            var confPayment = paymentRepository.ConfirmOrder(prepareOrder.ReturnedObject,
                                out var returnedObjectReturnUrl);
                            if (confPayment.IsSuccess.HasValue && (bool)confPayment.IsSuccess)
                                prepareOrder.ReturnedObject.ReturnUrl = returnedObjectReturnUrl;
                            else throw new Exception(confPayment.Message);

                        }
                        else
                        {
                            errorsList.Add(new Exception(prepareOrder.Message));
                            goto COMPLETEPROC;
                        }

                        var getInstantantForOrder = ApiLogic.GetInstantantForOrder(
                            Guid.Parse(resCheckToken.ReturnedObject.MerchantGuid),
                            Guid.Parse(model.OrderTokenInEwalletSystem), Guid.Parse(AppSecurity.CurrentUser.UserId),
                            model.ItemName, model.Items, model.Amount);
                        if (getInstantantForOrder.IsSuccesifull)
                        {
                            var paymentOrderProcess = ApiLogic.PaymentOrderProcess(
                                Guid.Parse(resCheckToken.ReturnedObject.MerchantGuid),
                                Guid.Parse(model.OrderTokenInEwalletSystem),
                                Guid.Parse(AppSecurity.CurrentUser.UserId), model.ItemName, model.Items, model.Amount);
                            if (paymentOrderProcess.IsSuccesifull)
                            {
                                model.NeedToRedirect = true;
                                return;
                            }
                            else
                            {
                                throw new Exception(paymentOrderProcess.ResultAnswer.ResultText);
                            }
                        }
                        else
                        {
                            throw new Exception(getInstantantForOrder.ResultAnswer.ResultText);
                        }
                    }
                    catch (Exception internalEx)
                    {
                        _logger.Error(internalEx);
                        errorsList.Add(internalEx);
                        checker = false;
                    }

                    COMPLETEPROC:
                    if (checker && errorsList.Count == 0)
                    {
                        model.IsSuccess = true;
                        model.Message = "Please confirm your payment";
                    }
                    else
                    {
                        string commulativeError = String.Empty;
                        foreach (var exception in errorsList)
                        {
                            _logger.Error(exception);
                            commulativeError += exception.Message + Environment.NewLine;
                        }

                        throw new Exception(commulativeError);
                    }
                }
                else
                {
                    throw new Exception("Data corrupted");
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);

                model.IsSuccess = false;
                model.Message = e.Message;
            }
        }
    }
}