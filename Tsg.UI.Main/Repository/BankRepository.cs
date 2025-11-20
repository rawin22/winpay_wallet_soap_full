using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Models;

namespace Tsg.UI.Main.Repository
{
    public class BankRepository
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private SqlConnection _con;

        private void Connection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["ewalletDbCon"].ToString();
            _con = new SqlConnection(conStr);
        }

        public class AddNewBankRes
        {
            public bool IsSuccesifully { get; set; }
            public string Message{ get; set; }
            public int BankId { get; set; }
            public int BankCcyId { get; set; }
        }

        public bool AddBank(BankModel obj)
        {
            Connection();
            SqlCommand com = new SqlCommand("AddBank", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@bankName", obj.BankName);
            com.Parameters.AddWithValue("@bankCountry", obj.BankCountryId);
            com.Parameters.AddWithValue("@bankId", DbType.Int32).Direction = ParameterDirection.Output;
            int bankId = 0;
            try
            {
                _logger.Info("Attempting to add bank info to database: " + obj);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                bankId = Convert.ToInt32(com.Parameters["@bankId"].Value);
                if (bankId != 0)
                {
                    com.Parameters.Clear();
                    com = new SqlCommand("AddBankCurrency", _con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@bankId", bankId);
                    com.Parameters.AddWithValue("@ccyId", obj.CurrencyId);
                    
                    _con.Open();
                    i = com.ExecuteNonQuery();
                    _con.Close();
                }
                
                if (i >= 1)
                {                   
                    
                    _logger.Info("Successfully added bank info to database: " + obj);
                    return true;
                }
                else
                {
                    _logger.Error("Could not add bank info to database");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return false;
        }

        public AddNewBankRes AddBank(BankModel obj, out int bankId)
        {
            Connection();
            var res = new AddNewBankRes() { IsSuccesifully = false, BankId = obj.BankId, BankCcyId = obj.CurrencyId, Message = "" };
            SqlCommand com = new SqlCommand("AddBank", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@bankName", obj.BankName);
            com.Parameters.AddWithValue("@bankCountry", obj.BankCountryId);
            com.Parameters.AddWithValue("@bankId", DbType.Int32).Direction = ParameterDirection.Output;
            bankId = 0;
            try
            {
                _logger.Info("Attempting to add bank info to database: " + obj);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                bankId = Convert.ToInt32(com.Parameters["@bankId"].Value);
                res.BankId = bankId;
                if (i >= 1)
                {
                    res.IsSuccesifully = true;
                    res.Message = GlobalRes.Bank_BankRepository_AddBank_AddSuccess;
                    _logger.Info("Successfully added bank info to database: " + obj);
                    return res;
                }
                else
                {
                    res.IsSuccesifully = false;
                    res.Message = GlobalRes.Bank_BankRepository_AddBank_AddUnSuccess;
                    _logger.Error("Could not add bank info to database");
                    return res;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return res;
        }

        public AddNewBankRes AddBankCurrencies(BankModel obj, bool isDef=false)
        {
            Connection();
            var res = new AddNewBankRes(){IsSuccesifully = false, BankId = obj.BankId, BankCcyId = obj.CurrencyId, Message = ""};
            try
            {
                int i = 0;
                _logger.Info("Attempting to add bank info to database: " + obj);
                
                if (obj.BankId != 0)
                {
                    SqlCommand com = new SqlCommand("AddBankCurrencyGetId", _con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@bankId", obj.BankId);
                    com.Parameters.AddWithValue("@ccyId", obj.CurrencyId);
                    com.Parameters.AddWithValue("@bankCcyId", DbType.Int32).Direction = ParameterDirection.Output;

                    _con.Open();
                    i = com.ExecuteNonQuery();
                    _con.Close();
                    res.BankCcyId = Convert.ToInt32(com.Parameters["@bankCcyId"].Value); ;

                }

                if (i >= 1)
                {
                    res.IsSuccesifully = true;
                    res.Message = GlobalRes.Bank_BankRepository_AddBankCurrencies_AddSuccess;
                    _logger.Info("Successfully added bank info to database: " + obj);
                    return res;
                }
                else
                {
                    res.IsSuccesifully = true;
                    res.Message = GlobalRes.Bank_BankRepository_AddBankCurrencies_AddUnSuccess;
                    _logger.Error("Could not add bank info to database");
                    return res;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return res;
        }

        public List<BankModel> GetBanks()
        {
            Connection();
            List<BankModel> bankList = new List<BankModel>();
            SqlCommand com = new SqlCommand("GetBanks", _con);
            com.CommandType = CommandType.StoredProcedure;
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


            bankList = (from DataRow dr in dt.Rows
                        select new BankModel()
                        {
                            BankId = Convert.ToInt32(dr["bankId"]),
                            BankName = Convert.ToString(dr["bankName"]),
                            BankCountry = Convert.ToString(dr["bankCountry"]),
                            BankCountryId = Convert.ToInt32(dr["bankCountryId"]),
                            IsDeleted = !dr["IsDeleted"].Equals(DBNull.Value) && Convert.ToBoolean(dr["IsDeleted"])
                            // CurrencyId = Convert.ToInt32(dr["ccyId"]),
                            // CurrencyCode = Convert.ToString(dr["ccyCode"])
                        }).Where(w=>!w.IsDeleted).ToList();

            return bankList;
        }
        public List<CurrencyViewModel> GetCurrencies()
        {
            Connection();
            List<CurrencyViewModel> currencyList = new List<CurrencyViewModel>();
            SqlCommand com = new SqlCommand("GetCurrencies", _con);
            com.CommandType = CommandType.StoredProcedure;
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


            currencyList = (from DataRow dr in dt.Rows
                        select new CurrencyViewModel()
                {
                    CurrencyId = Convert.ToInt32(dr["ccyId"]),
                    CurrencyCode = Convert.ToString(dr["ccyCode"]),
                    CurrencyName = Convert.ToString(dr["ccyName"]),
                    Symbol = Convert.ToString(dr["ccySymbol"])
                }).ToList();

            return currencyList;
        }

        public List<BankAndInstructionModel> GetBanksAndWiredInstruction()
        {
            Connection();
            List<BankAndInstructionModel> bankAndInstructionList = new List<BankAndInstructionModel>();
            SqlCommand com = new SqlCommand("GetBankAndInstruction", _con);
            com.CommandType = CommandType.StoredProcedure;
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


            bankAndInstructionList = (from DataRow dr in dt.Rows
                select new BankAndInstructionModel()
                {
                    BankCcyId = Convert.ToInt32(dr["BcId"]),
                    BankId = Convert.ToInt32(dr["BankId"]),
                    BankCountry = Convert.ToString(dr["BankCountry"]),
                    BankName = Convert.ToString(dr["BankName"]),
                    CurrencyId = Convert.ToInt32(dr["CurrencyId"]),
                    CurrencyName = Convert.ToString(dr["CurrencyName"]),
                    WireInstructionId = dr["WireId"].Equals(DBNull.Value) ? (int?)null : Convert.ToInt32(dr["WireId"]),
                    WireInstructionText = dr["WireText"].Equals(DBNull.Value) ? "" : Convert.ToString(dr["WireText"])
                }).ToList();

            return bankAndInstructionList;
        }
        public BankAndInstructionModel GetBanksAndWiredInstructionById(int bccy)
        {
            Connection();
            List<BankAndInstructionModel> bankAndInstructionList = new List<BankAndInstructionModel>();
            SqlCommand com = new SqlCommand("GetBankAndInstructionById", _con);
            com.Parameters.AddWithValue("@bankCcyId", bccy);
            com.CommandType = CommandType.StoredProcedure;
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


            bankAndInstructionList = (from DataRow dr in dt.Rows
                select new BankAndInstructionModel()
                {
                    BankCcyId = Convert.ToInt32(dr["BcId"]),
                    BankId = Convert.ToInt32(dr["BankId"]),
                    BankCountry = Convert.ToString(dr["BankCountryName"]),
                    BankCountryId = Convert.ToInt32(dr["BankCountry"]),
                    BankName = Convert.ToString(dr["BankName"]),
                    CurrencyId = Convert.ToInt32(dr["CurrencyId"]),
                    CurrencyName = Convert.ToString(dr["CurrencyName"]),
                    WireInstructionId = dr["WireId"].Equals(DBNull.Value) ? (int?)null : Convert.ToInt32(dr["WireId"]),
                    WireInstructionText = dr["WireText"].Equals(DBNull.Value) ? "" : Convert.ToString(dr["WireText"])
                }).ToList();

            return bankAndInstructionList.Count>0 ? bankAndInstructionList.FirstOrDefault():new BankAndInstructionModel();
        }

        public bool UpdateBankCurrency(int bCcyId, int bankId, int ccyId, int wireInstId)
        {
            Connection();
            SqlCommand com = new SqlCommand("UpdateBankCurrency", _con);

            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@bankId", bankId);
            com.Parameters.AddWithValue("@bCcyId", bCcyId);
            com.Parameters.AddWithValue("@ccyId", ccyId);
            com.Parameters.AddWithValue("@wireInstId", wireInstId);

            try
            {
                _logger.Info("Attempting to update bank currency info: [id]" + bCcyId);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    _logger.Info("Bank currency info is updated in database: " + bCcyId);
                    return true;
                }
                else
                {
                    _logger.Error("Bank currency info could not be updated");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return false;
        }

        public bool UpdateBank(BankModel obj)
        {
            Connection();
            SqlCommand com = new SqlCommand("UpdateBank", _con);

            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@bankId", obj.BankId);
            com.Parameters.AddWithValue("@bankName", obj.BankName);
            com.Parameters.AddWithValue("@bankCountry", obj.BankCountryId);
            
            try
            {
                _logger.Info("Attempting to update bank info: " + obj);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    _logger.Info("Bank info is updated in database: " + obj);
                    return true;
                }
                else
                {
                    _logger.Error("Bank info could not be updated");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return false;
        }

        public bool DeleteBank(int bankId)
        {
            Connection();
            SqlCommand com = new SqlCommand("DeleteBank", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@bankId", bankId);

            try
            {
                _logger.Info("Attempting to delete bank info from database. Bank Id=" + bankId);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    _logger.Info("Bank info is successfully deleted. Bank Id=" + bankId);
                    return true;
                }
                else
                {
                    _logger.Error("Could not delete bank information. Bank Id=" + bankId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return false;
        }

        public bool UpdateCurrency(CurrencyViewModel obj)
        {
            Connection();
            SqlCommand com = new SqlCommand("UpdateCurrency", _con);

            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@ccyCode", obj.CurrencyCode);
            com.Parameters.AddWithValue("@ccyName", obj.CurrencyName);
            com.Parameters.AddWithValue("@ccySymbol", obj.Symbol);
            com.Parameters.AddWithValue("@currId", obj.CurrencyId);

            try
            {
                _logger.Info("Attempting to update currency info: " + obj);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    _logger.Info("Currency info is updated in database: " + obj);
                    return true;
                }
                else
                {
                    _logger.Error("Currency info could not be updated");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return false;
        }

        public bool DeleteCurrency(int bankId)
        {
            Connection();
            SqlCommand com = new SqlCommand("DeleteCurrency", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@currId", bankId);

            try
            {
                _logger.Info("Attempting to delete currency info from database. Currency Id=" + bankId);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    _logger.Info("Currency info is successfully deleted. Currency Id=" + bankId);
                    return true;
                }
                else
                {
                    _logger.Error("Could not delete currency information. Currency Id=" + bankId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return false;
        }

        public bool DeleteBankAndPayInstruction(int bankCurrencyId)
        {
            Connection();
            SqlCommand com = new SqlCommand("DeletePayInInstruction", _con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@bankCurrencyId", bankCurrencyId);

            try
            {
                _logger.Info("Attempting to delete payIn info from database. payIn Id=" + bankCurrencyId);
                _con.Open();
                int i = com.ExecuteNonQuery();
                _con.Close();
                if (i >= 1)
                {
                    _logger.Info("PayIn info is successfully deleted. payIn Id=" + bankCurrencyId);
                    return true;
                }
                else
                {
                    _logger.Error("Could not delete payIn information. payIn Id=" + bankCurrencyId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return false;
        }

    }
}

