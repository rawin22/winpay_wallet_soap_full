using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TSG.Models.ServiceModels;

namespace Tsg.UI.Main.BlockchainIntegration
{
    public class BlockchainInfoApi
    {
        private readonly string _baseAddress = ConfigurationManager.AppSettings["blockChainInfoApiAddress"];
        private readonly string _xpubKey = ConfigurationManager.AppSettings["xPubKey"];
        private readonly string _apikey = ConfigurationManager.AppSettings["apiKey"];
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Recieve method for blockchain.info
        /// </summary>
        /// <param name="callback">returned address for check payment</param>
        /// <returns></returns>
        public BlockChainInfoReceiveResponse BlockChainInfo_RecieveMethod(string callback)
        {
            ServicePointManager.MaxServicePointIdleTime = 0;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                   | SecurityProtocolType.Tls11
                                                   | SecurityProtocolType.Tls12
                                                   | SecurityProtocolType.Ssl3;


            HttpClient client = new HttpClient();
            try
            {
                var uri = new Uri($"{_baseAddress}/receive?xpub={_xpubKey}&callback={callback}&key={_apikey}&gap_limit=1000");
                _logger.Info(uri.AbsoluteUri);
                client.BaseAddress = uri;
                var httpRes = client.GetAsync("").Result;
                var strings = httpRes.Content.ReadAsStringAsync().Result;
                _logger.Info(strings);
                return JsonConvert.DeserializeObject<BlockChainInfoReceiveResponse>(strings);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return new BlockChainInfoReceiveResponse() { Message = ex.Message };
            }
        }


        /// <summary>
        /// Recieve method for blockchain.info
        /// </summary>
        /// <param name="callback">returned address for check payment</param>
        /// <returns></returns>
        public BlockChainInfoUpdateResponse BlockChainInfo_UpdateMethod(string address, string callback)
        {
            ServicePointManager.MaxServicePointIdleTime = 0;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                   | SecurityProtocolType.Tls11
                                                   | SecurityProtocolType.Tls12
                                                   | SecurityProtocolType.Ssl3;


            HttpClient client = new HttpClient();
            try
            {
                var uri = new Uri($"{_baseAddress}/receive/balance_update");
                client.BaseAddress = uri;

                var content = new StringContent($"{{ \"key\":\"{_apikey}\", \"addr\": \"{address}\", \"callback\":\"{callback}\", \"onNotification\":\"KEEP\", \"op\":\"RECEIVE\", \"confs\": 3 }}", Encoding.UTF8, "text/plain");

                var httpRes = client.PostAsync(uri, content).Result;
                var strings = httpRes.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<BlockChainInfoUpdateResponse>(strings);
            }
            catch (Exception ex)
            {
                return new BlockChainInfoUpdateResponse() { Message = ex.Message };
            }
        }

        public string BlockChainInfo_GetPaymentLink(string address, string currencyCode, decimal amount, string message)
        {
            if(currencyCode.ToUpper().Contains("BTC"))
                return $"https://blockchain.info/payment_request?address={address}&amount={amount}&message={message}";
            return $"https://blockchain.info/payment_request?address={address}&amount_local={amount}&currency={currencyCode.ToUpper()}&nosavecurrency=true&message={message}";
        }
    }
}