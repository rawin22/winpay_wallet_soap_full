using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace TSG.Dal
{
    public class ConnectionFactory
    {
        public static SqlConnection GetConnection()
        {
            var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ewalletDbCon"].ConnectionString);
            return connection;
        }
    }
}
