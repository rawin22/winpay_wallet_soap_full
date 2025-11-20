using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tsg.UI.Main.Models;

namespace Tsg.UI.Main.Repository
{
    public class FundingRepository
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private SqlConnection _con;

        private void Connection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["ewalletDbCon"].ToString();
            _con = new SqlConnection(conStr);
        }

        public bool AddBankDeposit(BankDepositModel obj, out int insId)
        {
            Connection();
            SqlCommand com = new SqlCommand("AddBankDeposit", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@fndAmt", Convert.ToDecimal(obj.Amount));
            com.Parameters.AddWithValue("@username", obj.Username);
            com.Parameters.AddWithValue("@bankCcyId", obj.BankCurrencyId);
            com.Parameters.AddWithValue("@insId", DbType.Int32).Direction = ParameterDirection.Output;
            insId = 0;
            
            try
            {
                _logger.Info("Attempting to add funding to database: " + obj);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    insId = Convert.ToInt32(com.Parameters["@insId"].Value);
                    _logger.Info("Successfully added funding to database: " + obj);
                    return true;
                }
                else
                {
                    _logger.Error("Could not add funding information to database");
                    return false;
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return false;

        }

        public List<BankDepositModel> GetBankDeposits(string username)
        {
            Connection();
            List<BankDepositModel> bankDepositList = new List<BankDepositModel>();
            SqlCommand com = new SqlCommand("GetBankDeposits", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@username", username);
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();

            try
            {
                _logger.Info("Attempting to get list of fundings from database");
                _con.Open();
                da.Fill(dt);
                _con.Close();
                _logger.Info("List of fundings successfully obtained from database");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }


            bankDepositList = (from DataRow dr in dt.Rows
                                  select new BankDepositModel()
                                  {
                                      ProofDocId = (dr["proofDocId"] != DBNull.Value) ? Convert.ToInt32(dr["proofDocId"]) : -1,
                                      Status = Convert.ToString(dr["fndStatus"]),
                                      DepositId = Convert.ToInt32(dr["fndId"]),
                                      Bank = Convert.ToString(dr["bankName"]),
                                      CurrencyName = Convert.ToString(dr["ccyCode"]),
                                      BankCurrencyId=Convert.ToInt32(dr["bankCcyId"]),
                                      CreatedDate = Convert.ToDateTime(dr["paymentDate"]),
                                      Amount = Convert.ToDecimal(dr["amount"]),
                                      PathToFile = (dr["filePath"]!=DBNull.Value)? dr["filePath"].ToString():String.Empty
                                  }).OrderByDescending(ob=>ob.CreatedDate).ToList();

            return bankDepositList;
        }

        public List<FundingHistoryModel> GetHistoryByFundRequests()
        {
            Connection();
            List<FundingHistoryModel> fundHistoryModel = new List<FundingHistoryModel>();
            SqlCommand com = new SqlCommand("GetFundChanges", _con);
            com.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();

            try
            {
                _logger.Info("Attempting to get list of fundings from database");
                _con.Open();
                da.Fill(dt);
                _con.Close();
                _logger.Info("List of fundings successfully obtained from database");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }


            fundHistoryModel = (from DataRow dr in dt.Rows
                select new FundingHistoryModel()
                {
                    FundingId = Convert.ToInt32(dr["fndId"]),
                    AdminName = dr["changedBy"].ToString(),
                    AdminNote = dr["notes"] != DBNull.Value ? dr["notes"].ToString() : String.Empty,
                    ChangedDateTime = Convert.ToDateTime(dr["changedDate"]),
                    StatusNew = dr["toStatus"] != DBNull.Value ? dr["toStatus"].ToString() : String.Empty,
                    StatusOld = dr["fromStatus"] != DBNull.Value ? dr["fromStatus"].ToString() : String.Empty
                }).ToList();

            return fundHistoryModel;
        }

        public List<BankDepositModel> GetFundRequests()
        {
            Connection();
            List<BankDepositModel> bankDepositList = new List<BankDepositModel>();
            SqlCommand com = new SqlCommand("GetFundRequests", _con);
            com.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();

            try
            {
                _logger.Info("Attempting to get list of fundings from database");
                _con.Open();
                da.Fill(dt);
                _con.Close();
                _logger.Info("List of fundings successfully obtained from database");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }


            bankDepositList = (from DataRow dr in dt.Rows
                               select new BankDepositModel()
                               {
                                   DepositId = Convert.ToInt32(dr["fndId"]),
                                   Status = Convert.ToString(dr["fndStatus"]),
                                   BankId = Convert.ToInt32(dr["bankId"]),
                                   Bank = Convert.ToString(dr["bankName"]),
                                   BankCurrencyId = Convert.ToInt32(dr["ccyId"]),
                                   CurrencyName = Convert.ToString(dr["ccyCode"]),
                                   CreatedDate = Convert.ToDateTime(dr["paymentDate"]),
                                   Amount = Convert.ToDecimal(dr["fndAmt"]),
                                   ProofDocId = (dr["proofDocId"] != DBNull.Value) ? Convert.ToInt32(dr["proofDocId"]) : -1,
                                   ClientSendingBankName = dr["sendingBank"].ToString(),
                                   ClientAccountNumber = dr["lastFourDigits"].ToString(),
                                   ClientOtherInfo = (dr["other"] != DBNull.Value) ? dr["other"].ToString():"",
                                   PathToFile = (dr["filename"] != DBNull.Value) ? dr["filename"].ToString() : "",
                                   Notes = (dr["notes"] != DBNull.Value) ? dr["notes"].ToString() : "",
                                   CustomerName = (dr["custName"] != DBNull.Value) ? dr["custName"].ToString() : ""
                               }).OrderByDescending(ob=>ob.DepositId).ToList();

            return bankDepositList;
        }

        public bool DeleteBankDeposit(int depositId)
        {
            Connection();
            SqlCommand com = new SqlCommand("DeleteBankDeposit", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@depositId", depositId);

            try
            {
                _logger.Info("Attempting to delete funding from database. Funding Id=" + depositId);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    _logger.Info("Funding is successfully deleted. Funding Id=" + depositId);
                    return true;
                }
                else
                {
                    _logger.Error("Could not delete funding. Funding Id=" + depositId);
                    return false;
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message);
            }

            return false;
        }

        public bool UpdateFundRequestStatus(string responsibleUser, BankDepositModel obj)
        {
            Connection();
            SqlCommand com = new SqlCommand("UpdateFundRequest", _con);

            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@fndId", obj.DepositId);
            com.Parameters.AddWithValue("@newStatus", obj.Status);
            com.Parameters.AddWithValue("@username", responsibleUser);
            com.Parameters.AddWithValue("@notes", obj.Notes);

            try
            {
                _logger.Info("Attempting to update status of fund request: " + obj);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    _logger.Info("Fund request status is updated: " + obj);
                    return true;
                }
                else
                {
                    _logger.Info("Could not update fund request status");
                    return false;
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message);
            }

            return false;
        }
    }
}