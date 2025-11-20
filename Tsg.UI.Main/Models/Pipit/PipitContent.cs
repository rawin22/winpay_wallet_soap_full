using System;

namespace Tsg.UI.Main.Models.Pipit
{
    public class PipitContent
    {
            public string Barcode { get; set; }
            public string Reference { get; set; }
            public string VendorReference { get; set; }
            public string CurrencyCode { get; set; }
            public double OrderValue { get; set; }
            public double TotalValue { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime ExpiryDate { get; set; }
            public string Status { get; set; }
     }
}