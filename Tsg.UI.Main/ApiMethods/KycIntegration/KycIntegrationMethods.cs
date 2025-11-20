using System;
using System.Linq;
using Tsg.Business.Model.TsgGPWebService;
using TSG.Models.APIModels;
using TSG.Models.APIModels.KycIntegrationModel;

namespace Tsg.UI.Main.ApiMethods.KycIntegration
{
    public class KycIntegrationMethods : BaseApiMethods
    {
        private UserInfo _userInfo { get; set; }
        private ServiceCallerIdentity _serviceCallerIdentity  { get; set; }

        public KycIntegrationMethods(UserInfo ui, ServiceCallerIdentity callerIdentity) : base(ui)
        {
            _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            _userInfo = ui;
            _serviceCallerIdentity = callerIdentity;
        }

        public CustomerCreateFromTemplateRepsonseModel CreateCustomerFromTemplate(CustomerCreateFromTemplateModel requestModel)
        {
            var resultService = Service.CustomerCreateFromTemplate(new CustomerCreateFromTemplateRequest()
            {
                AccountNumber = requestModel.AccountNumber,
                AccountRepresentativeId = requestModel.AccountRepresentativeId,
                MailingAddressLine1 = requestModel.AddressLine1,
                MailingAddressLine2 = requestModel.AddressLine2,
                MailingCity = requestModel.City,
                CountryCode = requestModel.CountryCode,
                CustomerName = requestModel.CustomerName,
                CustomerTypeId = requestModel.CustomerTypeID,
                CustomerTemplateId = requestModel.CustomerTemplateId,
                FirstName = requestModel.FirstName,
                LastName = requestModel.LastName,
                MiddleName = requestModel.MiddleName,
                Phone = requestModel.Phone,
                Fax = requestModel.Fax,
                TaxId = requestModel.TaxId,
                MailingPostalCode = requestModel.PostalCode,
                MailingStateOrProvince = requestModel.State
            }, _serviceCallerIdentity);
            if (resultService.ServiceResponse.HasErrors)
            {
                return new CustomerCreateFromTemplateRepsonseModel()
                {
                    Success = false,
                    InfoBlock = new InfoBlock()
                    {
                        Code = ApiErrors.ErrorCodeState.UnspecifiedError,
                        UserMessage = $"{string.Join($"; {Environment.NewLine}", resultService.ServiceResponse.Responses.Select(s => s.MessageDetails))}",
                        DeveloperMessage = $"{string.Join($"; {Environment.NewLine}", resultService.ServiceResponse.Responses.Select(s => s.MessageDetails))}"
                    },
                    CustomerId = String.Empty
                };
            }
            return new CustomerCreateFromTemplateRepsonseModel()
            {
                Success = true,
                InfoBlock = new InfoBlock()
                {
                    Code = ApiErrors.ErrorCodeState.Success,
                    UserMessage = resultService.ServiceResponse.Responses[0]?.MessageDetails ?? "Ok",
                    DeveloperMessage = resultService.ServiceResponse.Responses[0]?.MessageDetails ?? "Ok"
                },
                CustomerId = resultService.CustomerCreateFromTemplateData.CustomerId
            };
        }

        public CustomerUserCreateResponseModel CustomerUserCreate(CustomerUserCreateModel requestModel)
        {
            var resultService = Service.CustomerUserCreate(new CustomerUserCreateRequest()
            {
                CustomerId = requestModel.CustomerId,
                UserName =  requestModel.UserName,
                EmailAddress = requestModel.EmailAddress,
                EmailPasswordToUser = requestModel.EmailPasswordToUser,
                FirstName = requestModel.FirstName,
                LastName = requestModel.LastName,
                IsApproved = requestModel.IsApproved,
                Password = requestModel.Password,
                UserMustChangePassword = requestModel.UserMustChangePassword
            }, _serviceCallerIdentity);

            if (resultService.ServiceResponse.HasErrors)
            {
                return new CustomerUserCreateResponseModel()
                {
                    Success = false,
                    InfoBlock = new InfoBlock()
                    {
                        Code = ApiErrors.ErrorCodeState.UnspecifiedError,
                        UserMessage = $"{string.Join($"; {Environment.NewLine}", resultService.ServiceResponse.Responses.Select(s => s.MessageDetails))}",
                        DeveloperMessage = $"{string.Join($"; {Environment.NewLine}",resultService.ServiceResponse.Responses.Select(s=>s.MessageDetails))}"
                    }
                };
            }
            return new CustomerUserCreateResponseModel()
            {
                Success = true,
                InfoBlock = new InfoBlock()
                {
                    Code = ApiErrors.ErrorCodeState.Success,
                    UserMessage = "Ok",
                    DeveloperMessage = "Ok"
                },
                UserId = resultService.CustomerUserInformation.UserId,
                NewPassword = resultService.CustomerUserInformation.NewPassword
            };

        }

        public UserAccessRightTemplateLinkResponseModel UserAccessRightTemplateLink(UserAccessRightTemplateLinkModel requestModel)
        {
            var resultService = Service.UserAccessRightTemplateLink(new UserAccessRightTemplateLinkRequest()
            {
                UserId = requestModel.UserId,
                AccessRightTemplateId = requestModel.AccessRightTemplateId
            }, _serviceCallerIdentity);

            if (resultService.ServiceResponse.HasErrors)
            {
                return new UserAccessRightTemplateLinkResponseModel()
                {
                    Success = false,
                    InfoBlock = new InfoBlock()
                    {
                        Code = ApiErrors.ErrorCodeState.UnspecifiedError,
                        UserMessage = $"{string.Join($"; {Environment.NewLine}", resultService.ServiceResponse.Responses.Select(s => s.MessageDetails))}",
                        DeveloperMessage = $"{string.Join($"; {Environment.NewLine}", resultService.ServiceResponse.Responses.Select(s => s.MessageDetails))}"
                    }
                };
            }
            return new UserAccessRightTemplateLinkResponseModel()
            {
                Success = true,
                InfoBlock = new InfoBlock()
                {
                    Code = ApiErrors.ErrorCodeState.Success,
                    UserMessage = resultService.ServiceResponse.Responses[0]?.MessageDetails??"Ok",
                    DeveloperMessage = resultService.ServiceResponse.Responses[0]?.MessageDetails??"Ok"
                }
            };
        }
    }
}