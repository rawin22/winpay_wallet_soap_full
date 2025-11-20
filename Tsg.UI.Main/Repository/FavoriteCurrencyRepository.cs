using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;

namespace Tsg.UI.Main.Repository
{
    public class FavoriteCurrencyRepository
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private SqlConnection _con;

        private void Connection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["ewalletDbCon"].ToString();
            _con = new SqlConnection(conStr);
        }

        public bool AddFavoriteCurrency(FavoriteCurrencyModel obj, out string res)
        {
            Connection();
            SqlCommand com = new SqlCommand("AddFavoritesCurrency", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@CurrencyCode", obj.CurrencyCode);
            com.Parameters.AddWithValue("@IDUser", obj.IdUser);
            res = String.Empty;
            try
            {
                _logger.Info("Attempting to add favorite currency info to database: " + obj);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    res = GlobalRes.FavoriteCurrency_FavoriteCurrencyRepository_AddFavoriteCurrencySuccess;
                    _logger.Info("Successfully added record info to database: " + obj);
                    return true;
                }
                else
                {
                    res = GlobalRes.FavoriteCurrency_FavoriteCurrencyRepository_AddFavoriteCurrency_UnSuccess;
                    _logger.Error(GlobalRes.FavoriteCurrency_FavoriteCurrencyRepository_AddFavoriteCurrency_UnSuccess);
                    return false;
                }
            }
            catch (Exception ex)
            {
                res = ex.Message;
                _logger.Error(ex.Message);
                _con.Close();
            }
            return false;
        }

        public List<FavoriteCurrencyModel> GetFavoriteCurrencyList()
        {
            Connection();
            List<FavoriteCurrencyModel> favoriteCurrencyList = new List<FavoriteCurrencyModel>();
            SqlCommand com = new SqlCommand("GetFavoriteCurrencyList", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@IDUser", AppSecurity.CurrentUser.UserId);
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();

            try
            {
                _logger.Info("Attempting to get list of banks from database");
                _con.Open();
                da.Fill(dt);
                _con.Close();
                _logger.Info("List of banks successfully obtained from database");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }


            favoriteCurrencyList = (from DataRow dr in dt.Rows
                select new FavoriteCurrencyModel()
                {
                    IdUser = dr["IDUser"].ToString(),
                    CurrencyCode = dr["CurrencyCode"].ToString()
                }).ToList();

            return favoriteCurrencyList;
        }
        public List<FavoriteCurrencyModel> GetFavoriteCurrencyList(string userId)
        {
            Connection();
            List<FavoriteCurrencyModel> favoriteCurrencyList = new List<FavoriteCurrencyModel>();
            SqlCommand com = new SqlCommand("GetFavoriteCurrencyList", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@IDUser", userId);
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();

            try
            {
                _logger.Info("Attempting to get list of banks from database");
                _con.Open();
                da.Fill(dt);
                _con.Close();
                _logger.Info("List of banks successfully obtained from database");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }


            favoriteCurrencyList = (from DataRow dr in dt.Rows
                select new FavoriteCurrencyModel()
                {
                    IdUser = dr["IDUser"].ToString(),
                    CurrencyCode = dr["CurrencyCode"].ToString()
                }).ToList();

            return favoriteCurrencyList;
        }

        

        public bool DeleteFavoriteCurrency(string currencyCode, string userId, out string res)
        {
            Connection();
            SqlCommand com = new SqlCommand("DeleteFavoriteCurrency", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@CurrencyCode", currencyCode);
            com.Parameters.AddWithValue("@IDUser", userId);
            res = String.Empty;
            
            try
            {
                _logger.Info("Attempting to delete favorite rec info from database. Currency code = " + currencyCode + "| User Id =" + userId);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    res = GlobalRes.FavoriteCurrency_FavoriteCurrencyRepository_DeleteFavoriteCurrency_Success;
                    _logger.Info("Contact us info is successfully deleted. Currency code = " + currencyCode + "| User Id =" + userId);
                    return true;
                }
                else
                {
                    res = GlobalRes.FavoriteCurrency_FavoriteCurrencyRepository_DeleteFavoriteCurrency_UnSuccess;
                    _logger.Error("Could not delete favorite currency rec information. Currency code = " + currencyCode + "| User Id ="+ userId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                res = ex.Message;
                _logger.Error(ex.Message);
            }
            return false;
        }
    }
}