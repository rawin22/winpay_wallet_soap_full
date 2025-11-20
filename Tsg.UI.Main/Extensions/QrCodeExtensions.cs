using QRCoder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Tsg.UI.Main.Extensions
{
    ///<Summary>
    /// Get settings from Web.config
    ///</Summary>
    public class QrCodeExtensions
    {
        ///<Summary>
        /// Generate QR Code as Bitmap image
        ///</Summary>
        public static Bitmap GetAsBitmap(string text)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);

            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            return qrCodeImage;
        }

        ///<Summary>
        /// Generate QR code as Base64 image
        ///</Summary>
        public static string GetAsBase64(string text)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/assets/images/qrcode-icon-winstantpay-3.png"); 
            // string serverpath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/assets/images/winstantpay-logo-notag-white-01-01.png"); 

            Base64QRCode qrCode = new Base64QRCode(qrCodeData);
            string qrCodeImageAsBase64 = qrCode.GetGraphic(20,Color.Black, Color.White, (Bitmap)Bitmap.FromFile(filePath));
            return qrCodeImageAsBase64;
        }

        ///<Summary>
        /// Generate QR code as Base64 image
        ///</Summary>
        public static string GetAsBase64(string text, int logo = 0)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/assets/images/qrcode-icon/" + GetLogoFile(logo));
            // string serverpath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/assets/images/winstantpay-logo-notag-white-01-01.png"); 

            Base64QRCode qrCode = new Base64QRCode(qrCodeData);
            string qrCodeImageAsBase64 = qrCode.GetGraphic(20, Color.Black, Color.White, (Bitmap)Bitmap.FromFile(filePath));
            return qrCodeImageAsBase64;
        }

        ///<Summary>
        /// Generate QR code as scalable vector graphic
        ///</Summary>
        public static string GetAsSvg(string text)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            SvgQRCode qrCode = new SvgQRCode(qrCodeData);
            string qrCodeAsSvg = qrCode.GetGraphic(20);

            return qrCodeAsSvg;
        }

        ///<Summary>
        /// Generate QR code as byte-array which contains a PNG graphic.
        ///</Summary>
        public static byte[] GetAsPngByteQRCode(string text)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(20);

            return qrCodeAsPngByteArr;
        }

        ///<Summary>
        /// Generate QR code as byte-array which contains a Bitmap graphic
        ///</Summary>
        public static byte[] GetAsBitmapByteQRCode(string text)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeAsBitmapByteArr = qrCode.GetGraphic(20);

            return qrCodeAsBitmapByteArr;
        }

        public static string GetLogoFile(int logo)
        {
            string fileName = String.Empty;
            switch (logo)
            {
                case (int)Logos.WorldKYC:
                    fileName = "qrcode-icon-kyc.png";
                    break;
                case (int)Logos.pqrdemerde:
                    fileName = "pqrdemerde-white-bg.png";
                    break;
                case (int)Logos.jenesuispasundanger:
                    fileName = "jenesuispasundanger.png";
                    break;
                default:
                    fileName = "qrcode-icon-winstantpay.png";
                    break;
            }

            return fileName;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum Logos
    {
        WinstantPay,            // 0
        WorldKYC,               // 1
        pqrdemerde,             // 2
        jenesuispasundanger,    // 3
    }
}