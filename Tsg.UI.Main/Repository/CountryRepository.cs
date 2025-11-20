using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;

namespace Tsg.UI.Main.Repository
{
    public class CountryRepository
    {
        readonly log4net.ILog _logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SqlConnection _con;

        private void Connection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["ewalletDbCon"].ToString();
            _con = new SqlConnection(conStr);
        }
        public List<CountryModel> GetCountryList()
        {
            Connection();
            List<CountryModel> countryList = new List<CountryModel>();
            SqlCommand com = new SqlCommand("GetCountries", _con);
            com.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();

            try
            {
                _logger.Info("Attempting to get list of contact us from database");
                _con.Open();
                da.Fill(dt);
                _con.Close();
                _logger.Info("List of Contacts Us successfully obtained from database");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }


            countryList = (from DataRow dr in dt.Rows
                             select new CountryModel()
                             {
                                 CountryId = Convert.ToInt32(dr["id"]),
                                 CountryName = dr["name"].ToString()
                             }).OrderBy(ob=>ob.CountryId).ToList();

            return countryList;
        }

    }
}