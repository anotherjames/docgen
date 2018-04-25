using System;

namespace DocGen.Web.Hosting
{
    public interface IWebHost : IHost
    {
        void Listen();
    }
}