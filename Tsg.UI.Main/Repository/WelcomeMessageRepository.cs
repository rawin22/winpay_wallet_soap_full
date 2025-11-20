using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tsg.UI.Main.Models;

namespace Tsg.UI.Main.Repository
{
    public class WelcomeMessageRepository
    {
        private SqlConnection _con;
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private void Connection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["ewalletDbCon"].ToString();
            _con = new SqlConnection(conStr);
        }

        public bool AddWelcomeMessage(WelcomeMessageModel obj)
        {

            Connection();
            SqlCommand com = new SqlCommand("AddWelcomeMessage", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@wlcMsgText", obj.WelcomeMessage);
            com.Parameters.AddWithValue("@wlcMsgTextRu", obj.WelcomeMessageRu);
            com.Parameters.AddWithValue("@wlcMsgTextFr", obj.WelcomeMessageFr);
            com.Parameters.AddWithValue("@wlcMsgTextPh", obj.WelcomeMessagePh);
            com.Parameters.AddWithValue("@wlcMsgTextTh", obj.WelcomeMessageTh);
            com.Parameters.AddWithValue("@wlcMsgTextAe", obj.WelcomeMessageAe);
            com.Parameters.AddWithValue("@wlcMsgTextKh", obj.WelcomeMessageKh);
            com.Parameters.AddWithValue("@wlcMsgTextCn", obj.WelcomeMessageCn);

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
        /// <summary>
        /// Method for admins to disable or enable statuses
        /// </summary>
        /// <param name="statuses"></param>
        /// <returns></returns>
        public bool ChangeWmStatusesForAllUsers(bool statuses)
        {
            var result = false;
            try
            {
                Connection();
                SqlCommand com = new SqlCommand("ChangeWMStatuses", _con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@userId", DBNull.Value);
                com.Parameters.AddWithValue("@status", statuses);

                try
                {
                    _logger.Info($"Welcome messages for " + (statuses ? "activation" : "deactivation") + " begin");
                    _con.Open();
                    int res = com.ExecuteNonQuery();
                    _con.Close();
                    _logger.Info($"Welcome messages for " + (statuses ? "activated" : "deactivated") + (res > 0 ? "success" : "unsuccess"));
                    result = res >= 0;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                }

                result = true;
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            return result;
        }


        public List<WelcomeMessageModel> GetWelcomeMessages()
        {
            Connection();
            List<WelcomeMessageModel> welcomeMessageList = new List<WelcomeMessageModel>();
            SqlCommand com = new SqlCommand("GetWelcomeMessages", _con);
            com.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();
            _con.Open();
            da.Fill(dt);
            _con.Close();

            //Bind WelcomeMessageViewModel generic list using LINQ 
            welcomeMessageList = (from DataRow dr in dt.Rows
                       select new WelcomeMessageModel()
                       {
                           WelcomeMessageId = Convert.ToInt32(dr["wlcMsgId"]),
                           WelcomeMessage = Convert.ToString(dr["wlcMsgText"]),
                           WelcomeMessageFr = Convert.ToString(dr["wlcMsgTextFr"]),
                           WelcomeMessageRu = Convert.ToString(dr["wlcMsgTextRu"]),
                           WelcomeMessagePh = Convert.ToString(dr["wlcMsgTextPh"]),
                           WelcomeMessageAe = Convert.ToString(dr["wlcMsgTextAe"]),
                           WelcomeMessageTh = Convert.ToString(dr["wlcMsgTextTh"]),
                           WelcomeMessageKh = Convert.ToString(dr["wlcMsgTextKh"]),
                           WelcomeMessageCn = Convert.ToString(dr["wlcMsgTextCn"]),
                           IsDefault = Convert.ToBoolean(dr["isDefault"])
                       }).ToList();

            return welcomeMessageList;
        }

        public bool UpdateWelcomeMessage(WelcomeMessageModel obj)
        {
            Connection();
            SqlCommand com = new SqlCommand("UpdateWelcomeMessage", _con);

            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@wlcMsgId", obj.WelcomeMessageId);
            com.Parameters.AddWithValue("@wlcMsgText", obj.WelcomeMessage);
            com.Parameters.AddWithValue("@wlcMsgTextRu", obj.WelcomeMessageRu);
            com.Parameters.AddWithValue("@wlcMsgTextFr", obj.WelcomeMessageFr);
            com.Parameters.AddWithValue("@wlcMsgTextPh", obj.WelcomeMessagePh);
            com.Parameters.AddWithValue("@wlcMsgTextAe", obj.WelcomeMessageAe);
            com.Parameters.AddWithValue("@wlcMsgTextTh", obj.WelcomeMessageTh);
            com.Parameters.AddWithValue("@wlcMsgTextKh", obj.WelcomeMessageKh);
            com.Parameters.AddWithValue("@wlcMsgTextCn", obj.WelcomeMessageCn);
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

        public bool DeleteWelcomeMessage(int id)
        {
            Connection();
            SqlCommand com = new SqlCommand("DeleteWelcomeMessage", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@wlcMsgId", id);

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

