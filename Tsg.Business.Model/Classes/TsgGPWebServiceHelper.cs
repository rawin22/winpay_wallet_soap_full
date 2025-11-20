using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Tsg.Business.Model.TsgGPWebService;

namespace Tsg.Business.Model.Classes
{
    public class TsgGPWebServiceHelper
    {
        public static void AddBearerToken(ClientBase<IGPWebService1> client, string bearerToken)
        {
            //var httpRequestProperty = new HttpRequestMessageProperty();
            //httpRequestProperty.Headers["Authorization"] = "Bearer " + bearerToken;

            //using (var scope = new OperationContextScope(client.InnerChannel))
            //{
            //    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
            //}

            using (new OperationContextScope(client.InnerChannel))
            {
                HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
                requestMessage.Headers["Authorization"] = string.Format("Bearer {0}", bearerToken);
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
            }
        }
    }
}
