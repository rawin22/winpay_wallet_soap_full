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
    /// Web API for generating QR code as Bitmap
    ///</Summary>
    public class ApiGenerateQrCodeAsBitmapController : ApiController
    {
        // GET: api/ApiGenerateQrCodeAsBitmap
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}
        public HttpResponseMessage Get(string text, int logoIndex)
        {
            HttpResponseMessage Response = new HttpResponseMessage(HttpStatusCode.OK);

            Byte[] b = QrCodeExtensions.GetAsBitmapByteQRCode(text);
            Response.Content = new ByteArrayContent(b);
            Response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/bmp");

            return Response;
        }

        // POST: api/ApiGenerateQrCodeAsBitmap
        /// <summary>
        /// Generate QR code as bitmap
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Byte array of bitmap of QR code</returns>
        public HttpResponseMessage Post([FromBody]GenerateQrCodeModelRequest model)
        {
            HttpResponseMessage Response = new HttpResponseMessage(HttpStatusCode.OK);
            // string serverpath = Server.MapPath("~/upload");

            Byte[] b = QrCodeExtensions.GetAsBitmapByteQRCode(model.Text);
            Response.Content = new ByteArrayContent(b);
            Response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/bmp");

            return Response;
        }
    }
}
