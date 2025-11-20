using System;
using System.Configuration;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using TSG.Models.APIModels.Fundings.Pipit;
using TSG.Models.ServiceModels.PipitModels;
using TSG.ServiceLayer.Interfaces.Fundings;
using WinstantPay.Common.CryptDecriptInfo;

namespace Tsg.UI.Main.APIControllers.Fundings.PipIt
{  
    public class PipitCallBackController : ApiController
    {
        readonly static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IFundingsService _fundingsService;
        private readonly IFundingSourcesService _fundingsSourceService;
        private readonly IFundingChangesService _fundingsChangesService;

        public PipitCallBackController(IFundingsService fundingsService, IFundingSourcesService fundingSourcesService, IFundingChangesService fundingsChangesService)
        {
            _fundingsService = fundingsService;
            _fundingsSourceService = fundingSourcesService;
            _fundingsChangesService = fundingsChangesService;
        }

        [HttpGet]
        public IHttpActionResult Get(string BARCODE = "", string VENDOR_ORDER_ID = "", string PAID_DATE = "", string SIGN = "")
        {
            _logger.Info("GET");
            _logger.Info($"BARCODE - {BARCODE}, VENDOR_ORDER_ID - {VENDOR_ORDER_ID}, PAID_DATE - {PAID_DATE}, SIGN - {SIGN}");

            try
            {
                if(string.IsNullOrEmpty(SIGN))
                    throw new Exception("Sign value is empty");

                var encoder = new UTF8Encoding();
                var sha256Hasher = new SHA256CryptoServiceProvider();
                var calcPhase = $"{BARCODE}{VENDOR_ORDER_ID}{PAID_DATE}{ConfigurationManager.AppSettings["pipitApiKeyIntegration"]}";

                var hashedDataBytes = sha256Hasher.ComputeHash(encoder.GetBytes(calcPhase));
                var cryptoHash = Crypto.ByteArrayToHexString(hashedDataBytes);

                if(SIGN != cryptoHash)
                    throw new Exception($"Orders does not equals by Hash");

                var currPipitFund = _fundingsService.GetPipitFundingByBarCode(BARCODE, VENDOR_ORDER_ID);
                if(currPipitFund.Obj == null)
                    throw new Exception($"Barcode is {BARCODE} does not exist in system");
                if(!DateTime.TryParseExact(PAID_DATE, "YYYYmmDDHHMMSS", null, DateTimeStyles.None, out var paymentdate))
                    throw new Exception($"Paid date is {BARCODE} not parsed or not correct");
                currPipitFund.Obj.AddFundsPipit_PaymentDate = paymentdate;

                var updateRes = _fundingsService.UpdatePipitTransfer(currPipitFund.Obj);
                if(!updateRes.Success)
                    throw new Exception($"Update failed {BARCODE}: Reason {updateRes.Message}");
                _logger.Info($"Update success for {BARCODE}");
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
            
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] PipitCallBackBody model
            /*[FromUri] string BARCODE="", [FromUri] string VENDOR_ORDER_ID = "", [FromUri] string PAID_DATE = "", [FromUri] string SIGN=""*/)
        {
            _logger.Info("POST");
            _logger.Info($"BARCODE - {model.BarCode}, VENDOR_ORDER_ID - {model.VendorOrderId}, PAID_DATE - {model.PaidDate}, CREATE_DATE - {model.CreateDate}, EXPIRE_DATE - {model.ExpireDate}, SIGN - {model.Sign}");

            if (String.IsNullOrEmpty(model.CreateDate) || String.IsNullOrEmpty(model.ExpireDate) || String.IsNullOrEmpty(model.PaidDate))
                return BadRequest($"Data corrupted. SENDED JSON is {Newtonsoft.Json.JsonConvert.SerializeObject(model)}");
            return Ok();

            //try
            //{
            //    if (string.IsNullOrEmpty(SIGN))
            //        throw new Exception("Sign value is empty");
            //    #region SIGN
            //    //var encoder = new UTF8Encoding();
            //    //var sha256Hasher = new SHA256CryptoServiceProvider();
            //    //var calcPhase = $"{BARCODE}{VENDOR_ORDER_ID}{PAID_DATE}{ConfigurationManager.AppSettings["pipitApiKeyIntegration"]}";

            //    //var hashedDataBytes = sha256Hasher.ComputeHash(encoder.GetBytes(calcPhase));
            //    //var cryptoHash = Crypto.ByteArrayToHexString(hashedDataBytes);


            //    //var alg = SHA256.Create();
            //    //alg.ComputeHash(encoder.GetBytes("60500226449200107610291837612345678920131114103000secret"));
            //    //var xx = BitConverter.ToString(alg.Hash);

            //    //var zz = Convert.ToBase64String(alg.Hash);
            //    //var zs = Convert.FromBase64String("60500226449200107610291837612345678920131114103000secret");
            //    //alg.ComputeHash(zs);
            //    //var xz = BitConverter.ToString(alg.Hash);




            //    //var hashedDataBytes_test = sha256Hasher.ComputeHash(encoder.GetBytes("60500226449200107610291837612345678920131114103000secret"));
            //    //var cryptoHash_test = Crypto.ByteArrayToHexString(hashedDataBytes);
            //    //var secrethash_test = "e9798b09e37edef38f16a021c5b12ca41ad837e2";

            //    //if (cryptoHash_test == secrethash_test)
            //    //    _logger.Info("Test succeseedd");
            //    //if (SIGN != cryptoHash)
            //    //    throw new Exception($"Orders does not equals by Hash"); 
            //    #endregion

            //    var currPipitFund = _fundingsService.GetPipitFundingByBarCode(BARCODE, VENDOR_ORDER_ID);
            //    if (currPipitFund.Obj == null)
            //        throw new Exception($"Barcode is {BARCODE} does not exist in system");
            //    if (!DateTime.TryParseExact(PAID_DATE, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture,
            //        System.Globalization.DateTimeStyles.None, out var paymentdate))
            //        throw new Exception($"Paid date is {PAID_DATE} not parsed or not correct");

            //    currPipitFund.Obj.AddFundsPipit_PaymentDate = paymentdate;

            //    var updateRes = _fundingsService.UpdatePipitTransfer(currPipitFund.Obj);
            //    if (!updateRes.Success)
            //        throw new Exception($"Update failed {BARCODE}: Reason {updateRes.Message}");
            //    _logger.Info($"Update success for {BARCODE}");
            //}
            //catch (Exception e)
            //{
            //    _logger.Error(e.Message);
            //}

        }

    }
}