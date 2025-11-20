using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using Tsg.Business.Model.Classes;
using TSG.Crypto;
using TSGWebApi.Controllers;
using TSGWebApi.Models;

namespace TSGWebApi.Logic
{
    public static class ApiLogic
    {
        public static TsgEwalletDbEntities EwalletDb { get; private set; }

        static ApiLogic()
        {
            EwalletDb = new TsgEwalletDbEntities();
        }

        private static TsgEwalletDbEntities CheckConnection()
        {
            if (EwalletDb.Database.Connection.State != ConnectionState.Open)
                EwalletDb.Database.Connection.Open();
            return EwalletDb;
        }

        /// <summary>
        ///     Checking if order existing
        /// </summary>
        /// <param name="orderGuid">Order token getted in ewallet system</param>
        /// <returns>Return bool variable: True if exist; False if not exist;</returns>
        public static OrderInfo CheckOrder(Guid orderGuid)
        {
            var res = new OrderInfo {Order = null, IsExist = false};
            CommonMethods.PrintInfo("[Static] Calling CheckOrder Method", true);
            CheckConnection();
            CommonMethods.PrintInfo("Db connection setted and activated");
            var order = EwalletDb.TokenKeysForOrders.AsNoTracking()
                .FirstOrDefault(a => a.CustomerOrderId == orderGuid);
            if (order != null)
            {
                res.IsExist = true;
                res.Order = order;
                CommonMethods.PrintInfo(string.Format(
                    "--------- Result succesiffully getted for Order Guid : {0} ---------", order.CustomerOrderId));
            }
            else
            {
                CommonMethods.PrintError("--------- Result is null, check values ---------");
            }
            return res;
        }

        /// <summary>
        ///     Checking if merchant existing
        /// </summary>
        /// <param name="merchantSecretKey">Secret key for merchant</param>
        /// <returns>Return bool variable: True if exist; False if not exist;</returns>
        public static MerchantInfo CheckMerchant(Guid merchantSecretKey)
        {
            var res = new MerchantInfo {Merchant = null, IsExist = false};
            CommonMethods.PrintInfo("[Static] Calling CheckMerchant Method", true);
            CheckConnection();
            CommonMethods.PrintInfo("Db connection setted and activated");
            var merchant = EwalletDb.Merchants.AsNoTracking().FirstOrDefault(a => a.UniqueID == merchantSecretKey);
            if (merchant != null)
            {
                res.IsExist = true;
                res.Merchant = merchant;
                CommonMethods.PrintInfo(string.Format(
                    "--------- Result succesiffully getted for Merchant: {0} ---------", merchant.Name));
            }
            else
            {
                CommonMethods.PrintError("--------- Result is null, check values ---------");
            }
            return res;
        }

