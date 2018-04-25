using System;
using System.Collections.Generic;
using System.Net.Http;

namespace DocGen.Web.Hosting
{
    public interface IHost : IDisposable
    {
        IReadOnlyCollection<string> Paths { get; }

        HttpClient CreateClient();
    }
}