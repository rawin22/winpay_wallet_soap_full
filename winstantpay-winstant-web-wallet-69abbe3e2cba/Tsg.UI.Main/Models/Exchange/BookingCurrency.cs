using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.Business.Model.TSGgpwsbeta;

namespace Tsg.UI.Main.Models
{
    public class BookingCurrency
    {
        public FXDealQuoteCreateResponse FxDealQuoteResult { get; set; }
        public FXDealQuoteBookResponse FxDealQuoteBookResult { get; set; }
    }
}