        /// <summary>
        ///     Static Method for getted new unique number for order
        /// </summary>
        /// <param name="merchantKey">Id for merchant in ewallet system</param>
        /// <param name="returnUrl">Return url setted in merchant side</param>
        /// <returns></returns>
        public static TsgApiModels.TokenGuid GetNewOrderKey(Guid merchantKey, string returnUrl)
        {
            CommonMethods.PrintInfo("------------ [Static] GetNewOrderKey Start ---------------", true);
            var tokenGuid = new TsgApiModels.TokenGuid
            {
                ResultAnswer = new TsgApiModels.ResultAnswer
                {
                    ResultCode = HttpStatusCode.NotImplemented,
                    ResultText = "Not implemented",
                    ResultDate = DateTime.Now
                },
                KeyOfTokenGuid = ""
            };
            CommonMethods.PrintInfo("[Static] GetNewOrderKey setted default value");

            try
            {
                CheckConnection();
                CommonMethods.PrintInfo("[Static] Db connection setted and activated");

                using (var transaction = EwalletDb.Database.BeginTransaction())
                {
                    var mi = CheckMerchant(merchantKey);
                    if (mi.IsExist)
                    {
                        CommonMethods.PrintInfo("Begin transaction");
                        try
                        {
                            var orderGuid = Guid.NewGuid();
                            EwalletDb.TokenKeysForOrders.Add(new TokenKeysForOrders
                            {
                                CustomerOrderId = orderGuid,
                                CustomerOrderStatus = (int) OrderStatuses.Initiated,
                                MerchantId = mi.Merchant.ID,
                                MerchantWebSite = returnUrl,
                                OrderExpiredDate = DateTime.Now.AddMinutes(30)
                            });
                            EwalletDb.SaveChanges();
                            EwalletDb.OrdersHistory.Add(new OrdersHistory
                            {
                                TokenKey = orderGuid,
                                StatusState = (int) OrderStatuses.Initiated,
                                DateOfChanging = DateTime.Now,
                                MerchantId = mi.Merchant.ID
                            });
                            EwalletDb.SaveChanges();
                            transaction.Commit();

                            tokenGuid.KeyOfTokenGuid = orderGuid.ToString();
                            tokenGuid.ResultAnswer = new TsgApiModels.ResultAnswer
                            {
                                ResultDate = DateTime.Now,
                                ResultText = "OK",
                                ResultCode = HttpStatusCode.OK
                            };
                            CommonMethods.PrintInfo(
                                string.Format("Commited transaction succesifully for order {0} for merchant {1}",
                                    orderGuid, mi.Merchant.Name));
                        }
                        catch (Exception dbEwalletDbException)
                        {
                            transaction.Rollback();
                            tokenGuid.KeyOfTokenGuid = string.Empty;
                            tokenGuid.ResultAnswer = new TsgApiModels.ResultAnswer
                            {
                                ResultDate = DateTime.Now,
                                ResultText = dbEwalletDbException.Message,
                                ResultCode = HttpStatusCode.BadRequest
                            };
                            CommonMethods.PrintError(dbEwalletDbException);
                        }
                    }
                    else
                    {
                        tokenGuid.KeyOfTokenGuid = string.Empty;
                        tokenGuid.ResultAnswer = new TsgApiModels.ResultAnswer
                        {
                            ResultDate = DateTime.Now,
                            ResultText = string.Format("[{0}] cann't be recognized or finded in ewallet system.",
                                merchantKey),
                            ResultCode = HttpStatusCode.NotImplemented
                        };
                    }
                }
            }
            catch (Exception connectionException)
            {
                tokenGuid.KeyOfTokenGuid = string.Empty;
                tokenGuid.ResultAnswer = new TsgApiModels.ResultAnswer
                {
                    ResultDate = DateTime.Now,
                    ResultText = connectionException.Message,
                    ResultCode = HttpStatusCode.BadRequest
                };
                CommonMethods.PrintError(connectionException);
            }
            CommonMethods.PrintInfo("------------  SetInstantantForOrder Finish ---------------");

            return tokenGuid;
        }

