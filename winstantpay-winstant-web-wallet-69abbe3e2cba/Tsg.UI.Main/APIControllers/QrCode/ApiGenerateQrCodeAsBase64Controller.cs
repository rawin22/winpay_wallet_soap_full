using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;
using Tsg.UI.Main.Extensions;
using TSG.Models.APIModels;

namespace Tsg.UI.Main.APIControllers
{
    ///<Summary>
    /// Web API for generating QR code as Base46
    ///</Summary>
    public class ApiGenerateQrCodeAsBase64Controller : ApiController
    {
        // GET: api/ApiTestQrCode
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // POST: api/ApiGenerateQrCodeAsBase64

        /// <summary>
        /// Generate QR code as Base 64
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Base64 of QR code of given text</returns>
        /// 
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult Post([FromBody]GenerateQrCodeModelRequest model)
        {
            GenerateQrCodeBase64ModelResponse response = new GenerateQrCodeBase64ModelResponse();

            response.QrCodeAsBase64 = QrCodeExtensions.GetAsBase64(model.Text, model.LogoIndex);

            return Ok(response);
        }
    }
}
