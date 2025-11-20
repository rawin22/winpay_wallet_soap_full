using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.Models.Helpers;
using Tsg.UI.Main.Models.Security;

namespace Tsg.UI.Main.Models
{
    public class CountryModel
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public List<CountryModel> ListOfCountries { get; set; }
    }
}