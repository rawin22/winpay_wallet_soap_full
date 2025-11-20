using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.Models.Security;

namespace Tsg.UI.Main.Controllers
{
    [AllowAnonymous]
    public class TestKycController : BaseController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IgpService Service { get; set; }
        private ServiceCallerIdentity _serviceCallerIdentity { get; }
        public TestKycController()
        {
#if DEBUG
            Service = new IgpService(ConfigurationManager.AppSettings["kycLogin"], ConfigurationManager.AppSettings["kycPassword"],ConfigurationManager.AppSettings["kycServiceCallerId"]);
#endif
            _serviceCallerIdentity = new ServiceCallerIdentity()
            {
                LoginId = ConfigurationManager.AppSettings["kycLogin"],
                Password = ConfigurationManager.AppSettings["kycPassword"],
                ServiceCallerId = ConfigurationManager.AppSettings["kycServiceCallerId"]
            };
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "FirstName,LastName,AddressLine1,City,CountryCode,PostalCode,Phone,AccountNumber")] FormCollection formCollection)
        {
            try
            {
                
                var x = Service.CustomerCreateFromTemplate(new CustomerCreateFromTemplateRequest()
                {
                    //ServiceCallerIdentity = _serviceCallerIdentity,
                    AccountRepresentativeId = ConfigurationManager.AppSettings["kycAccountRepresentativeId"],
                    CustomerTemplateId = ConfigurationManager.AppSettings["kycCustomerTemplateId"],
                    FirstName = formCollection[1],
                    LastName = formCollection[2],
                    MailingAddressLine1 = formCollection[3],
                    MailingCity = formCollection[4],
                    CountryCode = formCollection[5],
                    MailingPostalCode = formCollection[6],
                    Phone = formCollection[7],
                    AccountNumber = $"wkyc{formCollection[8]}"

                }, _serviceCallerIdentity);
                if (x.CustomerCreateFromTemplateData == null)
                    throw new Exception(x.ServiceResponse.Responses[0].MessageDetails);
                ViewBag.Result = $"Ok | {x.CustomerCreateFromTemplateData.CustomerId}";
            }
            catch (Exception e)
            {
                ViewBag.Result = e.Message;
            }
            return View();
        }

        public ActionResult CustomerUserCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CustomerUserCreate([Bind(Include = "CustomerId,EmailAddress,UserName,FirstName,LastName")] FormCollection formCollection)
        {
            try
            {
                var x = Service.CustomerUserCreate(new CustomerUserCreateRequest()
                {
                    //ServiceCallerIdentity = _serviceCallerIdentity,
                    CustomerId = formCollection[1],
                    EmailAddress = formCollection[2],
                    UserName = formCollection[3],
                    FirstName = formCollection[4],
                    LastName = formCollection[5],
                    UserMustChangePassword = false,
                    IsApproved = true
                }, _serviceCallerIdentity);
                if (x.CustomerUserInformation == null)
                    throw new Exception(x.ServiceResponse.Responses[0].MessageDetails);
                ViewBag.Result = $"Ok | UserId : {x.CustomerUserInformation.UserId} | Password : {x.CustomerUserInformation.NewPassword }";
            }
            catch (Exception e)
            {
                ViewBag.Result = e.Message;
            }
            return View();
        }

        public ActionResult UserAccessRightTemplateLink()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserAccessRightTemplateLink([Bind(Include = "UserId")] FormCollection formCollection)
        {
            try
            {
                var x = Service.UserAccessRightTemplateLink
                (new UserAccessRightTemplateLinkRequest()
                {
                    //ServiceCallerIdentity = _serviceCallerIdentity,
                    UserId = formCollection[1],
                    AccessRightTemplateId = ConfigurationManager.AppSettings["kycAccessRightTemplateId"]
                }, _serviceCallerIdentity);
                if (x.ServiceResponse.HasErrors)
                    throw new Exception(x.ServiceResponse.Responses[0].MessageDetails);
                ViewBag.Result = $"Ok | Result : {x.ServiceResponse.Responses[0].Message}";
            }
            catch (Exception e)
            {
                ViewBag.Result = e.Message;
            }
            return View();
        }
    }
}