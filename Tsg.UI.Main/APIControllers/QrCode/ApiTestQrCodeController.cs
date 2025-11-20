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
using TSG.Models.APIModels;

namespace Tsg.UI.Main.APIControllers
{
    //public class ApiTestQrCodeController : ApiController
    //{
    //    // GET: api/ApiTestQrCode
    //    public IEnumerable<string> Get()
    //    {
    //        return new string[] { "value1", "value2" };
    //    }

    //    // GET: api/ApiTestQrCode/5
    //    public string Get(int id)
    //    {
    //        return "value";
    //    }

    //    // POST: api/ApiTestQrCode
    //    public HttpResponseMessage Post([FromBody]GenerateQrCodeModelRequest model)
    //    {
    //        HttpResponseMessage Response = new HttpResponseMessage(HttpStatusCode.OK);
    //        GenerateQrCodeModelResponse response = new GenerateQrCodeModelResponse();
    //        QRCodeGenerator qrGenerator = new QRCodeGenerator();
    //        QRCodeData qrCodeData = qrGenerator.CreateQrCode(model.Text, QRCodeGenerator.ECCLevel.Q);

    //        QRCode qrCode = new QRCode(qrCodeData);
    //        Bitmap qrCodeImage = qrCode.GetGraphic(20);
    //        Byte[] b = BitmapToBytes(qrCodeImage);
    //        response.QrCodeBase64String = Convert.ToBase64String(b.ToArray());
    //        Response.Content = new ByteArrayContent(b);
    //        Response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
    //        //return File(b, "image/jpeg");
    //        // return Convert.ToBase64String(b.ToArray());
    //        return Response;
    //    }

    //    // PUT: api/ApiTestQrCode/5
    //    public void Put(int id, [FromBody]string value)
    //    {            
    //    }

    //    // DELETE: api/ApiTestQrCode/5
    //    public void Delete(int id)
    //    {
    //    }

    //    private static Byte[] BitmapToBytes(Bitmap img)
    //    {
    //        using (MemoryStream stream = new MemoryStream())
    //        {
    //            img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
    //            return stream.ToArray();
    //        }
    //    }
    //}
}
