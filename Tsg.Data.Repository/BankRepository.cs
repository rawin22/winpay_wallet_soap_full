using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tsg.UI.Main.Models;

namespace Tsg.Data.Repository
{
    public class BankRepository
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private SqlConnection _con;

        private void Connection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["ewalletDbCon"].ToString();
            _con = new SqlConnection(conStr);
        }

        public bool AddBank(BankModel obj)
        {
            Connection();
            SqlCommand com = new SqlCommand("AddBank", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@bankName", obj.BankName);
            com.Parameters.AddWithValue("@bankCountry", obj.BankCountry);

            try
            {
                _logger.Debug("Attempting to add bank info to database: " + obj);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    _logger.Debug("Successfully added bank info to database: " + obj);
                    return true;
                }
                else
                {
                    _logger.Error("Could not add bank info to database");
                    return false;
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return false;
        }

        public List<BankModel> GetBanks()
        {
            Connection();
            SqlCommand com = new SqlCommand("GetBanks", _con);
            com.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();

            try
            {
                _logger.Debug("Attempting to get list of banks from database");
                _con.Open();
                da.Fill(dt);
                _con.Close();
                _logger.Debug("List of banks successfully obtained from database");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }


            var bankList = (from DataRow dr in dt.Rows
                select new BankModel()
                {
                    BankId = Convert.ToInt32(dr["bankId"]),
                    BankName = Convert.ToString(dr["bankName"]),
                    BankCountry = Convert.ToString(dr["bankCountry"])
                }).ToList();

            return bankList;
        }

        public bool UpdateBank(BankModel obj)
        {
            Connection();
            SqlCommand com = new SqlCommand("UpdateBank", _con);

            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@bankId", obj.BankId);
            com.Parameters.AddWithValue("@bankName", obj.BankName);
            com.Parameters.AddWithValue("@bankCountry", obj.BankCountry);

            try
            {
                _logger.Debug("Attempting to update bank info: " + obj);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    _logger.Debug("Bank info is updated in database: " + obj);
                    return true;
                }
                else
                {
                    _logger.Error("Bank info could not be updated");
                    return false;
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return false;
        }

        public bool DeleteBank(int bankId)
        {
            Connection();
            SqlCommand com = new SqlCommand("DeleteBank", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@bankId", bankId);

            try
            {
                _logger.Debug("Attempting to delete bank info from database. Bank Id="+bankId);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    _logger.Debug("Bank info is successfully deleted. Bank Id=" + bankId);
                    return true;
                }
                else
                {
                    _logger.Error("Could not delete bank information. Bank Id=" + bankId);
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

