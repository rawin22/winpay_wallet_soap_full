using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.ApiMethods.KycIntegration;
using Tsg.UI.Main.ApiMethods.UserInfoMethods;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.APIModels.KycIntegrationModel;
using TSG.Models.DTO.Transfers;
using TSG.Models.ServiceModels.UsersDataBlock;
using TSG.ServiceLayer.Kyc;
using TSG.ServiceLayer.Users;

namespace Tsg.UI.Main.Controllers.MiniKyc
{
    public class MiniKycRegistrationController : Controller
    {

        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IKycNewUserServiceMethods _kycNewUserService;
        private readonly IUsersServiceMethods _usersServiceMethods;

        public MiniKycRegistrationController(IKycNewUserServiceMethods kycNewUserService, IUsersServiceMethods usersServiceMethods)
        {
            _kycNewUserService = kycNewUserService;
            _usersServiceMethods = usersServiceMethods;
        }

        [System.Web.Http.HttpPost]
        public ActionResult Post(MiniKycAddNewUser model)
        {
            if (ModelState.IsValid)
            {
                ServiceCallerIdentity serviceCallerIdentity = new ServiceCallerIdentity()
                {
                    LoginId = ConfigurationManager.AppSettings["kycLogin"],
                    Password = ConfigurationManager.AppSettings["kycPassword"],
                    ServiceCallerId = ConfigurationManager.AppSettings["kycServiceCallerId"]
                };


                KycIntegrationMethods method = new KycIntegrationMethods(AppSecurity.CurrentUser, serviceCallerIdentity);
                // step 1. Customer create from template
                _logger.Info("Customer create from template start");

                var resultStep1 = new CustomerCreateFromTemplateRepsonseModel
                {
                    Success = false,
                    InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty },
                };
                var step1RequestModel = new CustomerCreateFromTemplateModel()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    AddressLine1 = ConfigurationManager.AppSettings["kycDefaultAddress1"],
                    City = ConfigurationManager.AppSettings["kycDefaultCity"],
                    PostalCode = ConfigurationManager.AppSettings["kycDefaultPostalCode"],
                    Phone = ConfigurationManager.AppSettings["kycDefaultTelephone1"],
                    AccountNumber = $"winstantPay-{Guid.NewGuid().ToString()}",
                    CustomerTemplateId = ConfigurationManager.AppSettings["kycCustomerTemplateId"],
                    CountryCode = ConfigurationManager.AppSettings["kycDefaultCountryNationality"],
                    AccountRepresentativeId = ConfigurationManager.AppSettings["kycAccountRepresentativeId"]
                };
                try
                {
                    resultStep1 = method.CreateCustomerFromTemplate(step1RequestModel);
                    if (!resultStep1.Success)
                        return Json((resultStep1 as StandartResponse) ?? new StandartResponse("Error", "Customer create from template start step error"));
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                    return Json(new StandartResponse("Error in KYC process", $"Step 1: {e.Message + Environment.NewLine + e.InnerException?.Message}"));
                }
                // step 2. Customer user create
                _logger.Info("Customer user create start");
                var resultStep2 = new CustomerUserCreateResponseModel
                {
                    Success = false,
                    InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty }
                };
                var step2RequestModel = new CustomerUserCreateModel()
                {
                    CustomerId = resultStep1.CustomerId,
                    UserName = model.EmailAddress,
                    EmailAddress = model.EmailAddress,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserMustChangePassword = false,
                    IsApproved = true,
                    EmailPasswordToUser = true,
                    Password = String.Empty
                };
                try
                {
                    resultStep2 = method.CustomerUserCreate(step2RequestModel);
                    if (!resultStep2.Success)
                        return Json((resultStep2 as StandartResponse) ?? new StandartResponse("Error", "Customer create from template start step error"));
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                    return Json(new StandartResponse("Error in KYC process", $"Step 2: {e.Message + Environment.NewLine + e.InnerException?.Message}"));
                }
                // step 3. User access right template link
                _logger.Info("User access right template link");
                var resultStep3 = new UserAccessRightTemplateLinkResponseModel
                {
                    Success = false,
                    InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty },
                };

                var step3RequestModel = new UserAccessRightTemplateLinkModel()
                {
                    UserId = resultStep2.UserId,
                    AccessRightTemplateId = ConfigurationManager.AppSettings["kycAccessRightTemplateId"]
                };
                try
                {
                    resultStep3 = method.UserAccessRightTemplateLink(step3RequestModel);
                    if (!resultStep3.Success)
                        return Json((resultStep3 as StandartResponse) ?? new StandartResponse("Error", "Customer create from template start step error"));
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                    return Json(new StandartResponse("Error in KYC process", $"Step 3: {e.Message + Environment.NewLine + e.InnerException?.Message}"));
                }

                // Create alias to user
                bool isAliasCreated = false;
                UserLoginModel loginUser = new UserLoginModel() { Username = model.EmailAddress, Password = resultStep2.NewPassword };
                var loginRes = loginUser.ApiServiceLogin();
                if (loginRes == null || !loginRes.IsSuccess)
                {
                    return Json(new StandartResponse("Invalid user data login/password"));
                }
                var userInfoMethods = new UserInfoMethods(loginRes.UserInfo);
                string userAlias = model.EmailAddress;
                do
                {
                    if (!userInfoMethods.AddNewAlias(userAlias, out var resString))
                    {
                        _logger.Error(resString);
                        userAlias = userAlias + Guid.NewGuid().ToString("N").Substring(0, 5);
                    }
                    else isAliasCreated = true;


                } while (!isAliasCreated);

                // Insert logic in KYC users table
                var insertKycRes = _kycNewUserService.Insert(new KycNewUserDto()
                {
                    IsCreated = true,
                    KycUserEmail = model.EmailAddress,
                    KycUserFirstName = model.FirstName,
                    KycUserLastName = model.LastName,
                    UserInitiator = AppSecurity.CurrentUser.UserName
                });

                if (!insertKycRes.Success)
                    _logger.Warn(insertKycRes.Message);

                _usersServiceMethods.SaveUserAliases(new UserAliasesSo()
                {
                    Wpay_UserId = Guid.Parse(resultStep2.UserId),
                    Wpay_UserName = model.EmailAddress,
                    Wpay_Ids = new List<string>() { userAlias }
                });

                return Json(new StandartResponse<(string UserAlias, Guid UserId, string UserName)>((userAlias, insertKycRes.Obj.Id, model.EmailAddress),
                    true, "KYC user created successfully"));
            }
            else
            {
                return Json(new StandartResponse(false, "Invalid model",
                    string.Join($";{Environment.NewLine}",
                        ModelState.Values
                            .SelectMany(x => x.Errors)
                            .Select(x => x.ErrorMessage))));
            }
        }

    }
}