        public static TsgApiModels.ResultAnswer CheckingUserLogin(string userName, Guid orderToken, string stoke,
            Guid userTokenInTsGuid)
        {
            var result = new TsgApiModels.ResultAnswer
            {
                ResultCode = HttpStatusCode.NotImplemented,
                ResultText = "Not implemented",
                ResultDate = DateTime.Now
            };
            CommonMethods.PrintInfo("[Static] Calling CheckingUserLogin Method");
            try
            {
                CheckConnection();
                CommonMethods.PrintInfo("Db connection setted and activated");
                stoke = Crypto.Decrypt(stoke, orderToken.ToString());
                var service = new IgpService(userName, stoke, userTokenInTsGuid.ToString());
                var userData = service.GetUserData(userName, stoke);
                if (userData.ServiceResponse.HasErrors || userData.UserSettings.UserId != userTokenInTsGuid.ToString())
                    throw new Exception("Invalid user info");

                var oi = CheckOrder(orderToken);
                if (oi.IsExist)
                {
                    using (var transaction = EwalletDb.Database.BeginTransaction())
                    {
                        CommonMethods.PrintInfo("Begin transaction");
                        try
                        {
                            oi.Order.CustomerId = userTokenInTsGuid;
                            oi.Order.CustomerOrderStatus = (int) OrderStatuses.Unconfirmed;
                            oi.Order.Stoke = stoke;

                            EwalletDb.TokenKeysForOrders.Attach(oi.Order);
                            EwalletDb.Entry(oi.Order).State = EntityState.Modified;
                            EwalletDb.SaveChanges();
                            EwalletDb.OrdersHistory.Add(new OrdersHistory
                            {
                                TokenKey = oi.Order.CustomerOrderId,
                                StatusState = (int) OrderStatuses.Unconfirmed,
                                DateOfChanging = DateTime.Now,
                                MerchantId = (int) oi.Order.MerchantId
                            });
                            EwalletDb.SaveChanges();
                            transaction.Commit();
                            result.ResultDate = DateTime.Now;
                            result.ResultText = "OK";
                            result.ResultCode = HttpStatusCode.OK;

                            CommonMethods.PrintInfo(string.Format("Commited transaction succesifully for order {0}",
                                oi.Order.CustomerOrderId));
                        }
                        catch (Exception dbEwalletDbException)
                        {
                            transaction.Rollback();

                            result.ResultDate = DateTime.Now;
                            result.ResultText = dbEwalletDbException.Message;
                            result.ResultCode = HttpStatusCode.BadRequest;
                            CommonMethods.PrintError(dbEwalletDbException);
                        }
                    }
                }
                else
                {
                    result.ResultDate = DateTime.Now;
                    result.ResultText = string.Format(
                        "Order with guid [{0}] cann't be recognized or finded in ewallet system.", orderToken);
                    result.ResultCode = HttpStatusCode.NotImplemented;
                }
            }
            catch (Exception connectionException)
            {
                result.ResultDate = DateTime.Now;
                result.ResultText = connectionException.Message;
                result.ResultCode = HttpStatusCode.BadRequest;
                CommonMethods.PrintError(connectionException);
            }
            CommonMethods.PrintInfo("------------  CheckingUserLogin Finish ---------------");
            return result;
        }

