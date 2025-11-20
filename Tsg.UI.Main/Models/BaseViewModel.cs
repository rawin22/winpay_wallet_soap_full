using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.Models.Security;

namespace Tsg.UI.Main.Models
{
    public class BaseViewModel
    {
        protected log4net.ILog _logger;
        protected IgpService Service;
        public bool HasError { get; set; }
        public IList<ErrorViewModel> Errors { get; set; }

        public BaseViewModel()
        {
            Service = new IgpService(AppSecurity.CurrentUser.AccessToken);
            Errors = new List<ErrorViewModel>();
            _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public void GetErrorMessages(ServiceResponseData[] responses)
        {
            this.HasError = true;

            foreach (var error in responses)
            {
                this.Errors.Add(new ErrorViewModel
                {
                    Code = error.ResponseCode,
                    Type = (ErrorType)error.ResponseType,
                    Message = error.Message,
                    MessageDetails = error.MessageDetails,
                    FieldName = error.FieldName,
                    FieldValue = error.FieldValue
                });
            }
        }
    }
}