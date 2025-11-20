using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.InstantPayment
{
    public class InstantPaymentReceiveHistoryInfoModel : StandartResponse
    {
        [JsonProperty("instant_payment_receive_details")] public ApiInstantPaymentReceiveDetailsViewModel InstantPaymentReceiveDetails { get; set; }
    }

    public class ApiInstantPaymentReceiveDetailsViewModel
    {
        public Guid InstantPaymentReceiveId;
        public string Alias;
        public string Currency;
        public decimal Amount;
        public string Invoice;
        public string Memo;
        public string Name;
        public string Address;
        public string Email;
        public string KycId;
        public string ShortenedUrl { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate;

        // public IList<InstantPaymentViewModel> InstantPayments { get; set; }
        //-----------------------------------
    }
}