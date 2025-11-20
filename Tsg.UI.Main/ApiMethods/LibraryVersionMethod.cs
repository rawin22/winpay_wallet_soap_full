using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using Newtonsoft.Json;
using StaticExtensions;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.APIModels.ExchangeModels;
using TSG.Models.APIModels.InstantPayment;
using TSG.Models.APIModels.Payment;

namespace Tsg.UI.Main.ApiMethods
{
    /// <summary>
    /// Model for payout API
    /// </summary>
    public class LibraryVersionMethod
    {
        private UserInfo _userInfo { get; set; }
        protected IgpService Service;
        public static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Constructor
        /// </summary>
        public LibraryVersionMethod() 
        {
            Service = new IgpService();            
            _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// Get library version
        /// </summary>
        public string GetVersion()
        {
            return Service.GetLibraryVersion();                        
        }        
    }
}