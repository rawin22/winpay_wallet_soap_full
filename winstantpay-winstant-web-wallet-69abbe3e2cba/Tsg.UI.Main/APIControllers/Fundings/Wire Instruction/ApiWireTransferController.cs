using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http.Formatting;
using System.Text;
using System.Web.Hosting;
using System.Web.Http;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.APIModels.Fundings.Wire_Instructions;
using TSG.Models.DTO;
using TSG.Models.Enums;
using TSG.Models.ServiceModels;
using TSG.ServiceLayer.Interfaces.Fundings;
using WinstantPay.Common.Extension;

namespace Tsg.UI.Main.APIControllers.Fundings.Wire_Instruction
{
    [ApiFilter]
    public class ApiWireTransferController : ApiController
    {
        private readonly IFundingsService _fundingsService;
        public ApiWireTransferController(IFundingsService fundingsService)
        {
            _fundingsService = fundingsService;
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            var result = new NewWireTransferInfo
            {
                Success = false,
                InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty },
                ListOfLinkedBankAndCcy = new List<BankCurrencyData>()
            };

            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    CurrencyRepository curRepo = new CurrencyRepository();
                    var bankCurrencies = curRepo.GetBankCurrencies();

                    result.ListOfLinkedBankAndCcy.AddRange(bankCurrencies.Select(s => new BankCurrencyData()
                    {
                        Id = s.BankCurrencyId,
                        BankCcyName = s.BankCurrencyName
                    }));
                    result.FullName = $"{ui.FirstName} {ui.LastName}";
                    result.Success = true;
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK", UserMessage = "Data for instant payment selected correct" };
                }
                else
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.CheckUserError, DeveloperMessage = "Data for user not correct", UserMessage = "Data for user not correct" };
                }
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

        /// <summary>
        /// Save change by wire transfer method
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Post([FromBody] AddFundsWire model)
        {
            var result = new StandartResponse()
            {
                Success = false,
                InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty },
            };

            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    if (!ModelState.IsValid)
                    {
                        StringBuilder sb = new StringBuilder();
                        ModelState.Select(s => s.Value.Errors).ToList().ForEach(f => { sb.AppendLine(f.ToString()); });
                        result.InfoBlock = new InfoBlock { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = sb.ToString(), UserMessage = "Your entered parameters is not correct" };
                        return Content(HttpStatusCode.BadRequest, result, new JsonMediaTypeFormatter(), "application/json");
                    }

                    model.AddFundsWire_PaymentDate = DateTime.ParseExact(model.AddFundsWire_PaymentDateString.Replace(".", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    string path = HostingEnvironment.MapPath("~/Uploads/");

                    if (model.AddFundsWire_ParentId == default)
                    {
                        if (!String.IsNullOrEmpty(model.ImageInBase64))
                        {
                            FilesExtensions.SaveSystemFile(model.ImageInBase64, model.AddFundsWire_FileName, path, out string fileName, out string filePath);
                            model.AddFundsWire_FileName = fileName;
                            model.AddFundsWire_FilePath = filePath;
                        }

                        var insertRes = _fundingsService.InsertWireTransfer(model, ui.UserName);
                        var mail = new ContactUsRepository();

                        string text = String.Format(@GlobalRes.SysProperty_EmailForUploadProof_AdminText, $"{insertRes.Obj.AddFundsWire_ProofDocId}", $"{UiHelper.PrepareAvailableBankCurrencies().FirstOrDefault(f => f.Value == insertRes.Obj.AddFundsWire_BankCcyId.ToString())?.Text}",
                            insertRes.Obj.AddFundsWire_Fundings.Fundings_Amount, insertRes.Obj.AddFundsWire_PaymentDate.ToString("MMM dd, yyyy"), insertRes.Obj.AddFundsWire_CustName, insertRes.Obj.AddFundsWire_BankName, insertRes.Obj.AddFundsWire_LastFourDigits, insertRes.Obj.AddFundsWire_Other);
                        string mailSendRes;

                        UserRepository userRepo = new UserRepository();
                        var props = userRepo.SysValues();
                        SettingClass.SetSysEmailUploadProof(props.FirstOrDefault(f => f.PropertyName == "adminNotificationMailForUploadProof")?.PropertyValue.ToString());

                        if (mail.RegisterMail(0, ConfigurationManager.AppSettings["NoReplyMail"], SettingClass.SysNotificationEmailInUploadProof, "System Message", text, new string[] { model.AddFundsWire_FilePath }, "AdminMessage", out mailSendRes, out var mailId))
                            EmailExtension.EmailService.SendEmail("System Message", text, ConfigurationManager.AppSettings["NoReplyMail"], ConfigurationManager.AppSettings["OrganizationName"], SettingClass.SysNotificationEmailInUploadProof, "", null, new[] { model.AddFundsWire_FilePath });
                        
                        result.Success = insertRes.Success;
                        result.InfoBlock = new InfoBlock
                        {
                            Code = insertRes.Success ? ApiErrors.ErrorCodeState.Success : ApiErrors.ErrorCodeState.UnspecifiedError,
                            UserMessage = insertRes.Success ? "Your funding created successfully" : "Your funding was not created successfully",
                            DeveloperMessage = insertRes.Success ? "Funding was created successfully" : "Funding was not be created successfully"
                        };
                    }
                    else
                    {
                        var currentFundData = _fundingsService.GetWireFundingById(model.AddFundsWire_ParentId);
                        if (currentFundData.Obj.AddFundsWire_Fundings.Fundings_StatusByFund >
                            (int) FundingStatus.Pending)
                        {
                            result.Success = true;
                            result.InfoBlock = new InfoBlock
                            {
                                Code = ApiErrors.ErrorCodeState.Success,
                                UserMessage = $"Your funding set in status {EnumHelper<FundingStatus>.GetDisplayValue((FundingStatus)currentFundData.Obj.AddFundsWire_Fundings.Fundings_StatusByFund)}, because update does not allow",
                                DeveloperMessage = $"Funding set in status {EnumHelper<FundingStatus>.GetDisplayValue((FundingStatus)currentFundData.Obj.AddFundsWire_Fundings.Fundings_StatusByFund)}, because update does not allow"
                            };
                        }
                        else if(currentFundData.Obj.AddFundsWire_Fundings.Fundings_IsDeleted)
                        {
                            result.Success = true;
                            result.InfoBlock = new InfoBlock
                            {
                                Code = ApiErrors.ErrorCodeState.Success,
                                UserMessage = "Your funding deleted, because update does not allow",
                                DeveloperMessage = "Funding is deleted, because update does not allow"
                            };
                        }
                        else
                        {
                            if ((!String.IsNullOrEmpty(model.ImageInBase64)) && (!String.IsNullOrEmpty(model.ImageInBase64.Trim())))
                            {
                                FilesExtensions.SaveSystemFile(model.ImageInBase64, model.AddFundsWire_FileName, path,
                                    out string fileName, out string filePath);
                                model.AddFundsWire_FileName = fileName;
                                model.AddFundsWire_FilePath = filePath;
                            }
                            else
                            {
                                model.AddFundsWire_FilePath = currentFundData.Obj.AddFundsWire_FilePath;
                                model.AddFundsWire_FileName = currentFundData.Obj.AddFundsWire_FileName;
                            }
                            var updateRes =
                                _fundingsService.UpdateWireTransfer(model, ui.UserName, (int) FundingStatus.Pending);
                            result.Success = updateRes.Success;
                            result.InfoBlock = new InfoBlock
                            {
                                Code = updateRes.Success
                                    ? ApiErrors.ErrorCodeState.Success
                                    : ApiErrors.ErrorCodeState.UnspecifiedError,
                                UserMessage =
                                    updateRes.Success
                                        ? "Your funding updated successfully"
                                        : "Your funding was not updated successfully",
                                DeveloperMessage =
                                    updateRes.Success
                                        ? "Funding was updated successfully"
                                        : "Funding was not be updated successfully"
                            };
                        }
                    }
                }
                else return Unauthorized();
            }
            catch (ApiException apiException)
            {
                result.InfoBlock = apiException.CustomInfoBlock;
                return Content(HttpStatusCode.BadRequest, result);
            }
            catch (Exception e)
            {
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
                return Content(HttpStatusCode.BadRequest, result);
            }

            return Ok(result);
        }
    }
}