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
    public class ContactUsRepository
    {
        readonly log4net.ILog _logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SqlConnection _con;

        
        

        private void Connection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["ewalletDbCon"].ToString();
            _con = new SqlConnection(conStr);
        }

        public bool AddContactUs(ContactUsModel obj, out string res)
        {
            Connection();
            SqlCommand com = new SqlCommand("AddContactUsRec", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@DepartmentName", obj.DepartmentName);
            com.Parameters.AddWithValue("@DepartmentMail", obj.DepartmentMail);
            com.Parameters.AddWithValue("@DepartmentWeight", obj.DepartmentWeight);
            res = String.Empty;
            try
            {
                _logger.Info("Attempting to add contact us info to database: " + obj);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
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

        public List<ContactUsModel> GetContactUsList()
        {
            Connection();
            List<ContactUsModel> contactUsList = new List<ContactUsModel>();
            SqlCommand com = new SqlCommand("GetContactUsList", _con);
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


            contactUsList = (from DataRow dr in dt.Rows
                             select new ContactUsModel()
                             {
                                 DepartmentId = Convert.ToInt32(dr["ID"]),
                                 DepartmentName = Convert.ToString(dr["DepartmentName"]),
                                 DepartmentMail = Convert.ToString(dr["DepartmentMail"]),
                                 DepartmentWeight = Convert.ToInt32(dr["DepartmentWeight"])
                             }).OrderBy(ob=>ob.DepartmentName).ToList();

            return contactUsList;
        }

        public bool UpdateContactUs(ContactUsModel obj, out string res)
        {
            Connection();
            SqlCommand com = new SqlCommand("UpdateContactUsRec", _con);
            res = String.Empty;
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@cuID", obj.DepartmentId);
            com.Parameters.AddWithValue("@cuName", obj.DepartmentName);
            com.Parameters.AddWithValue("@cuMail", obj.DepartmentMail);
            com.Parameters.AddWithValue("@cuWeight", obj.DepartmentWeight);

            try
            {
                _logger.Info("Attempting to update contact us info: " + obj);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    res = GlobalRes.ContactUs_ContactUsRepository_UpdateContactUs_Success;
                    _logger.Info("Contact us info is succesifully updated in database: " + obj);
                    return true;
                }
                else
                {
                    res = GlobalRes.ContactUs_ContactUsRepository_UpdateContactUs_UnSuccess;
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

        public bool UpdateSystemRequirement(String name, string value, out string res)
        {
            Connection();
            SqlCommand com = new SqlCommand("UpdateSystemRequirement", _con);
            res = String.Empty;
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@systemName", name);
            com.Parameters.AddWithValue("@systemValue", value);

            try
            {
                _logger.Info("Attempting to system requirement info: " + name);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    res = GlobalRes.ContactUs_ContactUsRepository_UpdateContactUs_Success;
                    _logger.Info("System requirement info is succesifully updated in database: " + name);
                    return true;
                }
                else
                {
                    res = GlobalRes.ContactUs_ContactUsRepository_UpdateContactUs_UnSuccess;
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

        public bool DeleteContactUs(int cuId, out string res)
        {
            Connection();
            SqlCommand com = new SqlCommand("DeleteContactUsRec", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@ContactUsId", cuId);
            res = String.Empty;

            try
            {
                _logger.Info("Attempting to delete contact us info from database. Contact us Id=" + cuId);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    res = GlobalRes.ContactUs_ContactUsRepository_DeleteContactUs_Success;
                    _logger.Info("Contact us info is successfully deleted. Contact us Id=" + cuId);
                    return true;
                }
                else
                {
                    res = GlobalRes.ContactUs_ContactUsRepository_DeleteContactUsUnSuccess;
                    _logger.Error("Could not delete contact us information. Contact us Id=" + cuId);
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


        public bool RegisterMail(int depId, string mailFrom, string mailTo, string mailSubject, string text,
            HttpPostedFileBase attachments, string siteCategory, out string res, out int mailId, string mailCopy = "")
        {
            byte[] file = new byte[0];

            if (attachments != null && attachments.ContentLength > 0)
            {
                using (var stream = attachments.InputStream)
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        file = reader.ReadBytes((int)stream.Length);
                    }
                }
            }

            return RegisterMail(depId, mailFrom, mailTo, mailSubject, text, file, siteCategory, out res, out mailId, mailCopy);
        }

        public bool RegisterMail(int depId, string mailFrom, string mailTo, string mailSubject, string text,
            byte[] attachments, string siteCategory, out string res, out int mailId, string mailCopy = "")
        {
            Connection();
            bool checkMailFrom = IsValid(mailFrom);
            bool checkMailTo = IsValid(mailTo);
            mailId = 0; 
            res = String.Empty;
            
            if (checkMailFrom && checkMailTo)
            {
                SqlCommand com = new SqlCommand("AddSendedMail", _con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@DepID", depId);
                com.Parameters.AddWithValue("@MailFrom", mailFrom);
                com.Parameters.AddWithValue("@MailText", text);
                com.Parameters.AddWithValue("@MailSubject", mailSubject);
                com.Parameters.Add("@MailAttachment", SqlDbType.VarBinary, attachments.Length).Value = attachments;
                com.Parameters.AddWithValue("@SiteCategory", siteCategory);
                com.Parameters.AddWithValue("@MailId", DbType.Int32).Direction = ParameterDirection.Output;

                res = String.Empty;
                try
                {
                    _logger.Info("Attempting to add sended mail info to database");
                    _con.Open();
                    int i = com.ExecuteNonQuery();
                    _con.Close();
                    if (i >= 1)
                    {
                        mailId = Convert.ToInt32(com.Parameters["@MailId"].Value);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    res = ex.Message;
                    _logger.Error(ex.Message);
                    _con.Close();
                    return false;
                }
            }

            return false;
        }

        public bool RegisterMail(int depId, string mailFrom, string mailTo, string mailSubject, string text,
            string[] attachments, string siteCategory, out string res, out int mailId, string mailCopy = "")
        {
            mailId = -1;
            res = string.Empty;

            if (attachments != null && attachments.Length > 0)
            {
                byte[] files = {};

                foreach (var s in attachments)
                {
                    if (s!=null && !string.IsNullOrEmpty(s.Trim()) && File.Exists(s))
                    {
                        var file = File.ReadAllBytes(s);

                        byte[] ret = new byte[files.Length + file.Length];
                        Buffer.BlockCopy(files, 0, ret, 0, files.Length);
                        Buffer.BlockCopy(file, 0, ret, files.Length, file.Length);
                        files =  ret;
                    }
                }

                return RegisterMail(depId, mailFrom, mailTo, mailSubject, text, files, siteCategory, out res,
                    out mailId, mailCopy);
            }

            return false;
        }

        public bool RegistrationRequest(int mailId, out string number)
        {
            Connection();
            
            var com = new SqlCommand("GetTaskNumber", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@MailId", mailId);
            com.Parameters.AddWithValue("@Code", DbType.String).Direction = ParameterDirection.Output;
            number = String.Empty;
            
            try
            {
                _logger.Info("Attempting to add sended mail info to database");
                _con.Open();

                int m = com.ExecuteNonQuery();
                _con.Close();

                if (m >= 1)
                {
                    number = com.Parameters["@Code"].Value.ToString();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _con.Close();
            }
            return false;
        }

        //public bool SendMail(int depId, string mailFrom, string displayName, string mailTo, string mailSubject, string text,
        //HttpPostedFileBase attachments, string siteCategory, out string res, string mailCopy = "")
        //{
        //    res = String.Empty;
        //    try
        //    {
        //        byte[] file = new byte[0];
        //        bool checkMailFrom = IsValid(mailFrom);
        //        bool checkMailTo = IsValid(mailTo);
        //        if (checkMailFrom && checkMailTo)
        //        {
        //            var message =
        //                new MailMessage(
        //                    new MailAddress(mailFrom, displayName), new MailAddress(mailTo));
        //            message.Subject = mailSubject;
        //            message.Body = text;
        //            if (!String.IsNullOrEmpty(mailCopy.Trim()) && IsValid(mailCopy))
        //                message.CC.Add(new MailAddress(mailCopy));

        //            message.IsBodyHtml = true;
        //            if (attachments != null && attachments.ContentLength > 0)
        //            {
        //                using (var stream = attachments.InputStream)
        //                {
        //                    using (var reader = new BinaryReader(stream))
        //                    {
        //                        file = reader.ReadBytes((int) stream.Length);
        //                    }
        //                }

        //                MemoryStream ms = new MemoryStream(file);
        //                message.Attachments.Add(new Attachment(ms, attachments.FileName,
        //                    attachments.ContentType));
        //            }

        //            _smtp.Send(message);
        //            res = "ok";
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res = ex.Message;
        //    }

        //    return false;
        //}

        public bool AddAnonimUserMail(UserMailModel obj, string userIp, string userBrowser, string userBwVersion, bool isMobile, out string res)
        {
            Connection();
            SqlCommand com = new SqlCommand("AddAnonimMailRec", _con);
            com.CommandType = CommandType.StoredProcedure;

            /*
              @MailFrom nvarchar(100),
		      @MailSubject nvarchar(500)='',
		      @MailNotes nvarchar(1000),
		      @UserIpInRequest nvarchar(100)='',
		      @BrowserName nvarchar(100)='',
		      @BrowserVersion nvarchar(50)='',
		      @IsMobile bit = null
            */
            com.Parameters.AddWithValue("@MailFrom", obj.MailFrom);
            com.Parameters.AddWithValue("@MailSubject", obj.Subject);
            com.Parameters.AddWithValue("@MailNotes", obj.MailText);

            com.Parameters.AddWithValue("@UserIpInRequest", userIp);
            com.Parameters.AddWithValue("@BrowserName", userBrowser);
            com.Parameters.AddWithValue("@BrowserVersion", userBwVersion);
            com.Parameters.AddWithValue("@IsMobile", isMobile);


            res = String.Empty;
            try
            {
                _logger.Info("Attempting to add contact us info to database: " + obj);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
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

        public bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}