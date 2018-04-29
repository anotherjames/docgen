using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DocGen.Web.Hosting.Impl;
using Xunit;

namespace DocGen.Web.Tests
{
    public class AppBaseAppendMessageHandlerTests
    {
        [Theory]
        [InlineData("", "", "http://localhost/")]
        [InlineData("", "/", "http://localhost/")]
        [InlineData("", "/test", "http://localhost/test")]
        [InlineData("", "test", "http://localhost/test")]
        [InlineData("/base", "", "http://localhost/base/")]
        [InlineData("/base", "/", "http://localhost/base/")]
        [InlineData("/base", "/test", "http://localhost/base/test")]
        [InlineData("/base", "test", "http://localhost/base/test")]
        [InlineData("/base", "test", "http://localhost:5000/base/test", "http://localhost:5000")]
        [InlineData("/base", "test", "http://localhost:5000/base/test", "http://localhost:5000/")]
        public async Task Can_append_app_base_to_request(string appBase, string path, string expected, string baseAddress = null)
        {
            if (string.IsNullOrEmpty(baseAddress)) baseAddress = "http://localhost";
            var inner = new HttpClient(new EmptyHandler());
            var client = new HttpClient(new AppBaseAppendMessageHandler(inner, appBase));
            client.BaseAddress = new Uri(baseAddress);
            using (client)
            {
                var response = await client.GetAsync(path);
                Assert.Equal(expected, response.RequestMessage.RequestUri.ToString());
            }
        }

        class EmptyHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    RequestMessage = request
                });
            }
        }
    }
}