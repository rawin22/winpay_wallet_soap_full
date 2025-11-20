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
using Tsg.UI.Main.Extensions;
using TSG.Models.APIModels;

namespace Tsg.UI.Main.APIControllers
{
    ///<Summary>
    /// Web API for generating QR code as SVG
    ///</Summary>
    public class ApiGenerateQrCodeAsSvgController : ApiController
    {
        // GET: api/ApiTestQrCode
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}
        //public IHttpActionResult Get(string text)
        //{
        //    GenerateQrCodeSvgModelResponse response = new GenerateQrCodeSvgModelResponse();

        //    response.QrCodeAsSvg = QrCodeExtensions.GetAsSvg(text);

        //    return Ok(response);
        //}

        // POST: api/ApiGenerateQrCodeAsBitmap

        /// <summary>
        /// Generate QR code as SVG
        /// </summary>
        /// <param name="model"></param>
        /// <returns>SVG of QR code of given text</returns>
        public IHttpActionResult Post([FromBody]GenerateQrCodeModelRequest model)
        {
            GenerateQrCodeSvgModelResponse response = new GenerateQrCodeSvgModelResponse();

            response.QrCodeAsSvg = QrCodeExtensions.GetAsSvg(model.Text);

            return Ok(response);
        }
    }
}
