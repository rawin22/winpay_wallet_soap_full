using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSG.Models.APIModels
{
    public class GenerateQrCodeModelRequest
    {
        public string Text { get; set; }
        public int LogoIndex { get; set; } // Default to WinstantPay. 0 = WinstantPay, 1 = WorlKYC

    }

    public class GenerateQrCodeModelResponse
    {
        public string QrCodeBase64String { get; set; }

    }

    public class GenerateQrCodeSvgModelResponse
    {
        public string QrCodeAsSvg{ get; set; }
    }
    
    public class GenerateQrCodeBase64ModelResponse
    {
        public string QrCodeAsBase64{ get; set; }
    }
}
