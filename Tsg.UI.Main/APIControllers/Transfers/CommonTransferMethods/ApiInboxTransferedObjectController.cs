using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Exceptions;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.Enums;
using TSG.Models.ServiceModels.Transfers.Reports;
using TSG.ServiceLayer.Transfers.Reports;

namespace Tsg.UI.Main.APIControllers.Transfers.CommonTransferMethods
{
    [ApiFilter]
    public class ApiInboxTransferedObjectController : ApiController
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IGetInboxListMethods _getInboxListMethods;
        public ApiInboxTransferedObjectController(IGetInboxListMethods getInboxListMethods)
        {
            _getInboxListMethods = getInboxListMethods;
        }

        [System.Web.Http.HttpGet]
        public IHttpActionResult Get()
        {
            UserRepository userRepository = new UserRepository();
            var result = new StandartResponse();
            try
            {
                List<Tuple<int, string>> res = new List<Tuple<int, string>>();
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out var ui))
                {
                    var resQuery = _getInboxListMethods.GetByRecipient(ui.UserName).Obj;
                    resQuery.ForEach(f =>
                    {
                        f.GetInboxList_Source = Enum.GetName(typeof(TransfersSourceTypeEnum), f.GetInboxList_SourceType)
                            ?.ToUpper();
                        f.GetInboxList_StatusEnumString = Enum.GetName(typeof(TransferStatusesEnum), f.GetInboxList_Status)
                            ?.ToUpper();
                        f.GetInboxList_TypeRecBySourceEnumString = Enum.GetName(typeof(DelegatedAuthorirySourceLimitationTypeEnum), f.GetInboxList_TypeRecBySource)
                            ?.ToUpper();
                    });

                    return Ok(new StandartResponse<List<GetInboxListSo>>(resQuery.OrderByDescending(ob=>ob.GetInboxList_CreatedDate).ToList(), true,"Ok"));
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
                result.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, DeveloperMessage = e.InnerException?.ToString() ?? e.Message, UserMessage = "We have some problem with your query. Please try again" };
                _logger.Error(e);
            }
            return Ok(result);
        }
    }

}