using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Merchants;
using Tsg.UI.Main.Repository;

namespace Tsg.UI.Main.NopCommerceChekingRules
{
    public static class NopCommerceRules
    {
        #region CheckVariableRule
        public static NopCommerceRepository.ResultOfDbOperation<MerchantModel> CheckToken(string token, out Exception errors)
        {
            errors = null;
            NopCommerceRepository.ResultOfDbOperation<MerchantModel> result = new NopCommerceRepository.ResultOfDbOperation<MerchantModel>();
            try
            {
                NopCommerceRepository nopCommerceRepository = new NopCommerceRepository();
                var repRes = nopCommerceRepository.ChechMerchant(token.Trim());
                if(!repRes.IsSuccess)
                    throw new Exception(repRes.Message);
                result = repRes;
            }
            catch (Exception e)
            {
                errors = e;
            }    
            return result;
        }
        
        public static bool CheckString(string input, out Exception errors, out string output, bool isNullAvaliable = true)
        {
            errors = null;
            bool result = false;
            output = String.Empty;
            try
            {
                if (!isNullAvaliable)
                    result = !String.IsNullOrEmpty(input.Trim());
                else result = String.IsNullOrEmpty(input.Trim());
                output = input.Trim();
            }
            catch (Exception e)
            {
                errors = e;
            }    
            return result;
        }

        public static bool CheckGuid(string input, out Exception errors, out Guid output, bool isNullAvaliable = true)
        {
            errors = null;
            bool result = false;
            output = Guid.Empty;
            try
            {
                result = Guid.TryParse(input.Trim(), out output);
            }
            catch (Exception e)
            {
                errors = e;
            }    
            return result;
        }
        public static bool CheckDecimal(string input, out Exception errors, out decimal output)
        {
            errors = null;
            bool result = false;
            output = Decimal.Zero;
            try
            {
                result = Decimal.TryParse(input, out output);
            }
            catch (Exception e)
            {
                errors = e;
            }    
            return result;
        }
        
        #endregion

        public static bool CheckCompatibleCurrency(string currencyCode, out Exception errors, out string output)
        {
            errors = null;
            bool result = false;
            output = String.Empty;
            try
            {
                NewInstantPaymentViewModel paymentViewModel = new NewInstantPaymentViewModel();
                var currenciesOnSystem = paymentViewModel.PrepareAllAvailablePaymentCurrencies();
                if(!(result = currenciesOnSystem.Select(s=>s.Value).Contains(currencyCode)))
                    throw new Exception("Ewallet doesn't work with this currency");
                output = currencyCode;
            }
            catch (Exception e)
            {
                errors = e;
            }
            return result;
        }
    }
}