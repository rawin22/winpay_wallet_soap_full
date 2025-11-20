using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.Business.Model.TSGgpwsbeta;
using Newtonsoft.Json.Linq;
using log4net;

namespace Tsg.Business.Model.Classes
{
	public class IgpService
	{
		readonly ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		//private const string LOGIN_ID = "channthoeun";
		//private const string PASSWORD = "Thoeun123456";
		private static readonly string CallerId = ConfigurationManager.AppSettings["callerId"] ?? string.Empty;
		private static readonly string BlockedCurrency = ConfigurationManager.AppSettings["blockedCurrencies"] ?? string.Empty;

		//private const string CUSTOMER_ID = "315622a5-fa28-e211-96b3-002590067f61";

		private string LoginId { get; set; }
		private string Password { get; set; }
		private string CustomerId { get; set; }
		private string AccessToken { get; set; }

		#region PipIt integration Api
		/************************************************/
		/*************  PipIt Integration  **************/
		/************************************************/
		private static readonly string PipitLogin = ConfigurationManager.AppSettings["pipitLogin"] ?? string.Empty;
		private static readonly string PipitPassword = ConfigurationManager.AppSettings["pipitPassword"] ?? string.Empty;
		private static readonly string PipitAddress = ConfigurationManager.AppSettings["pipitAddress"] ?? string.Empty;

		private string PipitSessionName { get; set; }
		private string PipitSessionId { get; set; }

		/************************************************/
		public List<HttpCookie> LoginInPipIt()
		{
			try
			{
				var bodyParameters = new List<KeyValuePair<string, string>>();
				bodyParameters.Add(new KeyValuePair<string, string>("userName", PipitLogin));
				bodyParameters.Add(new KeyValuePair<string, string>("password", PipitPassword));
				CookieContainer cookies = new CookieContainer();
				HttpClientHandler handler = new HttpClientHandler();
				HttpCookieCollection httpCookieCollection = new HttpCookieCollection();
				handler.CookieContainer = cookies;

				using (var httpClient = new HttpClient(handler))
				{
					httpClient.BaseAddress = new Uri(PipitAddress + "/login");
					bodyParameters.Add(new KeyValuePair<string, string>("userName", PipitLogin));
					bodyParameters.Add(new KeyValuePair<string, string>("password", PipitPassword));
					using (var content = new FormUrlEncodedContent(bodyParameters))
					{
						content.Headers.Clear();
						content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
						HttpResponseMessage response = httpClient.PostAsync(httpClient.BaseAddress, content).Result;
						IEnumerable<Cookie> responseCookies = cookies.GetCookies(new Uri(PipitAddress)).Cast<Cookie>();
						List<HttpCookie> httpCookie = new List<HttpCookie>();

						foreach (var cookie in responseCookies)
						{
							PipitSessionName = cookie.Name;
							PipitSessionId = cookie.Value;
							HttpCookie hc = new HttpCookie(cookie.Name);

							hc.Domain = cookie.Domain;
							hc.Expires = cookie.Expires;
							hc.Name = cookie.Name;
							hc.Path = cookie.Path;
							hc.Secure = cookie.Secure;
							hc.Value = cookie.Value;

							httpCookie.Add(hc);
						}
						return httpCookie;
					}
				}

			}
			catch (Exception e) { }

			return null;
		}

		public string GetOrdersByType(HttpCookie cook, string type)
		{
			try
			{
				CookieContainer cookies = new CookieContainer();
				cookies.Add(new Cookie(cook.Name, cook.Value, cook.Path, cook.Domain));
				HttpClientHandler handler = new HttpClientHandler();
				handler.CookieContainer = cookies;
				using (var httpClient = new HttpClient(handler))
				{
					httpClient.BaseAddress = new Uri(PipitAddress + "/api/merchant/orders/" + type);
					var response = httpClient.GetAsync(httpClient.BaseAddress);
					var contentBody = response.Result.Content.ReadAsStringAsync().Result;
					return contentBody;
				}

			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
			return String.Empty;
		}

		public string PostCreateOrders(HttpCookie cook, string body)
		{
			try
			{
				CookieContainer cookies = new CookieContainer();
				cookies.Add(new Cookie(cook.Name, cook.Value, cook.Path, cook.Domain));
				HttpClientHandler handler = new HttpClientHandler();
				handler.CookieContainer = cookies;
				using (var httpClient = new HttpClient(handler))
				{
					httpClient.BaseAddress = new Uri(PipitAddress + "/api/order");
					var response = httpClient.PostAsync(httpClient.BaseAddress, new StringContent(body, Encoding.UTF8, "application/json"));
					var contentBody = response.Result.Content.ReadAsStringAsync().Result;
					return contentBody;
				}

			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
			return String.Empty;
		}

		public bool PipItVerification()
		{
			bool isVerified = false;




			return isVerified;
		}

		/************************************************/

		#endregion


		public CustomerCreateFromTemplateResponse CustomerCreateFromTemplate(CustomerCreateFromTemplateRequest request)
		{
			//request.ServiceCallerIdentity = GetServiceCallerIdentity();
			return _service.CustomerCreateFromTemplate(request);
		}


		public CustomerUserCreateResponse CustomerUserCreate(CustomerUserCreateRequest request)
		{
			//request.ServiceCallerIdentity = GetServiceCallerIdentity();
			return _service.CustomerUserCreate(request);
		}

		public UserAccessRightTemplateLinkResponse UserAccessRightTemplateLink(UserAccessRightTemplateLinkRequest request)
		{
			//request.ServiceCallerIdentity = GetServiceCallerIdentity();
			return _service.UserAccessRightTemplateLink(request);
		}

		public CustomerCreateFromTemplateResponse CustomerCreateFromTemplate(CustomerCreateFromTemplateRequest request, ServiceCallerIdentity callerIdentity)
		{
			//request.ServiceCallerIdentity = callerIdentity;
			return _service.CustomerCreateFromTemplate(request);
		}

		public CustomerUserCreateResponse CustomerUserCreate(CustomerUserCreateRequest request, ServiceCallerIdentity callerIdentity)
		{
			//request.ServiceCallerIdentity = callerIdentity;
			return _service.CustomerUserCreate(request);
		}

		public UserAccessRightTemplateLinkResponse UserAccessRightTemplateLink(UserAccessRightTemplateLinkRequest request, ServiceCallerIdentity callerIdentity)
		{
			//request.ServiceCallerIdentity = callerIdentity;
			return _service.UserAccessRightTemplateLink(request);
		}

		private readonly GPWebService1Client _service;

		public IgpService()
		{
			_service = new GPWebService1Client();

			//System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		}

		public IgpService(string accessToken)
			: this()
		{
			this.AccessToken = accessToken;

			// Remove exiting bearer token
			RemoveExistingBearerTokenEndpointBehavior();

			_service.Endpoint.Behaviors.Add(new BearerTokenEndpointBehavior(accessToken));
		}

		public IgpService(string accessToken, string customerId)
			: this()
		{
			this.AccessToken = accessToken;
			this.CustomerId = customerId;

			// Remove exiting bearer token
			RemoveExistingBearerTokenEndpointBehavior();

			// Apply the BearerTokenEndpointBehavior to the client's endpoint
			_service.Endpoint.Behaviors.Add(new BearerTokenEndpointBehavior(accessToken));


			//using (new OperationContextScope(_service.InnerChannel))
			//{
			//    // Add a HTTP Header to an outgoing request
			//    HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
			//    requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", AccessToken);
			//    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
			//}
		}

		public IgpService(string username, string password, string customerId)
			: this()
		{
			this.LoginId = username;
			this.Password = password;
			this.CustomerId = customerId;
			var response = Authenticate(username, password);
			if (!response.ServiceResponse.HasErrors)
			{
				AccessToken = response.AccessToken;

				// Remove exiting bearer token
				RemoveExistingBearerTokenEndpointBehavior();

				//// Retrieve the existing BearerTokenEndpointBehavior
				//BearerTokenEndpointBehavior existingBehavior = null;
				//foreach (var behavior in _service.Endpoint.Behaviors)
				//{
				//    if (behavior is BearerTokenEndpointBehavior)
				//    {
				//        existingBehavior = (BearerTokenEndpointBehavior)behavior;
				//        break;
				//    }
				//}

				//// Check if the existing behavior was found and remove it
				//if (existingBehavior != null)
				//{
				//    _service.Endpoint.Behaviors.Remove(existingBehavior);
				//}

				//_service.Endpoint.Behaviors.Add(new BearerTokenEndpointBehavior(AccessToken));
			}

		}

		public void RemoveExistingBearerTokenEndpointBehavior()
		{
			// Find the existing BearerTokenEndpointBehavior
			BearerTokenEndpointBehavior existingBehavior = _service.Endpoint.Behaviors
				.OfType<BearerTokenEndpointBehavior>()
				.FirstOrDefault();

			// Check if the existing behavior was found and remove it
			if (existingBehavior != null)
			{
				_service.Endpoint.Behaviors.Remove(existingBehavior);
			}
		}

		public void SetUserCredentials(string username, string password, string customerId)
		{
			this.LoginId = username;
			this.Password = password;
			this.CustomerId = customerId;
		}

		private ServiceCallerIdentity GetServiceCallerIdentity()
		{
			return new ServiceCallerIdentity()
			{
				LoginId = LoginId,
				Password = Password,
				ServiceCallerId = CallerId
			};
		}

		private ServiceCallerIdentity GetServiceCallerIdentity(string login, string password)
		{
			this.LoginId = login;
			this.Password = password;
			return GetServiceCallerIdentity();
		}

		public Guid GetCustomerIdGuid
		{
			get
			{
				return new Guid(CustomerId);
			}
		}

		public GetCustomerAccountBalancesResponse GetAccountBalances()
		{
			GetCustomerAccountBalancesRequest request = new GetCustomerAccountBalancesRequest
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				CustomerId = CustomerId
			};

			//using (new OperationContextScope(_service.InnerChannel))
			//{
			//    // Add a HTTP Header to an outgoing request
			//    HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
			//    requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", this.AccessToken);
			//    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
			//    var response = _service.GetCustomerAccountBalances(request);
			//    response.Balances = response.Balances.Where(w => !BlockedCurrency.Contains(w.CCY)).ToArray();
			//    return response;
			//}

			var response = _service.GetCustomerAccountBalances(request);
			response.Balances = response.Balances.Where(w => !BlockedCurrency.Contains(w.CCY)).ToArray();
			return response;
		}

		public InstantPaymentSearchResponse GetLatestInstantPayments(int count, InstantPaymentStatus status = InstantPaymentStatus.All)
		{
			int pageIndex = 0, pageSize = count, amountMin = 0, amountMax = 100000000;
			IFormatProvider cultureThai = CultureInfo.CreateSpecificCulture("th-TH");
			IFormatProvider cultureEng = CultureInfo.CreateSpecificCulture("en-US");

			var valueDateMin = DateTime.Now.AddYears(-2);
			var valueDateMax = DateTime.Now.AddYears(2);

			var c = CultureInfo.CurrentCulture;
			if (c.ToString() == cultureThai.ToString())
			{
				ThaiBuddhistCalendar thaiCalendar = new ThaiBuddhistCalendar();
				valueDateMin = thaiCalendar.ToDateTime(valueDateMin.Year, valueDateMin.Month, valueDateMin.Day, 0, 0, 0, 0);
				valueDateMax = thaiCalendar.ToDateTime(valueDateMax.Year, valueDateMax.Month, valueDateMax.Day, 0, 0, 0, 0);
			}

			var sortDirection = InstantPaymentSortDirection.Descending;
			var sortBy = InstantPaymentSearchSortBy.CreatedTime;
			InstantPaymentSearchResponse response = new InstantPaymentSearchResponse();
			InstantPaymentSearchRequest request = new InstantPaymentSearchRequest
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				PageIndex = pageIndex,
				PageSize = pageSize,
				SortBy = sortBy,
				SortDirection = sortDirection,
				SearchOptions = new InstantPaymentSearchOptions
				{
					PaymentReference = "",
					FromCustomerName = "",
					ToCustomerName = "",
					AmountMin = amountMin,
					AmountMax = amountMax,
					CCY = "",
					Status = status,
					ValueDateMin = valueDateMin.ToString("yyyy-MM-dd"),
					ValueDateMax = valueDateMax.ToString("yyyy-MM-dd"),
				}
			};
			try
			{
				//using (new OperationContextScope(_service.InnerChannel))
				//{
				//    // Add a HTTP Header to an outgoing request
				//    HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
				//    requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", this.AccessToken);
				//    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
				//    response = _service.InstantPaymentSearch(request);
				//    response.Payments = response.Payments.Where(w => !BlockedCurrency.Contains(w.CCY)).ToArray();
				//}
				response = _service.InstantPaymentSearch(request);
				response.Payments = response.Payments.Where(w => !BlockedCurrency.Contains(w.CCY)).ToArray();
			}
			catch (Exception e)
			{
				response.ServiceResponse.HasErrors = true;
			}

			return response;
		}

		public BankUserSearchResponse GetBankUsersSearch(int count)
		{
			int pageIndex = 0, pageSize = count, amountMin = 0, amountMax = 100000000;
			var valueDateMin = DateTime.Now.AddYears(-2);
			var valueDateMax = DateTime.Now.AddYears(2);
			var sortDirection = BankUserSearchSortDirection.Descending;
			var sortBy = BankUserSearchSortBy.UserName;
			var status = InstantPaymentStatus.All;
			BankUserSearchResponse response = new BankUserSearchResponse();
			BankUserSearchRequest request = new BankUserSearchRequest
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				PageIndex = pageIndex,
				PageSize = pageSize,
				SortBy = sortBy,
				SortDirection = sortDirection,
				SearchOptions = new BankUserSearchOptions()
				{
					UserName = String.Empty,
					FirstName = String.Empty,
					LastName = String.Empty,
				}
			};
			try
			{
				response = _service.BankUserSearch(request);
			}
			catch (Exception e)
			{
				response.ServiceResponse.HasErrors = true;
			}

			return response;
		}

