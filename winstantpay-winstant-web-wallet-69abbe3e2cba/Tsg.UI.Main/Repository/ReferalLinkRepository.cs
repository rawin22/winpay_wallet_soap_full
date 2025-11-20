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
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;

namespace Tsg.UI.Main.Repository
{
    public class ReferalLinkRepository
    {
        readonly log4net.ILog _logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SqlConnection _con;

        private void Connection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["ewalletDbCon"].ToString();
            _con = new SqlConnection(conStr);
        }
        public List<ReferalLinks> GetReferalLinksList()
        {
            Connection();
            List<ReferalLinks> referalLinksList = new List<ReferalLinks>();
            SqlCommand com = new SqlCommand("GetReferalLinksList", _con);
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
            referalLinksList = (from DataRow dr in dt.Rows
                             select new ReferalLinks()
                             {
                                 Id = Convert.ToInt32(dr["ID"]),
                                 IsActive = dr["IsActive"] != DBNull.Value && Convert.ToBoolean(dr["IsActive"]),
                                 IsDefault = Convert.ToBoolean(dr["IsDefault"]),
                                 LinkText = dr["LinkText"].ToString(),
                                 ExparedDate = Convert.ToDateTime(dr["ExpiredDate"])
                             }).ToList();

            return referalLinksList;
        }

        public MessageInfoModel AddOrUpdateReferalLink(ReferalLinks model)
        {
            Connection();
            var res = new MessageInfoModel(){IsModal = false, IsSuccess = false, MessageTitle = "", MessageText = ""};
            try
            {
                SqlCommand com = new SqlCommand("AddOrUpdateReferalLink", _con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@refLinkId", model.Id);
                com.Parameters.AddWithValue("@refLinkText", model.LinkText);
                com.Parameters.AddWithValue("@refLinkOutId", DbType.Int32).Direction = ParameterDirection.Output;
                
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    model.Id = Convert.ToInt32(com.Parameters["@refLinkOutId"].Value);
                    _logger.Info("Successfully added currency info to database: " + model);
                }
                else
                {
                    _logger.Error("Could not add currency info to database");
                }
                res.IsSuccess = true;
                res.MessageText = GlobalRes.ReferalLink_ReferalLinkRepository_Success;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                res.IsSuccess = false;
                res.MessageText = e.Message;
            }


            return res;
        }


    }
}