using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;

namespace Tsg.UI.Main.Models
{
	public class InstantPaymentPostResultViewModel : BaseViewModel
	{
		readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		public InstantPaymentPostResultViewModel(Guid paymentId)
		{
			PaymentId = paymentId;
		}

		public Guid PaymentId { get; set; }
		public string Result { get; set; }

		public void Post()
		{
			InstantPaymentPostResponse response = Service.PostInstantPayment(this.PaymentId);
			_logger.Debug(string.Format("Post, response:{0}", JsonConvert.SerializeObject(response)));
			if (!response.ServiceResponse.HasErrors)
			{
				this.Result = "Success";
			}
			else
			{
				this.Result = response.ServiceResponse.Responses.FirstOrDefault().MessageDetails;
				this.GetErrorMessages(response.ServiceResponse.Responses);
			}
		}

	}
}