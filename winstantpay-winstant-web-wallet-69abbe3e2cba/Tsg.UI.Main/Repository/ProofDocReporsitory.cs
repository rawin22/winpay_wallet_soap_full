using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using Tsg.UI.Main.Models;
using TSG.Models.Enums;

namespace Tsg.UI.Main.Repository
{
    public class ProofDocRepository
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private SqlConnection _con;

        private void Connection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["ewalletDbCon"].ToString();
            _con = new SqlConnection(conStr);
        }

        public bool UploadProofDoc(ProofDocModel obj)
        {
            Connection();
            SqlCommand com = new SqlCommand("AddProofDoc", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@custName", obj.CustomerName);
            com.Parameters.AddWithValue("@bankName", obj.BankName);
            com.Parameters.AddWithValue("@lastFourDigits", obj.LastFourDigits);
            com.Parameters.AddWithValue("@other", obj.OtherInfo ?? String.Empty);
            com.Parameters.AddWithValue("@fileName", obj.ProofDocName ?? String.Empty);
            com.Parameters.AddWithValue("@filePath", obj.ProofDocPath ?? String.Empty);
            com.Parameters.AddWithValue("@bankCcyId", obj.BankCurrencyId);
            com.Parameters.AddWithValue("@paymentDate", obj.PaymentDate);
            com.Parameters.AddWithValue("@amount", obj.Amount);
            com.Parameters.AddWithValue("@depositId", obj.DepositId);
            com.Parameters.AddWithValue("@proofDocId", obj.ProofDocId ?? -1);
            com.Parameters.AddWithValue("@retproofDocId", DbType.Int32).Direction = ParameterDirection.Output;

            try
            {
                _logger.Info("Attempting to add proof of document info into database: " + obj);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    obj.ProofDocId = Convert.ToInt32(com.Parameters["@retproofDocId"].Value);
                    _logger.Info("Successfully added proof of document into database: " + obj);

                    return true;
                }
                else
                {
                    _logger.Error("Could not add proof of document into database");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return false;   

        }

        public string GetDocumentName(int docId)
        {
            Connection();
            SqlCommand com = new SqlCommand("GetProofDocName", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@docId", docId);
            SqlParameter returnValue = com.Parameters.Add("@docName", SqlDbType.VarChar);
            returnValue.Direction = ParameterDirection.ReturnValue;
            try
            {
                _logger.Info("Attempting to get name of proof document from database. Document Id=" + docId);
                _con.Open();
                com.ExecuteNonQuery();
                _con.Close();
                _logger.Info("Successfully obtained the name of proof document. Document Id=" + docId);
                return returnValue.Value.ToString();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return null;
        }

        public ProofDocModel GetProofDocById(int id)
        {
            Connection();
            ProofDocModel currentDoc = new ProofDocModel();
            SqlCommand com = new SqlCommand("GetProofDocById", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@ID", id);
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


           var proofDocModelList = (from DataRow dr in dt.Rows
                select new ProofDocModel()
                {
                    ProofDocId = Convert.ToInt32(dr["proofDocId"]),
                    CustomerName = dr["custName"].ToString(),
                    BankName = dr["bankName"].ToString(),
                    LastFourDigits = dr["lastFourDigits"].ToString(),
                    OtherInfo = dr["other"].ToString(),
                    ProofDocName = dr["fileName"].ToString(),
                    ProofDocPath = dr["filePath"].ToString(),
                    BankCurrencyId = Convert.ToInt32(dr["bankCcyId"]),
                    PaymentDate = Convert.ToDateTime(dr["paymentDate"]),
                    Amount = Convert.ToDecimal(dr["amount"]),
                    FundingStatus = dr["fndStatus"].ToString(),
                    PaymentDateString = Convert.ToDateTime(dr["paymentDate"]).ToString("dd/MM/yyyy"),
                    FundingEnumStatus = (dr["fndStatus"] != DBNull.Value) ? (dr["fndStatus"].ToString() == "Canceled" ? FundingStatus.Canceled : (dr["fndStatus"].ToString() == "Credited") ? FundingStatus.Credited : (dr["fndStatus"].ToString() == "Pending") ? FundingStatus.Pending : FundingStatus.InProcess) : FundingStatus.Pending
                }).ToList();
          
            return proofDocModelList.FirstOrDefault();
        }

        public bool DeleteProofDoc(int docId)
        {
            Connection();
            SqlCommand com = new SqlCommand("DeleteProofDoc", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@proofDocId", docId);

            try
            {
                _logger.Info("Attempting to delete proof document from database. Document Id=" + docId);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    _logger.Info("Proof document is successfully deleted from database. Document Id=" + docId);
                    return true;
                }
                else
                {
                    _logger.Error("Could not delete proof document from database");
                    return false;
                }
            }catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return false;
        }
    }
}

