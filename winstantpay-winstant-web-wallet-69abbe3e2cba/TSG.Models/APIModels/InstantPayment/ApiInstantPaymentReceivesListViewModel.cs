using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.InstantPayment
{
    public class ApiInstantPaymentReceivesListViewModel : StandartResponse
    {
        [JsonProperty("instant_payment_receives_list")] public IList<ApiInstantPaymentReceiveDetailsViewModel> InstantPaymentReceivesList { get; set; } = new List<ApiInstantPaymentReceiveDetailsViewModel>();
        // public IList<ApiAccountBalanceViewModel> Balances { get; set; } = new List<ApiAccountBalanceViewModel>();

    }

    //public class ApiInstantPaymentReceiveDetailsViewModel
    //{
    //    public Guid InstantPaymentReceiveId;
    //    public string Alias;
    //    public string Currency;
    //    public decimal Amount;
    //    public string Invoice;
    //   public string Memo;
    //    public string Name;
    //    public string Address;
    //    public string Email;
    //    public string ShortenedUrl { get; set; }
    //    [DataType(DataType.Date)]
    //    [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
    //    public DateTime CreatedDate;

    //    // public IList<InstantPaymentViewModel> InstantPayments { get; set; }
    //    //-----------------------------------
    //}
}