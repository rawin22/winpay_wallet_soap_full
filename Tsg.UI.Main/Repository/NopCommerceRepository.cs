using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.Business.Model.TSGgpwsbeta;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Merchants;
using Tsg.UI.Main.Models.NopCommerce;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.NopCommerceChekingRules;
using Crypto = WinstantPay.Common.CryptDecriptInfo;


namespace Tsg.UI.Main.Repository
{
    public class NopCommerceRepository
    {
        readonly log4net.ILog _logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SqlConnection _con;

        private void Connection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["ewalletDbCon"].ToString();
            _con = new SqlConnection(conStr);
        }

        public class ResultOfDbOperation<T>
        {
            public bool IsSuccess { get; set; }
            public string Message { get; set; }
            public T ReturnedObject { get; set; }
        }

        public ResultOfDbOperation<MerchantModel> ChechMerchant(string merchantTokenKey)
        {
            ResultOfDbOperation<MerchantModel> res =
                new ResultOfDbOperation<MerchantModel>() { IsSuccess = false, Message = String.Empty };
            MerchantsRepository merchantsRepository = new MerchantsRepository();
            res.IsSuccess =
                merchantsRepository.ChechMerchantIfExist(String.Empty, out var resMessage, merchantTokenKey);
            res.Message = resMessage;
            if (res.IsSuccess)
            {
                res.ReturnedObject = merchantsRepository
                    .GetAllLocalMerchants()
                    .FirstOrDefault(f => f.MerchantPublicTokenKey == merchantTokenKey);
            }
            return res;
        }

        public NopCommerceModel ParseData(NopCommerceModel model, string[] splittedData, out bool checker,
            out List<Exception> errorsList)
        {
            // Check Order number
            checker = true;
            errorsList = new List<Exception>();

            checker &= NopCommerceRules.CheckString(
                splittedData[0].Contains("item_name") ? splittedData[0].Replace("item_name=", "") : "",
                out var itemNameExc,
                out var outItemName, false);
            if (itemNameExc != null)
            {
                errorsList.Add(itemNameExc);
            }
            model.ItemName = outItemName;

            // Check custom (included order id for checking order)
            checker &= NopCommerceRules.CheckGuid(splittedData[1].Replace("custom=", ""),
                out var checkCustomExc,
                out var outputCustom, false);
            if (checkCustomExc != null) errorsList.Add(checkCustomExc);
            if (outputCustom == Guid.Empty) errorsList.Add(new Exception("Custom can't be zero"));
            model.Custom = outputCustom;

            // Check avaliable currency
            checker &= NopCommerceRules.CheckCompatibleCurrency(splittedData[2].Replace("currency_code=", ""),
                out var checkCurrencyCodeExc,
                out string outputcurrency);
            if (checkCurrencyCodeExc != null) errorsList.Add(checkCurrencyCodeExc);
            model.CurrencyCode = outputcurrency;

            // Check amount
            checker &= NopCommerceRules.CheckDecimal(splittedData[3].Replace("amount=", ""),
                out var checkAmountExc,
                out decimal amountDec);
            if (checkAmountExc != null) errorsList.Add(checkAmountExc);
            if (amountDec == Decimal.Zero) errorsList.Add(new Exception("Amount can't be zero"));
            model.Amount = amountDec;

            // Check url return
            checker &= NopCommerceRules.CheckString(splittedData[4].Replace("url_return=", ""),
                out var checkDomainnOnReturnUrlExc,
                out string outputReturnUrl, false);
            if (checkDomainnOnReturnUrlExc != null) errorsList.Add(checkDomainnOnReturnUrlExc);
            model.UrlReturn = HttpUtility.UrlDecode(outputReturnUrl);

            // Check cancel url
            checker &= NopCommerceRules.CheckString(splittedData[5].Replace("cancel_return=", ""),
                out var checkDomainnOnCancelUrlExc,
                out string outputCancelOrderUrl, false);
            if (checkDomainnOnCancelUrlExc != null) errorsList.Add(checkDomainnOnCancelUrlExc);
            model.CancelReturn = HttpUtility.UrlDecode(outputCancelOrderUrl);

            // Check Items name and quantity
            checker &= NopCommerceRules.CheckString(
                splittedData[6].Contains("items") ? splittedData[6].Replace("items=", "") : "",
                out var itemsExc,
                out var outItems, false);
            if (itemsExc != null)
            {
                errorsList.Add(itemsExc);
            }
            model.Items = outItems;

            #region Not Mandatory fields
            //// Check first name
            //checker &= NopCommerceRules.CheckString(splittedData[6].Replace("first_name=", ""),
            //    out var checkFirstNameExc,
            //    out string outputFirstName, false);
            //if (checkFirstNameExc != null) errorsList.Add(checkFirstNameExc);
            //model.FirstName = outputFirstName;

            //// Check last name
            //checker &= NopCommerceRules.CheckString(splittedData[7].Replace("last_name=", ""),
            //    out var checkLastNameExc, out string outputLastName,
            //    false);
            //if (checkLastNameExc != null) errorsList.Add(checkLastNameExc);
            //model.LastName = outputLastName;

            //// Check email
            //checker &= NopCommerceRules.CheckString(splittedData[14].Replace("email=", ""),
            //    out var checkEmailExc,
            //    out string outputEmail, false);
            //if (checkEmailExc != null) errorsList.Add(checkEmailExc);
            //model.Email = HttpUtility.UrlDecode(outputEmail);

            //model.Address1 = splittedData[8].Replace("address1=", "");
            //model.Address2 = splittedData[9].Replace("address2=", "");
            //model.Country = splittedData[10].Replace("country=", "");
            //model.State = splittedData[11].Replace("state=", "");
            //model.City = splittedData[12].Replace("city=", "");
            //model.Zip = splittedData[13].Replace("zip=", "");
            #endregion

            return model;
        }


