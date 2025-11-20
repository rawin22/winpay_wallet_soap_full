using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Tsg.UI.Main.ApiMethods.Payments;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Extensions;
using Tsg.UI.Main.Models.Security;
using TSG.Models.APIModels;
using TSG.Models.APIModels.InstantPayment;
using TSG.Models.Enums;
using TSG.Models.ServiceModels.Shop;
using TSG.ServiceLayer.WinstantPayShop.ShopInfoService;
using TSG.ServiceLayer.WinstantPayShop.ShopOrderItemsService;
using TSG.ServiceLayer.WinstantPayShop.ShopOrdersService;
using TSG.ServiceLayer.WinstantPayShop.ShopPaymentService;
using TSG.ServiceLayer.WinstantPayShop.ShopProductImagesService;
using TSG.ServiceLayer.WinstantPayShop.ShopProductService;
using WinstantPay.Common.Extension;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;
using Guid = System.Guid;

namespace Tsg.UI.Main.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShopOrdersService _shopOrdersService;
        private readonly IShopOrderItemsService _shopOrderItemsService;
        private readonly IShopProductService _shopProductService;
        private readonly IShopProductImagesService _shopProductImagesService;
        private readonly IShopInfoService _shopInfoService;
        private readonly IShopPaymentService _shopPaymentService;

        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ShoppingCartController(IShopOrdersService shopOrdersService, IShopOrderItemsService shopOrderItemsService, IShopProductService shopProductService,
            IShopProductImagesService shopProductImagesService, IShopInfoService shopInfoService, IShopPaymentService shopPaymentService)
        {
            _shopOrdersService = shopOrdersService;
            _shopOrderItemsService = shopOrderItemsService;
            _shopProductService = shopProductService;
            _shopProductImagesService = shopProductImagesService;
            _shopInfoService = shopInfoService;
            _shopPaymentService = shopPaymentService;
        }

        [HttpPost]
        public JsonResult AddProductItemToOrder(Guid productId, int quantity, string position)
        {
            var orderQueryRes = _shopOrdersService.GetOpenOrder(AppSecurity.CurrentUser.UserName);
            var productQueryRes = _shopProductService.GetById(productId);
            if (!productQueryRes.Success)
                return Json(new { res = productQueryRes.Success, message = productQueryRes.Message }, JsonRequestBehavior.AllowGet);

            if (!orderQueryRes.Success)
                return Json(new { res = orderQueryRes.Success, message = orderQueryRes.Message }, JsonRequestBehavior.AllowGet);

            if (quantity < 1)
                return Json(new { res = false, message = GlobalRes.ShopCard_QuantityNotPositive }, JsonRequestBehavior.AllowGet);

            var insItemToOrderQueryRes = _shopOrderItemsService.InsertUpdate(new ShopOrderItems()
            {
                ShopOrderItems_Price = productQueryRes.Obj.ShopProducts_Price,
                ShopOrderItems_Timestamp = DateTime.Now.Ticks,
                ShopOrderItems_Quantity = quantity,
                ShopOrderItems_CurrencyCode = productQueryRes.Obj.ShopProducts_CurrencyCode,
                ShopOrderItems_ProductId = productQueryRes.Obj.ShopProducts_ID,
                ShopOrderItems_OrderId = orderQueryRes.Obj.ShopOrders_ID,
                ShopOrderItems_Position = position
            });
            if (!insItemToOrderQueryRes.Success)
                return Json(new { res = insItemToOrderQueryRes.Success, message = insItemToOrderQueryRes.Message },
                    JsonRequestBehavior.AllowGet);
            else return Json(new { res = true, message = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCount()
        {
            try
            {
                var orderQueryRes = _shopOrdersService.GetOpenOrder(AppSecurity.CurrentUser.UserName);
                var count = orderQueryRes.Obj.ShopOrders_ShopOrderItems?.Count ?? 0;
                return Json(new { res = true, message = GlobalRes.CommonObjects_OK, count = count });
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Json(new { res = true, message = GlobalRes.CommonObjects_Error, count = -1 });
            }
        }

        [HttpGet]
        public ActionResult GetOrdersItem()
        {
            try
            {
                var orderQueryRes = _shopOrdersService.GetOpenOrder(AppSecurity.CurrentUser.UserName);
                if (orderQueryRes.Obj.ShopOrders_ShopOrderItems.Count == 0)
                    return RedirectToAction("Index", "ShopWindow");
                var dictCurrensies = new Dictionary<string, decimal>();
                var ccys = orderQueryRes.Obj.ShopOrders_ShopOrderItems.GroupBy(gb => gb.ShopOrderItems_CurrencyCode);
                foreach (var ccy in ccys)
                {
                    var allByCcy = orderQueryRes.Obj.ShopOrders_ShopOrderItems
                        .Where(w => w.ShopOrderItems_CurrencyCode == ccy.Key).Select(s =>
                            s.ShopOrderItems_Price * Convert.ToDecimal(s.ShopOrderItems_Quantity)).Sum();

                    dictCurrensies.Add(ccy.Key, allByCcy);
                }

                ViewBag.TotalCost = dictCurrensies;
                return View(orderQueryRes.Obj);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return View(new ShopOrders());
            }
        }
        
        [HttpPost]
        public ActionResult PayOrder(Guid orderId)
        {
            _logger.Info($"*Query started* UserId [{AppSecurity.CurrentUser.UserId}] User name [{AppSecurity.CurrentUser.UserName}]");
            _logger.Info($"Order ID[{orderId}]");

            _logger.Info("Get last opened order process");

            var orderQueryRes = _shopOrdersService.GetOpenOrder(AppSecurity.CurrentUser.UserName);
            var orderQueryResById  = _shopOrdersService.GetById(orderId);
            _logger.Info($"Query for last opened order process is: {(orderQueryRes.Message == String.Empty ? "Success" : orderQueryRes.Message)}");

            PayedShopModel returnModel = new PayedShopModel();
            try
            {
                if (orderQueryResById==null && (orderQueryRes.Obj == null || orderQueryRes.Obj.ShopOrders_ID != orderId))
                {
                    _logger.Error($"Order by Id[{orderId}] not found");
                    _logger.Info("******* End query *********");
                    return HttpNotFound(GlobalRes.CommonObjects_OrderNotFound);
                }

                if (orderQueryResById.Obj != null && orderQueryResById.Obj.ShopOrders_LastShopOrderLog.ShopOrderLog_Status == (int)ShopOrderEnum.SuccessifullyPayed)
                {
                    _logger.Error($"Order by Id[{orderId}] payed early");
                    returnModel.PaymentOrderId = orderQueryResById.Obj.ShopOrders_OrderCounter.ToString();
                    returnModel.Success = true;
                    returnModel.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.Success, UserMessage = "Order succesifull payed.", DeveloperMessage = "Order succesifull payed." };
                    _logger.Info("******* End query *********");
                    return View("~/Views/ShoppingCart/_OrderSuccessful.cshtml", returnModel);
                }

                _logger.Info("Process for getting currencies by order");

                var dictCurrensies = new Dictionary<string, decimal>();

                var ccys = orderQueryRes.Obj.ShopOrders_ShopOrderItems.Where(w => !w.ShopOrderItems_IsPayed).GroupBy(gb => gb.ShopOrderItems_CurrencyCode);

                foreach (var ccy in ccys)
                {
                    var allByCcy = orderQueryRes.Obj.ShopOrders_ShopOrderItems.Where(w => w.ShopOrderItems_CurrencyCode == ccy.Key).Select(s =>
                           s.ShopOrderItems_Price * Convert.ToDecimal(s.ShopOrderItems_Quantity)).Sum();
                    dictCurrensies.Add(ccy.Key, allByCcy);
                }

                ViewBag.TotalCost = dictCurrensies;
                _logger.Info($"Create Instant Payment object for {AppSecurity.CurrentUser.UserName}");
                NewInstantPaymentMethods nimp = new NewInstantPaymentMethods(AppSecurity.CurrentUser);
                _logger.Info($"Prepare account balances");
                var balances = nimp.PrepareAccountBalances();

                _logger.Info($"Cheking balances");

                var joinedListBalancesAndTotalAmounts = dictCurrensies.Join(balances.Balances.ToList(),
                    pair => pair.Key, balance => balance.CCY, (pair, balance) => new { pair, balance }).ToList();
                bool avaliablePayment = joinedListBalancesAndTotalAmounts.All(a => a.pair.Value <= a.balance.BalanceAvailable);
                if (!avaliablePayment)
                {
                    _logger.Info($"Insufficient balance");
                    _logger.Info("******* End query *********");
                    returnModel.Success = false;
                    StringBuilder sb = new StringBuilder();
                    foreach (var joinedItem in joinedListBalancesAndTotalAmounts.Where(w => w.pair.Value > w.balance.BalanceAvailable))
                    {
                        sb.AppendLine(String.Format(GlobalRes.ShopingCardController_InsufficientBalanceMessage, joinedItem.pair.Key));
                    }

                    returnModel.InfoBlock = new InfoBlock() { DeveloperMessage = GlobalRes.ShopingCardController_InsufficientBalance, UserMessage = sb.ToString() };
                    return View("~/Views/ShoppingCart/_OrderSuccessful.cshtml", returnModel);
                }

                _logger.Info("Grouping payments by merchant");
                var groupedMerchants = orderQueryRes.Obj.ShopOrders_ShopOrderItems.Where(w => !w.ShopOrderItems_IsPayed)
                    .Select(s => s.ShopOrderItems_ShopProduct.ShopProducts_ShopInfo.ShopInfo_ID).GroupBy(gb => gb);
                _logger.Info("Get all shops");
                var shops = _shopInfoService.GetAll();
                _logger.Info($"Get all aliases for {AppSecurity.CurrentUser.UserName} [{AppSecurity.CurrentUser.UserId}]");
                var alias = nimp.PrepareAccountAliases()[0].Value;
                var dict = new List<StandartResponse>();
                Guid? paymentGuid = null;

                foreach (var groupedMerchant in groupedMerchants)
                {
                    _logger.Info($"Current merchant is: {groupedMerchant.Key}");
                    var allItemsByMerchant = orderQueryRes.Obj.ShopOrders_ShopOrderItems.Where(w =>
                        w.ShopOrderItems_ShopOrder.ShopOrders_BuyerWpayId == AppSecurity.CurrentUser.UserName &&
                        w.ShopOrderItems_ShopProduct.ShopProducts_ShopInfo.ShopInfo_ID == groupedMerchant.Key).ToList();

                    var merchantCcys = allItemsByMerchant.GroupBy(gb => gb.ShopOrderItems_CurrencyCode);
                    foreach (var merchantCcy in merchantCcys)
                    {
                        _logger.Info($"Current currency {merchantCcy.Key } for merchant: {groupedMerchant.Key}");

                        var orderItems = allItemsByMerchant.Where(w => w.ShopOrderItems_CurrencyCode == merchantCcy.Key).ToList();

                        var itemSum = orderItems.Select(s => s.ShopOrderItems_Price * Convert.ToDecimal(s.ShopOrderItems_Quantity)).Sum();

                        ApiNewInstantPaymentViewModel apiNewInstantPayment = new ApiNewInstantPaymentViewModel();
                        apiNewInstantPayment.FromCustomer = alias;
                        apiNewInstantPayment.ToCustomer = shops.Obj.FirstOrDefault(f => f.ShopInfo_ID == groupedMerchant.Key)?.ShopInfo_OwnerWpayId;
                        apiNewInstantPayment.Amount = itemSum;
                        apiNewInstantPayment.CurrencyCode = merchantCcy.Key;
                        apiNewInstantPayment.Invoice = $"{shops.Obj.FirstOrDefault(f => f.ShopInfo_ID == groupedMerchant.Key)?.ShopInfo_Name ?? "UNDEFINED"}-{DateTime.Now:yyMMdd}{orderQueryRes.Obj.ShopOrders_OrderCounter}";
                        apiNewInstantPayment.Memo = $"Purchase from {shops.Obj.FirstOrDefault(f => f.ShopInfo_ID == groupedMerchant.Key)?.ShopInfo_Name ?? "UNDEFINED"}: {orderItems.Select(s => s.ShopOrderItems_ShopProduct.ShopProducts_Name).AppendAll(", ")}";

                        _logger.Info("Create new payment");
                        var minpCreate = nimp.Create(apiNewInstantPayment);
                        List<Tuple<Guid, bool, string>> dicPayments = new List<Tuple<Guid, bool, string>>();
                        if (!minpCreate.ServiceResponse.HasErrors)
                        {
                            _logger.Info($"Created successifully:  {minpCreate.ServiceResponse.Responses[0].Message}");
                            _logger.Info($"Full message:  {minpCreate.ServiceResponse.Responses[0].MessageDetails}");

                            _logger.Info($"Create record in db by order Item");
                            foreach (var item in orderItems)
                            {
                                if (orderQueryRes.Obj.ShopOrders_ShopPaymentByOrder.FirstOrDefault(w =>
                                        w.ShopPayment_OrderItemId == item.ShopOrderItems_ID &&
                                        w.ShopPayment_OrderId == item.ShopOrderItems_OrderId) == null)
                                {
                                    var queryRes = _shopPaymentService.InsertUpdate(new ShopPayment()
                                    {
                                        ShopPayment_OrderId = orderId,
                                        ShopPayment_OrderItemId = item.ShopOrderItems_ID,
                                        ShopPayment_PaymentAttemptDate = DateTime.Now,
                                        ShopPayment_Status = (int)ShopOrderEnum.Created,
                                        ShopPayment_PaymentNumber = minpCreate.PaymentInformation.PaymentReference
                                    });
                                    dicPayments.Add(new Tuple<Guid, bool, string>(item.ShopOrderItems_ID,
                                        queryRes.Success, queryRes.Message));
                                }
                            }

                            if (dicPayments.Any(a => a.Item2 == false))
                                break;

                            var post = nimp.Post(Guid.Parse(minpCreate.PaymentInformation.PaymentId));
                            if (post.ServiceResponse.HasErrors)
                            {
                                _logger.Error($"Posted error:  {post.ServiceResponse.Responses[0].Message}");
                                _logger.Error($"Full message:  {post.ServiceResponse.Responses[0].MessageDetails}");
                                dict.Add(new StandartResponse() { Success = !post.ServiceResponse.HasErrors, InfoBlock = new InfoBlock() { UserMessage = post.ServiceResponse.Responses[0].MessageDetails } });
                                _shopPaymentService.UpdateByOrderItem(orderId, orderItems.Select(s => s.ShopOrderItems_ID).ToList(), (int)ShopOrderEnum.PayedFail, $"{post.ServiceResponse.Responses[0].MessageDetails}", null);
                            }
                            else
                            {
                                _logger.Info($"Posted success:  {post.ServiceResponse.Responses[0].Message}");
                                _logger.Info($"Full message:  {post.ServiceResponse.Responses[0].MessageDetails}");
                                paymentGuid = Guid.Parse(minpCreate.PaymentInformation.PaymentId);
                                _shopPaymentService.UpdateByOrderItem(orderId,
                                    orderItems.Select(s => s.ShopOrderItems_ID).ToList(), (int)ShopOrderEnum.SuccessifullyPayed, $"{post.ServiceResponse.Responses[0].MessageDetails}", paymentGuid);
                            }
                        }
                        else
                        {
                            _logger.Error($"Created error:  {minpCreate.ServiceResponse.Responses[0].Message}");
                            _logger.Error($"Full message:  {minpCreate.ServiceResponse.Responses[0].MessageDetails}");
                            dict.Add(new StandartResponse() { Success = !minpCreate.ServiceResponse.HasErrors, InfoBlock = new InfoBlock() { UserMessage = minpCreate.ServiceResponse.Responses[0].MessageDetails } });
                        }
                    }
                }
                if (dict.All(a => a.Success))
                {
                    var closeQuery = _shopOrdersService.CloseOrder(orderId, (int)ShopOrderEnum.SuccessifullyPayed);

                    if (closeQuery.Success)
                    {
                        _logger.Info($"Save payment by order ID [{orderId}] success");
                        returnModel.Success = closeQuery.Success;
                        returnModel.PaymentOrderId = $"{orderQueryRes.Obj.ShopOrders_OrderCounter}";
                        returnModel.PaymentOrderGuid = orderId.ToString();
                    }
                    else
                    {
                        _logger.Error($"Db error for payment saving Id [{orderId}], message: {closeQuery.Message}");
                        returnModel.Success = closeQuery.Success;
                        returnModel.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, UserMessage = GlobalRes.ShopingCardController_UnspecError, DeveloperMessage = closeQuery.Message };
                    }
                }
                else
                {
                    _logger.Error(dict[0].InfoBlock);
                    returnModel.InfoBlock = dict[0].InfoBlock;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                returnModel.InfoBlock = new InfoBlock() { Code = ApiErrors.ErrorCodeState.UnspecifiedError, UserMessage = GlobalRes.ShopingCardController_UnspecError, DeveloperMessage = e.Message };
            }
            _logger.Info("******* End query *********");

            return View("~/Views/ShoppingCart/_OrderSuccessful.cshtml", returnModel);
        }

        [HttpPost]
        public ActionResult GetProductItemDetailsModal(Guid orderItemId)
        {
            _logger.Info($"*Query started* UserId [{AppSecurity.CurrentUser.UserId}] User name [{AppSecurity.CurrentUser.UserName}]");
            _logger.Info($"Get order item by ID {orderItemId}");
            var shopOrderItem = _shopOrderItemsService.GetById(orderItemId);
            _logger.Info($"Query message is: {(shopOrderItem.Message == String.Empty ? "Success" : shopOrderItem.Message)}");

            if (shopOrderItem.Success && shopOrderItem.Obj != null)
            {
                _logger.Info($"Object with ID=[{orderItemId}] exist and status is {EnumExtensions.GetDictionary<ShopOrderEnum>().FirstOrDefault(f => f.Value == shopOrderItem.Obj.ShopOrderItems_ShopOrder.ShopOrders_LastShopOrderLog?.ShopOrderLog_Status).Key}");
                if (shopOrderItem.Obj.ShopOrderItems_ShopOrder.ShopOrders_LastShopOrderLog?.ShopOrderLog_Status == (int)ShopOrderEnum.Opened ||
                    shopOrderItem.Obj.ShopOrderItems_ShopOrder.ShopOrders_LastShopOrderLog?.ShopOrderLog_Status == (int)ShopOrderEnum.Created)
                {
                    shopOrderItem.Obj.ShopOrderItems_ShopOrder =
                        _shopOrdersService.GetById(shopOrderItem.Obj.ShopOrderItems_OrderId).Obj;
                    shopOrderItem.Obj.ShopOrderItems_ShopProduct =
                        _shopProductService.GetById(shopOrderItem.Obj.ShopOrderItems_ProductId).Obj;
                    shopOrderItem.Obj.ShopOrderItems_ShopProduct.ShopProducts_ShopProductImages = _shopProductImagesService
                        .GetImagesByProductId(shopOrderItem.Obj.ShopOrderItems_ShopProduct.ShopProducts_ID).Obj;
                    _logger.Info("******* End query *********");

                    return PartialView("~/Views/ShoppingCart/_EditOrderItem.cshtml", shopOrderItem.Obj);
                }
                else
                {
                    _logger.Warn("Order does not avaliable to change");
                    _logger.Info("******* End query *********");
                    return PartialView("~/Views/ShoppingCart/_OrderSuccessful.cshtml", new PayedShopModel() { Success = false, InfoBlock = new InfoBlock() { UserMessage = GlobalRes.ShopingCardController_UnavaliableOrderChange } });
                }
            }
            else if (shopOrderItem.Success && shopOrderItem.Obj == null)
            {
                _logger.Warn($"Order by this item Id[{orderItemId}] is not exist");
                _logger.Warn($"{shopOrderItem.Message}");
                _logger.Info("******* End query *********");
                return PartialView("~/Views/ShoppingCart/_OrderSuccessful.cshtml", new PayedShopModel() { Success = false, InfoBlock = new InfoBlock() { UserMessage = GlobalRes.ShopingCardController_OrderNotExists } });
            }
            _logger.Info("******* End query *********");

            return PartialView("~/Views/ShoppingCart/_OrderSuccessful.cshtml", new PayedShopModel() { Success = false, InfoBlock = new InfoBlock() { UserMessage = shopOrderItem.Message } });
        }

        [HttpPost]
        public JsonResult UpdateProductItem(ShopOrderItems model)
        {
            _logger.Info($"*Query started* UserId [{AppSecurity.CurrentUser.UserId}] User name [{AppSecurity.CurrentUser.UserName}]");
            _logger.Info($"Get order item by ID {model.ShopOrderItems_ID}");

            var currItem = _shopOrderItemsService.GetById(model.ShopOrderItems_ID);
            _logger.Info($"Query message is: {(currItem.Message == String.Empty ? "Success" : currItem.Message)}");

            currItem.Obj.ShopOrderItems_ShopOrder = _shopOrdersService.GetById(currItem.Obj.ShopOrderItems_OrderId).Obj;

            if (!(currItem.Obj.ShopOrderItems_ShopOrder?.ShopOrders_LastShopOrderLog?.ShopOrderLog_Status == (int)ShopOrderEnum.Created || currItem.Obj.ShopOrderItems_ShopOrder?.ShopOrders_LastShopOrderLog?.ShopOrderLog_Status == (int)ShopOrderEnum.Opened))
            {
                _logger.Warn("Order does not avaliable to change");
                _logger.Info("******* End query *********");
                return Json(new Result(GlobalRes.ShopingCardController_UnavaliableOrderChange));
            }
            if (currItem.Obj.ShopOrderItems_IsPayed)
            {
                _logger.Warn("Order does not avaliable to change. Order item payed");
                _logger.Info("******* End query *********");
                return Json(new Result(GlobalRes.ShopingCardController_OrderAlreadyPayed));
            }

            var res = new Result();
            if (currItem.Success && currItem.Obj != null)
            {
                if (model.ShopOrderItems_Quantity < 1)
                {
                    _logger.Warn($"Order Item quantity is : {model.ShopOrderItems_Quantity}");
                    _logger.Info("Deletion process started");
                    res = _shopOrderItemsService.Delete(model.ShopOrderItems_ID) as Result;
                    _logger.Info($"Deletion process ended with result: Item Id [{model.ShopOrderItems_ID}] Result: {(res?.Message == String.Empty ? "Success" : res?.Message)} ");

                }
                else
                {
                    _logger.Info($"Order Item quantity is : {model.ShopOrderItems_Quantity}");
                    _logger.Info("Changing process started");
                    currItem.Obj.ShopOrderItems_Quantity = model.ShopOrderItems_Quantity;
                    if (!string.IsNullOrEmpty(model.ShopOrderItems_Position))
                        currItem.Obj.ShopOrderItems_Position = model.ShopOrderItems_Position;

                    res = _shopOrderItemsService.InsertUpdate(currItem.Obj) as Result;
                    _logger.Info($"Deletion process ended with result: Item Id [{model.ShopOrderItems_ID}] Result: {(res?.Message == String.Empty ? "Success" : res?.Message)} ");

                }
                _logger.Info("******* End query *********");
            }
            else
            {
                _logger.Warn($"Warning for Item [{model.ShopOrderItems_ID}], with message: {currItem.Message}");
            }
            return Json(res);
        }

        [HttpPost]
        public JsonResult Delete(Guid ID)
        {
            _logger.Info($"*Query started* UserId [{AppSecurity.CurrentUser.UserId}] User name [{AppSecurity.CurrentUser.UserName}]");
            _logger.Info($"Get order item by ID {ID}");
            var queryRes = _shopOrderItemsService.GetById(ID);
            _logger.Info($"Query message is: {(queryRes.Message == String.Empty ? "Success" : queryRes.Message)}");
            if (queryRes.Success && queryRes.Obj != null)
            {
                var order = _shopOrdersService.GetById(queryRes.Obj.ShopOrderItems_OrderId);

                _logger.Info($"Object with ID=[{ID}] exist and status is {EnumExtensions.GetDictionary<ShopOrderEnum>().FirstOrDefault(f => f.Value == queryRes.Obj.ShopOrderItems_ShopOrder.ShopOrders_LastShopOrderLog?.ShopOrderLog_Status).Key}");
                if (queryRes.Obj.ShopOrderItems_ShopOrder.ShopOrders_LastShopOrderLog?.ShopOrderLog_Status == (int)ShopOrderEnum.Opened ||
                 queryRes.Obj.ShopOrderItems_ShopOrder.ShopOrders_LastShopOrderLog?.ShopOrderLog_Status == (int)ShopOrderEnum.Created)
                {
                    if (queryRes.Obj.ShopOrderItems_IsPayed)
                    {
                        _logger.Warn("Order does not avaliable to change. Order item payed");
                        _logger.Info("******* End query *********");
                        return Json(new Result(GlobalRes.ShopingCardController_OrderAlreadyPayed));
                    }

                    var deleteQuery = _shopOrderItemsService.Delete(ID);
                    _logger.Info($"Delete item result by Item Id[{ID}] is {(deleteQuery.Message == String.Empty ? "Success" : deleteQuery.Message)}");
                    var deletePaymentItemRec = _shopPaymentService.DeleteByOrderItemId(ID);
                    _logger.Info($"Delete payment item result by Item Id[{ID}] is {(deletePaymentItemRec.Message == String.Empty ? "Success" : deletePaymentItemRec.Message)}");
                    _logger.Info("Check order items");
                    if (order.Obj.ShopOrders_ShopPaymentByOrder.Count > 0 &&
                        order.Obj.ShopOrders_ShopPaymentByOrder.All(a =>
                            a.ShopPayment_Status == (int)ShopOrderEnum.SuccessifullyPayed))
                    {
                        _logger.Info("All items is payed. Order will be close");
                        var closeOrder = _shopOrdersService.CloseOrder(order.Obj.ShopOrders_ID, (int)ShopOrderEnum.SuccessifullyPayed);
                        _logger.Info(closeOrder.Success ? "Success" : closeOrder.Message);
                    }
                    else if (order.Obj.ShopOrders_ShopPaymentByOrder.Count == 0)
                    {
                        _logger.Info("Order is empty, delete order");
                        var deleteOrder = _shopOrdersService.Delete(order.Obj.ShopOrders_ID);
                        _logger.Info(deleteOrder.Success ? "Success" : deleteOrder.Message);
                    }

                    _logger.Info("******* End query *********");

                    return Json(deleteQuery as Result);


                }
                else
                {
                    _logger.Warn($"Order by this item Id[{ID}] is closed");
                    _logger.Info("******* End query *********");
                    return Json(new Result(String.Format(GlobalRes.ShopingCardController_OrderClosed, ID)));
                }


            }
            else if (queryRes.Success && queryRes.Obj == null)
            {
                _logger.Warn($"Order by this item Id[{ID}] is not exist");
                _logger.Warn($"{queryRes.Message}");
                _logger.Info("******* End query *********");

                return Json(new Result(String.Format(GlobalRes.ShopingCardController_OrderNotExistsWithId, ID)));
            }
            
            _logger.Info("******* End query *********");
            return Json(new Result($"Object not found by Id {ID}"));
        }

        [HttpGet]
        public ActionResult GetListOrdersByUser()
        {
            if (AppSecurity.CurrentUser == null)
                RedirectToAction("Login", "User");

            var allOrders = _shopOrdersService.GetAll().Obj
                .Where(w => w.ShopOrders_BuyerWpayId == AppSecurity.CurrentUser.UserName
                            && !(w.ShopOrders_LastShopOrderLog.ShopOrderLog_Status == (int)ShopOrderEnum.Opened || w.ShopOrders_LastShopOrderLog.ShopOrderLog_Status == (int)ShopOrderEnum.Created)
                            && w.ShopOrders_ShopOrderItems.Count > 0);
            foreach (var shopOrderse in allOrders)
            {
                shopOrderse.ShopOrders_ShopPaymentByOrder =
                    _shopPaymentService.GetByOrderId(shopOrderse.ShopOrders_ID).Obj;
            }

            return View(allOrders);
        }

        [HttpPost]
        public ActionResult GetOrderDetailtById(Guid orderId)
        {
            _logger.Info($"*Query started* UserId [{AppSecurity.CurrentUser.UserId}] User name [{AppSecurity.CurrentUser.UserName}]");
            _logger.Info($"Get order by ID {orderId}");
            var item = _shopOrdersService.GetById(orderId);
            _logger.Info($"Query message is: {(item.Message == String.Empty ? "Success" : item.Message)}");

            if (item.Success && item.Obj != null)
            {
                _logger.Info($"Object with ID=[{orderId}] exist and status is {EnumExtensions.GetDictionary<ShopOrderEnum>().FirstOrDefault(f => f.Value == item.Obj.ShopOrders_LastShopOrderLog.ShopOrderLog_Status).Key}");
                var joined = item.Obj.ShopOrders_ShopPaymentByOrder.Join(item.Obj.ShopOrders_ShopOrderItems,
                    payment => payment.ShopPayment_OrderItemId, items => items.ShopOrderItems_ID,
                    (payment, items) => new {payment, items}).ToList();

                List<Tuple<string, string, decimal, string>> dictCurrensies = new List<Tuple<string, string, decimal, string>>();

                var groupedByMerchAndCurrency = joined.GroupBy(gb => gb.payment.ShopPayment_PaymentId);
                foreach (var merchantAndCcy in groupedByMerchAndCurrency)
                {
                    var currRec = joined.Where(f => f.payment.ShopPayment_PaymentId == merchantAndCcy.Key).ToList();
                    var items = currRec.Select(s => s.items).ToList();
                    dictCurrensies.Add(new Tuple<string, string, decimal, string>(currRec.FirstOrDefault()?.payment.ShopPayment_PaymentNumber, items.FirstOrDefault()?.ShopOrderItems_CurrencyCode,
                        items.Where(w => w.ShopOrderItems_CurrencyCode == items.FirstOrDefault()?.ShopOrderItems_CurrencyCode).Select(s =>
                            s.ShopOrderItems_Price * Convert.ToDecimal(s.ShopOrderItems_Quantity)).Sum(), currRec.FirstOrDefault()?.payment.ShopPayment_PaymentId?.ToString()?? ""));
                }
                ViewBag.TotalCost = dictCurrensies;

                return PartialView("~/Views/ShoppingCart/_ViewOrder.cshtml", item.Obj);
            }
            _logger.Info("******* End query *********");

            return RedirectToAction("Index", "Home");
        }
    }
}