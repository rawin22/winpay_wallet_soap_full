using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TSG.Models.APIModels.QrCode
{
    /// <summary>
    /// Result for get user aliases method
    /// </summary>
    public class InstantPaymentReceiveDetailResponse : StandartResponse
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
        public string ShortenedUrl { get; set; }
        public DateTime CreatedDate;
    }
}