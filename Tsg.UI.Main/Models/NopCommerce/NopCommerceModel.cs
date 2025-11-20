using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsg.UI.Main.Repository;

namespace Tsg.UI.Main.Models.NopCommerce
{
    /// <summary>
    /// Model for nopcommerce order
    /// </summary>
    public class NopCommerceModel : PaymentRepository.ResultOfDbOperation
    {
        public string Token{get; set;}
        public string ItemName{get; set;}
        public decimal Amount{get; set;}
        public Guid Custom {get; set;}
        public string CurrencyCode{get; set;}
        [AllowHtml]
        public string UrlReturn{get; set;}
        [AllowHtml]
        public string CancelReturn{get; set;}
        public string Alias { get; set; }
        //public int Account { get; set; }
        public string Account { get; set; }
        public string AccountSingleText { get; set; }
        public string AccountSingleNumber { get; set; }
        public string Items { get; set; }
        public string QueryString { get; set; }
        //public string FirstName{get; set;}
        //public string LastName{get; set;}
        //public string Address1{get; set;}
        //public string Address2{get; set;}
        //public string Country{get; set;}
        //public string State{get; set;}
        //public string City{get; set;}
        //public string Zip{get; set;}
        //public string Email { get; set; }
        public bool IsSandbox { get; set; }
        public string FromCustomer { get; set; }
        public string OrderTokenInEwalletSystem { get; set; }

        public bool NeedToRedirect { get; set; }
        //public CheckoutModel StandartCheckoutModel { get; set; } 
    }
}