using System;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

public class BearerTokenEndpointBehavior : IEndpointBehavior
{
    private readonly string _bearerToken;

    public BearerTokenEndpointBehavior(string bearerToken)
    {
        _bearerToken = bearerToken;
    }

    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
        clientRuntime.ClientMessageInspectors.Add(new BearerTokenMessageInspector(_bearerToken));
    }

    public void Validate(ServiceEndpoint endpoint)
    {
    }

    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
    {
    }

    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
    {
    }
}

public class BearerTokenMessageInspector : IClientMessageInspector
{
    private readonly string _bearerToken;

    public BearerTokenMessageInspector(string bearerToken)
    {
        _bearerToken = bearerToken;
    }

    public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
    {
        HttpRequestMessageProperty httpRequestProperty;
        if (request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
        {
            httpRequestProperty = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
        }
        else
        {
            httpRequestProperty = new HttpRequestMessageProperty();
            request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestProperty);
        }

        httpRequestProperty.Headers["Authorization"] = "Bearer " + _bearerToken;

        return null;
    }

    public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
    {
    }
}