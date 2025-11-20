using TSG.Models.APIModels;

namespace TSG.Models.ServiceModels.Shop
{
    public class PayedShopModel: StandartResponse
    {
        public string PaymentOrderId { get; set; }
        public string PaymentOrderGuid { get; set; }
    }
}
