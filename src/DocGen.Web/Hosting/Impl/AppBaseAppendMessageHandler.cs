using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DocGen.Web.Hosting.Impl
{
    public class AppBaseAppendMessageHandler : HttpMessageHandler
    {
        private readonly HttpClient _innerHttpClient;
        private readonly PathString _appBase;

        public AppBaseAppendMessageHandler(HttpClient innerHttpClient, PathString appBase)
        {
            _innerHttpClient = innerHttpClient;
            _appBase = appBase;
        }
        
        private async Task<HttpResponseMessage> CloneResponseAsync(HttpResponseMessage response)
        {
            var newResponse = new HttpResponseMessage(response.StatusCode);
            var ms = new MemoryStream();

            foreach (var v in response.Headers)
            {
                newResponse.Headers.TryAddWithoutValidation(v.Key, v.Value);
            }

            if (response.Content != null)
            {
                await response.Content.CopyToAsync(ms).ConfigureAwait(false);
                ms.Position = 0;
                newResponse.Content = new StreamContent(ms);

                foreach (var v in response.Content.Headers)
                {
                    newResponse.Content.Headers.TryAddWithoutValidation(v.Key, v.Value);
                }

            }
            return newResponse;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
                
            if (disposing)
            {
                _innerHttpClient.Dispose();
            }
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var copy = new HttpRequestMessage(request.Method, request.RequestUri);
            
            copy.Content = request.Content;
            
            foreach (var v in request.Headers)
            {
                copy.Headers.TryAddWithoutValidation(v.Key, v.Value);
            }

            foreach (var v in request.Properties)
            {
                copy.Properties[v.Key] = v.Value;
            }

            copy.Version = request.Version;

            var uri = copy.RequestUri.ToString();

            foreach (var component in Enum.GetValues(typeof(UriComponents)))
            {
                foreach (var format in Enum.GetValues(typeof(UriFormat)))
                {
                    var value = request.RequestUri.GetComponents((UriComponents) component, (UriFormat) format);
                    Console.WriteLine($"{Enum.GetName(typeof(UriComponents), component)}:{Enum.GetName(typeof(UriFormat), format)}:{value}");
                }
            }

            if (!_appBase.HasValue) return _innerHttpClient.SendAsync(copy, cancellationToken);
            
            var pathString = _appBase;
            pathString = pathString.Add(copy.RequestUri.PathAndQuery);

            copy.RequestUri = new Uri(copy.RequestUri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped) + pathString);
                
            
            return _innerHttpClient.SendAsync(copy, cancellationToken);
        }
    }
}