        public PaymentRepository.ResultOfDbOperation ConfirmOrder(string token)
        {
            PaymentRepository.ResultOfDbOperation result =
                new PaymentRepository.ResultOfDbOperation { IsSuccess = false, Message = String.Empty };
            Connection();
            SqlCommand com = _con.CreateCommand();
            try
            {
                _con.Open();
                com.Connection = _con;
                Guid guidtokenKey;
                if (Guid.TryParse(token, out guidtokenKey))
                {
                    com = new SqlCommand("CheckOrderIfExist", _con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Transaction = null;
                    com.Parameters.AddWithValue("@orderToken", guidtokenKey);
                    var resMerchantId = (int)com.ExecuteScalar();

                    if (resMerchantId != 0)
                    {
                        com.Parameters.Clear();
                        Guid customerIdInTsg;
                        if (Guid.TryParse(AppSecurity.CurrentUser.UserId, out customerIdInTsg))
                        {
                            SqlTransaction transaction = _con.BeginTransaction();
                            try
                            {
                                string login = AppSecurity.CurrentUser.UserName.ToLower();
                                com = new SqlCommand("UpdateTsgIdViaTsgService", _con);
                                com.CommandType = CommandType.StoredProcedure;
                                com.Transaction = transaction;
                                com.Parameters.AddWithValue("@userLogin", login);
                                com.Parameters.AddWithValue("@userTokenByTSG", customerIdInTsg);
                                com.ExecuteNonQuery();
                                com.Parameters.Clear();
                                com.CommandText =
                                    String.Format(
                                        "SELECT [CustomerOrderStatus] FROM [dbo].[TokenKeysForOrders] WHERE [CustomerOrderId]='{0}'",
                                        guidtokenKey);
                                com.CommandType = CommandType.Text;
                                com.Transaction = transaction;
                                var stateOfOrder = (int)com.ExecuteScalar();
                                com.Parameters.Clear();
                                if (stateOfOrder == 5)
                                    throw new Exception("This order has already been paid.");
                                var createStoke = Crypto.Crypto.Encrypt(AppSecurity.CurrentUser.Password,
                                    guidtokenKey.ToString());
                                com.CommandText =
                                    String.Format(
                                        "UPDATE [dbo].[TokenKeysForOrders] SET [CustomerId] = '{0}'," +
                                        " [CustomerOrderStatus]=2, [Stoke]='{1}' WHERE [CustomerOrderId]='{2}'",
                                        customerIdInTsg, createStoke, guidtokenKey);
                                com.CommandType = CommandType.Text;
                                com.Transaction = transaction;
                                com.ExecuteNonQuery();
                                com.Parameters.Clear();
                                com = new SqlCommand("AddOrderHistoryRecs", _con);
                                com.CommandType = CommandType.StoredProcedure;
                                com.Transaction = transaction;
                                com.Parameters.AddWithValue("@OrderToken", guidtokenKey);
                                com.Parameters.AddWithValue("@StatusState", 2);
                                com.Parameters.AddWithValue("@Comment",
                                    "Successifully setted unconfirm status");
                                com.Parameters.AddWithValue("@MerchantId", resMerchantId);
                                com.ExecuteNonQuery();

                                transaction.Commit();
                                _logger.Info("Commited changes");
                                result.IsSuccess = true;
                                result.Message = "Success to change order status";
                            }
                            catch (Exception e)
                            {
                                String.Format(GlobalRes.User_UserController_CheckoutLogin_ApplicationError, e.Message);
                                transaction.Rollback();
                                result.IsSuccess = false;
                                result.Message = e.Message;
                            }
                        }
                        else throw new Exception("can't find user");
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "User " + AppSecurity.CurrentUser.UserName +
                                         " attempted to log in but failed [Order token don't match]";
                        _logger.Error(result.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                result.IsSuccess = false;
                result.Message = GlobalRes.User_UserController_CheckoutLogin_ApplicationErrorEmptyParam;
                _logger.Error("User " + AppSecurity.CurrentUser.UserName + " attempted to log in but failed [Order token don't match]\n\r" + ex.Message);
            }
            finally
            {
                _con.Close();
            }
            return result;
        }

        public ResultOfDbOperation<CheckoutModel> PrepareOrderToPay(Guid token)
        {
            ResultOfDbOperation<CheckoutModel> result =
                new ResultOfDbOperation<CheckoutModel> { IsSuccess = false, Message = String.Empty, ReturnedObject = new CheckoutModel() };
            string customerAlias = "";
            try
            {
                Guid userInTsgSystem;
                if (Guid.TryParse(AppSecurity.CurrentUser.UserId, out userInTsgSystem))
                {
                    NewInstantPaymentViewModel paymentViewModel = new NewInstantPaymentViewModel(userInTsgSystem);
                    paymentViewModel.GetLastCustomerAlias();

                    Connection();
                    _con.Open();
                    SqlCommand com = _con.CreateCommand();
                    com.Connection = _con;
                    try
                    {
                        com = new SqlCommand("CheckOrderIfExist", _con);
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@orderToken", token);
                        var resMerchantId = (int)com.ExecuteScalar();
                        com.Parameters.Clear();
                        com = new SqlCommand("GetMerchantName", _con);
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@merchantId", resMerchantId);
                        var resMerchantName = com.ExecuteScalar().ToString();
                        result.ReturnedObject.OrderToken = token;
                        var avaliableAccounts = PrepareAccountBalances();
                        if (paymentViewModel.AccountAliases == null || avaliableAccounts == null || (paymentViewModel.AccountAliases != null && paymentViewModel.AccountAliases.Count == 0))
                        {
                            if (paymentViewModel.AccountAliases != null && paymentViewModel.AccountAliases.Count == 0)
                                throw new Exception(GlobalRes.HomeController_CheckoutIndex_Error_NotAlias);
                            throw new Exception(GlobalRes.HomeController_CheckoutIndex_Error_NotExist);
                        }

                        result.ReturnedObject.CustomerName = AppSecurity.CurrentUser.FirstName + " " + AppSecurity.CurrentUser.LastName;
                        result.ReturnedObject.MerchantName = resMerchantName;
                        
                        _con.Close();
                        
                        result.IsSuccess = true;
                        result.Message = "Success confirmed order";
                    }
                    catch (Exception e)
                    {
                        _logger.Error("CheckoutIndex Error:\n\r--->" + e.Message);
                        _con.Close();
                        result.IsSuccess = false;
                        result.Message = e.Message;
                        return result;
                    }
                }
                else
                {
                    string error = "CheckoutIndex can't understandind userGuid in TSG";
                    _logger.Error(error);
                    throw new Exception(error);
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Message = e.Message;
            }
            return result;
        }

        public IList<SelectListItem> PrepareAccountBalances()
        {
            IgpService service = new IgpService(AppSecurity.CurrentUser.UserName, AppSecurity.CurrentUser.Password, AppSecurity.CurrentUser.UserId);
            var allItems = new List<SelectListItem>();

            var response = service.GetAccountBalances();
            if (!response.ServiceResponse.HasErrors)
            {

                foreach (CustomerBalanceData data in response.Balances)
                {
                    allItems.Add
                    (
                        new SelectListItem
                        {
                            Text = data.AccountNumber,
                            Value = data.AccountNumber
                        }
                    );
                }
            }
            return allItems;
        }
    }
}