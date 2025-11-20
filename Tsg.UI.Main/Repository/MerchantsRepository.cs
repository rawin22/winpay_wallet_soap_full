using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Merchants;

namespace Tsg.UI.Main.Repository
{
    public class MerchantsRepository
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private SqlConnection _con;
        private void Connection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["ewalletDbCon"].ToString();
            _con = new SqlConnection(conStr);
        }

        public List<MerchantModel> GetAllLocalMerchants()
        {
            var merchantsForUser = new List<MerchantModel>();
            Connection();
            
            SqlCommand com = new SqlCommand("SELECT * FROM dbo.Merchants", _con);
            com.CommandType = CommandType.Text;
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
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
            string merchantApi = ConfigurationManager.AppSettings["merchantApi"];
            merchantsForUser = (from DataRow dr in dt.Rows
                                select new MerchantModel()
                                {
                                    Id = Convert.ToInt32(dr["ID"]),
                                    MerchantGuid = Convert.ToString(dr["UniqueID"]),
                                    MerchantName = Convert.ToString(dr["Name"]),
                                    MerchantAddress = dr["Address"] != DBNull.Value ? Convert.ToString(dr["Address"]) : String.Empty,
                                    MerchantPhone = dr["Phone"] != DBNull.Value ? Convert.ToString(dr["Phone"]) : String.Empty,
                                    MerchantUserId = dr["UserId"] != DBNull.Value ? Convert.ToString(dr["UserId"]) : String.Empty,
                                    MerchantUserGuid = dr["UserGuid"] != DBNull.Value ? Convert.ToString(dr["UserGuid"]) : String.Empty,
                                    MerchantWebSiteCallBackAddress = dr["CallBackAddress"] != DBNull.Value ? Convert.ToString(dr["CallBackAddress"]) : String.Empty,
                                    MerchantIsSandbox = dr["IsSandBox"] != DBNull.Value && Convert.ToBoolean(dr["IsSandBox"]),
                                    MerchantPublicTokenKey = dr["PublicTokenKey"] != DBNull.Value ? Convert.ToString(dr["PublicTokenKey"]) : String.Empty,
                                    MerchantPrivateTokenKey = dr["PrivateToken"] != DBNull.Value ? Convert.ToString(dr["PrivateToken"]) : String.Empty,
                                    MerchantAlias = dr["MerchantAlias"] != DBNull.Value ? Convert.ToString(dr["MerchantAlias"]) : String.Empty,
                                    MerchantApi = merchantApi
                                }).ToList();

            return merchantsForUser;
        }


        public List<MerchantModel> GetMerchantsForUser(string userId, string userGuid = "")
        {
            var merchantsForUser = new List<MerchantModel>();
            Connection();
            SqlCommand com = new SqlCommand("GetListOfMerchants", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@UserId", userId);
            com.Parameters.AddWithValue("@UserGuid", userGuid);
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();
            try
            {
                _logger.Info("Attempting to get list of banks from database");
                _con.Open();
                da.Fill(dt);
                _con.Close();
                string merchantApi = ConfigurationManager.AppSettings["merchantApi"];
                string merchantJsPath = ConfigurationManager.AppSettings["merchantPathForJs"];
                merchantsForUser = (from DataRow dr in dt.Rows
                    select new MerchantModel()
                    {
                        Id = Convert.ToInt32(dr["ID"]),
                        MerchantGuid = Convert.ToString(dr["UniqueID"]),
                        MerchantName = Convert.ToString(dr["Name"]),
                        MerchantAddress = dr["Address"] != DBNull.Value ? Convert.ToString(dr["Address"]) : String.Empty,
                        MerchantPhone = dr["Phone"] != DBNull.Value ? Convert.ToString(dr["Phone"]) : String.Empty,
                        MerchantUserId = dr["UserId"] != DBNull.Value ? Convert.ToString(dr["UserId"]) : String.Empty,
                        MerchantUserGuid = dr["UserGuid"] != DBNull.Value ? Convert.ToString(dr["UserGuid"]) : String.Empty,
                        MerchantWebSiteCallBackAddress = dr["CallBackAddress"] != DBNull.Value ? Convert.ToString(dr["CallBackAddress"]) : String.Empty,
                        MerchantIsSandbox = dr["IsSandBox"] != DBNull.Value && Convert.ToBoolean(dr["IsSandBox"]),
                        MerchantPublicTokenKey = dr["PublicTokenKey"] != DBNull.Value ? Convert.ToString(dr["PublicTokenKey"]) : String.Empty,
                        MerchantPrivateTokenKey = dr["PrivateToken"] != DBNull.Value ? Convert.ToString(dr["PrivateToken"]) : String.Empty,
                        MerchantAlias = dr["MerchantAlias"] != DBNull.Value ? Convert.ToString(dr["MerchantAlias"]) : String.Empty,
                        MerchantApi = merchantApi,
                        MerchantPathForJs = merchantJsPath
                    }).ToList();
                _logger.Info("List of banks successfully obtained from database");
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
            
            return merchantsForUser;
        }

        public bool AddMerchants(MerchantModel obj, out string res)
        {
            Connection();
            SqlCommand com = new SqlCommand("AddMerchant", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@MerchantName", obj.MerchantName);
            com.Parameters.AddWithValue("@MerchantPhone", obj.MerchantPhone ?? String.Empty);
            com.Parameters.AddWithValue("@MerchantAddress", obj.MerchantAddress ?? String.Empty);
            com.Parameters.AddWithValue("@MerchantCallBackAddress", obj.MerchantWebSiteCallBackAddress ?? String.Empty);
            com.Parameters.AddWithValue("@UserId", obj.MerchantUserId);
            com.Parameters.AddWithValue("@UserGuid", obj.MerchantUserGuid);
            com.Parameters.AddWithValue("@IsSandingBox", obj.MerchantIsSandbox);
            com.Parameters.AddWithValue("@PublicTokenKey", obj.MerchantPublicTokenKey);
            com.Parameters.AddWithValue("@PrivateToken", obj.MerchantPrivateTokenKey);
            com.Parameters.AddWithValue("@MerchantAlias", obj.MerchantAlias);

            SqlParameter output = com.Parameters.Add("@ID", SqlDbType.Int);
            output.Direction = ParameterDirection.Output;
            res = String.Empty;
            try
            {
                _logger.Info("Attempting to add contact us info to database: " + obj);
                _con.Open();
                com.ExecuteNonQuery();
                _con.Close();
                if (!output.IsNullable && Convert.ToInt32(output.Value) > 0)
                {
                    res = GlobalRes.ContactUs_ContactUsRepository_AddContactUs_Successfull;
                    _logger.Info("Successfully added record info to database: " + obj);
                    return true;
                }
                else
                {
                    res = GlobalRes.ContactUs_ContactUsRepository_AddContactUs_UnSuccess;
                    _logger.Error(GlobalRes.ContactUs_ContactUsRepository_AddContactUs_UnSuccess);
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

        public bool UpdateMerchants(MerchantModel obj, out string res)
        {
            Connection();
            SqlCommand com = new SqlCommand("UpdateMerchantsRec", _con);
            res = String.Empty;
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@MerchantId", obj.Id);
            com.Parameters.AddWithValue("@MerchantGuid", new Guid(obj.MerchantGuid));
            com.Parameters.AddWithValue("@MerchantName", obj.MerchantName);
            com.Parameters.AddWithValue("@MerchantPhone", string.IsNullOrEmpty(obj.MerchantPhone) ? "" : obj.MerchantPhone);
            com.Parameters.AddWithValue("@MerchantAddress", string.IsNullOrEmpty(obj.MerchantAddress) ? "" : obj.MerchantAddress);
            com.Parameters.AddWithValue("@MerchantCallBackAddress", string.IsNullOrEmpty(obj.MerchantWebSiteCallBackAddress) ? "" : obj.MerchantWebSiteCallBackAddress);
            com.Parameters.AddWithValue("@IsSandingBox", obj.MerchantIsSandbox);
            com.Parameters.AddWithValue("@MerchantAlias", obj.MerchantAlias);

            try
            {
                _logger.Info("Attempting to update merchants: " + obj);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    res = GlobalRes.Merchants_MerchantsRepository_UpdateMerchant_Success;
                    _logger.Info("Contact us info is succesifully updated in database: " + obj);
                    return true;
                }
                else
                {
                    res = GlobalRes.Merchants_MerchantsRepository_UpdateMerchant_UnSuccess;
                    _logger.Error(GlobalRes.ContactUs_ContactUsRepository_UpdateContactUs_UnSuccess);
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
        public bool UpdateMerchantCallBackAddress(string merchantGuid, string callBackAddress, out string res)
        {
            Connection();
            SqlCommand com = new SqlCommand("UpdateMerchantCallBackAddress", _con);
            res = String.Empty;
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@MerchantGuid", new Guid(merchantGuid));
            com.Parameters.AddWithValue("@MerchantCallBackAddress", callBackAddress);

            try
            {
                _logger.Info("Attempting to update merchants callbackAddress: " + callBackAddress);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    res = GlobalRes.Merchants_MerchantsRepository_UpdateMerchant_Success;
                    _logger.Info("Contact us info is succesifully updated in database: " + callBackAddress);
                    return true;
                }
                else
                {
                    res = GlobalRes.Merchants_MerchantsRepository_UpdateMerchant_UnSuccess;
                    _logger.Error(GlobalRes.Merchants_MerchantsRepository_UpdateMerchant_UnSuccess);
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

        public bool ChechMerchantIfExist(string merchantGuid, out string res, string openTokenkey = "")
        {
            try
            {
                Connection();
                SqlCommand com = new SqlCommand("CheckMerchantIfExist", _con);
                com.CommandType = CommandType.StoredProcedure;
                res = String.Empty;

                if (!String.IsNullOrEmpty(openTokenkey.Trim()))
                    com.Parameters.AddWithValue("@merchantTokenKey", openTokenkey);
                else com.Parameters.AddWithValue("@merchantSecKey", merchantGuid);
                
                _logger.Info("Check merchant to exist [merchant guid]: " + merchantGuid);
                _con.Open();
                object i = com.ExecuteScalar();
                _con.Close();
                if ((int)i >= 1)
                {
                    res = GlobalRes.Merchants_MerchantsRepository_CheckMerchant_Success;
                    _logger.Info("Merchant checked [merchant guid]: " + merchantGuid);
                    return true;
                }
                else
                {
                    res = GlobalRes.Merchants_MerchantsRepository_CheckMerchant_UnSuccess;
                    _logger.Error(GlobalRes.Merchants_MerchantsRepository_CheckMerchant_UnSuccess + "[merchant guid]: " + merchantGuid);
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
        

        //public bool DeleteContactUs(int cuId, out string res)
        //{
        //    connection();
        //    SqlCommand com = new SqlCommand("DeleteContactUsRec", con);
        //    com.CommandType = CommandType.StoredProcedure;
        //    com.Parameters.AddWithValue("@ContactUsId", cuId);
        //    res = String.Empty;

        //    try
        //    {
        //        logger.Debug("Attempting to delete contact us info from database. Contact us Id=" + cuId);
        //        con.Open();
        //        int i = com.ExecuteNonQuery();
        //        con.Close();
        //        if (i >= 1)
        //        {
        //            res = GlobalRes.ContactUs_ContactUsRepository_DeleteContactUs_Success;
        //            logger.Debug("Contact us info is successfully deleted. Contact us Id=" + cuId);
        //            return true;
        //        }
        //        else
        //        {
        //            res = GlobalRes.ContactUs_ContactUsRepository_DeleteContactUsUnSuccess;
        //            logger.Error("Could not delete contact us information. Contact us Id=" + cuId);
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res = ex.Message;
        //        logger.Error(ex.Message);
        //    }
        //    return false;
        //}
    }
}