using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Http;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.APIModels.TransfersApiModel;
using TSG.Models.DTO.LimitPayment;
using TSG.Models.DTO.Transfers;
using TSG.Models.Enums;
using TSG.Models.ServiceModels.LimitPayment;
using TSG.Models.ServiceModels.Transfers.RedEnvelope;
using TSG.ServiceLayer.AutomaticExchange.DependencyLiquidForUserServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidCcyListServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidOverDraftUserServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTabServiceMethods;
using TSG.ServiceLayer.RedEnvelope;
using TSG.ServiceLayer.Transfers;
using TSG.ServiceLayer.Users;

namespace Tsg.UI.Main.APIControllers.Transfers.TrustedTokenTransfers
{
    public class ApiTrustedTokenDetailsController : ApiController
    {
        readonly log4net.ILog _logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IUsersServiceMethods _usersServiceMethods;
        private readonly IDependencyLiquidForUserServiceMethods _dependencyLiquidForUserService;
        private readonly ILiquidCcyListServiceMethods _liquidCcyListServiceMethods;
        private readonly ILiquidOverDraftUserServiceMethods _liquidOverDraftUserServiceMethods;
        private readonly IRedEnvelopeServiceMethods _envelopeServiceMethods;
        private readonly ITransfersServiceMethods _transfersServiceMethods;
        private readonly IDaPayLimitsServiceMethods _daPayLimitsService;
        private readonly IDaPayLimitsTabServiceMethods _daPayLimitsTabServiceMethods;
        private static readonly string cryptoCurrency = ConfigurationManager.AppSettings["cryptoCurrencies"];

        public ApiTrustedTokenDetailsController(IDependencyLiquidForUserServiceMethods dependencyLiquidForUserService,
            ILiquidCcyListServiceMethods liquidCcyListServiceMethods,
            ILiquidOverDraftUserServiceMethods liquidOverDraftUserServiceMethods,
            IUsersServiceMethods usersServiceMethods,
            IRedEnvelopeServiceMethods envelopeServiceMethods, ITransfersServiceMethods transfersServiceMethods,
            IDaPayLimitsServiceMethods daPayLimitsService,
            IDaPayLimitsTabServiceMethods daPayLimitsTabServiceMethods)
        {
            _usersServiceMethods = usersServiceMethods;
            _dependencyLiquidForUserService = dependencyLiquidForUserService;
            _liquidCcyListServiceMethods = liquidCcyListServiceMethods;
            _liquidOverDraftUserServiceMethods = liquidOverDraftUserServiceMethods;
            _envelopeServiceMethods = envelopeServiceMethods;
            _transfersServiceMethods = transfersServiceMethods;
            _daPayLimitsService = daPayLimitsService;
            _daPayLimitsTabServiceMethods = daPayLimitsTabServiceMethods;
        }

        [HttpGet]
        public IHttpActionResult Get(Guid rowId)
        {
            UserRepository userRepository = new UserRepository();
            var result = new StandartResponse();
            try
            {
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out var ui))
                {
                    var transfer = _transfersServiceMethods.GetById(rowId);
                    if (!transfer.Success || transfer.Obj == null || transfer.Obj.Transfers_SourceType != TransfersSourceTypeEnum.TokenTransfers)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("Error by token get details", transfer.Message));

                    if (!transfer.Obj.Transfers_LinkToSourceRow.HasValue)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("Error by token get details", transfer.Message));

                    var trustedToken = _daPayLimitsService.GetById(transfer.Obj.Transfers_LinkToSourceRow.Value);
                    if (!trustedToken.Success || trustedToken.Obj == null)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("Error by token get details", trustedToken.Message));
                    if(trustedToken.Obj.DaPayLimits_UserName != ui.UserName)
                        return Content(HttpStatusCode.BadRequest, new StandartResponse("Error by token get details", trustedToken.Message));

                    var trustedTokenDetails = _daPayLimitsTabServiceMethods.GetAllLimitsByKey(trustedToken.Obj.DaPayLimits_ID);
                    //trustedToken.Obj.DaPayLimitsTabs = trustedTokenDetails.Obj;
                    return Content(HttpStatusCode.OK, new StandartResponse<DaPayLimitsSo>(trustedToken.Obj, trustedToken.Success, trustedToken.Message));
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (ApiException apiException)
            {
                result.InfoBlock = apiException.CustomInfoBlock;
                _logger.Error(apiException);
            }
            catch (Exception e)
            {
                result.InfoBlock = new InfoBlock()
                {
                    Code = ApiErrors.ErrorCodeState.UnspecifiedError,
                    DeveloperMessage = e.InnerException?.ToString() ?? e.Message,
                    UserMessage = "We have some problem with your query. Please try again"
                };
                _logger.Error(e);
            }

            return Ok(result);
        }
    }
}