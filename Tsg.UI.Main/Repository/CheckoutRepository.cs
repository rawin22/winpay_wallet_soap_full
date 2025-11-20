using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Tsg.UI.Main.Repository
{
    public class CheckoutRepository
    {
        private SqlConnection _con;

        private void Connection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["ewalletDbCon"].ToString();
            _con = new SqlConnection(conStr);
        }
    }
}