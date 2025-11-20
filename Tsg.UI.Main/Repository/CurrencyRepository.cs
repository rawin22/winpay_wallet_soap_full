using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tsg.UI.Main.Models;

namespace Tsg.UI.Main.Repository
{
    public class CurrencyRepository
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private SqlConnection _con;

        private void Connection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["ewalletDbCon"].ToString();
            _con = new SqlConnection(conStr);
        }

        public bool AddCurrency(CurrencyViewModel obj, out int ccyId)
        {
            Connection();
            SqlCommand com = new SqlCommand("AddCurrency", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@ccyCode", obj.CurrencyCode);
            com.Parameters.AddWithValue("@ccyName", obj.CurrencyName);
            com.Parameters.AddWithValue("@ccySymbol", obj.Symbol);
            com.Parameters.AddWithValue("@ccyOutId", DbType.Int32).Direction = ParameterDirection.Output;
            ccyId = 0;
            try
            {
                _logger.Info("Attempting to add currency info to database: " + obj);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    ccyId = Convert.ToInt32(com.Parameters["@ccyOutId"].Value);
                    _logger.Info("Successfully added currency info to database: " + obj);
                    return true;
                }
                else
                {
                    _logger.Error("Could not add currency info to database");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return false;
        }

        public List<CurrencyViewModel> GetCurrencies()
        {
            Connection();
            List<CurrencyViewModel> currencyList = new List<CurrencyViewModel>();
            SqlCommand com = new SqlCommand("GetCurrencies", _con);
            com.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();

            try
            {
                _logger.Info("Attempting to get list of currencies from database");
                _con.Open();
                da.Fill(dt);
                _con.Close();
                _logger.Info("List of currencies successfully obtained from database");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

            currencyList = (from DataRow dr in dt.Rows
                            select new CurrencyViewModel()
                            {
                                CurrencyId = Convert.ToInt32(dr["ccyId"]),
                                CurrencyName = Convert.ToString(dr["ccyName"]),
                                CurrencyCode = Convert.ToString(dr["ccyCode"]),
                                Symbol = Convert.ToString(dr["ccySymbol"])
                            }).ToList();

            return currencyList;
        }

        public List<BankCurrencyModel> GetBankCurrencies()
        {
            Connection();
            List<BankCurrencyModel> bankCurrencyList = new List<BankCurrencyModel>();
            SqlCommand com = new SqlCommand("GetBankCurrencies", _con);
            com.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();

            try
            {
                _logger.Info("Attempting to get list of bank and currencies from database");
                _con.Open();
                da.Fill(dt);
                _con.Close();
                _logger.Info("List of bank and currencies successfully obtained from database");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

            bankCurrencyList = (from DataRow dr in dt.Rows
                                select new BankCurrencyModel()
                                {
                                    BankCurrencyId = Convert.ToInt32(dr["bankCcyId"]),
                                    BankCurrencyName = Convert.ToString(dr["bankCcyName"])
                                }).ToList();

            return bankCurrencyList;
        }
    }
}