using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tsg.UI.Main.Models;

namespace Tsg.UI.Main.Repository
{
    public class WireInstructionRepository
    {
        private SqlConnection _con;

        private void Connection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["ewalletDbCon"].ToString();
            _con = new SqlConnection(conStr);
        }

        public bool AddWireInstruction(WireInstructionModel obj)
        {
            Connection();
            SqlCommand com = new SqlCommand("AddWireInstruction", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@wireInstText", obj.WireInstruction);
            com.Parameters.AddWithValue("@bankCcyId", obj.BankCurrencyId);

            _con.Open();
            int i = com.ExecuteNonQuery();
            _con.Close();
            if (i >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<WireInstructionModel> GetWireInstructions()
        {
            Connection();
            List<WireInstructionModel> wireInstructionList = new List<WireInstructionModel>();
            SqlCommand com = new SqlCommand("GetWireInstructions", _con);
            com.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();
            _con.Open();
            da.Fill(dt);
            _con.Close();

            wireInstructionList = (from DataRow dr in dt.Rows
                                  select new WireInstructionModel()
                                  {
                                      WireInstructionId = Convert.ToInt32(dr["wireInstId"]),
                                      WireInstruction = Convert.ToString(dr["wireInstText"]),
                                      BankCurrencyName=Convert.ToString(dr["bankCcyName"])
                                  }).ToList();

            return wireInstructionList;
        }

        public string GetWireInstruction(int bankCcyId)
        {
            Connection();
            SqlCommand com = new SqlCommand("GetWireInstruction", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@bankCcyId", bankCcyId);
            SqlParameter returnValue = com.Parameters.Add("@wireInstId", SqlDbType.VarChar);
            returnValue.Direction = ParameterDirection.ReturnValue;

            _con.Open();
            com.ExecuteNonQuery();
            _con.Close();
            return returnValue.Value.ToString();
        }

        public bool UpdateWireInstruction(WireInstructionModel obj)
        {
            Connection();
            SqlCommand com = new SqlCommand("UpdateWireInstruction", _con);

            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@wireInstId", obj.WireInstructionId);
            com.Parameters.AddWithValue("@wireInstText", obj.WireInstruction);
            _con.Open();
            int i = com.ExecuteNonQuery();
            _con.Close();
            if (i >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteWireInstruction(int id)
        {
            Connection();
            SqlCommand com = new SqlCommand("DeleteWireInstruction", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@wireInstId", id);

            _con.Open();
            int i = com.ExecuteNonQuery();
            _con.Close();
            if (i >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}