		public CustomerUserSearchResponse GetCustomerUsersSearch(int count)
		{
			int pageIndex = 0, pageSize = count, amountMin = 0, amountMax = 100000000;
			var valueDateMin = DateTime.Now.AddYears(-2);
			var valueDateMax = DateTime.Now.AddYears(2);
			var sortDirection = CustomerUserSortDirection.Descending;
			var sortBy = "CREATEDTIME";
			var status = InstantPaymentStatus.All;
			CustomerUserSearchResponse response = new CustomerUserSearchResponse();
			CustomerUserSearchRequest request = new CustomerUserSearchRequest
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				PageIndex = pageIndex,
				PageSize = pageSize,
				SortBy = sortBy,
				SortDirection = sortDirection,
				SearchOptions = new CustomerUserSearchOptions()
				{
					UserName = String.Empty,
					FirstName = String.Empty,
					LastName = String.Empty,
					CustomerName = String.Empty
				}
			};
			try
			{
				response = _service.CustomerUserSearch(request);
			}
			catch (Exception e)
			{
				response.ServiceResponse.HasErrors = true;
			}

			return response;
		}

		public CustomerUserSearchResponse SearchCustomerUsers(string customerName, string firstName, string lastName, string userName, string wkycId)
		{
			int pageIndex = 0, pageSize = 100;
			var sortDirection = CustomerUserSortDirection.Descending;
			var sortBy = "CreatedTime";
			CustomerUserSearchResponse response = new CustomerUserSearchResponse();
			CustomerUserSearchRequest request = new CustomerUserSearchRequest
			{
				PageIndex = pageIndex,
				PageSize = pageSize,
				SortBy = sortBy,
				SortDirection = sortDirection,
				SearchOptions = new CustomerUserSearchOptions()
				{
					UserName = userName,
					FirstName = firstName,
					LastName = lastName,
					CustomerName = customerName,
					WKYCId = wkycId
				}
			};

			try
			{
				response = _service.CustomerUserSearch(request);
			}
			catch (Exception e)
			{
				response.ServiceResponse.HasErrors = true;
			}

			return response;
		}

		public CustomerUserSearchResponse GetCustomerUsersSearch(string userName)
		{
			int pageIndex = 0, pageSize = 100;
			var sortDirection = CustomerUserSortDirection.Descending;
			var sortBy = "CreatedTime";
			CustomerUserSearchResponse response = new CustomerUserSearchResponse();
			CustomerUserSearchRequest request = new CustomerUserSearchRequest
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				PageIndex = pageIndex,
				PageSize = pageSize,
				SortBy = sortBy,
				SortDirection = sortDirection,
				SearchOptions = new CustomerUserSearchOptions()
				{
					UserName = userName,
					FirstName = String.Empty,
					LastName = String.Empty,
					CustomerName = String.Empty,
					WKYCId = string.Empty
				}
			};
			_logger.Debug("GetCustomerUsersSearch, request: " + JsonConvert.SerializeObject(request));

			try
			{
				if (string.IsNullOrEmpty(AccessToken))
				{
					var banker_authenticate_response = AuthenticateBankerCredential();
					if (!banker_authenticate_response.ServiceResponse.HasErrors)
					{
						AccessToken = banker_authenticate_response.AccessToken;
					}
				}

				using (new OperationContextScope(_service.InnerChannel))
				{
					//Add a HTTP Header to an outgoing request
					HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
					requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", AccessToken);
					OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
					response = _service.CustomerUserSearch(request);
				}

			}
			catch (Exception e)
			{
				response.ServiceResponse.HasErrors = true;
			}

