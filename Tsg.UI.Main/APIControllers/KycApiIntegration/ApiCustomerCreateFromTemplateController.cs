using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.ApiMethods.KycIntegration;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.APIModels.KycIntegrationModel;

namespace Tsg.UI.Main.APIControllers.KycApiIntegration
{
    public class ApiCustomerCreateFromTemplateController : ApiController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpPost]
        public IHttpActionResult Post([FromBody] CustomerCreateFromTemplateModel model)
        {
            _logger.Info("[POST] ApiCustomerCreateFromTemplateController start");

            var result = new CustomerCreateFromTemplateRepsonseModel
            {
                Success = false,
                InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty },
            };

            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();
            try
            {
                if(!ModelState.IsValid)
                    throw new Exception(string.Join($";{Environment.NewLine}", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)));
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    ServiceCallerIdentity serviceCallerIdentity = new ServiceCallerIdentity()
                    {
                        LoginId = ConfigurationManager.AppSettings["kycLogin"],
                        Password = ConfigurationManager.AppSettings["kycPassword"],
                        ServiceCallerId = ConfigurationManager.AppSettings["kycServiceCallerId"]
                    };
                    KycIntegrationMethods method = new KycIntegrationMethods(ui, serviceCallerIdentity);
                    result = method.CreateCustomerFromTemplate(model);
                }
                else return Unauthorized();
            }
            catch (Exception e)
            {
                _logger.Error(e);
                result.InfoBlock = new InfoBlock(e.Message);
                result.InfoBlock.Code = ApiErrors.ErrorCodeState.UnspecifiedError; // { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
            }

            return Content(result.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest, result,
                new JsonMediaTypeFormatter(), "application/json");
        }
    }
}
