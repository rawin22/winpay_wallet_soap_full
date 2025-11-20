using Newtonsoft.Json;

namespace TSG.Models.ServiceModels.PipitModels
{
    public class PipitCallBackBody
    {
        [JsonProperty("CREATE_DATE")] public string CreateDate { get; set; }
        [JsonProperty("EXPIRE_DATE")] public string ExpireDate { get; set; }
        [JsonProperty("PAID_DATE")] public string PaidDate { get; set; }
        [JsonProperty("BARCODE")] public string BarCode { get; set; }
        [JsonProperty("SIGN")] public string Sign { get; set; }
        [JsonProperty("VENDOR_ORDER_ID")] public string VendorOrderId { get; set; }
    }
}