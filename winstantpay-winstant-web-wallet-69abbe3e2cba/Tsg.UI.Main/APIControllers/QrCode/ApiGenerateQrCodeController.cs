using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tsg.UI.Main.Attributes;
using TSG.Models.APIModels;

namespace Tsg.UI.Main.APIControllers
{

    /// <summary>  
    ///  This class generates QR code.  
    /// </summary>
    //public class ApiGenerateQrCodeController : ApiController
    //{
    //    //// GET: api/ApiGenerateQrCode
    //    //public IEnumerable<string> Get()
    //    //{
    //    //    return new string[] { "value1", "value2" };
    //    //}

    //    /// <summary>  
    //    ///  Return Base64 string of QR code of request text.  
    //    /// </summary>
    //    // POST: api/ApiGenerateQrCode
    //    public IHttpActionResult Post([FromBody]GenerateQrCodeModelRequest model)
    //    {
    //        GenerateQrCodeModelResponse response = new GenerateQrCodeModelResponse();
    //        QRCodeGenerator qrGenerator = new QRCodeGenerator();
    //        QRCodeData qrCodeData = qrGenerator.CreateQrCode(model.Text, QRCodeGenerator.ECCLevel.Q);

    //        QRCode qrCode = new QRCode(qrCodeData);
    //        Bitmap qrCodeImage = qrCode.GetGraphic(20);
    //        Byte[] b = BitmapToBytes(qrCodeImage);
    //        response.QrCodeBase64String = Convert.ToBase64String(b.ToArray());
    //        //return File(b, "image/jpeg");
    //        // return Convert.ToBase64String(b.ToArray());
    //        return Ok(response);
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
