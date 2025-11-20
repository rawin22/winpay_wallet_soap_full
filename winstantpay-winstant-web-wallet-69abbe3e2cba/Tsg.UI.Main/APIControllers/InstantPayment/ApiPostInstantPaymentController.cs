using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Tsg.UI.Main.ApiMethods.Payments;
using Tsg.UI.Main.App_LocalResources;
using TSG.Models.APIModels;
using TSG.Models.APIModels.InstantPayment;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Controllers;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.ServiceLayer.Users;

namespace Tsg.UI.Main.APIControllers.InstantPayment
{
    [ApiFilter]
    public class ApiPostInstantPaymentController : ApiController
    {
        private readonly IUsersServiceMethods _usersServiceMethods;
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ApiPostInstantPaymentController(IUsersServiceMethods usersServiceMethods) => _usersServiceMethods = usersServiceMethods;
        
        /// <summary>
        /// Post instant payment in WinstantPay Api
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        public IHttpActionResult Post([FromBody] ApiInstantPaymentModel.ApiInstantPaymentResponse body)
        {
            var result = new ApiInstantPaymentAfterPost()
            {
                Success = false,
                InfoBlock = new InfoBlock()
                {
                    Code = ApiErrors.ErrorCodeState.EmptyResult,
                    DeveloperMessage = "Empty result. Method started.",
                    UserMessage = String.Empty
                }
            };
            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();

            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (!(isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui)))
                {
                    result.InfoBlock = new InfoBlock()
                    {
                        Code = ApiErrors.ErrorCodeState.CheckUserError,
                        DeveloperMessage = "Data for user not correct",
                        UserMessage = "Data for user not correct"
                    };
                    return Ok(result);
                }
                if (!Guid.TryParse(body.PaymentId, out var gpaymentId))
                    throw new Exception("Invalid payment token");

                ApiNewInstantPaymentViewModel apiNewInstantPayment = new ApiNewInstantPaymentViewModel();
                InstantPaymentMethods ipm = new InstantPaymentMethods(ui);
                NewInstantPaymentMethods nipm = new NewInstantPaymentMethods(ui);

                var model = ipm.PrepareDetails(gpaymentId);
                if (!nipm.Post(gpaymentId).ServiceResponse.HasErrors)
                {
                    model.PaymentStatus = "Posted";
                    var photoId = ipm.GetImageLink(model, String.Empty, ui);
                    result = new ApiInstantPaymentAfterPost()
                    {
                        ImageLink =
                            $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/SharedViewer/PaymentDetails?photoId={photoId}",
                        Success = true,
                        InfoBlock = new InfoBlock()
                        {
                            Code = ApiErrors.ErrorCodeState.Success,
                            DeveloperMessage = "Ok",
                            UserMessage = body.CurrencyCode.ToLower().Contains("btc")
                                ? $"Payment sent!\nYou have successfully sent {body.Amount.ToString("N8", CultureInfo.GetCultureInfo("en-US"))} {body.CurrencyCode} to {body.ToCustomer}"
                                : $"Payment sent!\nYou have successfully sent {body.Amount.ToString("N2", CultureInfo.GetCultureInfo("en-US"))} {body.CurrencyCode} to {body.ToCustomer}"
                        }
                    };
                    _logger.Info(Newtonsoft.Json.JsonConvert.SerializeObject(model));

                    Task.Run(()=>TSG.UI.Main.Notifications.Notification.SendNotification(model.ToCustomerId, gpaymentId, "Instant payment", String.Format(GlobalRes.SuccessPayment_InvoiceNotificationText, model.FromCustomerAlias)));
                }
                else
                    result = new ApiInstantPaymentAfterPost()
                    {
                        Success = false,
                        InfoBlock = new InfoBlock()
                        {
                            Code = ApiErrors.ErrorCodeState.PostpaymentError,
                            DeveloperMessage = "Error: Post payment is false",
                            UserMessage = "Payment canceled. Your post payment is aborted"
                        }
                    };
            }
            catch (ApiException apiException)
            {
                result.InfoBlock = apiException.CustomInfoBlock;
            }
            catch (Exception e)
            {
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
            }
            return Ok(result);
        }
    }
}