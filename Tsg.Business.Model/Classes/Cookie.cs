using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tsg.Business.Model.Classes
{
    public class CookieDelegateHandler : DelegatingHandler
    {
        public CookieContainer CookieContainer { get; set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return
                base.SendAsync(request, cancellationToken).ContinueWith(responseToCompleteTask
                    =>
                {
                    var result = responseToCompleteTask.Result;
                    var cookieHeaders = result.Headers.Where(pair => pair.Key == "JSESSIONID");
                    foreach (var value in cookieHeaders.SelectMany(header => header.Value))
                    {
                        CookieContainer.SetCookies(request.RequestUri, value);
                    }
                    return result;
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}