using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tsg.Business.Model.TsgGPWebService;

namespace TSG.Models.APIModels.Payment
{
    public class ApiValidatePaymentResponse : StandartResponse
    {
        public IList<ApiErrorModel> Errors { get; set; }


        public ApiValidatePaymentResponse()
        {
            Errors = new List<ApiErrorModel>();
        }

        public void GetErrorMessages(ServiceResponseData[] responses)
        {

            foreach (var error in responses)
            {
                this.Errors.Add(new ApiErrorModel
                {
                    Code = error.ResponseCode,
                    Type = (ApiErrorType)error.ResponseType,
                    Message = error.Message,
                    MessageDetails = error.MessageDetails,
                    FieldName = error.FieldName,
                    FieldValue = error.FieldValue
                });
            }
        }
    }
}