        public static TsgApiModels.AnswerForOrder GetInstantantForOrder(Guid merchantKey, Guid orderToken,
            Guid userToken, decimal quantity, bool isPayed = false)
        {
            var token = new TsgApiModels.AnswerForOrder
            {
                ResultAnswer = new TsgApiModels.ResultAnswer(),
                KeyOfTokenGuid = orderToken.ToString(),
                IsSuccesifull = false
            };

            CommonMethods.PrintInfo("------------ [Static] GetInstantantForOrder Start ---------------");
            try
            {
                CheckConnection();
                CommonMethods.PrintInfo("Db connection setted and activated");
                var mi = CheckMerchant(merchantKey);
                var oi = CheckOrder(orderToken);
                if (mi.IsExist && oi.IsExist)
                {
                    if (oi.Order.CustomerOrderStatus == 5)
                        throw new Exception("This order is closed");
                    if (oi.Order.CustomerOrderStatus < 3)
                        throw new Exception("This order is unconfirmed");
                    if (oi.Order.OrderExpiredDate < DateTime.Now)
                        throw new Exception("Order date is expired");
                    var currUser = EwalletDb.User.FirstOrDefault(f => f.userIdByTSG == userToken);
                    if (currUser == null)
                        throw new Exception("Unregistred user detected");
                    var service = new IgpService(currUser.username,
                        Crypto.Decrypt(oi.Order.Stoke, oi.Order.CustomerOrderId.ToString()), userToken.ToString());
                    var accountInOrder = service.GetAccountBalances()
                        .Balances.FirstOrDefault(w => w.AccountNumber == oi.Order.CustomerAccounting.ToString());
                    if (accountInOrder != null)
                    {
                        if (accountInOrder.Balance > quantity)
                        {
                            token.IsSuccesifull = true;
                            token.ResultAnswer.ResultCode = HttpStatusCode.OK;
                            token.ResultAnswer.ResultDate = DateTime.Now;
                            token.ResultAnswer.ResultText = "Order can be paying";
                        }
                        else
                        {
                            token.IsSuccesifull = false;
                            token.ResultAnswer.ResultCode = HttpStatusCode.Conflict;
                            token.ResultAnswer.ResultDate = DateTime.Now;
                            token.ResultAnswer.ResultText = "Order can not be paying. Insufficient balance";
                            EwalletDb.OrdersHistory.Add(new OrdersHistory
                            {
                                TokenKey = oi.Order.CustomerOrderId,
                                StatusState = (int) OrderStatuses.InsufficientBalance,
                                DateOfChanging = DateTime.Now,
                                MerchantId = (int) oi.Order.MerchantId
                            });
                            return token;
                        }
                        if (!isPayed)
                            return token;

                        token.IsSuccesifull = false;
                        token.ResultAnswer = new TsgApiModels.ResultAnswer
                        {
                            ResultCode = HttpStatusCode.NotImplemented,
                            ResultDate = DateTime.Now,
                            ResultText = "Not implement"
                        };
                        var userAliasRec =
                            EwalletDb.OrdersHistory.AsNoTracking()
                                .ToList()
                                .LastOrDefault(
                                    ld => ld.TokenKey == orderToken &&
                                          ld.StatusState == (int) OrderStatuses.Confirmed &&
                                          ld.MerchantId == mi.Merchant.ID);
                        if (userAliasRec == null || string.IsNullOrEmpty(userAliasRec.AliasForUser))
                            throw new Exception("User values is corrupted");
                        var isPayedSuccesifull = false;
                        try
                        {
                            var newPayment = service.CreateNewPayment(userAliasRec.AliasForUser, mi.Merchant.Name,
                                quantity, accountInOrder.CCY, "Paid thru Merchant API",
                                mi.Merchant.UniqueID.ToString(), "MerchantPurchase");
                            if (!newPayment.ServiceResponse.HasErrors)
                            {
                                var paymentId = Guid.Parse(newPayment.PaymentInformation.PaymentId);
                                var postResult = service.PostInstantPayment(paymentId);
                                if (!postResult.ServiceResponse.HasErrors ||
                                    postResult.ServiceResponse.Responses[0].Message == "Success")
                                {
                                    isPayedSuccesifull = true;
                                    CommonMethods.PrintInfo(
                                        string.Format("{0} order sussesiffully payed in inner TSG system",
                                            oi.Order.CustomerOrderId));
                                }
                                if (!isPayedSuccesifull)
                                {
                                    throw new Exception(String.Format("Create new payment message [{0}] \n\r InnerDetail {1}" +
                                                                      "Pay error {2}\r\n Detail:{3}", newPayment.ServiceResponse.Responses[0].Message, newPayment.ServiceResponse.Responses[0].MessageDetails,
                                        postResult.ServiceResponse.Responses[0].Message, postResult.ServiceResponse.Responses[0].MessageDetails));
                                }
                            }
                            if (!isPayedSuccesifull)
                            {
                                throw new Exception(String.Format("Create new payment message [{0}] \n\r InnerDetail {1}", newPayment.ServiceResponse.Responses[0].Message, newPayment.ServiceResponse.Responses[0].MessageDetails));
                            }
                        }
                        catch (Exception tsgInnerException)
                        {
                            token.IsSuccesifull = false;
                            token.ResultAnswer = new TsgApiModels.ResultAnswer()
                            {
                                ResultCode = HttpStatusCode.Conflict,
                                ResultDate = DateTime.Now,
                                ResultText = String.Format("{0}\r\n{1}","TSG inner error", tsgInnerException.Message)
                            };
                            oi.Order.CustomerOrderStatus = (int)OrderStatuses.FaildPaymentTsg;

                            EwalletDb.Set<TokenKeysForOrders>().AddOrUpdate(oi.Order);
                            EwalletDb.SaveChanges();
                            EwalletDb.OrdersHistory.Add(new OrdersHistory
                            {
                                TokenKey = oi.Order.CustomerOrderId,
                                StatusState = (int) OrderStatuses.FaildPaymentTsg,
                                DateOfChanging = DateTime.Now,
                                MerchantId = (int) oi.Order.MerchantId,
                                Comment = tsgInnerException.Message
                            });
                            EwalletDb.SaveChanges();
                            CommonMethods.PrintError(tsgInnerException);
                        }
                        try
                        {
                            CheckConnection();
                            if (isPayedSuccesifull)
                            {
                                oi.Order.Quantity = quantity;
                                oi.Order.Currency = accountInOrder.CCY;
                                oi.Order.CustomerOrderStatus = (int) OrderStatuses.SuccessifulPayment;

                                EwalletDb.Set<TokenKeysForOrders>().AddOrUpdate(oi.Order);
                                EwalletDb.SaveChanges();
                                EwalletDb.OrdersHistory.Add(new OrdersHistory
                                {
                                    TokenKey = oi.Order.CustomerOrderId,
                                    StatusState = (int) OrderStatuses.SuccessifulPayment,
                                    DateOfChanging = DateTime.Now,
                                    MerchantId = (int) oi.Order.MerchantId
                                });
                                EwalletDb.SaveChanges();
                                token.IsSuccesifull = true;
                                token.ResultAnswer = new TsgApiModels.ResultAnswer
                                {
                                    ResultCode = HttpStatusCode.OK,
                                    ResultDate = DateTime.Now,
                                    ResultText = "Order successed payed"
                                };
                            }
                        }
                        catch (Exception ewalletInnerException)
                        {
                            token.IsSuccesifull = false;
                            token.ResultAnswer = new TsgApiModels.ResultAnswer()
                            {
                                ResultCode = HttpStatusCode.Conflict,
                                ResultDate = DateTime.Now,
                                ResultText = "Ewallet error"
                            };
                            oi.Order.CustomerOrderStatus = (int)OrderStatuses.FaildPaymentEWallet;

                            EwalletDb.Set<TokenKeysForOrders>().AddOrUpdate(oi.Order);
                            EwalletDb.SaveChanges();
                            EwalletDb.OrdersHistory.Add(new OrdersHistory
                            {
                                TokenKey = oi.Order.CustomerOrderId,
                                StatusState = (int) OrderStatuses.FaildPaymentEWallet,
                                DateOfChanging = DateTime.Now,
                                MerchantId = (int) oi.Order.MerchantId,
                                Comment = ewalletInnerException.Message
                            });
                            EwalletDb.SaveChanges();
                            CommonMethods.PrintError(ewalletInnerException);
                        }
                    }
                }
                else
                {
                    throw new Exception("Unregistred user detected");
                }
            }
            catch (Exception ex)
            {
                token.ResultAnswer.ResultCode = HttpStatusCode.BadRequest;
                token.ResultAnswer.ResultText = "Failed" + ex.Message;
                token.ResultAnswer.ResultDate = DateTime.Now;

                CommonMethods.PrintError(ex);
            }
            CommonMethods.PrintInfo("------------  GetInstantantForOrder Finish ---------------");

            return token;
        }

        public static TsgApiModels.AnswerForOrder PaymentOrderProcess(Guid merchantKey, Guid orderToken, Guid userToken,
            decimal quantity)
        {
            var token = GetInstantantForOrder(merchantKey, orderToken, userToken, quantity, true);
            return token;
        }

        #region Data structures

        public class MerchantInfo
        {
            public bool IsExist { get; set; }
            public Merchants Merchant { get; set; }
        }

        public class OrderInfo
        {
            public bool IsExist { get; set; }
            public TokenKeysForOrders Order { get; set; }
        }

        #endregion
    }
}