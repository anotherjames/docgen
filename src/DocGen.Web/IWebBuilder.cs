using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace DocGen.Web
{
    public interface IWebBuilder
    {
        void Register(string path, Func<HttpContext, Task> action);

        void RegisterFiles(IFileProvider fileProvider);

        void RegisterServices(Action<IServiceCollection> action);

        IWeb BuildWeb(int port = 8000);
    }
}