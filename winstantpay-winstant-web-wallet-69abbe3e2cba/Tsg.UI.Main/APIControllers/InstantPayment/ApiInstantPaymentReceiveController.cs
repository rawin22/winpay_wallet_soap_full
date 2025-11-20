using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using Tsg.Business.Model.TSGgpwsbeta;
using Tsg.UI.Main.ApiMethods.Payments;
using TSG.Models.APIModels;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels.InstantPayment;
using Tsg.UI.Main.ApiMethods.AutomaticExchange;
using TSG.ServiceLayer.AutomaticExchange.DependencyLiquidForUserServiceMethods;
using TSG.Models.ServiceModels.AutomaticExchange;
using TSG.ServiceLayer.AutomaticExchange.LiquidCcyListServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidOverDraftUserServiceMethods;
using TSG.ServiceLayer.InstantPayment;
using TSG.Models.DTO.InstantPayment;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Tsg.UI.Main.APIControllers
{
    [ApiFilter]
    public class ApiInstantPaymentReceiveController : ApiController
    {
        private readonly IDependencyLiquidForUserServiceMethods _dependencyLiquidForUserService;
        private readonly ILiquidCcyListServiceMethods _liquidCcyListServiceMethods;
        private readonly ILiquidOverDraftUserServiceMethods _liquidOverDraftUserServiceMethods;
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IInstantPaymentReceiveMethods _instantPaymentReceiveService;
        private readonly IInstantPaymentReceiveMappingMethods _instantPaymentReceiveMappingService;


        private static readonly string cryptoCurrency = ConfigurationManager.AppSettings["cryptoCurrencies"];

        public ApiInstantPaymentReceiveController(IDependencyLiquidForUserServiceMethods dependencyLiquidForUserService, 
            ILiquidCcyListServiceMethods liquidCcyListServiceMethods,
            ILiquidOverDraftUserServiceMethods liquidOverDraftUserServiceMethods,
            IInstantPaymentReceiveMethods instantPaymentReceiveService,
            IInstantPaymentReceiveMappingMethods instantPaymentReceiveMappingService) {
            _dependencyLiquidForUserService = dependencyLiquidForUserService;
            _liquidCcyListServiceMethods = liquidCcyListServiceMethods;
            _liquidOverDraftUserServiceMethods = liquidOverDraftUserServiceMethods;
            _instantPaymentReceiveService = instantPaymentReceiveService;
            _instantPaymentReceiveMappingService = instantPaymentReceiveMappingService;
        }

        public IHttpActionResult Get(Guid id)
        {
            var result = new InstantPaymentReceiveHistoryInfoModel
            {
                Success = false,
                InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty },
                InstantPaymentReceiveDetails = new ApiInstantPaymentReceiveDetailsViewModel()
            };           

            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (!(isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui)))
                    return Unauthorized();

                result.InstantPaymentReceiveDetails = PrepareApiInstantPaymentReceiveDetails(id);

                if (result.InstantPaymentReceiveDetails == null)
                    throw new Exception("Null object");

                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "Ok", UserMessage = "Payment receive successfully retrieved" };
                result.Success = true;
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

        public IHttpActionResult Get()
        {
            var result = new ApiInstantPaymentReceivesListViewModel
            {
                Success = false,
                InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty },
                InstantPaymentReceivesList = new List<ApiInstantPaymentReceiveDetailsViewModel>()
            };

            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (!(isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui)))
                    return Unauthorized();

                result.InstantPaymentReceivesList = PrepareApiInstantPaymentReceivesList();

                if (result.InstantPaymentReceivesList == null)
                    throw new Exception("Null object");

                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "Ok", UserMessage = "Payment receives list successfully retrieved" };
                result.Success = true;
            }
            catch (ApiException apiException)
            {
                result.InfoBlock = apiException.CustomInfoBlock;
            }
            catch (Exception e)
            {
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
            }

            //var result = new ApiInstantPaymentModel.InstantPaymentPageModelResponse
            //{
            //    Success = false,
            //    InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty },
            //    Data = new ApiInstantPaymentModel.SendedDataByInstantPayment()
            //};

            //TSG.Models.APIModels.UserInfo ui;
            //UserRepository userRepository = new UserRepository();
            //try
            //{
            //    IEnumerable<string> outerUserToken;
            //    var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
            //    if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
            //    {
            //        NewInstantPaymentMethods m = new NewInstantPaymentMethods(ui);

            //        result.Data.CurrencyList = m.PrepareAllAvailablePaymentCurrencies().Select(s => s.Value).ToList();
            //        result.Data.ToAlias = m.PreparePriorUsedAliases().Select(s => s.Value).ToList();
            //        result.Data.FromAlias = m.PrepareAccountAliases().Select(s => s.Value).ToList();
            //        result.Success = true;
            //        result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK", UserMessage = "Data for instant payment selected correct" };
            //    }
            //    else
            //    {
            //        result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.CheckUserError, DeveloperMessage = "Data for user not correct", UserMessage = "Data for user not correct" };
            //    }
            //}
            //catch (ApiException apiException)
            //{
            //    result.InfoBlock = apiException.CustomInfoBlock;
            //}
            //catch (Exception e)
            //{
            //    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
            //}
            // return Ok(PrepareApiInstantPaymentReceivesList());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]ApiInstantPaymentReceiveModel.ApiCreateInstantPaymentReceiveRequest  model)
        {
            var result = new ApiInstantPaymentReceiveModel.ApiCreateInstantPaymentReceiveResponse()
            {
                Success = false,
                InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty },
                InstantPaymentReceiveDetails = new ApiInstantPaymentReceiveDetailsViewModel()

            };
            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();

            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (!(isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui)))
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.CheckUserError, DeveloperMessage = "Data for user not correct", UserMessage = "Data for user not correct" };
                    return Ok(result);
                }
                
                bool isDefValue = false;
                var listSpecParams = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(w => Attribute.IsDefined(w, typeof(RequiredAttribute)));
                
                foreach (var propertyInfo in listSpecParams)
                {
                    var x = CheckValueByType.GetDefault(propertyInfo.PropertyType);
                    var @equals = propertyInfo.GetValue(model) is string ? String.IsNullOrEmpty(propertyInfo.GetValue(model).ToString()) : Equals(x, propertyInfo.GetValue(model));
                    isDefValue |= @equals;
                }
                if (isDefValue)
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "InfoBlock. can't not be value is null", UserMessage = "InfoBlock. Value is null" };
                    return Ok(result);
                }
                Guid receiveId;

                InstantPaymentReceiveDto receiveDto = new InstantPaymentReceiveDto();

                receiveDto.TsgUserGuId = new Guid(ui.UserId);
                receiveDto.Alias = model.Alias;
                receiveDto.Currency = model.Currency;
                receiveDto.Amount = model.Amount;
                receiveDto.Invoice = model.Invoice;

                InstantPaymentReceiveMemoViewModel receiveMemoModel = new InstantPaymentReceiveMemoViewModel();
                receiveMemoModel.Memo = model.Memo;
                receiveMemoModel.Name = model.Name;
                receiveMemoModel.Address = model.Address;
                receiveMemoModel.Email = model.Email;
                receiveMemoModel.KycId = model.KycId;


                receiveDto.Memo = JsonConvert.SerializeObject(receiveMemoModel);

                _instantPaymentReceiveService.Insert(receiveDto);
                receiveId = receiveDto.Id;

                

                receiveDto.ShortenedUrl = await getShortenedUrl(receiveDto.Id);
                _instantPaymentReceiveService.Update(receiveDto);

                result.InstantPaymentReceiveDetails.InstantPaymentReceiveId = receiveDto.Id;
                result.InstantPaymentReceiveDetails.Alias = receiveDto.Alias;
                result.InstantPaymentReceiveDetails.Currency = model.Currency;
                result.InstantPaymentReceiveDetails.Amount = model.Amount;
                result.InstantPaymentReceiveDetails.Invoice = model.Invoice;
                result.InstantPaymentReceiveDetails.Memo = model.Memo;
                result.InstantPaymentReceiveDetails.Name = model.Name;
                result.InstantPaymentReceiveDetails.Address = model.Address;
                result.InstantPaymentReceiveDetails.Email = model.Email;
                result.InstantPaymentReceiveDetails.KycId = model.KycId;
                result.InstantPaymentReceiveDetails.ShortenedUrl = receiveDto.ShortenedUrl;
                result.InstantPaymentReceiveDetails.CreatedDate = receiveDto.CreatedDate;

                result.Success = true;
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK", UserMessage = "Instant payment receive was created successfully" };
            }
            catch (Exception e)
            {
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
            }
            return Ok(result);
        }

        //public IHttpActionResult Put([FromBody]string value)
        //{
        //    return Ok("This is Put");
        //}

        //[HttpPut]
        public IHttpActionResult Put([FromBody]ApiInstantPaymentReceiveModel.ApiEditInstantPaymentReceiveRequest model)
        {
            var result = new ApiInstantPaymentReceiveModel.ApiEditInstantPaymentReceiveResponse()
            {
                Success = false,
                InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "Empty result. Method started.", UserMessage = String.Empty },
                InstantPaymentReceiveDetails = new ApiInstantPaymentReceiveDetailsViewModel()

            };
            TSG.Models.APIModels.UserInfo ui;
            UserRepository userRepository = new UserRepository();

            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (!(isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui)))
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.CheckUserError, DeveloperMessage = "Data for user not correct", UserMessage = "Data for user not correct" };
                    return Ok(result);
                }

                bool isDefValue = false;
                var listSpecParams = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(w => Attribute.IsDefined(w, typeof(RequiredAttribute)));

                foreach (var propertyInfo in listSpecParams)
                {
                    var x = CheckValueByType.GetDefault(propertyInfo.PropertyType);
                    var @equals = propertyInfo.GetValue(model) is string ? String.IsNullOrEmpty(propertyInfo.GetValue(model).ToString()) : Equals(x, propertyInfo.GetValue(model));
                    isDefValue |= @equals;
                }
                if (isDefValue)
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.EmptyResult, DeveloperMessage = "InfoBlock. can't not be value is null", UserMessage = "InfoBlock. Value is null" };
                    return Ok(result);
                }

                // InstantPaymentReceiveDto receiveDto = new InstantPaymentReceiveDto();
                var receiveDto = _instantPaymentReceiveService.GetAll().Obj.Where(r => r.Id == model.InstantPaymentReceiveId).FirstOrDefault();
                if (receiveDto != null)
                {
                    receiveDto.Alias = model.Alias;
                    receiveDto.Currency = model.Currency;
                    receiveDto.Amount = model.Amount;
                    receiveDto.Invoice = model.Invoice;

                    InstantPaymentReceiveMemoViewModel receiveMemoModel = new InstantPaymentReceiveMemoViewModel();
                    receiveMemoModel.Memo = model.Memo;
                    receiveMemoModel.Name = model.Name;
                    receiveMemoModel.Address = model.Address;
                    receiveMemoModel.Email = model.Email;
                    receiveMemoModel.KycId = model.KycId;

                    receiveDto.Memo = JsonConvert.SerializeObject(receiveMemoModel);

                    _instantPaymentReceiveService.Update(receiveDto);

                    result.InstantPaymentReceiveDetails.InstantPaymentReceiveId = model.InstantPaymentReceiveId;
                    result.InstantPaymentReceiveDetails.Alias = model.Alias;
                    result.InstantPaymentReceiveDetails.Currency = model.Currency;
                    result.InstantPaymentReceiveDetails.Amount = model.Amount;
                    result.InstantPaymentReceiveDetails.Invoice = model.Invoice;
                    result.InstantPaymentReceiveDetails.Memo = model.Memo;
                    result.InstantPaymentReceiveDetails.Name = model.Name;
                    result.InstantPaymentReceiveDetails.Address = model.Address;
                    result.InstantPaymentReceiveDetails.Email = model.Email;
                    result.InstantPaymentReceiveDetails.KycId = model.KycId;
                    result.InstantPaymentReceiveDetails.ShortenedUrl = receiveDto.ShortenedUrl;
                    result.InstantPaymentReceiveDetails.CreatedDate = receiveDto.CreatedDate;

                    result.Success = true;
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK", UserMessage = "Instant payment receive was edited succesifully" };
                }
                else
                {
                    result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, DeveloperMessage = "OK", UserMessage = "receiveDto is null" };
                }
            }
            catch (Exception e)
            {
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "Editing Instant Payment Receive failed. There is some problem with your query. Please try again" };
            }
            return Ok(result);
        }

        [NonAction]
        private IList<InstantPaymentViewModel> PrepareIntantPaymentsByReceiveId(Guid receiveId)
        {
            var mappings = _instantPaymentReceiveMappingService.GetByInstantPaymentReceiveId(receiveId).Obj;

            var instantPayments = new InstantPaymentSearchViewModel();

            if (mappings != null && mappings.Count > 0)
            {
                instantPayments.PrepareInstantPayments();
                instantPayments.Payments = instantPayments.Payments.Where(p => mappings.Any(z => p.PaymentId == z.InstantPaymentId)).ToList();
            }

            return instantPayments.Payments;

            //        var CategoriesNotOnLst = Categories
            //.Where(x => !lst.Any(z => x.Description == z))
            //.ToList();

        }

        [NonAction]
        private ApiInstantPaymentReceiveDetailsViewModel PrepareApiInstantPaymentReceiveDetails(Guid instantPaymentReceiveId)
        {
            var model = new ApiInstantPaymentReceiveDetailsViewModel();

            var receive = _instantPaymentReceiveService.GetAll().Obj.Where(r => r.Id == instantPaymentReceiveId).FirstOrDefault();

            if (receive != null)
            {
                model.InstantPaymentReceiveId = receive.Id;
                model.Alias = receive.Alias;
                model.Currency = receive.Currency;
                model.Amount = receive.Amount;
                model.Invoice = receive.Invoice;
                // model.Memo = receive.Memo;
                model.CreatedDate = receive.CreatedDate;

                InstantPaymentReceiveMemoViewModel memo = JsonConvert.DeserializeObject<InstantPaymentReceiveMemoViewModel>(receive.Memo);

                model.Memo = memo.Memo;
                model.Name = memo.Name;
                model.Address = memo.Address;
                model.Email = memo.Email;
                model.KycId = memo.KycId;

                model.ShortenedUrl = receive.ShortenedUrl;
            }

            // model.InstantPayments = PrepareIntantPaymentsByReceiveId(instantPaymentReceiveId);

            return model;
        }

        [NonAction]
        private IList<ApiInstantPaymentReceiveDetailsViewModel> PrepareApiInstantPaymentReceivesList()
        {
            IList<ApiInstantPaymentReceiveDetailsViewModel> list = new List<ApiInstantPaymentReceiveDetailsViewModel>();

            // var userId = new Guid(AppSecurity.CurrentUser.UserId);
            //  var receives = _instantPaymentReceiveService.GetByUser(userId);
            var receives = _instantPaymentReceiveService.GetAll();
            // var model = new InstantPaymentReceiveSearchViewModel();
            //if (model == null)
            //    model = new InstantPaymentReceiveSearchViewModel();
            if (receives != null && receives.Obj != null)
            {
                foreach (var receive in receives.Obj)
                {
                    ApiInstantPaymentReceiveDetailsViewModel instantPaymentReceive = new ApiInstantPaymentReceiveDetailsViewModel()
                    {
                        InstantPaymentReceiveId = receive.Id,
                        Alias = receive.Alias,
                        Amount = receive.Amount,
                        Currency = receive.Currency,
                        CreatedDate = receive.CreatedDate,
                    };

                    InstantPaymentReceiveMemoViewModel memo = JsonConvert.DeserializeObject<InstantPaymentReceiveMemoViewModel>(receive.Memo);

                    instantPaymentReceive.Memo = memo.Memo;
                    instantPaymentReceive.Name = memo.Name;
                    instantPaymentReceive.Address = memo.Address;
                    instantPaymentReceive.Email = memo.Email;
                    instantPaymentReceive.KycId = memo.KycId;

                    instantPaymentReceive.ShortenedUrl = receive.ShortenedUrl;

                    list.Add(instantPaymentReceive);
                }
            }
            return list;

        }

        [NonAction]
        private async Task<string>  getShortenedUrl(Guid receiveId)
        {
            string shortenedUrl = string.Empty;

            using (var client = new HttpClient())
            {
                // New code:
                client.BaseAddress = new Uri(AppSettingHelper.GetUrlShortenerBaseAddress());
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // HTTP POST
                // var request = new ShortenedUrlRequest() { LongUrl = "http://localhost:4988/Payment/Create?receiveId=" + receiveId };
                var ShortenRequest = new ShortenedUrlRequest() { LongUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/Payment/Create?receiveId=" + receiveId };
                //request.GetRequestContext().VirtualPathRoo
                //            var domain = string.IsNullOrEmpty(request.Domain) ? this.Request.Host.ToString() + Url.Content("~") : request.Domain;
                //HttpContext.Current.Request.Url.Host

                var response = await client.PostAsJsonAsync("api/ShortenedUrls", ShortenRequest);
                if (response.IsSuccessStatusCode)
                {
                    // Get the URI of the created resource.
                    Uri gizmoUrl = response.Headers.Location;
                    ShortenedUrlResponse res = await response.Content.ReadAsAsync<ShortenedUrlResponse>();
                    shortenedUrl = res.ShortUrl;
                }
            }


            return shortenedUrl;
        }

    }
}
