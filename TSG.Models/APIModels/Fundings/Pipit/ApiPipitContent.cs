using System;

namespace TSG.Models.APIModels.Fundings.Pipit
{
    public class ApiPipitContent
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