using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TSG.Models.APIModels
{
    public class ApiErrors
    {
        public enum ErrorCodeState
        {
            EmptyResult = 0,
            Success = 200,


            #region Api Errors [1-1000]
                #region LoginErrors
                UnspecifiedError = 1,
                AuthorizationFailed = 2,
                LoginOrPasswordIsEmpty = 3,
                EmptyApiKey = 4, 
                #endregion

                #region InstantPaymentErrors
                CheckUserError = 51,
                PostpaymentError = 52,
            #endregion

            #region Delegated Authority
                NeedToSetUpPin = 100,
            #endregion

            #endregion
            #region SQL Errors [1000-2000]
            SqlTokenGenerationError = 1000
            #endregion
        }
    }
}