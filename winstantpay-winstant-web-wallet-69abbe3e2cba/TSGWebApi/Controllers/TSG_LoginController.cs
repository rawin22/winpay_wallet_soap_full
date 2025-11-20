using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using Tsg.UI.Main.Models.API;
using TSGWebApi.Logic;

namespace TSGWebApi.Controllers
{
    public class TSG_LoginController : ApiController
    {
        /// <summary>
        /// Login user for merchant
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Remarks: Login user for merchant
        /// </remarks>
        /// <response code="200"></response>
        [ResponseType(typeof(TsgApiModels.CustomerDetails))]
        public TsgApiModels.CustomerDetails Post([FromBody]TsgApiModels.LoginStruct loginStruct)
        {
            if (loginStruct == null)
                return null;
            if (String.IsNullOrEmpty(loginStruct.KeyOfTokenGuid) || String.IsNullOrEmpty(loginStruct.Login)
                || String.IsNullOrEmpty(loginStruct.Password) || String.IsNullOrEmpty(loginStruct.Source))
                return null;
            CommonMethods.PrintInfo("Calling Login Method", true);
            HttpClient client = new HttpClient();
            var values = new Dictionary<string, string>
            {
                { "username", "leapfrog" },
                { "password", "password" }
            };
            var request = (HttpWebRequest)WebRequest.Create("http://localhost:9000/User/CheckoutLogin?src=instant-checkout&token=jdf7sfg7898f78");

            var postData = "username=leapfrog";
            postData += "&password=password";
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();


            //var content = new FormUrlEncodedContent(values);
            ////Post
            //var responsePost = client.PostAsync("http://localhost:9000/User/CheckoutLogin", content);
            ////Get
            //var responseGet = client.GetAsync("http://localhost:9000/User/CheckoutLogin?src=instant-checkout&token=jdf7sfg7898f78");
            //var resgetstr = responseGet.Result.ToString();
            //                                var responseString = responsePost.Result.Content.ReadAsStringAsync();
            TsgApiModels.CustomerDetails customerDetails = new TsgApiModels.CustomerDetails();
            try
            {
                var entity = new TsgEwalletDbEntities();
                if (entity.Database.Connection.State != ConnectionState.Open)
                    entity.Database.Connection.Open();
                
                logger.Info("Db connection setted and activated");
                try
                {
                    DBLogic.MerchantInfo mi = DBLogic.CheckMerchant(loginStruct.MerchantUniqueKey);
                    if (mi.IsExist)
                    {
                        var orderGuid = Guid.Parse(loginStruct.KeyOfTokenGuid);
                        var merchantGuid = Guid.Parse(loginStruct.KeyOfTokenGuid);
                        var merchant = entity.Merchants.FirstOrDefault(f => f.UniqueID == merchantGuid);
                        var order = entity.TokenKeysForOrders.FirstOrDefault(f => f.CustomerOrderId == orderGuid);
                        
                        if (order != null && merchant != null)
                        {
                            order.CustomerOrderStatus = 2;
                            ///////////////////////////
                            ////////
                            // Call TSG api methods
                            ////////
                            try
                            {
        
                            }
                            catch (Exception tsgException)
                            {
                                order.CustomerOrderStatus = 6;
                            }
                            ///////////////////////////
                            
                            entity.TokenKeysForOrders.Attach(order);
                            entity.Entry(order).State = EntityState.Modified;
                            entity.SaveChanges();
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception dbException)
                {
                    entity.Database.Connection.Close();
                    logger.Error(String.Format("[{0}] Connection failure", DateTime.Now), dbException);
                }
            }
            catch (Exception exception)
            {
                logger.Error(String.Format("[{0}] Code failure", DateTime.Now), exception);
            }
            return customerDetails;

        }
    }
}