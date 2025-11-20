using System.Collections.Generic;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;
using TSG.Models.APIModels;

namespace Tsg.UI.Main.ApiMethods
{
    public class BaseApiMethods
    {
        protected IgpService Service;
        public static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public BaseApiMethods(UserInfo ui)
        {
            Service = new IgpService(ui.AccessToken, ui.UserId);
        }        
    }
}