			return response;
		}

		public CurrencyListGetResponse GetPaymentCurrencies()
		{
			CurrencyListGetRequest request = new CurrencyListGetRequest
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity()
			};

			//using (new OperationContextScope(_service.InnerChannel))
			//{
			//    //Add a HTTP Header to an outgoing request
			//    HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
			//    requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", this.AccessToken);
			//    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
			//    var response = _service.CurrencyListGetPaymentCurrencies(request);
			//    response.Currencies = response.Currencies.Where(w => !BlockedCurrency.Contains(w.CurrencyCode)).ToArray();
			//    return response;
			//}

			var response = _service.CurrencyListGetPaymentCurrencies(request);
			response.Currencies = response.Currencies.Where(w => !BlockedCurrency.Contains(w.CurrencyCode)).ToArray();
			return response;
		}

		public GetCustomerAccountAliasListResponse GetAccountAliases()
		{
			GetCustomerAccountAliasListRequest request = new GetCustomerAccountAliasListRequest
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				CustomerId = CustomerId
			};

			//TsgGPWebServiceHelper.AddBearerToken(_service, AccessToken);
			var response = _service.GetCustomerAccountAliasList(request);
			return response;
			//return _service.GetCustomerAccountAliasList(request);
			//using (new OperationContextScope(_service.InnerChannel))
			//{
			//    // Add a HTTP Header to an outgoing request
			//    HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
			//    requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", this.AccessToken);
			//    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
			//    return _service.GetCustomerAccountAliasList(request);
			//}
		}

		public UserPasswordResetResponse ResetPassword(string userId)
		{
			UserPasswordResetRequest request = new UserPasswordResetRequest()
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				UserId = userId,
				SendEmail = true
			};

			using (new OperationContextScope(_service.InnerChannel))
			{
				//Add a HTTP Header to an outgoing request
				HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
				requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", AccessToken);
				OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
				return _service.UserPasswordReset(request);
			}
		}

		public UserPasswordChangeResponse ChangePassword(string oldPassword, string newPassword)
		{
			UserPasswordChangeRequest request = new UserPasswordChangeRequest()
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				OldPassword = oldPassword,
				NewPassword = newPassword
			};

			return _service.UserPasswordChange(request);
		}

		public InstantPaymentCreateResponse CreateNewPayment(string from, string to, decimal amount, string currencyCode, string memo, string externalReference = "", string reasonForPayment = "")
		{
			InstantPaymentCreateRequest request = new InstantPaymentCreateRequest
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				FromCustomer = from,
				ToCustomer = to,
				Amount = amount,
				CurrencyCode = currencyCode,
				ValueDate = "",
				Memo = memo,
				ExternalReference = externalReference,
				ReasonForPayment = reasonForPayment
			};

			using (new OperationContextScope(_service.InnerChannel))
			{
				// Add a HTTP Header to an outgoing request
				HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
				requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", this.AccessToken);
				OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
				return _service.InstantPaymentCreate(request);
			}
			//return _service.InstantPaymentCreate(request);
		}

		public InstantPaymentCreateResponse CreateInstantPayment(string from, string to, decimal amount, string currencyCode, int paymentTypeId, string valueDate = "", string memo = "", string externalReference = "", string reasonForPayment = "")
		{
			InstantPaymentCreateRequest request = new InstantPaymentCreateRequest
			{
				FromCustomer = from,
				ToCustomer = to,
				Amount = amount,
				CurrencyCode = currencyCode,
				PaymentTypeId = paymentTypeId,
				ValueDate = valueDate,
				Memo = memo,
				ExternalReference = externalReference,
				ReasonForPayment = reasonForPayment
			};

			using (new OperationContextScope(_service.InnerChannel))
			{
				// Add a HTTP Header to an outgoing request
				HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
				requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", this.AccessToken);
				OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
				return _service.InstantPaymentCreate(request);
			}
			//return _service.InstantPaymentCreate(request);
		}

		public InstantPaymentGetSingleResponse GetInstantPaymentDetails(Guid paymentId)
		{
			InstantPaymentGetSingleRequest request = new InstantPaymentGetSingleRequest
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				InstantPaymentId = paymentId.ToString()
			};
			using (new OperationContextScope(_service.InnerChannel))
			{
				// Add a HTTP Header to an outgoing request
				HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
				requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", this.AccessToken);
				OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
				return _service.InstantPaymentGetSingle(request);
			}
		}

		public InstantPaymentPostResponse PostInstantPayment(Guid paymentId)
		{
			InstantPaymentPostRequest request = new InstantPaymentPostRequest
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				InstantPaymentId = paymentId.ToString()
			};
			using (new OperationContextScope(_service.InnerChannel))
			{
				// Add a HTTP Header to an outgoing request
				HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
				requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", this.AccessToken);
				OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
				return _service.InstantPaymentPost(request);
			}
		}

		public CustomerAccountStatementGetResponse GetAccontStatements(Guid accountId, DateTime startDate, DateTime endDate)
		{
			//var valueDateMin = DateTime.Now.AddYears(-2);
			//var valueDateMax = DateTime.Now.AddYears(2);
			IFormatProvider cultureThai = CultureInfo.CreateSpecificCulture("th-TH");
			IFormatProvider cultureEng = CultureInfo.CreateSpecificCulture("en-US");

			var c = CultureInfo.CurrentCulture;
			if (c.ToString() == cultureThai.ToString())
			{
				ThaiBuddhistCalendar thaiCalendar = new ThaiBuddhistCalendar();
				startDate = thaiCalendar.ToDateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0, 0);
				endDate = thaiCalendar.ToDateTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0, 0);
			}
			CustomerAccountStatementGetRequest request = new CustomerAccountStatementGetRequest
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				AccountId = accountId.ToString(),
				StartDate = startDate.ToString("yyyy-MM-dd"),
				EndDate = endDate.ToString("yyyy-MM-dd")
			};

			using (new OperationContextScope(_service.InnerChannel))
			{
				// Add a HTTP Header to an outgoing request
				HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
				requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", this.AccessToken);
				OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
				return _service.CustomerAccountStatementGetSingle(request);
			}
		}

		public UserSettingsGetResponse GetUserData(string username, string password)
		{
			UserSettingsGetRequest request = new UserSettingsGetRequest
			{
				UserId = string.Empty
				//ServiceCallerIdentity = GetServiceCallerIdentity(username, password)
			};

			return _service.UserSettingsGetSingle(request);

		}

		public UserSettingsGetResponse GetUserData()
		{
			UserSettingsGetRequest request = new UserSettingsGetRequest
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity()
			};
			using (new OperationContextScope(_service.InnerChannel))
			{
				// Add a HTTP Header to an outgoing request
				HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
				requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", this.AccessToken);
				OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
				return _service.UserSettingsGetSingle(request);
			}
		}

		public CustomerGetSingleFromAliasResponse GetCustomerGetSingleFromAlias(string alias)
		{
			var bankUserName = ConfigurationManager.AppSettings["kycLogin"] ?? string.Empty;
			var bankUserPassword = ConfigurationManager.AppSettings["kycPassword"] ?? string.Empty;
			if (!string.IsNullOrEmpty(bankUserName) && !string.IsNullOrEmpty(bankUserPassword))
			{
				var authenticateResponse = Authenticate(bankUserName, bankUserPassword);
				if (!authenticateResponse.ServiceResponse.HasErrors)
				{

					CustomerGetSingleFromAliasRequest request = new CustomerGetSingleFromAliasRequest
					{
						Alias = alias
					};
					using (new OperationContextScope(_service.InnerChannel))
					{
						// Add a HTTP Header to an outgoing request
						HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
						requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", authenticateResponse.AccessToken);
						OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
						return _service.CustomerGetSingleFromAlias(request);
					}
				}
			}
			return new CustomerGetSingleFromAliasResponse();
		}

		public CustomerGetSingleResponse CustomerGetSingle()
		{
			CustomerGetSingleRequest request = new CustomerGetSingleRequest
			{
				CustomerId = CustomerId
			};

			using (new OperationContextScope(_service.InnerChannel))
			{
				// Add a HTTP Header to an outgoing request
				HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
				requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", AccessToken);
				OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
				return _service.CustomerGetSingle(request);
			}
		}

		public AuthenticateResponse Authenticate(string userName, string password)
		{
			AuthenticateRequest request = new AuthenticateRequest
			{
				ServiceCallerIdentity = GetServiceCallerIdentity(userName, password)
			};


			var response = _service.Authenticate(request);
			_logger.Debug("Authenticate, response: " + JsonConvert.SerializeObject(response));
			if (!response.ServiceResponse.HasErrors)
			{
				string accessToken = response.AccessToken;
				this.AccessToken = accessToken;
			}

			return response;

		}

		public AuthenticateResponse AuthenticateBankerCredential()
		{
			var bankUserName = ConfigurationManager.AppSettings["kycLogin"] ?? string.Empty;
			var bankUserPassword = ConfigurationManager.AppSettings["kycPassword"] ?? string.Empty;

			AuthenticateRequest request = new AuthenticateRequest
			{
				ServiceCallerIdentity = GetServiceCallerIdentity(bankUserName, bankUserPassword)
			};

			var response = _service.Authenticate(request);
			//_logger.debug("Authenticate, response: " + JsonConvert.SerializeObject(response));
			if (!response.ServiceResponse.HasErrors)
			{
				string accessToken = response.AccessToken;
				this.AccessToken = accessToken;
			}

			return response;

		}

		private void SetAuthorizationBearer(string accessToken)
		{
			using (new OperationContextScope(_service.InnerChannel))
			{
				// Add a HTTP Header to an outgoing request
				HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
				requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", accessToken);
				OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
				//string result = _service.GetData(123);
			}
		}

		#region FXChanges
		public FXDealQuoteCreateResponse FxDealQuoteCreate(string buyCurrencyCode, string sellCurrencyCode, decimal amount, string amountCurrencyCode, bool isForCurrencyCalculator, string dealType, string customerReference = "371380249582007")
		{
			FXDealQuoteCreateResponse res = new FXDealQuoteCreateResponse();
			try
			{
				using (new OperationContextScope(_service.InnerChannel))
				{
					// Add a HTTP Header to an outgoing request
					HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
					requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", this.AccessToken);
					OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
					res = _service.FXDealQuoteCreate(new FXDealQuoteCreateRequest()
					{
						BuyCCY = buyCurrencyCode,
						SellCCY = sellCurrencyCode,
						Amount = amount,
						AmountCCY = amountCurrencyCode,
						DealType = dealType,
						//CustomerId = string.Empty, // for bank
						IsForCurrencyCalculator = isForCurrencyCalculator,
						FinalValueDate = String.Empty,
						WindowOpenDate = String.Empty,
					});
				}
			}
			catch (Exception e)
			{

			}

			return res;
		}

		public FXDealQuoteCreateResponse FxDealQuoteCreate(string buyCurrencyCode, string sellCurrencyCode, decimal amount, string amountCurrencyCode, bool isForCurrencyCalculator, string dealType, string windowOpenDate, string finalValueDate, string customerReference = "371380249582007")
		{
			FXDealQuoteCreateResponse res = new FXDealQuoteCreateResponse();
			try
			{
				using (new OperationContextScope(_service.InnerChannel))
				{
					// Add a HTTP Header to an outgoing request
					HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
					requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", this.AccessToken);
					OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
					res = _service.FXDealQuoteCreate(new FXDealQuoteCreateRequest()
					{
						BuyCCY = buyCurrencyCode,
						SellCCY = sellCurrencyCode,
						Amount = amount,
						AmountCCY = amountCurrencyCode,
						DealType = dealType,
						//CustomerId = string.Empty, // for bank
						IsForCurrencyCalculator = isForCurrencyCalculator,
						FinalValueDate = finalValueDate,
						WindowOpenDate = windowOpenDate,
						//ServiceCallerIdentity = GetServiceCallerIdentity()
					});
				}
			}
			catch (Exception e)
			{

			}

			return res;
		}

		public FXDealQuoteCreateResponse FxDealQuoteCreate(FXDealQuoteCreateRequest request)
		{

			FXDealQuoteCreateResponse res = new FXDealQuoteCreateResponse();
			try
			{
				//request.ServiceCallerIdentity = GetServiceCallerIdentity();
				using (new OperationContextScope(_service.InnerChannel))
				{
					// Add a HTTP Header to an outgoing request
					HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
					requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", this.AccessToken);
					OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
					res = _service.FXDealQuoteCreate(request);
				}
			}
			catch (Exception e)
			{

			}

			return res;
		}

		public FXDealQuoteBookResponse FxDealQuoteBook(string quoteId)
		{
			FXDealQuoteBookResponse res = new FXDealQuoteBookResponse();
			try
			{
				res = _service.FXDealQuoteBook(new FXDealQuoteBookRequest() { QuoteId = quoteId });
			}
			catch (Exception e)
			{

			}

			return res;
		}

		public FXDealQuoteBookAndInstantDepositResponse FxDealQuoteBookAndInstantDeposit(string quoteId)
		{
			FXDealQuoteBookAndInstantDepositResponse res = null;
			try
			{
				using (new OperationContextScope(_service.InnerChannel))
				{
					// Add a HTTP Header to an outgoing request
					HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
					requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", this.AccessToken);
					OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
					res = _service.FXDealQuoteBookAndInstantDeposit(new FXDealQuoteBookAndInstantDepositRequest()
					{
						QuoteId = quoteId,
					});
				}
			}
			catch (Exception e)
			{

			}

			return res;
		}

		public FXDealSearchResponse FXDealSearch(int count)
		{
			int pageIndex = 0, pageSize = count, amountMin = 0, amountMax = 100000000;
			IFormatProvider cultureThai = CultureInfo.CreateSpecificCulture("th-TH");
			IFormatProvider cultureEng = CultureInfo.CreateSpecificCulture("en-US");

			var valueDateMin = DateTime.Now.AddYears(-2);
			var valueDateMax = DateTime.Now.AddYears(2);

			var c = CultureInfo.CurrentCulture;
			if (c.ToString() == cultureThai.ToString())
			{
				ThaiBuddhistCalendar thaiCalendar = new ThaiBuddhistCalendar();
				valueDateMin = thaiCalendar.ToDateTime(valueDateMin.Year, valueDateMin.Month, valueDateMin.Day, 0, 0, 0, 0);
				valueDateMax = thaiCalendar.ToDateTime(valueDateMax.Year, valueDateMax.Month, valueDateMax.Day, 0, 0, 0, 0);
			}

			var sortDirection = FXDealSearchSortDirection.Descending;
			var sortBy = FXDealSearchSortBy.ValueDate;
			FXDealSearchResponse response = new FXDealSearchResponse();
			FXDealSearchRequest request = new FXDealSearchRequest
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				PageIndex = pageIndex,
				PageSize = pageSize,
				SortBy = sortBy,
				SortDirection = sortDirection,
				SearchOptions = new FXDealSearchOptions
				{
					AccountNumber = "",
					AmountMin = amountMin,
					AmountMax = amountMax,
					BranchName = string.Empty,
					ClientBuySell = FXDealSearchClientBuySell.Any,
					CurrencyCode = string.Empty,
					CustomerName = string.Empty,
					DateMax = valueDateMax.ToString("yyyy-MM-dd"),
					DateMin = valueDateMin.ToString("yyyy-MM-dd"),
					DateType = FXDealSearchDateType.Deal,
					FXDealReference = string.Empty
				}
			};
			try
			{
				using (new OperationContextScope(_service.InnerChannel))
				{
					// Add a HTTP Header to an outgoing request
					HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
					requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", this.AccessToken);
					OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
					response = _service.FXDealSearch(request);
				}
			}
			catch (Exception e)
			{
				response.ServiceResponse.HasErrors = true;
			}

			return response;
		}

		public FXDealSearchResponse FXDealSearchByReference(string FXDealReference)
		{
			int pageIndex = 0, pageSize = 1000000;
			decimal amountMin = 0, amountMax = 100000000;
			var sortDirection = FXDealSearchSortDirection.Descending;
			var sortBy = FXDealSearchSortBy.ValueDate;
			FXDealSearchResponse response = new FXDealSearchResponse();
			FXDealSearchRequest request = new FXDealSearchRequest
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				PageIndex = pageIndex,
				PageSize = pageSize,
				SortBy = sortBy,
				SortDirection = sortDirection,
				SearchOptions = new FXDealSearchOptions
				{
					AccountNumber = string.Empty,
					AmountMin = amountMin,
					AmountMax = amountMax,
					BranchName = string.Empty,
					ClientBuySell = FXDealSearchClientBuySell.Any,
					CurrencyCode = string.Empty,
					CustomerName = string.Empty,
					DateMax = string.Empty,
					DateMin = string.Empty,
					DateType = FXDealSearchDateType.Deal,
					FXDealReference = FXDealReference
				}
			};
			try
			{
				using (new OperationContextScope(_service.InnerChannel))
				{
					// Add a HTTP Header to an outgoing request
					HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
					requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", this.AccessToken);
					OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
					response = _service.FXDealSearch(request);
				}
			}
			catch (Exception e)
			{
				response.ServiceResponse.HasErrors = true;
			}

			return response;
		}

		public FXDealSearchResponse FXDealSearch(string FXDealReference, string AccountNumber = "", decimal AmountMax = 0, decimal AmountMin = 0, string BranchName = "", string ClientBuySell = "", string CurrencyCode = "", string CustomerName = "", string DateMax = "", string DateMin = "", string DateType = "")
		{
			int pageIndex = 0, pageSize = 1000000;
			//amountMin = 0, amountMax = 100000000;
			IFormatProvider cultureThai = CultureInfo.CreateSpecificCulture("th-TH");
			IFormatProvider cultureEng = CultureInfo.CreateSpecificCulture("en-US");

			var valueDateMin = DateTime.Now.AddYears(-2);
			var valueDateMax = DateTime.Now.AddYears(2);

			var c = CultureInfo.CurrentCulture;
			if (c.ToString() == cultureThai.ToString())
			{
				ThaiBuddhistCalendar thaiCalendar = new ThaiBuddhistCalendar();
				valueDateMin = thaiCalendar.ToDateTime(valueDateMin.Year, valueDateMin.Month, valueDateMin.Day, 0, 0, 0, 0);
				valueDateMax = thaiCalendar.ToDateTime(valueDateMax.Year, valueDateMax.Month, valueDateMax.Day, 0, 0, 0, 0);
			}

			var sortDirection = FXDealSearchSortDirection.Descending;
			var sortBy = FXDealSearchSortBy.ValueDate;
			FXDealSearchResponse response = new FXDealSearchResponse();
			FXDealSearchRequest request = new FXDealSearchRequest
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				PageIndex = pageIndex,
				PageSize = pageSize,
				SortBy = sortBy,
				SortDirection = sortDirection,
				SearchOptions = new FXDealSearchOptions
				{
					AccountNumber = AccountNumber,
					AmountMin = AmountMin,
					AmountMax = AmountMax,
					BranchName = BranchName,
					ClientBuySell = FXDealSearchClientBuySell.Any,
					CurrencyCode = CurrencyCode,
					CustomerName = CustomerName,
					DateMax = valueDateMax.ToString("yyyy-MM-dd"),
					DateMin = valueDateMin.ToString("yyyy-MM-dd"),
					DateType = FXDealSearchDateType.Deal,
					FXDealReference = string.Empty
				}
			};
			try
			{
				using (new OperationContextScope(_service.InnerChannel))
				{
					// Add a HTTP Header to an outgoing request
					HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
					requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", this.AccessToken);
					OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
					response = _service.FXDealSearch(request);
				}
			}
			catch (Exception e)
			{
				response.ServiceResponse.HasErrors = true;
			}

			return response;
		}

		public FXDealGetSingleResponse GetFXDealDetails(Guid fxDealId)
		{
			FXDealGetSingleRequest request = new FXDealGetSingleRequest
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				FXDealId = fxDealId.ToString()
			};
			using (new OperationContextScope(_service.InnerChannel))
			{
				// Add a HTTP Header to an outgoing request
				HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
				requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", this.AccessToken);
				OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
				return _service.FXDealGetSingle(request);
			}
		}

		#endregion

		#region Work with Aliases

		public CustomerAccountAliasCreateResponse AddNewUserAliases(string accountAlias, int aliasType = 1)
		{
			CustomerAccountAliasCreateResponse res;
			try
			{
				var req = new CustomerAccountAliasCreateRequest()
				{
					CustomerId = CustomerId,
					AccountAlias = accountAlias,
					AccountAliasType = aliasType,
					//ServiceCallerIdentity = GetServiceCallerIdentity()
				};

				_logger.Error(string.Format("Create new alias request: {0}", JsonConvert.SerializeObject(req)));
				// Remove existing bearer token
				// RemoveExistingBearerTokenEndpointBehavior();
				// TsgGPWebServiceHelper.AddBearerToken(_service, AccessToken);
				res = _service.CustomerAccountAliasCreate(req);
				_logger.Error(string.Format("Create new alias response: {0}", JsonConvert.SerializeObject(res)));
				//using (new OperationContextScope(_service.InnerChannel))
				//{
				//    // Add a HTTP Header to an outgoing request
				//    HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
				//    requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", this.AccessToken);
				//    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
				//    res = _service.CustomerAccountAliasCreate(req);
				//}
				//res = _service.CustomerAccountAliasCreate(req);
			}
			catch (Exception e)
			{
				_logger.Error(string.Format("Create new alias failed. Error: {0}", JsonConvert.SerializeObject(e)));
				res = new CustomerAccountAliasCreateResponse() { ServiceResponse = new ServiceResponse() { HasErrors = true, Responses = new ServiceResponseData[] { new ServiceResponseData() { Message = e.Message, MessageDetails = e.StackTrace } } } };
			}
			return res;
		}

		public CustomerAccountAliasDeleteResponse DeleteUserAlias(string accountAlias)
		{
			CustomerAccountAliasDeleteResponse res;
			try
			{
				var req = new CustomerAccountAliasDeleteRequest()
				{
					CustomerId = CustomerId,
					Alias = accountAlias,
					//ServiceCallerIdentity = GetServiceCallerIdentity()
				};
				res = _service.CustomerAccountAliasDelete(req);
			}
			catch (Exception e)
			{
				res = new CustomerAccountAliasDeleteResponse() { ServiceResponse = new ServiceResponse() { HasErrors = true, Responses = new ServiceResponseData[] { new ServiceResponseData() { Message = e.Message, MessageDetails = e.StackTrace } } } };
			}
			return res;
		}

		public CustomerAccountAliasSetDefaultResponse SetUserDefaultAlias(string alias)
		{
			CustomerAccountAliasSetDefaultResponse res;
			try
			{
				var req = new CustomerAccountAliasSetDefaultRequest()
				{
					CustomerId = CustomerId,
					Alias = alias,
				};
				res = _service.CustomerAccountAliasSetDefault(req);
			}
			catch (Exception e)
			{
				res = new CustomerAccountAliasSetDefaultResponse() { ServiceResponse = new ServiceResponse() { HasErrors = true, Responses = new ServiceResponseData[] { new ServiceResponseData() { Message = e.Message, MessageDetails = e.StackTrace } } } };
			}
			return res;
		}

		#endregion

		#region Payment
		public PaymentCreateResponse CreatePayment(PaymentCreateRequest request)
		{
			PaymentCreateResponse response;

			try
			{
				//request.ServiceCallerIdentity = GetServiceCallerIdentity();
				response = _service.PaymentCreate(request);
			}
			catch (Exception exeption)
			{
				response = new PaymentCreateResponse() { ServiceResponse = new ServiceResponse() { HasErrors = true, Responses = new ServiceResponseData[] { new ServiceResponseData() { Message = exeption.Message, MessageDetails = exeption.StackTrace } } } };
			}

			return response;
		}

		public PaymentCreateResponse CreatePayment(string BankOperationCode, string BeneficiaryABN, string BeneficiaryACN, string BeneficiaryARBN, string BeneficiaryAccountNumber, string BeneficiaryAccountNumberPrefix, string BeneficiaryAccountNumberSuffix, string BeneficiaryAccountTypeCode, string BeneficiaryAddress1, string BeneficiaryAddress2, string BeneficiaryAddress3, string BeneficiaryBankAddress1, string BeneficiaryBankAddress2, string BeneficiaryBankAddress3, string BeneficiaryBankBIC, string BeneficiaryBankCity, string BeneficiaryBankCode, string BeneficiaryBankCountryCode, string BeneficiaryBankName, string BeneficiaryBankNationalCode, string BeneficiaryBankNationalCodeType, string BeneficiaryBankPostalCode, string BeneficiaryBankStateOrProvince, string BeneficiaryBankStreetAddress1, string BeneficiaryBankStreetAddress2, string BeneficiaryBranchBIC, string BeneficiaryBranchCity, string BeneficiaryBranchCode, string BeneficiaryBranchCountryCode, string BeneficiaryBranchId, string BeneficiaryBranchName, string BeneficiaryBranchNationalCode, string BeneficiaryBranchNationalCodeType, string BeneficiaryBranchPostalCode, string BeneficiaryBranchStateOrProvince, string BeneficiaryBranchStreetAddress1, string BeneficiaryBranchStreetAddress2, string BeneficiaryBusinessStructureTypeCode, string BeneficiaryCellPhone, string BeneficiaryCity, string BeneficiaryCompanyRegistrationCountryCode, string BeneficiaryCompanyRegistrationNumber, string BeneficiaryCountryCode, string BeneficiaryCountryOfBirthCode, string BeneficiaryDateOfBirth, string BeneficiaryEmail, string BeneficiaryFirstName, string BeneficiaryIdentificationCountryCode, string BeneficiaryIdentificationNumber, int BeneficiaryIdentificationTypeId, string BeneficiaryInfoLine1, string BeneficiaryInfoLine2, string BeneficiaryInfoLine3, string BeneficiaryInfoLine4, string BeneficiaryLastName, string BeneficiaryMiddleName, string BeneficiaryName, string BeneficiaryOccupationCode, string BeneficiaryOccupationDescription, int BeneficiaryOccupationTypeId, string BeneficiaryPostalCode, string BeneficiaryStateOrProvince, string BeneficiaryStreetAddress1, string BeneficiaryStreetAddress2, string BeneficiaryTaxId, int BeneficiaryTypeId, string ChargeDetail, string InitiatingInstitutionABN, string InitiatingInstitutionACN, string InitiatingInstitutionARBN, string InitiatingInstitutionBIC, string InitiatingInstitutionCity, string InitiatingInstitutionCountryCode, string InitiatingInstitutionName, string InitiatingInstitutionNationalCode, string InitiatingInstitutionNationalCodeType, string InitiatingInstitutionPostalCode, bool InitiatingInstitutionSameAsOrderingInstitution, string InitiatingInstitutionStateOrProvince, string InitiatingInstitutionStreetAddress1, string InitiatingInstitutionStreetAddress2, string IntermediaryBankAddress1, string IntermediaryBankAddress2, string IntermediaryBankAddress3, string IntermediaryBankBIC, string IntermediaryBankCity, string IntermediaryBankCountryCode, string IntermediaryBankName, string IntermediaryBankNationalCode, string IntermediaryBankNationalCodeType, string IntermediaryBankPostalCode, string IntermediaryBankStateOrProvince, string IntermediaryBankStreetAddress1, string IntermediaryBankStreetAddress2, string OrderingCustomerBankBIC, string OrderingCustomerBankCity, string OrderingCustomerBankCountryCode, string OrderingCustomerBankName, string OrderingCustomerBankNationalCode, string OrderingCustomerBankNationalCodeType, string OrderingCustomerBankPostalCode, string OrderingCustomerBankStateOrProvince, string OrderingCustomerBankStreetAddress1, string OrderingCustomerBankStreetAddress2, string ReasonForPayment, string ReasonForPaymentCode, string ReceiverBIC, string ReceivingInstitutionBIC, string ReceivingInstitutionCity, string ReceivingInstitutionCountryCode, string ReceivingInstitutionName, string ReceivingInstitutionNationalCode, string ReceivingInstitutionNationalCodeType, string ReceivingInstitutionPostalCode, string ReceivingInstitutionStateOrProvince, string ReceivingInstitutionStreetAddress1, string ReceivingInstitutionStreetAddress2, string SenderToReceiverInfo1, string SenderToReceiverInfo2, string SenderToReceiverInfo3, string SenderToReceiverInfo4, string SenderToReceiverInfo5, string SenderToReceiverInfo6, string SendingInstitutionABN, string SendingInstitutionACN, string SendingInstitutionARBN, string SendingInstitutionBIC, string SendingInstitutionBusinessStructureTypeCode, string SendingInstitutionCity, string SendingInstitutionCountryCode, string SendingInstitutionEmail, string SendingInstitutionName, string SendingInstitutionNationalCode, string SendingInstitutionNationalCodeType, string SendingInstitutionOccupationCode, string SendingInstitutionOccupationDescription, int SendingInstitutionOccupationTypeId, string SendingInstitutionPhone, string SendingInstitutionPostalCode, bool SendingInstitutionSameAsOrderingInstitution, string SendingInstitutionStateOrProvince, string SendingInstitutionStreetAddress1, string SendingInstitutionStreetAddress2, string CustomerId, string FXDealId, decimal Amount, string AmountCurrencyCode, string CountryCode, string ValueDate, decimal FeeAmount, string FeeAmountCurrencyCode, string PaymentValueType)
		{
			PaymentCreateRequest request = new PaymentCreateRequest()
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				BankOperationCode = BankOperationCode,
				BeneficiaryABN = BeneficiaryABN,
				BeneficiaryACN = BeneficiaryACN,
				BeneficiaryARBN = BeneficiaryARBN,
				BeneficiaryAccountNumber = BeneficiaryAccountNumber,
				BeneficiaryAccountNumberPrefix = BeneficiaryAccountNumberPrefix,
				BeneficiaryAccountNumberSuffix = BeneficiaryAccountNumberSuffix,
				BeneficiaryAccountTypeCode = BeneficiaryAccountTypeCode,
				BeneficiaryAddress1 = BeneficiaryAddress1,
				BeneficiaryAddress2 = BeneficiaryAddress2,
				BeneficiaryAddress3 = BeneficiaryAddress3,
				BeneficiaryBankAddress1 = BeneficiaryBankAddress1,
				BeneficiaryBankAddress2 = BeneficiaryBankAddress2,
				BeneficiaryBankAddress3 = BeneficiaryBankAddress3,
				BeneficiaryBankBIC = BeneficiaryBankBIC,
				BeneficiaryBankCity = BeneficiaryBankCity,
				BeneficiaryBankCode = BeneficiaryBankCode,
				BeneficiaryBankCountryCode = BeneficiaryBankCountryCode,
				BeneficiaryBankName = BeneficiaryBankName,
				BeneficiaryBankNationalCode = BeneficiaryBankNationalCode,
				BeneficiaryBankNationalCodeType = BeneficiaryBankNationalCodeType,
				BeneficiaryBankPostalCode = BeneficiaryBankPostalCode,
				BeneficiaryBankStateOrProvince = BeneficiaryBankStateOrProvince,
				BeneficiaryBankStreetAddress1 = BeneficiaryBankStreetAddress1,
				BeneficiaryBankStreetAddress2 = BeneficiaryBankStreetAddress2,
				BeneficiaryBranchBIC = BeneficiaryBranchBIC,
				BeneficiaryBranchCity = BeneficiaryBranchCity,
				BeneficiaryBranchCode = BeneficiaryBranchCode,
				BeneficiaryBranchCountryCode = BeneficiaryBranchCountryCode,
				BeneficiaryBranchId = BeneficiaryBranchId,
				BeneficiaryBranchName = BeneficiaryBranchName,
				BeneficiaryBranchNationalCode = BeneficiaryBranchNationalCode,
				BeneficiaryBranchNationalCodeType = BeneficiaryBranchNationalCodeType,
				BeneficiaryBranchPostalCode = BeneficiaryBranchPostalCode,
				BeneficiaryBranchStateOrProvince = BeneficiaryBranchStateOrProvince,
				BeneficiaryBranchStreetAddress1 = BeneficiaryBranchStreetAddress1,
				BeneficiaryBranchStreetAddress2 = BeneficiaryBranchStreetAddress2,
				BeneficiaryBusinessStructureTypeCode = BeneficiaryBusinessStructureTypeCode,
				BeneficiaryCellPhone = BeneficiaryCellPhone,
				BeneficiaryCity = BeneficiaryCity,
				BeneficiaryCompanyRegistrationCountryCode = BeneficiaryCompanyRegistrationCountryCode,
				BeneficiaryCompanyRegistrationNumber = BeneficiaryCompanyRegistrationNumber,
				BeneficiaryCountryCode = BeneficiaryCountryCode,
				BeneficiaryCountryOfBirthCode = BeneficiaryCountryOfBirthCode,
				BeneficiaryDateOfBirth = BeneficiaryDateOfBirth,
				BeneficiaryEmail = BeneficiaryEmail,
				BeneficiaryFirstName = BeneficiaryFirstName,
				BeneficiaryIdentificationCountryCode = BeneficiaryIdentificationCountryCode,
				BeneficiaryIdentificationNumber = BeneficiaryIdentificationNumber,
				BeneficiaryIdentificationTypeId = BeneficiaryIdentificationTypeId,
				BeneficiaryInfoLine1 = BeneficiaryInfoLine1,
				BeneficiaryInfoLine2 = BeneficiaryInfoLine2,
				BeneficiaryInfoLine3 = BeneficiaryInfoLine3,
				BeneficiaryInfoLine4 = BeneficiaryInfoLine4,
				BeneficiaryLastName = BeneficiaryLastName,
				BeneficiaryMiddleName = BeneficiaryMiddleName,
				BeneficiaryName = BeneficiaryName,
				BeneficiaryOccupationCode = BeneficiaryOccupationCode,
				BeneficiaryOccupationDescription = BeneficiaryOccupationDescription,
				BeneficiaryOccupationTypeId = BeneficiaryOccupationTypeId,
				BeneficiaryPostalCode = BeneficiaryPostalCode,
				BeneficiaryStateOrProvince = BeneficiaryStateOrProvince,
				BeneficiaryStreetAddress1 = BeneficiaryStreetAddress1,
				BeneficiaryStreetAddress2 = BeneficiaryStreetAddress2,
				BeneficiaryTaxId = BeneficiaryTaxId,
				BeneficiaryTypeId = BeneficiaryTypeId,
				ChargeDetail = ChargeDetail,
				InitiatingInstitutionABN = InitiatingInstitutionABN,
				InitiatingInstitutionACN = InitiatingInstitutionACN,
				InitiatingInstitutionARBN = InitiatingInstitutionARBN,
				InitiatingInstitutionBIC = InitiatingInstitutionBIC,
				InitiatingInstitutionCity = InitiatingInstitutionCity,
				InitiatingInstitutionCountryCode = InitiatingInstitutionCountryCode,
				InitiatingInstitutionName = InitiatingInstitutionName,
				InitiatingInstitutionNationalCode = InitiatingInstitutionNationalCode,
				InitiatingInstitutionNationalCodeType = InitiatingInstitutionNationalCodeType,
				InitiatingInstitutionPostalCode = InitiatingInstitutionPostalCode,
				InitiatingInstitutionSameAsOrderingInstitution = InitiatingInstitutionSameAsOrderingInstitution,
				InitiatingInstitutionStateOrProvince = InitiatingInstitutionStateOrProvince,
				InitiatingInstitutionStreetAddress1 = InitiatingInstitutionStreetAddress1,
				InitiatingInstitutionStreetAddress2 = InitiatingInstitutionStreetAddress2,
				IntermediaryBankAddress1 = IntermediaryBankAddress1,
				IntermediaryBankAddress2 = IntermediaryBankAddress2,
				IntermediaryBankAddress3 = IntermediaryBankAddress3,
				IntermediaryBankBIC = IntermediaryBankBIC,
				IntermediaryBankCity = IntermediaryBankCity,
				IntermediaryBankCountryCode = IntermediaryBankCountryCode,
				IntermediaryBankName = IntermediaryBankName,
				IntermediaryBankNationalCode = IntermediaryBankNationalCode,
				IntermediaryBankNationalCodeType = IntermediaryBankNationalCodeType,
				IntermediaryBankPostalCode = IntermediaryBankPostalCode,
				IntermediaryBankStateOrProvince = IntermediaryBankStateOrProvince,
				IntermediaryBankStreetAddress1 = IntermediaryBankStreetAddress1,
				IntermediaryBankStreetAddress2 = IntermediaryBankStreetAddress2,
				OrderingCustomerBankBIC = OrderingCustomerBankBIC,
				OrderingCustomerBankCity = OrderingCustomerBankCity,
				OrderingCustomerBankCountryCode = OrderingCustomerBankCountryCode,
				OrderingCustomerBankName = OrderingCustomerBankName,
				OrderingCustomerBankNationalCode = OrderingCustomerBankNationalCode,
				OrderingCustomerBankNationalCodeType = OrderingCustomerBankNationalCodeType,
				OrderingCustomerBankPostalCode = OrderingCustomerBankPostalCode,
				OrderingCustomerBankStateOrProvince = OrderingCustomerBankStateOrProvince,
				OrderingCustomerBankStreetAddress1 = OrderingCustomerBankStreetAddress1,
				OrderingCustomerBankStreetAddress2 = OrderingCustomerBankStreetAddress2,
				ReasonForPayment = ReasonForPayment,
				ReasonForPaymentCode = ReasonForPaymentCode,
				ReceiverBIC = ReceiverBIC,
				ReceivingInstitutionBIC = ReceivingInstitutionBIC,
				ReceivingInstitutionCity = ReceivingInstitutionCity,
				ReceivingInstitutionCountryCode = ReceivingInstitutionCountryCode,
				ReceivingInstitutionName = ReceivingInstitutionName,
				ReceivingInstitutionNationalCode = ReceivingInstitutionNationalCode,
				ReceivingInstitutionNationalCodeType = ReceivingInstitutionNationalCodeType,
				ReceivingInstitutionPostalCode = ReceivingInstitutionPostalCode,
				ReceivingInstitutionStateOrProvince = ReceivingInstitutionStateOrProvince,
				ReceivingInstitutionStreetAddress1 = ReceivingInstitutionStreetAddress1,
				ReceivingInstitutionStreetAddress2 = ReceivingInstitutionStreetAddress2,
				SenderToReceiverInfo1 = SenderToReceiverInfo1,
				SenderToReceiverInfo2 = SenderToReceiverInfo2,
				SenderToReceiverInfo3 = SenderToReceiverInfo3,
				SenderToReceiverInfo4 = SenderToReceiverInfo4,
				SenderToReceiverInfo5 = SenderToReceiverInfo5,
				SenderToReceiverInfo6 = SenderToReceiverInfo6,
				SendingInstitutionABN = SendingInstitutionABN,
				SendingInstitutionACN = SendingInstitutionACN,
				SendingInstitutionARBN = SendingInstitutionARBN,
				SendingInstitutionBIC = SendingInstitutionBIC,
				SendingInstitutionBusinessStructureTypeCode = SendingInstitutionBusinessStructureTypeCode,
				SendingInstitutionCity = SendingInstitutionCity,
				SendingInstitutionCountryCode = SendingInstitutionCountryCode,
				SendingInstitutionEmail = SendingInstitutionEmail,
				SendingInstitutionName = SendingInstitutionName,
				SendingInstitutionNationalCode = SendingInstitutionNationalCode,
				SendingInstitutionNationalCodeType = SendingInstitutionNationalCodeType,
				SendingInstitutionOccupationCode = SendingInstitutionOccupationCode,
				SendingInstitutionOccupationDescription = SendingInstitutionOccupationDescription,
				SendingInstitutionOccupationTypeId = SendingInstitutionOccupationTypeId,
				SendingInstitutionPhone = SendingInstitutionPhone,
				SendingInstitutionPostalCode = SendingInstitutionPostalCode,
				SendingInstitutionSameAsOrderingInstitution = SendingInstitutionSameAsOrderingInstitution,
				SendingInstitutionStateOrProvince = SendingInstitutionStateOrProvince,
				SendingInstitutionStreetAddress1 = SendingInstitutionStreetAddress1,
				SendingInstitutionStreetAddress2 = SendingInstitutionStreetAddress2,
				CustomerId = CustomerId,
				FXDealId = FXDealId,
				Amount = Amount,
				AmountCurrencyCode = AmountCurrencyCode,
				DestinationCountryCode = CountryCode,
				ValueDate = ValueDate,
				FeeAmount = FeeAmount,
				FeeAmountCurrencyCode = FeeAmountCurrencyCode,
				PaymentValueType = PaymentValueType,
			};

			return CreatePayment(request);
		}

		public PaymentGetSingleResponse GetPaymentDetails(Guid paymentId)
		{
			PaymentGetSingleRequest request = new PaymentGetSingleRequest
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				PaymentId = paymentId.ToString()
			};
			return _service.PaymentGetSingle(request);
		}

		public PaymentSearchResponse PaymentSearch(int count)
		{
			int pageIndex = 0, pageSize = count, amountMin = 0, amountMax = 100000000;
			IFormatProvider cultureThai = CultureInfo.CreateSpecificCulture("th-TH");
			IFormatProvider cultureEng = CultureInfo.CreateSpecificCulture("en-US");

			var valueDateMin = DateTime.Now.AddYears(-2);
			var valueDateMax = DateTime.Now.AddYears(2);

			var c = CultureInfo.CurrentCulture;
			if (c.ToString() == cultureThai.ToString())
			{
				ThaiBuddhistCalendar thaiCalendar = new ThaiBuddhistCalendar();
				valueDateMin = thaiCalendar.ToDateTime(valueDateMin.Year, valueDateMin.Month, valueDateMin.Day, 0, 0, 0, 0);
				valueDateMax = thaiCalendar.ToDateTime(valueDateMax.Year, valueDateMax.Month, valueDateMax.Day, 0, 0, 0, 0);
			}

			var sortDirection = PaymentSearchSortDirection.Descending;
			var sortBy = PaymentSearchSortBy.CreatedTime;
			var status = PaymentStatus.All;
			PaymentSearchResponse response = new PaymentSearchResponse();
			PaymentSearchRequest request = new PaymentSearchRequest()
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				PageIndex = pageIndex,
				PageSize = pageSize,
				SortBy = sortBy,
				SortDirection = sortDirection,
				SearchOptions = new PaymentSearchOptions
				{
					PaymentReference = "",
					CustomerName = "",
					BranchName = "",
					AccountNumber = "",
					AmountMin = amountMin,
					AmountMax = amountMax,
					CCY = "",
					Status = status,
					ValueDateMin = valueDateMin.ToString("yyyy-MM-dd"),
					ValueDateMax = valueDateMax.ToString("yyyy-MM-dd"),
				}
			};
			try
			{
				response = _service.PaymentSearch(request);
				response.Payments = response.Payments.Where(w => !BlockedCurrency.Contains(w.AmountCurrencyCode)).ToArray();
			}
			catch (Exception e)
			{
				response.ServiceResponse.HasErrors = true;
			}

			return response;
		}

		public PaymentSubmitResponse SubmitPayment(Guid paymentId, string timestamp)
		{
			PaymentSubmitRequest request = new PaymentSubmitRequest
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				PaymentId = paymentId.ToString(),
				Timestamp = timestamp
			};
			return _service.PaymentSubmit(request);
		}

		public PaymentSubmitResponse SubmitPayment(PaymentSubmitRequest request)
		{
			//request.ServiceCallerIdentity = GetServiceCallerIdentity();

			return _service.PaymentSubmit(request);
		}

		public PaymentDeleteResponse DeletePayment(Guid paymentId, string timestamp)
		{
			PaymentDeleteRequest request = new PaymentDeleteRequest
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				PaymentId = paymentId.ToString(),
				Timestamp = timestamp
			};
			return _service.PaymentDelete(request);
		}

		public PaymentDeleteResponse DeletePayment(PaymentDeleteRequest request)
		{
			PaymentDeleteResponse response;

			try
			{
				//request.ServiceCallerIdentity = GetServiceCallerIdentity();

				response = _service.PaymentDelete(request);
			}
			catch (Exception exeption)
			{
				response = new PaymentDeleteResponse() { ServiceResponse = new ServiceResponse() { HasErrors = true, Responses = new ServiceResponseData[] { new ServiceResponseData() { Message = exeption.Message, MessageDetails = exeption.StackTrace } } } };
			}

			return response;
		}

		public PaymentValidateResponse ValidatePayment(PaymentValidateRequest request)
		{
			PaymentValidateResponse response;

			try
			{
				//request.ServiceCallerIdentity = GetServiceCallerIdentity();
				response = _service.PaymentValidate(request);
			}
			catch (Exception exeption)
			{
				response = new PaymentValidateResponse() { ServiceResponse = new ServiceResponse() { HasErrors = true, Responses = new ServiceResponseData[] { new ServiceResponseData() { Message = exeption.Message, MessageDetails = exeption.StackTrace } } } };
			}

			return response;
		}

		public CountryListGetPaymentCountriesResponse GetPaymentCountries(CountryListGetPaymentCountriesRequest request)
		{
			CountryListGetPaymentCountriesResponse response;

			try
			{

				response = _service.CountryListGetPaymentCountries(request);
			}
			catch (Exception exeption)
			{
				response = new CountryListGetPaymentCountriesResponse() { ServiceResponse = new ServiceResponse() { HasErrors = true, Responses = new ServiceResponseData[] { new ServiceResponseData() { Message = exeption.Message, MessageDetails = exeption.StackTrace } } } };
			}

			return response;
		}

		public CountryListGetPaymentCountriesResponse GetPaymentCountries()
		{
			CountryListGetPaymentCountriesResponse response;

			try
			{
				CountryListGetPaymentCountriesRequest request​ = new CountryListGetPaymentCountriesRequest()
				{
					//ServiceCallerIdentity = GetServiceCallerIdentity()
				};

				response = _service.CountryListGetPaymentCountries(request);
			}
			catch (Exception exeption)
			{
				response = new CountryListGetPaymentCountriesResponse() { ServiceResponse = new ServiceResponse() { HasErrors = true, Responses = new ServiceResponseData[] { new ServiceResponseData() { Message = exeption.Message, MessageDetails = exeption.StackTrace } } } };
			}

			return response;
		}


		public NationalCodeListGetAllResponse GetPaymentNationalCodes(NationalCodeListGetAllRequest request)
		{
			NationalCodeListGetAllResponse response;

			try
			{

				response = _service.NationalCodeListGetAll(request);
			}
			catch (Exception exeption)
			{
				response = new NationalCodeListGetAllResponse() { ServiceResponse = new ServiceResponse() { HasErrors = true, Responses = new ServiceResponseData[] { new ServiceResponseData() { Message = exeption.Message, MessageDetails = exeption.StackTrace } } } };
			}

			return response;
		}

		public NationalCodeListGetAllResponse GetPaymentNationalCodes()
		{
			NationalCodeListGetAllResponse response;

			try
			{
				NationalCodeListGetAllRequest request​ = new NationalCodeListGetAllRequest()
				{
					//ServiceCallerIdentity = GetServiceCallerIdentity()
				};

				response = _service.NationalCodeListGetAll(request);
			}
			catch (Exception exeption)
			{
				response = new NationalCodeListGetAllResponse() { ServiceResponse = new ServiceResponse() { HasErrors = true, Responses = new ServiceResponseData[] { new ServiceResponseData() { Message = exeption.Message, MessageDetails = exeption.StackTrace } } } };
			}

			return response;
		}

		public NationalCodeListGetAllForCurrencyResponse GetPaymentNationalCodes(NationalCodeListGetAllForCurrencyRequest request)
		{
			NationalCodeListGetAllForCurrencyResponse response;

			try
			{

				response = _service.NationalCodeListGetAllForCurrency(request);
			}
			catch (Exception exeption)
			{
				response = new NationalCodeListGetAllForCurrencyResponse() { ServiceResponse = new ServiceResponse() { HasErrors = true, Responses = new ServiceResponseData[] { new ServiceResponseData() { Message = exeption.Message, MessageDetails = exeption.StackTrace } } } };
			}

			return response;
		}

		public NationalCodeListGetAllForCurrencyResponse GetPaymentNationalCodes(string currencyCode)
		{
			NationalCodeListGetAllForCurrencyResponse response;

			try
			{
				NationalCodeListGetAllForCurrencyRequest request​ = new NationalCodeListGetAllForCurrencyRequest()
				{
					//ServiceCallerIdentity = GetServiceCallerIdentity(),
					CurrencyCode = currencyCode
				};

				response = _service.NationalCodeListGetAllForCurrency(request);
			}
			catch (Exception exeption)
			{
				response = new NationalCodeListGetAllForCurrencyResponse() { ServiceResponse = new ServiceResponse() { HasErrors = true, Responses = new ServiceResponseData[] { new ServiceResponseData() { Message = exeption.Message, MessageDetails = exeption.StackTrace } } } };
			}

			return response;
		}

		public BankDirectorySearchResponse BankDirectorySearch(string bankCode, string codeType)
		{
			BankDirectorySearchResponse response;

			try
			{
				BankDirectorySearchRequest request​ = new BankDirectorySearchRequest()
				{
					//ServiceCallerIdentity = GetServiceCallerIdentity(),
					BankCode = bankCode,
					CodeType = GetBankDirectorySearchCodeType(codeType),
				};

				response = _service.BankDirectorySearch(request);
			}
			catch (Exception exeption)
			{
				response = new BankDirectorySearchResponse() { ServiceResponse = new ServiceResponse() { HasErrors = true, Responses = new ServiceResponseData[] { new ServiceResponseData() { Message = exeption.Message, MessageDetails = exeption.StackTrace } } } };
			}

			return response;
		}

		public BankDirectorySearchResponse BankDirectorySearch(string bankCode, BankDirectorySearchCodeType codeType)
		{
			BankDirectorySearchResponse response;

			try
			{
				BankDirectorySearchRequest request​ = new BankDirectorySearchRequest()
				{
					//ServiceCallerIdentity = GetServiceCallerIdentity(),
					BankCode = bankCode,
					CodeType = codeType,
				};

				response = _service.BankDirectorySearch(request);
			}
			catch (Exception exeption)
			{
				response = new BankDirectorySearchResponse() { ServiceResponse = new ServiceResponse() { HasErrors = true, Responses = new ServiceResponseData[] { new ServiceResponseData() { Message = exeption.Message, MessageDetails = exeption.StackTrace } } } };
			}

			return response;
		}

		private BankDirectorySearchCodeType GetBankDirectorySearchCodeType(string codeType)
		{
			var resultCodeType = BankDirectorySearchCodeType.BIC;

			switch (codeType)
			{
				case "FW":
					resultCodeType = BankDirectorySearchCodeType.FW;
					break;
				case "AU":
					resultCodeType = BankDirectorySearchCodeType.AU;
					break;
				case "NZ":
					resultCodeType = BankDirectorySearchCodeType.NZ;
					break;
				case "IN":
					resultCodeType = BankDirectorySearchCodeType.IN;
					break;
				default:
					resultCodeType = BankDirectorySearchCodeType.BIC;
					break;
			}

			return resultCodeType;
		}

		#endregion

		#region Deposit
		public DepositCreateResponse CreateDeposit(DepositCreateRequest request)
		{

			DepositCreateResponse res = new DepositCreateResponse();
			try
			{
				//request.ServiceCallerIdentity = GetServiceCallerIdentity();
				res = _service.DepositCreate(request);
			}
			catch (Exception e)
			{

			}

			return res;
		}
		#endregion

		#region Library Version
		public string GetLibraryVersion()
		{
			return _service.GetLibraryVersion(); ;
		}
		#endregion

		#region File Attachment
		public FileAttachmentAddFileResponse FileAttachmentAddFile(FileAttachmentAddFileRequest request)
		{
			FileAttachmentAddFileResponse response;

			try
			{
				//request.ServiceCallerIdentity = GetServiceCallerIdentity();

				response = _service.FileAttachmentAddFile(request);
			}
			catch (Exception exeption)
			{
				response = new FileAttachmentAddFileResponse() { ServiceResponse = new ServiceResponse() { HasErrors = true, Responses = new ServiceResponseData[] { new ServiceResponseData() { Message = exeption.Message, MessageDetails = exeption.StackTrace } } } };
			}

			return response;
		}

		public FileAttachmentUpdateFileInfoResponse FileAttachmentUpdateFileInfo(FileAttachmentUpdateFileInfoRequest request)
		{
			FileAttachmentUpdateFileInfoResponse response;

			try
			{
				//request.ServiceCallerIdentity = GetServiceCallerIdentity();

				response = _service.FileAttachmentUpdateFileInfo(request);
			}
			catch (Exception exeption)
			{
				response = new FileAttachmentUpdateFileInfoResponse() { ServiceResponse = new ServiceResponse() { HasErrors = true, Responses = new ServiceResponseData[] { new ServiceResponseData() { Message = exeption.Message, MessageDetails = exeption.StackTrace } } } };
			}

			return response;
		}

		public FileAttachmentGetFileDataResponse FileAttachmentGetFileData(FileAttachmentGetFileDataRequest request)
		{
			FileAttachmentGetFileDataResponse response;

			try
			{
				//request.ServiceCallerIdentity = GetServiceCallerIdentity();

				response = _service.FileAttachmentGetFileData(request);
			}
			catch (Exception exeption)
			{
				response = new FileAttachmentGetFileDataResponse() { ServiceResponse = new ServiceResponse() { HasErrors = true, Responses = new ServiceResponseData[] { new ServiceResponseData() { Message = exeption.Message, MessageDetails = exeption.StackTrace } } } };
			}

			return response;
		}

		public FileAttachmentGetFileListForObjectResponse FileAttachmentGetFileListForObject(FileAttachmentGetFileListForObjectRequest request)
		{
			FileAttachmentGetFileListForObjectResponse response;

			try
			{
				//request.ServiceCallerIdentity = GetServiceCallerIdentity();

				response = _service.FileAttachmentGetFileListForObject(request);
			}
			catch (Exception exeption)
			{
				response = new FileAttachmentGetFileListForObjectResponse() { ServiceResponse = new ServiceResponse() { HasErrors = true, Responses = new ServiceResponseData[] { new ServiceResponseData() { Message = exeption.Message, MessageDetails = exeption.StackTrace } } } };
			}

			return response;
		}

		public FileAttachmentDeleteFileResponse FileAttachmentDeleteFile(FileAttachmentDeleteFileRequest request)
		{
			FileAttachmentDeleteFileResponse response;

			try
			{
				//request.ServiceCallerIdentity = GetServiceCallerIdentity();

				response = _service.FileAttachmentDeleteFile(request);
			}
			catch (Exception exeption)
			{
				response = new FileAttachmentDeleteFileResponse() { ServiceResponse = new ServiceResponse() { HasErrors = true, Responses = new ServiceResponseData[] { new ServiceResponseData() { Message = exeption.Message, MessageDetails = exeption.StackTrace } } } };
			}

			return response;
		}

		#endregion White Label Profile

		#region White Label
		public WhiteLabelProfileSearchResponse WhiteLabelProfileSearch(int count, string whiteLabelProfileName = "")
		{
			int pageIndex = 0, pageSize = count;
			var sortDirection = WhiteLabelProfileSearchSortDirection.Descending;
			var sortBy = WhiteLabelProfileSearchSortBy.WhiteLabelProfileName;
			WhiteLabelProfileSearchResponse response = new WhiteLabelProfileSearchResponse();
			WhiteLabelProfileSearchRequest request = new WhiteLabelProfileSearchRequest
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				PageIndex = pageIndex,
				PageSize = pageSize,
				SortBy = sortBy,
				SortDirection = sortDirection,
				SearchOptions = new WhiteLabelProfileSearchOptions()
				{
					WhiteLabelProfileName = String.Empty,
				}
			};
			try
			{
				response = _service.WhiteLabelProfileSearch(request);
			}
			catch (Exception e)
			{
				response.ServiceResponse.HasErrors = true;
			}

			return response;
		}

		public WhiteLabelProfileGetSingleResponse WhiteLabelProfileGetSingle(string username, string password, string whiteLabelProfileId)
		{
			WhiteLabelProfileGetSingleRequest request = new WhiteLabelProfileGetSingleRequest
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(username, password),
				WhiteLabelProfileId = whiteLabelProfileId
			};

			return _service.WhiteLabelProfileGetSingle(request);

		}

		public WhiteLabelProfileGetSingleResponse WhiteLabelProfileGetSingle(string whiteLabelProfileId)
		{
			WhiteLabelProfileGetSingleRequest request = new WhiteLabelProfileGetSingleRequest
			{
				//ServiceCallerIdentity = GetServiceCallerIdentity(),
				WhiteLabelProfileId = whiteLabelProfileId
			};
			return _service.WhiteLabelProfileGetSingle(request);
		}

		#endregion

		#region Customer Liquidation Preferences

		public CustomerLiquidationPreferencesGetAllResponse GetCustomerLiquidationPreferences(string customerId)
		{
			CustomerLiquidationPreferencesGetAllRequest request = new CustomerLiquidationPreferencesGetAllRequest
			{
				CustomerId = customerId
			};

			_logger.InfoFormat("GetCustomerLiquidationPreferences, request: {0}", JsonConvert.SerializeObject(request));

			using (new OperationContextScope(_service.InnerChannel))
			{
				// Add a HTTP Header to an outgoing request
				HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
				requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", AccessToken);
				OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
				return _service.CustomerLiquidationPreferencesGetAll(request);
			}
		}

		public CustomerLiquidationPreferencesGetAllResponse GetCustomerLiquidationPreferences()
		{
			return GetCustomerLiquidationPreferences(CustomerId);
		}


		public CustomerLiquidationPreferencesAddOrUpdateCurrencyResponse AddOrUpdateCustomerLiquidationPreferenceCurrency(string currencyCode, string customerId, int liquidationOrder)
		{
			CustomerLiquidationPreferencesAddOrUpdateCurrencyRequest request = new CustomerLiquidationPreferencesAddOrUpdateCurrencyRequest
			{
				CurrencyCode = currencyCode,
				CustomerId = customerId,
				LiquidationOrder = liquidationOrder
			};

			using (new OperationContextScope(_service.InnerChannel))
			{
				// Add a HTTP Header to an outgoing request
				HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
				requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", AccessToken);
				OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
				return _service.CustomerLiquidationPreferencesAddOrUpdateCurrency(request);
			}
		}

		public CustomerLiquidationPreferencesAddOrUpdateCurrencyResponse AddOrUpdateCustomerLiquidationPreferenceCurrency(string currencyCode, int liquidationOrder)
		{
			return AddOrUpdateCustomerLiquidationPreferenceCurrency(currencyCode, CustomerId, liquidationOrder);
		}

		public CustomerLiquidationPreferencesRemoveCurrencyResponse RemoveCustomerLiquidationPreferenceCurrency(string currencyCode, string customerId)
		{
			CustomerLiquidationPreferencesRemoveCurrencyRequest request = new CustomerLiquidationPreferencesRemoveCurrencyRequest
			{
				CurrencyCode = currencyCode,
				CustomerId = customerId,
			};

			using (new OperationContextScope(_service.InnerChannel))
			{
				// Add a HTTP Header to an outgoing request
				HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
				requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", AccessToken);
				OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
				return _service.CustomerLiquidationPreferencesRemoveCurrency(request);
			}
		}

		public CustomerLiquidationPreferencesRemoveCurrencyResponse RemoveCustomerLiquidationPreferenceCurrency(string currencyCode)
		{
			return RemoveCustomerLiquidationPreferenceCurrency(currencyCode, CustomerId);
		}

		public CustomerLiquidationPreferencesUpdateAllResponse UpdateCustomerLiquidationPreferences(string currencyList, string customerId)
		{
			CustomerLiquidationPreferencesUpdateAllRequest request = new CustomerLiquidationPreferencesUpdateAllRequest
			{
				CurrencyList = currencyList,
				CustomerId = customerId,
			};

			_logger.InfoFormat("UpdateCustomerLiquidationPreferences, request: {0}", JsonConvert.SerializeObject(request));

			using (new OperationContextScope(_service.InnerChannel))
			{
				// Add a HTTP Header to an outgoing request
				HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
				requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", AccessToken);
				OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
				return _service.CustomerLiquidationPreferencesUpdateAll(request);
			}
		}

		public CustomerLiquidationPreferencesUpdateAllResponse UpdateCustomerLiquidationPreferences(string currencyList)
		{
			return UpdateCustomerLiquidationPreferences(currencyList, CustomerId);
		}

		#endregion
	}
}
