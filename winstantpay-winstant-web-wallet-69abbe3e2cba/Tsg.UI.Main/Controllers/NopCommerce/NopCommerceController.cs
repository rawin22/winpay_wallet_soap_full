using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using Tsg.UI.Main.APIControllers;

namespace Tsg.UI.Main.Controllers
{
    public class NopCommerceController : BaseController
    {
        // GET: NopCommerce
        [AllowAnonymous]
        public ActionResult GetPaymentDetails(string tx)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                var cookieToken = Request.Cookies["PublicToken"];
                string token = String.Empty;
                if (cookieToken != null)
                    token = cookieToken.Value;
                if (Guid.TryParse(tx, out var txId))
                {
                    var checkOrderIfExist = ApiLogic.CheckOrder(txId);
                    if (checkOrderIfExist.IsExist && checkOrderIfExist.Order.PaymentGuid!=null)
                    {
                        sb.AppendLine("SUCCESS");
                        sb.AppendLine("custom:" + checkOrderIfExist.Order.CustomerOrderId);
                        sb.AppendLine("mc_gross:" + checkOrderIfExist.Order.Quantity);
                        sb.AppendLine("payment_status:" + "completed");
                        sb.AppendLine("pending_reason:" + String.Empty);
                        sb.AppendLine("txn_id:" + txId);
                    }
                    else
                    {
                        sb.AppendLine("SUCCESS");
                        sb.AppendLine("custom:" + checkOrderIfExist.Order.CustomerOrderId);
                        sb.AppendLine("mc_gross:" + checkOrderIfExist.Order.Quantity);
                        sb.AppendLine("payment_status:" + "pending");
                        sb.AppendLine("pending_reason:" + String.Empty);
                        sb.AppendLine("txn_id:" + txId);
                    }
                }
                else
                {
                    sb.AppendLine("ERROR");
                    sb.AppendLine("custom:" + tx);
                    sb.AppendLine("mc_gross:" + decimal.Zero);
                    sb.AppendLine("payment_status:" + "failed");
                    sb.AppendLine("pending_reason:" + String.Empty);
                    sb.AppendLine("txn_id:" + tx);
                }
            }
            catch (Exception e)
            {
                sb.Clear();
                sb.AppendLine("ERROR");
                sb.AppendLine("custom:" + tx);
                sb.AppendLine("mc_gross:" + decimal.Zero);
                sb.AppendLine("payment_status:" + "failed");
                sb.AppendLine("pending_reason:" + String.Empty);
                sb.AppendLine("txn_id:" + tx);
            }

            return Json(sb.ToString());
        }
    }
}