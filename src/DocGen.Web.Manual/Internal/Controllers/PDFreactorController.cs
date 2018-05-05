using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;

namespace DocGen.Web.Manual.Internal.Controllers
{
    public class PDFreactorController : Controller
    {
        public IActionResult Archive()
        {
            return new ArchiveActionResult();
        }

        public async Task<ActionResult> Pdf()
        {
            var serverUrl = Url.ServerBaseUrl();
            using (var client = new HttpClient())
            {
                var streamingArchiveContent = new StreamedUrlHttpContent(serverUrl + "/archive.zip");
                streamingArchiveContent.Headers.ContentType = new StringContent("", Encoding.Default, "application/zip").Headers.ContentType;
                var response = await client.PostAsync("http://localhost:8080/service/rest/convert.pdf", streamingArchiveContent);
                response.EnsureSuccessStatusCode();
                return new FileStreamResult(await response.Content.ReadAsStreamAsync(), "application/pdf");
            }
        }

        private static async Task SaveUrlToStream(string url, Stream stream)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                using (var requestStream = await response.Content.ReadAsStreamAsync())
                    await requestStream.CopyToAsync(stream);
            }
        }
        
        class StreamedUrlHttpContent : HttpContent
        {
            readonly string _url;

            public StreamedUrlHttpContent(string url)
            {
                _url = url;
            }
            
            protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                await SaveUrlToStream(_url, stream);
            }

            protected override bool TryComputeLength(out long length)
            {
                length = 0;
                return false;
            }
        }

        private class ArchiveActionResult : IActionResult
        {
            public async Task ExecuteResultAsync(ActionContext context)
            {
                context.HttpContext.Response.ContentType = "application/zip";
                var serverUrl = new UrlHelperFactory().GetUrlHelper(context).ServerBaseUrl();

                using (var archive = new ZipArchive(context.HttpContext.Response.Body, ZipArchiveMode.Create, true))
                {
                    // Create the configuration.json
                    var configuration = archive.CreateEntry("configuration.json");
                    using (var entryStream = configuration.Open())
                    using (StreamWriter entryStreamWriter = new StreamWriter(entryStream))
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(entryStreamWriter))
                    {
                        var serializer = new JsonSerializer();
                        serializer.Serialize(jsonWriter, new
                        {
                            document = "input.html"
                        });
                        jsonWriter.Flush();
                    }
                
                    // Creat the input.html
                    var input = archive.CreateEntry("input.html");
                    using (var entryStream = input.Open())
                    {
                        await SaveUrlToStream(serverUrl + "/", entryStream);
                    }
                }
            }
        }
    }
}