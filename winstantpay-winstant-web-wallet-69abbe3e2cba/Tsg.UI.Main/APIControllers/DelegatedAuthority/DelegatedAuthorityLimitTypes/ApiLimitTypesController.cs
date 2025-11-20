using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Tsg.UI.Main.Attributes;
using TSG.Models.APIModels;
using TSG.Models.ServiceModels.LimitPayment;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTypeServiceMethods;
using WinstantPay.Common.Extension;

namespace Tsg.UI.Main.APIControllers.DelegatedAuthority
{
    [ApiFilter]
    public class ApiLimitTypesController : ApiController
    {
        readonly static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IDaPayLimitsTypeServiceMethods _daPayLimitsTypeServiceMethods;

        public ApiLimitTypesController(IDaPayLimitsTypeServiceMethods daPayLimitsTypeServiceMethods)
        {
            _daPayLimitsTypeServiceMethods = daPayLimitsTypeServiceMethods;
        }

        // GET
        [HttpGet]
        public IHttpActionResult Get()
        {
            var getAllResQuery = _daPayLimitsTypeServiceMethods.GetAll();
            var newObjList = getAllResQuery.Obj.Where(w => !w.DaPayLimitsType_IsDeleted).ToList();
            return Ok(new StandartResponse<List<DaPayLimitsTypeSo>>()
                { Obj = newObjList, Success = newObjList.Count>0,
                    InfoBlock = new InfoBlock()
                    {
                        UserMessage = newObjList.Count > 0 ? $"Access getted by {newObjList.Count} type limitation": "Access denied",
                        DeveloperMessage = $"Total counts by limits {newObjList.Count}"
                    }
            });
        }
    }
}