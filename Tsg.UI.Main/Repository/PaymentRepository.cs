using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Models;

namespace Tsg.UI.Main.Repository
{
    public class PaymentRepository
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private SqlConnection _con;

        private void Connection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["ewalletDbCon"].ToString();
            _con = new SqlConnection(conStr);
        }

        public class ResultOfDbOperation
        {
            public bool? IsSuccess { get; set; }
            public string Message { get; set; }
        }

        public ResultOfDbOperation ConfirmOrder(CheckoutModel cm, out string returnUrl)
        {
            var result = new ResultOfDbOperation() { IsSuccess = false, Message = string.Empty };
            returnUrl = String.Empty;

            try
            {
                Connection();
                _con.Open();
                SqlCommand com = _con.CreateCommand();
                com.Connection = _con;
                com = new SqlCommand("CheckOrderIfExist", _con);
                com.CommandType = CommandType.StoredProcedure;
                com.Transaction = null;
                com.Parameters.AddWithValue("@orderToken", cm.OrderToken);
                var resMerchantId = (int)com.ExecuteScalar();

                com.Parameters.Clear();

                com.CommandText = String.Format(
                    "SELECT TOP 1 [MerchantWebSite] FROM TokenKeysForOrders WHERE CustomerOrderId = '{0}'",
                    cm.OrderToken);
                com.CommandType = CommandType.Text;
                com.Transaction = null;
                var resMerchantSite = com.ExecuteScalar().ToString();
                returnUrl = resMerchantSite.Trim();
                if (resMerchantId != 0)
                {
                    _logger.Info("------------- Confirm Order Start ------------- ");
                    com.Parameters.Clear();
                    SqlTransaction transaction = _con.BeginTransaction();
                    com.CommandText = String.Format(
                        "IF (SELECT TOP 1 OrderExpiredDate FROM TokenKeysForOrders WHERE [CustomerOrderId]='{0}') IS NULL \n\rBEGIN\n\r " +
                        "UPDATE [dbo].[TokenKeysForOrders] SET [CustomerOrderStatus]=3, [CustomerAccounting]={1}, [OrderExpiredDate]=CONVERT(DATETIME, '{2}', 104)  WHERE [CustomerOrderId]='{3}' \n\r END \n\r " +
                        "ELSE" + "\n\rBEGIN\n\r " +
                        "UPDATE [dbo].[TokenKeysForOrders] SET [CustomerOrderStatus]=3, [CustomerAccounting]={4}  WHERE [CustomerOrderId]='{5}' \n\r END \n\r ",
                        cm.OrderToken, cm.Account, DateTime.Now.AddMinutes(30), cm.OrderToken, cm.Account, cm.OrderToken);
                    com.CommandType = CommandType.Text;
                    com.Transaction = transaction;
                    com.ExecuteNonQuery();
                    com.Parameters.Clear();
                    com = new SqlCommand("AddOrderHistoryRecs", _con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Transaction = transaction;
                    com.Parameters.AddWithValue("@OrderToken", cm.OrderToken);
                    com.Parameters.AddWithValue("@StatusState", 3);
                    com.Parameters.AddWithValue("@Comment", "Successifully setted CONFIRM status");
                    com.Parameters.AddWithValue("@MerchantId", resMerchantId);
                    if (!String.IsNullOrEmpty(cm.Alias))
                        com.Parameters.AddWithValue("@AliasName", cm.Alias);
                    else com.Parameters.AddWithValue("@AliasName", cm.CustomerName);
                    com.ExecuteNonQuery();
                    transaction.Commit();

                    _logger.Info("------------- Confirm Order End ------------- ");
                    result.Message = "success";
                    result.IsSuccess = true;
                    _con.Close();
                }
                else
                {
                    string errorMsg = "CheckoutIndex can't understandind userGuid in TSG";
                    _logger.Error(errorMsg);
                    result.Message = errorMsg;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                result.Message = e.Message;
            }

            return result;
        }

        public ResultOfDbOperation AddOrUpdateSharedPhoto(string paymentId, out string imageGuid)
        {
            ResultOfDbOperation res;
            imageGuid = String.Empty;

            try
            {
                Connection();
                SqlCommand com = new SqlCommand("AddOrUpdateSharedPhoto", _con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@PaymentId", paymentId);
                com.Parameters.AddWithValue("@Comment", "Winstpay memo");

                var returnParameter = com.Parameters.Add("@PhotoId", SqlDbType.UniqueIdentifier);
                returnParameter.Direction = ParameterDirection.Output;
                _con.Open();
                com.ExecuteNonQuery();
                imageGuid = returnParameter.Value.ToString();
                _con.Close();
                res = new ResultOfDbOperation() { Message = GlobalRes.Payment_PaymentRepository_Success, IsSuccess = true };
            }
            catch (Exception e)
            {
                _con.Close();
                _logger.Error(e);
                res = new ResultOfDbOperation() { Message = e.Message, IsSuccess = false };
            }
            return res;
        }

        public ResultOfDbOperation GetSharedPhoto(string paymentId, out string imageGuid, out DateTime expiredDate)
        {
            ResultOfDbOperation res;
            imageGuid = String.Empty;
            expiredDate = new DateTime();
            try
            {
                Connection();
                SqlCommand com = new SqlCommand("GetSharedPhoto", _con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@PaymentId", paymentId);

                var returnPhotoId = com.Parameters.Add("@PhotoId", SqlDbType.UniqueIdentifier);

                returnPhotoId.IsNullable = true;
                returnPhotoId.Direction = ParameterDirection.Output;
                var returnExpiredDate = com.Parameters.Add("@ExpiredDate", SqlDbType.DateTime);
                returnExpiredDate.IsNullable = true;
                returnExpiredDate.Direction = ParameterDirection.Output;
                _con.Open();
                com.ExecuteNonQuery();
                imageGuid = returnPhotoId.Value.ToString();
                if (String.IsNullOrEmpty(imageGuid))
                    imageGuid = Guid.Empty.ToString();
                if (returnExpiredDate.Value != DBNull.Value)
                    expiredDate = (DateTime)returnExpiredDate.Value;
                _con.Close();
                res = new ResultOfDbOperation() { Message = GlobalRes.Payment_PaymentRepository_Success, IsSuccess = true };
            }
            catch (Exception e)
            {
                _con.Close();
                _logger.Error(e);
                res = new ResultOfDbOperation() { Message = e.Message, IsSuccess = false };
            }
            return res;
        }
    }
}