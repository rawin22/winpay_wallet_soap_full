using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Attributes;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.APIModels.AutomaticExchange;
using TSG.ServiceLayer.AutomaticExchange.DependencyLiquidForUserServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidCcyListServiceMethods;

namespace Tsg.UI.Main.APIControllers.AutomaticExchange.LiquidCurrenciesUserSetting
{
    /// <summary>
    /// 
    /// </summary>
    [ApiFilter]
    [Route("api/")]
    public class ApiLiquidCurrenciesUserSettingController : ApiController
    {
        readonly static log4net.ILog _logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ILiquidCcyListServiceMethods _liquidCcyListServiceMethods;
        private readonly IDependencyLiquidForUserServiceMethods _dependencyLiquidForUserService;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyLiquidForUserService"></param>
        /// <param name="liquidCcyListServiceMethods"></param>
        public ApiLiquidCurrenciesUserSettingController(IDependencyLiquidForUserServiceMethods dependencyLiquidForUserService, ILiquidCcyListServiceMethods liquidCcyListServiceMethods)
        {
            _dependencyLiquidForUserService = dependencyLiquidForUserService;
            _liquidCcyListServiceMethods = liquidCcyListServiceMethods;
        }

        // GET
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("LiquidCurrenciesUserSetting")]
        public IHttpActionResult Get()
        {
            try
            {
                TSG.Models.APIModels.UserInfo ui;
                UserRepository userRepository = new UserRepository();
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    AutomaticExchangeCurrencies res = new AutomaticExchangeCurrencies();

                    var allliquidCurrency = _liquidCcyListServiceMethods.GetAll().Obj
                        .Where(w => w.LiquidCcyList_IsLiquidCurrency)
                        .OrderBy(ob=>ob.LiquidCcyList_LiquidOrder)
                        .Select(s => new AutomaticExchangeCurrency { CurrencyId = s.LiquidCcyList_Id, CurrencyCode = s.LiquidCcyList_CurrencyCode }).ToList();
                    var liquidsForUser = _dependencyLiquidForUserService.GetAllSoByUser(Guid.Parse(ui.UserId)).Obj
                        .OrderBy(ob=>ob.DependencyLiquidForUser_LiquidOrder)
                        .Select(s => new AutomaticExchangeCurrency { CurrencyId = s.DependencyLiquidForUser_LiquidCcyId, CurrencyCode = s.DependencyLiquidForUser_LiquidCcyList.LiquidCcyList_CurrencyCode }).ToList();
                    liquidsForUser.ForEach(f => allliquidCurrency.RemoveAll(a => a.CurrencyId == f.CurrencyId));
                    res.AvaliableLiquids = allliquidCurrency;
                    res.UserLiquids = liquidsForUser;
                    res.Success = true; res.InfoBlock = new InfoBlock("Ok");
                    return Ok(res);
                }

                return Content(HttpStatusCode.BadRequest, new StandartResponse("Undefined user"));
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Content(HttpStatusCode.BadRequest,
                    new StandartResponse()
                    {
                        Success = false,
                        InfoBlock = new InfoBlock()
                            {UserMessage = @GlobalRes.ShopingCardController_UnspecError, DeveloperMessage = e.Message}
                    });
            }
        }

        

        [HttpPost]
        [Route("LiquidCurrenciesUserSetting")]
        public IHttpActionResult Post([FromBody] LiquidCurrency model)
        {
            try
            {
                TSG.Models.APIModels.UserInfo ui;
                UserRepository userRepository = new UserRepository();
                IEnumerable<string> outerUserToken;
                var isUserTokenContains = Request.Headers.TryGetValues("X-User-Token", out outerUserToken);
                if (isUserTokenContains && userRepository.IsVerificatedUser(outerUserToken.First(), out ui))
                {
                    _dependencyLiquidForUserService.DeleteAllByUserId(Guid.Parse(ui.UserId));
                    if (model.ids!=null)
                    {
                        try
                        {
                            var resIns = _dependencyLiquidForUserService.BulkInsertLiquidCurrencyForUser(model.ids,Guid.Parse(ui.UserId));
                            return Json(new StandartResponse(resIns.Success, resIns.Message));
                        }
                        catch (Exception)
                        {

                        }
                        return Json(new StandartResponse(true, null));

                    }
                    return Content(HttpStatusCode.BadRequest, new StandartResponse("Invalid data"));
                }

                return Content(HttpStatusCode.Unauthorized, new StandartResponse("Undefined user"));
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Content(HttpStatusCode.BadRequest,
                    new StandartResponse()
                    {
                        Success = false,
                        InfoBlock = new InfoBlock()
                        { UserMessage = @GlobalRes.ShopingCardController_UnspecError, DeveloperMessage = e.Message }
                    });
            }
        }



    }
}