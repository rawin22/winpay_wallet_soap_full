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
    /// Web API for generating QR code as PNG
    ///</Summary>
    public class ApiGenerateQrCodeAsPngController : ApiController
    {
        // GET: api/ApiTestQrCode
        public HttpResponseMessage Get(string text)
        {
            // return new string[] { "value1", "value2" };
            HttpResponseMessage Response = new HttpResponseMessage(HttpStatusCode.OK);

            Byte[] b = QrCodeExtensions.GetAsPngByteQRCode(text);
            Response.Content = new ByteArrayContent(b);
            Response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

            return Response;
        }

        // POST: api/ApiGenerateQrCodeAsPng

        /// <summary>
        /// Generate QR code as PNG
        /// </summary>
        /// <param name="model"></param>
        /// <returns>PNG of QR code of given text</returns>
        public HttpResponseMessage Post([FromBody]GenerateQrCodeModelRequest model)
        {
            HttpResponseMessage Response = new HttpResponseMessage(HttpStatusCode.OK);

            Byte[] b = QrCodeExtensions.GetAsPngByteQRCode(model.Text);
            Response.Content = new ByteArrayContent(b);
            Response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

            return Response;
        }
    }
}
