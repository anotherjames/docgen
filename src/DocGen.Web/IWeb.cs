using System;

namespace DocGen.Web
{
    public interface IWeb : IDisposable
    {
        void Listen();
    }
}